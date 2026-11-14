namespace ALife.Spatial.Packed;

/// <summary>
/// A uniform grid rebuilt every frame by a counting sort: each element is filed in one cell by the center of its box, and
/// the boxes are stored cell by cell in flat arrays, so a cell and its right-hand neighbor, or a row of three cells, are one
/// unbroken run of memory. As long as no element is bigger than a cell, overlapping elements are in the same or neighboring
/// cells, so finding every pair only means comparing each cell with half of its neighbors.
/// </summary>
/// <remarks>
/// Size cells a little bigger than your biggest common element, since an element has to be smaller than a cell (by at least
/// 0.1%, to stay clear of rounding) to go in the grid; a few elements per cell is the sweet spot. Bigger elements still work:
/// they go in a <see cref="PackedQuadTree"/> on the side (<see cref="OversizedCount"/>), and each is compared with the grid
/// by a query over its area. That suits a few big elements among many small ones; if most are oversized, the cells are too
/// small.
/// </remarks>
public sealed class PackedGrid : IPackedSpatialIndex
{
    // Elements must be a hair smaller than a cell to go in the grid, so float rounding can never put two overlapping
    // centers two cells apart.
    private const float FitFraction = 0.999f;

    private readonly PackedBoxes _sorted = new();
    private readonly PackedQuadTree _large;
    private readonly int[] _cellStarts;
    private readonly int[] _cursors;
    private readonly float _inverseCellWidth, _inverseCellHeight;
    private Aabb[] _boxes = [];
    private int[] _cells = [];
    private int[] _oversized = [];
    private Aabb[] _largeBoxes = [];

    // How far a gridded element's box can reach from its center, which is how far a query has to look past its area.
    private float _reachX, _reachY;

