using ALife.Spatial.Packed;

namespace ALife.Spatial;

/// <summary>
/// A stack of uniform grids, each layer's cells a quarter the size of the one above. Each element goes in the finest layer
/// whose cells it fits in, by the center of its box, so small elements share small cells and big ones big cells, and sizes
/// that vary widely don't slow the small ones down. Unlike <see cref="UniformGrid"/>, it keeps each element's exact box.
/// </summary>
/// <remarks>
/// <para>
/// Size the top layer's cells a little bigger than your biggest element; bigger ones still work (see
/// <see cref="OversizedCount"/>), but widen every search of the top layer. Add layers until the finest cells are a little
/// bigger than your smallest common element.
/// </para>
/// <para>
/// Besides the <see cref="ISpatialIndex"/> members, it finds every intersecting pair in one pass with
/// <see cref="ForEachPair{TVisitor}"/>, or in parallel with <see cref="ForEachNeighbor{TVisitor}"/>, which splits the work
/// by top cell over <see cref="Threads"/> threads. Unlike the other indexes, <see cref="Query"/> uses no scratch buffers, so
/// it can run on several threads at once while nothing changes the grid.
/// </para>
/// </remarks>
public sealed class LayeredGrid : ISpatialIndex
{
    /// <summary>The most layers a grid can have.</summary>
    public const int MaxLayers = GridLayers.MaxLayers;

    private readonly GridLayers _layers;
    private readonly FreeList<Element> _elements = new();

    // Per layer: the first element in each cell's list, how many each cell holds, and how many the whole layer holds.
    private readonly int[][] _heads;
    private readonly int[][] _counts;
    private readonly int[] _layerCounts;

    // How far an oversized element in the top layer reaches from its center. Every other element reaches half a cell at most.
    private float _oversizedReachX, _oversizedReachY;
    private int _threads;

