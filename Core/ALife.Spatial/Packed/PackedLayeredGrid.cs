namespace ALife.Spatial.Packed;

/// <summary>
/// A stack of uniform grids rebuilt every frame, each layer's cells a quarter the size of the one above. Each element goes in
/// the finest layer whose cells it fits in, by the center of its box, and each layer is counting-sorted into flat arrays as
/// in <see cref="PackedGrid"/>. So small elements share small cells and big ones big cells, and mixed sizes don't need a
/// side structure.
/// </summary>
/// <remarks>
/// Size the top layer's cells a little bigger than your biggest element; bigger ones still work (see
/// <see cref="OversizedCount"/>), but widen every search of the top layer. Add layers until the finest cells are a little
/// bigger than your smallest common element. <see cref="ForEachNeighbor{TVisitor}"/> splits its work by top cell over
/// <see cref="Threads"/> threads.
/// </remarks>
public sealed class PackedLayeredGrid : IPackedSpatialIndex
{
    /// <summary>The most layers a grid can have.</summary>
    public const int MaxLayers = GridLayers.MaxLayers;

    private readonly GridLayers _layers;
    private readonly PackedBoxes _sorted = new();

    // Per layer: where each cell's run starts in _sorted (one more than the cells, so a cell ends where the next starts), a
    // cursor per cell for filling them, and how far its elements' boxes reach from their centers.
    private readonly int[][] _cellStarts;
    private readonly int[][] _cursors;
    private readonly float[] _reachX, _reachY;
    private Aabb[] _boxes = [];
    private int[] _cells = [];
    private byte[] _elementLayers = [];
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
    public PackedLayeredGrid(Aabb bounds, int columns, int rows, int layers = 4, int threads = 4)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threads, 1);
        _layers = new GridLayers(bounds, columns, rows, layers);
        _threads = threads;
        _cellStarts = new int[layers][];
        _cursors = new int[layers][];
        _reachX = new float[layers];
        _reachY = new float[layers];
        for (int layer = 0; layer < layers; layer++)
        {
            int cells = _layers.Columns[layer] * _layers.Rows[layer];
            _cellStarts[layer] = new int[cells + 1];
            _cursors[layer] = new int[cells];
        }
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
    public int Count { get; private set; }

    /// <summary>How many elements in the last build were too big even for a top cell. They're in the top layer anyway.</summary>
    public int OversizedCount { get; private set; }

    /// <inheritdoc />
    public void Build(ReadOnlySpan<Aabb> boxes)
    {
        int count = boxes.Length;
        if (count > _boxes.Length)
        {
            _boxes = new Aabb[Math.Max(count, _boxes.Length * 2)];
            _cells = new int[_boxes.Length];
            _elementLayers = new byte[_boxes.Length];
        }

        boxes.CopyTo(_boxes);
        _sorted.EnsureCapacity(count);
        Count = count;
        OversizedCount = 0;
        for (int layer = 0; layer < Layers; layer++)
        {
            Array.Clear(_cellStarts[layer]);
            _reachX[layer] = 0;
            _reachY[layer] = 0;
        }

        // Count each cell's elements, one along so the running total below turns the counts into starts.
        for (int i = 0; i < count; i++)
        {
            Aabb box = _boxes[i];
            if (box.IsEmpty)
            {
                _cells[i] = -1;
                continue;
            }

            int layer = _layers.LayerFor(box);
            if (layer == 0 && !_layers.Fits(0, box))
            {
                OversizedCount++;
            }

            int cell = _layers.CellOf(layer, box.CenterX, box.CenterY);
            _cells[i] = cell;
            _elementLayers[i] = (byte)layer;
            _cellStarts[layer][cell + 1]++;
            _reachX[layer] = MathF.Max(_reachX[layer], box.Width * 0.5f);
            _reachY[layer] = MathF.Max(_reachY[layer], box.Height * 0.5f);
        }

        // The layers follow one another in the sorted boxes, top layer first.
        int total = 0;
        for (int layer = 0; layer < Layers; layer++)
        {
            int[] starts = _cellStarts[layer], cursors = _cursors[layer];
            starts[0] = total;
            for (int cell = 0; cell < cursors.Length; cell++)
            {
                starts[cell + 1] += starts[cell];
                cursors[cell] = starts[cell];
            }

            total = starts[^1];
        }

        for (int i = 0; i < count; i++)
        {
            int cell = _cells[i];
            if (cell >= 0)
            {
                _sorted.Set(_cursors[_elementLayers[i]][cell]++, i, _boxes[i]);
            }
        }
    }

    /// <inheritdoc />
    public Aabb GetBounds(int element) => _boxes[element];

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (area.IsEmpty)
        {
            return;
        }

        for (int layer = 0; layer < Layers; layer++)
        {
            if (IsEmpty(layer))
            {
                continue;
            }

            int[] starts = _cellStarts[layer];
            int columns = _layers.Columns[layer];
            GridCells.Range range = _layers.Around(layer, area, _reachX[layer], _reachY[layer]);
            for (int y = range.Y1; y <= range.Y2; y++)
            {
                _sorted.AddRun(results, area, starts[y * columns + range.X1], starts[y * columns + range.X2 + 1], omitElement);
            }
        }
    }

    /// <inheritdoc />
    public void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor
    {
        for (int layer = 0; layer < Layers; layer++)
        {
            if (IsEmpty(layer))
            {
                continue;
            }

            int[] starts = _cellStarts[layer];
            int columns = _layers.Columns[layer], rows = _layers.Rows[layer];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int cell = y * columns + x;
                    for (int p = starts[cell]; p < starts[cell + 1]; p++)
                    {
                        int element = _sorted.ElementAt(p);
                        Aabb box = _sorted.BoxAt(p);

                        // In its own layer, only what comes after it, so each pair is compared once: the rest of this row
                        // as far as it reaches, which is one run, then the rows below.
                        GridCells.Range range = _layers.Around(layer, box, _reachX[layer], _reachY[layer]);
                        _sorted.VisitRun(ref visitor, element, box, p + 1, starts[y * columns + range.X2 + 1], -1);
                        for (int row = y + 1; row <= range.Y2; row++)
                        {
                            _sorted.VisitRun(ref visitor, element, box, starts[row * columns + range.X1], starts[row * columns + range.X2 + 1], -1);
                        }

                        // Pairs across layers are found from the bigger element, whose box covers fewer, smaller cells.
                        for (int finer = layer + 1; finer < Layers; finer++)
                        {
                            VisitLayer(ref visitor, finer, element, box, -1);
                        }
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>Each top cell is one piece of work, for every layer's elements under it, spread over <see cref="Threads"/> threads.</remarks>
    public void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor =>
        _layers.ForEachTopCell(_threads, top =>
        {
            TVisitor local = visitor;
            NeighborsInTopCell(ref local, top);
        });

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
            int[] starts = _cellStarts[layer];
            int columns = _layers.Columns[layer], rows = _layers.Rows[layer];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int cell = y * columns + x;
                    int count = starts[cell + 1] - starts[cell];
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
            if (IsEmpty(layer))
            {
                continue;
            }

            int[] starts = _cellStarts[layer];
            int columns = _layers.Columns[layer];
            int x1 = topX << layer, x2 = (topX + 1) << layer;
            for (int y = topY << layer; y < (topY + 1) << layer; y++)
            {
                for (int p = starts[y * columns + x1]; p < starts[y * columns + x2]; p++)
                {
                    int element = _sorted.ElementAt(p);
                    Aabb box = _sorted.BoxAt(p);
                    for (int other = 0; other < Layers; other++)
                    {
                        VisitLayer(ref visitor, other, element, box, other == layer ? p : -1);
                    }
                }
            }
        }
    }

    // Visits (element, e) for each element e in a layer whose box intersects the box, other than the one at position skip.
    private void VisitLayer<TVisitor>(ref TVisitor visitor, int layer, int element, Aabb box, int skip) where TVisitor : IPairVisitor
    {
        if (IsEmpty(layer))
        {
            return;
        }

        int[] starts = _cellStarts[layer];
        int columns = _layers.Columns[layer];
        GridCells.Range range = _layers.Around(layer, box, _reachX[layer], _reachY[layer]);
        for (int y = range.Y1; y <= range.Y2; y++)
        {
            _sorted.VisitRun(ref visitor, element, box, starts[y * columns + range.X1], starts[y * columns + range.X2 + 1], skip);
        }
    }

    private bool IsEmpty(int layer) => _cellStarts[layer][0] == _cellStarts[layer][^1];
}