    /// <summary>Creates an empty grid.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed in its edge cells.</param>
    /// <param name="columns">The number of cell columns.</param>
    /// <param name="rows">The number of cell rows.</param>
    public PackedGrid(Aabb bounds, int columns, int rows)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columns, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, 1);
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentException("The bounds must have an area.", nameof(bounds));
        }

        Bounds = bounds;
        Columns = columns;
        Rows = rows;
        CellWidth = bounds.Width / columns;
        CellHeight = bounds.Height / rows;
        _inverseCellWidth = columns / bounds.Width;
        _inverseCellHeight = rows / bounds.Height;
        _cellStarts = new int[columns * rows + 1];
        _cursors = new int[columns * rows];
        _large = new PackedQuadTree(bounds);
    }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <summary>The number of cell columns.</summary>
    public int Columns { get; }

    /// <summary>The number of cell rows.</summary>
    public int Rows { get; }

    /// <summary>The width of a cell. Elements have to be a little narrower to go in the grid.</summary>
    public float CellWidth { get; }

    /// <summary>The height of a cell. Elements have to be a little shorter to go in the grid.</summary>
    public float CellHeight { get; }

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <summary>How many elements in the last build were too big for a cell and went in the side tree.</summary>
    public int OversizedCount { get; private set; }

    /// <inheritdoc />
    public void Build(ReadOnlySpan<Aabb> boxes)
    {
        int count = boxes.Length;
        if (count > _boxes.Length)
        {
            _boxes = new Aabb[Math.Max(count, _boxes.Length * 2)];
            _cells = new int[_boxes.Length];
        }

        boxes.CopyTo(_boxes);
        _sorted.EnsureCapacity(count);
        Count = count;
        OversizedCount = 0;
        _reachX = 0;
        _reachY = 0;
        Array.Clear(_cellStarts);

        // Count each cell's elements, one along so the running total below turns the counts into starts.
        float maxWidth = CellWidth * FitFraction, maxHeight = CellHeight * FitFraction;
        for (int i = 0; i < count; i++)
        {
            Aabb box = _boxes[i];
            if (box.IsEmpty)
            {
                _cells[i] = -1;
            }
            else if (box.Width <= maxWidth && box.Height <= maxHeight)
            {
                int cell = CellOf(box.CenterX, box.CenterY);
                _cells[i] = cell;
                _cellStarts[cell + 1]++;
                _reachX = MathF.Max(_reachX, box.Width * 0.5f);
                _reachY = MathF.Max(_reachY, box.Height * 0.5f);
            }
            else
            {
                _cells[i] = -1;
                if (OversizedCount == _oversized.Length)
                {
                    Array.Resize(ref _oversized, Math.Max(16, _oversized.Length * 2));
                    Array.Resize(ref _largeBoxes, _oversized.Length);
                }

                _largeBoxes[OversizedCount] = box;
                _oversized[OversizedCount++] = i;
            }
        }

        for (int cell = 0; cell < _cursors.Length; cell++)
        {
            _cellStarts[cell + 1] += _cellStarts[cell];
            _cursors[cell] = _cellStarts[cell];
        }

        for (int i = 0; i < count; i++)
        {
            int cell = _cells[i];
            if (cell >= 0)
            {
                _sorted.Set(_cursors[cell]++, i, _boxes[i]);
            }
        }

        _large.Build(_largeBoxes.AsSpan(0, OversizedCount));
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

        // An element whose box reaches the area has its center within the area grown by the reach.
        int x1 = Column(area.Left - _reachX), x2 = Column(area.Right + _reachX);
        int y1 = Row(area.Top - _reachY), y2 = Row(area.Bottom + _reachY);
        for (int y = y1; y <= y2; y++)
        {
            _sorted.AddRun(results, area, _cellStarts[y * Columns + x1], _cellStarts[y * Columns + x2 + 1], omitElement);
        }

        if (OversizedCount > 0)
        {
            LargeCollector collector = new(results, _oversized, omitElement);
            _large.VisitArea(ref collector, -1, area);
        }
    }

    /// <inheritdoc />
    public void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor
    {
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                int cell = y * Columns + x;
                int start = _cellStarts[cell], end = _cellStarts[cell + 1];
                if (start == end)
                {
                    continue;
                }

                // Half the neighbors, so each pair of cells is compared once: the rest of this cell and the cell to its right
                // are one run, and the three cells below are another.
                int sameRowEnd = _cellStarts[Math.Min(x + 2, Columns) + y * Columns];
                int belowStart = 0, belowEnd = 0;
                if (y + 1 < Rows)
                {
                    belowStart = _cellStarts[(y + 1) * Columns + Math.Max(x - 1, 0)];
                    belowEnd = _cellStarts[(y + 1) * Columns + Math.Min(x + 1, Columns - 1) + 1];
                }

                for (int p = start; p < end; p++)
                {
                    int element = _sorted.ElementAt(p);
                    Aabb box = _sorted.BoxAt(p);
                    _sorted.VisitRun(ref visitor, element, box, p + 1, sameRowEnd, -1);
                    _sorted.VisitRun(ref visitor, element, box, belowStart, belowEnd, -1);
                }
            }
        }

        if (OversizedCount > 0)
        {
            for (int k = 0; k < OversizedCount; k++)
            {
                VisitGridded(ref visitor, _oversized[k], _largeBoxes[k]);
            }

            RemapBoth<TVisitor> remapped = new(visitor, _oversized);
            _large.ForEachPair(ref remapped);
            visitor = remapped.Visitor;
        }
    }

    /// <inheritdoc />
    public void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor
    {
        // Gridded elements first, then oversized ones, so no element is ever the first of a pair on two threads at once.
        Parallel.For(0, Rows, y =>
        {
            TVisitor local = visitor;
            NeighborsInRow(ref local, y);
        });

        if (OversizedCount > 0)
        {
            Parallel.For(0, OversizedCount, k =>
            {
                TVisitor local = visitor;
                int element = _oversized[k];
                VisitGridded(ref local, element, _largeBoxes[k]);
                Remap<TVisitor> remapped = new(local, _oversized);
                _large.VisitArea(ref remapped, element, _largeBoxes[k]);
            });
        }
    }

    /// <inheritdoc />
    /// <remarks>Reports every cell, at depth 0, as a leaf with its element count. Oversized elements aren't counted.</remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                float left = Bounds.Left + x * CellWidth, top = Bounds.Top + y * CellHeight;
                int cell = y * Columns + x;
                visitor(new Aabb(left, top, left + CellWidth, top + CellHeight), 0, true, _cellStarts[cell + 1] - _cellStarts[cell]);
            }
        }
    }

    // Every gridded element in a row against the three rows of cells around it, which are three unbroken runs, and against
    // the oversized elements.
    private void NeighborsInRow<TVisitor>(ref TVisitor visitor, int y) where TVisitor : IPairVisitor
    {
        Remap<TVisitor> remapped = new(visitor, _oversized);
        int above = Math.Max(y - 1, 0), below = Math.Min(y + 1, Rows - 1);
        for (int x = 0; x < Columns; x++)
        {
            int cell = y * Columns + x;
            int start = _cellStarts[cell], end = _cellStarts[cell + 1];
            int x1 = Math.Max(x - 1, 0), x2 = Math.Min(x + 1, Columns - 1);
            for (int p = start; p < end; p++)
            {
                int element = _sorted.ElementAt(p);
                Aabb box = _sorted.BoxAt(p);
                for (int row = above; row <= below; row++)
                {
                    _sorted.VisitRun(ref visitor, element, box, _cellStarts[row * Columns + x1], _cellStarts[row * Columns + x2 + 1], p);
                }

                if (OversizedCount > 0)
                {
                    _large.VisitArea(ref remapped, element, box);
                }
            }
        }
    }

    // Visits (element, e) for each gridded element e whose box intersects the box.
    private void VisitGridded<TVisitor>(ref TVisitor visitor, int element, Aabb box) where TVisitor : IPairVisitor
    {
        int x1 = Column(box.Left - _reachX), x2 = Column(box.Right + _reachX);
        int y1 = Row(box.Top - _reachY), y2 = Row(box.Bottom + _reachY);
        for (int y = y1; y <= y2; y++)
        {
            _sorted.VisitRun(ref visitor, element, box, _cellStarts[y * Columns + x1], _cellStarts[y * Columns + x2 + 1], -1);
        }
    }

    private int CellOf(float x, float y) => Row(y) * Columns + Column(x);

    private int Column(float x) => GridCells.Index(x - Bounds.Left, _inverseCellWidth, Columns);

    private int Row(float y) => GridCells.Index(y - Bounds.Top, _inverseCellHeight, Rows);

    /// <summary>Passes pairs on with the second element, a position in the side tree, turned back into an element; skips (e, e).</summary>
    private struct Remap<TVisitor>(TVisitor visitor, int[] oversized) : IPairVisitor where TVisitor : IPairVisitor
    {
        private TVisitor _visitor = visitor;

        public void Visit(int a, int b)
        {
            int other = oversized[b];
            if (other != a)
            {
                _visitor.Visit(a, other);
            }
        }
    }

    /// <summary>Passes pairs from the side tree on with both elements turned back into elements.</summary>
    private struct RemapBoth<TVisitor>(TVisitor visitor, int[] oversized) : IPairVisitor where TVisitor : IPairVisitor
    {
        public TVisitor Visitor = visitor;

        public void Visit(int a, int b) => Visitor.Visit(oversized[a], oversized[b]);
    }

    /// <summary>Adds elements from the side tree to a query's results.</summary>
    private readonly struct LargeCollector(List<int> results, int[] oversized, int omitElement) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            int element = oversized[b];
            if (element != omitElement)
            {
                results.Add(element);
            }
        }
    }
}