    /// <summary>Creates an empty grid.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed in its edge cells.</param>
    /// <param name="columns">The number of cell columns in the top layer. Each layer below has twice as many.</param>
    /// <param name="rows">The number of cell rows in the top layer. Each layer below has twice as many.</param>
    /// <param name="layers">How many layers, from 1 (a plain grid) to <see cref="MaxLayers"/>.</param>
    /// <param name="threads">How many threads <see cref="ForEachNeighbor{TVisitor}"/> uses; see <see cref="Threads"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A count is below 1, <paramref name="layers"/> is above <see cref="MaxLayers"/>, or the finest layer would have more
    /// than 2^24 cells.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="bounds"/> has no area.</exception>
    public LayeredGrid(Aabb bounds, int columns, int rows, int layers = 4, int threads = 4)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threads, 1);
        _layers = new GridLayers(bounds, columns, rows, layers);
        _threads = threads;
        _heads = new int[layers][];
        _counts = new int[layers][];
        _layerCounts = new int[layers];
        for (int layer = 0; layer < layers; layer++)
        {
            int cells = _layers.Columns[layer] * _layers.Rows[layer];
            _heads[layer] = new int[cells];
            _counts[layer] = new int[cells];
        }

        Clear();
    }

    /// <inheritdoc />
    public Aabb Bounds => _layers.Bounds;

    /// <summary>The number of cell columns in the top layer.</summary>
    public int Columns => _layers.Columns[0];

    /// <summary>The number of cell rows in the top layer.</summary>
    public int Rows => _layers.Rows[0];

    /// <summary>The number of layers.</summary>
    public int Layers => _layers.Count;

    /// <summary>The width of a top cell. Each layer below halves it, and elements have to be a little narrower to go in a layer.</summary>
    public float CellWidth => _layers.CellWidth[0];

    /// <summary>The height of a top cell. Each layer below halves it, and elements have to be a little shorter to go in a layer.</summary>
    public float CellHeight => _layers.CellHeight[0];

    /// <summary>
    /// How many threads <see cref="ForEachNeighbor{TVisitor}"/> uses at most. The work is split by top cell, so there's no
    /// point in more threads than top cells. 1 runs it on the calling thread.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is below 1.</exception>
    public int Threads
    {
        get => _threads;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
            _threads = value;
        }
    }

    /// <inheritdoc />
    public int Count => _elements.Count;

    /// <summary>How many elements are too big even for a top cell. They're in the top layer anyway.</summary>
    public int OversizedCount { get; private set; }

    /// <inheritdoc />
    public int Insert(int id, Aabb bounds)
    {
        int layer = _layers.LayerFor(bounds);
        int cell = _layers.CellOf(layer, bounds.CenterX, bounds.CenterY);
        int element = _elements.Insert(new Element(-1, -1, id, layer, cell, bounds));
        Link(element);
        NoteSize(layer, bounds, 1);
        return element;
    }

    /// <inheritdoc />
    public void Remove(int element)
    {
        Element e = _elements[element];
        NoteSize(e.Layer, e.Bounds, -1);
        Unlink(element);
        _elements.Erase(element);
    }

    /// <inheritdoc />
    public void Move(int element, Aabb bounds)
    {
        ref Element e = ref _elements[element];
        int layer = _layers.LayerFor(bounds);
        int cell = _layers.CellOf(layer, bounds.CenterX, bounds.CenterY);
        NoteSize(e.Layer, e.Bounds, -1);
        NoteSize(layer, bounds, 1);
        e.Bounds = bounds;
        if (layer != e.Layer || cell != e.Cell)
        {
            Unlink(element);
            e.Layer = layer;
            e.Cell = cell;
            Link(element);
        }
    }

    /// <inheritdoc />
    public int GetId(int element) => _elements[element].Id;

    /// <inheritdoc />
    /// <remarks>Returns the exact box the element was inserted or last moved with.</remarks>
    public Aabb GetBounds(int element) => _elements[element].Bounds;

    /// <inheritdoc />
    /// <remarks>Uses no scratch buffers, so it's safe to run on several threads at once while nothing changes the grid.</remarks>
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (area.IsEmpty)
        {
            return;
        }

        for (int layer = 0; layer < Layers; layer++)
        {
            if (_layerCounts[layer] == 0)
            {
                continue;
            }

            int[] heads = _heads[layer];
            int columns = _layers.Columns[layer];
            GridCells.Range range = Around(layer, area);
            for (int y = range.Y1; y <= range.Y2; y++)
            {
                for (int x = range.X1; x <= range.X2; x++)
                {
                    for (int other = heads[y * columns + x]; other != -1; other = _elements[other].Next)
                    {
                        if (other != omitElement && _elements[other].Bounds.Intersects(area))
                        {
                            results.Add(other);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calls <c>visitor.Visit(a, b)</c> once for each pair of elements whose boxes intersect, with their handles, on this
    /// thread. The visitor is passed by reference, so a struct visitor can count or collect as it goes.
    /// </summary>
    /// <typeparam name="TVisitor">The visitor's type; a struct, so the JIT can inline its <see cref="IPairVisitor.Visit"/>.</typeparam>
    /// <param name="visitor">The visitor.</param>
    public void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor
    {
        for (int layer = 0; layer < Layers; layer++)
        {
            if (_layerCounts[layer] == 0)
            {
                continue;
            }

            int[] heads = _heads[layer];
            int columns = _layers.Columns[layer], rows = _layers.Rows[layer];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    for (int element = heads[y * columns + x]; element != -1; element = _elements[element].Next)
                    {
                        Aabb box = _elements[element].Bounds;

                        // In its own layer, only what comes after it, so each pair is compared once: the rest of its own
                        // cell's list, the rest of the row as far as it reaches, then the rows below.
                        GridCells.Range range = Around(layer, box);
                        VisitList(ref visitor, element, box, _elements[element].Next, -1);
                        for (int cell = y * columns + x + 1; cell <= y * columns + range.X2; cell++)
                        {
                            VisitList(ref visitor, element, box, heads[cell], -1);
                        }

                        for (int row = y + 1; row <= range.Y2; row++)
                        {
                            for (int column = range.X1; column <= range.X2; column++)
                            {
                                VisitList(ref visitor, element, box, heads[row * columns + column], -1);
                            }
                        }

                        // Pairs across layers are found from the bigger element, whose box covers fewer, smaller cells.
                        for (int finer = layer + 1; finer < Layers; finer++)
                        {
                            VisitLayer(ref visitor, finer, element, box);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calls <c>visitor.Visit(a, b)</c> for every element <c>a</c> and every other element <c>b</c> whose box intersects it,
    /// with their handles, on up to <see cref="Threads"/> threads. Each pair comes up twice, once from each side. Returns when
    /// every call has finished.
    /// </summary>
    /// <remarks>
    /// Each top cell is one piece of work, for every layer's elements under it. Calls with the same <c>a</c> never run at the
    /// same time, so the visitor may write <c>a</c>'s own state, but it must only read the others'. Each piece of work gets
    /// its own copy of a struct visitor. The grid mustn't change until it returns.
    /// </remarks>
    /// <typeparam name="TVisitor">The visitor's type; a struct, so the JIT can inline its <see cref="IPairVisitor.Visit"/>.</typeparam>
    /// <param name="visitor">The visitor.</param>
    public void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor =>
        _layers.ForEachTopCell(_threads, top =>
        {
            TVisitor local = visitor;
            NeighborsInTopCell(ref local, top);
        });

    /// <inheritdoc />
    /// <remarks>
    /// Shrinks how far the top layer's searches reach to fit the oversized elements still there. The grid never changes
    /// shape, so there's nothing else to do.
    /// </remarks>
    public void Cleanup()
    {
        _oversizedReachX = 0;
        _oversizedReachY = 0;
        if (OversizedCount == 0)
        {
            return;
        }

        foreach (int head in _heads[0])
        {
            for (int element = head; element != -1; element = _elements[element].Next)
            {
                Aabb box = _elements[element].Bounds;
                if (!_layers.Fits(0, box))
                {
                    _oversizedReachX = MathF.Max(_oversizedReachX, box.Width * 0.5f);
                    _oversizedReachY = MathF.Max(_oversizedReachY, box.Height * 0.5f);
                }
            }
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        _elements.Clear();
        for (int layer = 0; layer < Layers; layer++)
        {
            Array.Fill(_heads[layer], -1);
            Array.Clear(_counts[layer]);
        }

        Array.Clear(_layerCounts);
        OversizedCount = 0;
        _oversizedReachX = 0;
        _oversizedReachY = 0;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Reports every top cell, at depth 0, and every cell of the finer layers that holds anything, at its layer's depth, all
    /// as leaves with their element counts.
    /// </remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        for (int layer = 0; layer < Layers; layer++)
        {
            int[] counts = _counts[layer];
            int columns = _layers.Columns[layer], rows = _layers.Rows[layer];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int count = counts[y * columns + x];
                    if (layer == 0 || count > 0)
                    {
                        visitor(_layers.CellBounds(layer, x, y), layer, true, count);
                    }
                }
            }
        }
    }

    // Every element under a top cell, in every layer, against everything it touches in every layer.
    private void NeighborsInTopCell<TVisitor>(ref TVisitor visitor, int top) where TVisitor : IPairVisitor
    {
        int topX = top % Columns, topY = top / Columns;
        for (int layer = 0; layer < Layers; layer++)
        {
            if (_layerCounts[layer] == 0)
            {
                continue;
            }

            int[] heads = _heads[layer];
            int columns = _layers.Columns[layer];
            for (int y = topY << layer; y < (topY + 1) << layer; y++)
            {
                for (int x = topX << layer; x < (topX + 1) << layer; x++)
                {
                    for (int element = heads[y * columns + x]; element != -1; element = _elements[element].Next)
                    {
                        Aabb box = _elements[element].Bounds;
                        for (int other = 0; other < Layers; other++)
                        {
                            VisitLayer(ref visitor, other, element, box);
                        }
                    }
                }
            }
        }
    }

    // Visits (element, e) for each other element e in a layer whose box intersects the box.
    private void VisitLayer<TVisitor>(ref TVisitor visitor, int layer, int element, Aabb box) where TVisitor : IPairVisitor
    {
        if (_layerCounts[layer] == 0)
        {
            return;
        }

        int[] heads = _heads[layer];
        int columns = _layers.Columns[layer];
        GridCells.Range range = Around(layer, box);
        for (int y = range.Y1; y <= range.Y2; y++)
        {
            for (int x = range.X1; x <= range.X2; x++)
            {
                VisitList(ref visitor, element, box, heads[y * columns + x], element);
            }
        }
    }

    // Visits (element, e) for each e from first to the end of its cell's list whose box intersects the box, other than skip.
    private void VisitList<TVisitor>(ref TVisitor visitor, int element, Aabb box, int first, int skip) where TVisitor : IPairVisitor
    {
        for (int other = first; other != -1; other = _elements[other].Next)
        {
            if (other != skip && _elements[other].Bounds.Intersects(box))
            {
                visitor.Visit(element, other);
            }
        }
    }

    // The cells of a layer holding the centers of its elements that could reach a box.
    private GridCells.Range Around(int layer, Aabb box)
    {
        float reachX = _layers.CellWidth[layer] * 0.5f, reachY = _layers.CellHeight[layer] * 0.5f;
        if (layer == 0)
        {
            reachX = MathF.Max(reachX, _oversizedReachX);
            reachY = MathF.Max(reachY, _oversizedReachY);
        }

        return _layers.Around(layer, box, reachX, reachY);
    }

    // Keeps the layer counts and the oversized count and reach up to date as a box comes (change 1) or goes (change -1).
    private void NoteSize(int layer, Aabb bounds, int change)
    {
        _layerCounts[layer] += change;
        if (layer != 0 || _layers.Fits(0, bounds))
        {
            return;
        }

        OversizedCount += change;
        if (change > 0)
        {
            _oversizedReachX = MathF.Max(_oversizedReachX, bounds.Width * 0.5f);
            _oversizedReachY = MathF.Max(_oversizedReachY, bounds.Height * 0.5f);
        }
    }

    private void Link(int element)
    {
        ref Element e = ref _elements[element];
        int[] heads = _heads[e.Layer];
        e.Previous = -1;
        e.Next = heads[e.Cell];
        if (e.Next != -1)
        {
            _elements[e.Next].Previous = element;
        }

        heads[e.Cell] = element;
        _counts[e.Layer][e.Cell]++;
    }

    private void Unlink(int element)
    {
        Element e = _elements[element];
        if (e.Previous == -1)
        {
            _heads[e.Layer][e.Cell] = e.Next;
        }
        else
        {
            _elements[e.Previous].Next = e.Next;
        }

        if (e.Next != -1)
        {
            _elements[e.Next].Previous = e.Previous;
        }

        _counts[e.Layer][e.Cell]--;
    }

    /// <summary>An element: its exact box, and its place in its cell's doubly linked list.</summary>
    private record struct Element(int Next, int Previous, int Id, int Layer, int Cell, Aabb Bounds);
}
