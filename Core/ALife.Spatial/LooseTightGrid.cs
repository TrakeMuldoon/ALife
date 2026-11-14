namespace ALife.Spatial;

/// <summary>
/// A loose/tight double grid, after user4842163's "Loose/Tight Double-Grid" answer on Stack Overflow. Each element is filed
/// in exactly one loose cell, by the center of its box, and each loose cell keeps a box around its elements. A coarser tight
/// grid lists the loose cells whose boxes overlap each of its cells, so a query only looks at loose cells near the area,
/// however far a big element has stretched one.
/// </summary>
/// <remarks>
/// Inserts, moves and removals take constant time, and memory use is nearly fixed: there's one loose cell per grid cell and
/// an element is always in exactly one. Loose boxes only grow as elements are added; <see cref="Cleanup"/> shrinks them and
/// rebuilds the tight grid.
/// </remarks>
public sealed class LooseTightGrid : ISpatialIndex
{
    private readonly FreeList<Element> _elements = new();
    private readonly FreeList<LooseCellNode> _looseCellNodes = new();
    private readonly LooseCell[] _looseCells;
    private readonly int[] _tightHeads;
    private readonly int[] _queryStamps;
    private readonly float _looseInverseWidth, _looseInverseHeight, _tightInverseWidth, _tightInverseHeight;
    private int _queryStamp;

    /// <summary>Creates an empty grid.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed in its edge cells.</param>
    /// <param name="looseColumns">The number of loose cell columns. Aim for cells about the size of a common element.</param>
    /// <param name="looseRows">The number of loose cell rows.</param>
    /// <param name="tightColumns">The number of tight cell columns, usually a few times fewer than the loose ones.</param>
    /// <param name="tightRows">The number of tight cell rows.</param>
    public LooseTightGrid(Aabb bounds, int looseColumns, int looseRows, int tightColumns, int tightRows)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(looseColumns, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(looseRows, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(tightColumns, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(tightRows, 1);
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentException("The bounds must have an area.", nameof(bounds));
        }

        Bounds = bounds;
        LooseColumns = looseColumns;
        LooseRows = looseRows;
        TightColumns = tightColumns;
        TightRows = tightRows;
        _looseInverseWidth = looseColumns / bounds.Width;
        _looseInverseHeight = looseRows / bounds.Height;
        _tightInverseWidth = tightColumns / bounds.Width;
        _tightInverseHeight = tightRows / bounds.Height;
        _looseCells = new LooseCell[looseColumns * looseRows];
        _queryStamps = new int[_looseCells.Length];
        _tightHeads = new int[tightColumns * tightRows];
        Clear();
    }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <summary>The number of loose cell columns.</summary>
    public int LooseColumns { get; }

    /// <summary>The number of loose cell rows.</summary>
    public int LooseRows { get; }

    /// <summary>The number of tight cell columns.</summary>
    public int TightColumns { get; }

    /// <summary>The number of tight cell rows.</summary>
    public int TightRows { get; }

    /// <inheritdoc />
    public int Count => _elements.Count;

    /// <inheritdoc />
    public int Insert(int id, Aabb bounds)
    {
        int element = _elements.Insert(new Element(-1, id, bounds));
        Link(LooseCellOf(bounds), element);
        return element;
    }

    /// <inheritdoc />
    public void Remove(int element)
    {
        Unlink(LooseCellOf(_elements[element].Bounds), element);
        _elements.Erase(element);
    }

    /// <inheritdoc />
    public void Move(int element, Aabb bounds)
    {
        int from = LooseCellOf(_elements[element].Bounds);
        int to = LooseCellOf(bounds);
        _elements[element].Bounds = bounds;
        if (from == to)
        {
            Grow(to, bounds);
            return;
        }

        Unlink(from, element);
        Link(to, element);
    }

    /// <inheritdoc />
    public int GetId(int element) => _elements[element].Id;

    /// <inheritdoc />
    public Aabb GetBounds(int element) => _elements[element].Bounds;

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (area.IsEmpty)
        {
            return;
        }

        // A loose cell can be listed in several tight cells; the stamp makes sure each is searched once.
        if (++_queryStamp == int.MaxValue)
        {
            Array.Clear(_queryStamps);
            _queryStamp = 1;
        }

        GridCells.Range range = TightRangeOf(area);
        for (int ty = range.Y1; ty <= range.Y2; ty++)
        {
            for (int tx = range.X1; tx <= range.X2; tx++)
            {
                for (int node = _tightHeads[ty * TightColumns + tx]; node != -1; node = _looseCellNodes[node].Next)
                {
                    int cell = _looseCellNodes[node].Cell;
                    if (_queryStamps[cell] == _queryStamp || !_looseCells[cell].Bounds.Intersects(area))
                    {
                        continue;
                    }

                    _queryStamps[cell] = _queryStamp;
                    for (int element = _looseCells[cell].Head; element != -1; element = _elements[element].Next)
                    {
                        if (element != omitElement && _elements[element].Bounds.Intersects(area))
                        {
                            results.Add(element);
                        }
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    public void Cleanup()
    {
        Array.Fill(_tightHeads, -1);
        _looseCellNodes.Clear();
        for (int cell = 0; cell < _looseCells.Length; cell++)
        {
            Aabb bounds = Aabb.Empty;
            for (int element = _looseCells[cell].Head; element != -1; element = _elements[element].Next)
            {
                bounds = bounds.Union(_elements[element].Bounds);
            }

            _looseCells[cell].Bounds = bounds;
            _looseCells[cell].Registered = GridCells.Range.None;
            Grow(cell, bounds);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        _elements.Clear();
        _looseCellNodes.Clear();
        Array.Fill(_looseCells, new LooseCell(-1, 0, Aabb.Empty, GridCells.Range.None));
        Array.Fill(_tightHeads, -1);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Reports the tight cells first, at depth 0 and not as leaves, with how many loose cells each lists; then each non-empty
    /// loose cell's box, at depth 1, as a leaf with its element count.
    /// </remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        float tightWidth = Bounds.Width / TightColumns, tightHeight = Bounds.Height / TightRows;
        for (int ty = 0; ty < TightRows; ty++)
        {
            for (int tx = 0; tx < TightColumns; tx++)
            {
                int count = 0;
                for (int node = _tightHeads[ty * TightColumns + tx]; node != -1; node = _looseCellNodes[node].Next)
                {
                    count++;
                }

                float left = Bounds.Left + tx * tightWidth, top = Bounds.Top + ty * tightHeight;
                visitor(new Aabb(left, top, left + tightWidth, top + tightHeight), 0, false, count);
            }
        }

        foreach (LooseCell cell in _looseCells)
        {
            if (cell.Count > 0)
            {
                visitor(cell.Bounds, 1, true, cell.Count);
            }
        }
    }

    private void Link(int cell, int element)
    {
        _elements[element].Next = _looseCells[cell].Head;
        _looseCells[cell].Head = element;
        _looseCells[cell].Count++;
        Grow(cell, _elements[element].Bounds);
    }

    private void Unlink(int cell, int element)
    {
        int previous = -1;
        int current = _looseCells[cell].Head;
        while (current != -1 && current != element)
        {
            previous = current;
            current = _elements[current].Next;
        }

        if (current == -1)
        {
            return;
        }

        if (previous == -1)
        {
            _looseCells[cell].Head = _elements[current].Next;
        }
        else
        {
            _elements[previous].Next = _elements[current].Next;
        }

        _looseCells[cell].Count--;
    }

    // Grows a loose cell's box to take in a box, and lists the cell in any tight cells its box now reaches. Between cleanups
    // boxes only grow, so the tight cells it's already listed in are a subset of the new ones.
    private void Grow(int cell, Aabb bounds)
    {
        ref LooseCell looseCell = ref _looseCells[cell];
        looseCell.Bounds = looseCell.Bounds.Union(bounds);
        GridCells.Range range = TightRangeOf(looseCell.Bounds);
        if (range == looseCell.Registered)
        {
            return;
        }

        for (int ty = range.Y1; ty <= range.Y2; ty++)
        {
            for (int tx = range.X1; tx <= range.X2; tx++)
            {
                if (looseCell.Registered.Contains(tx, ty))
                {
                    continue;
                }

                int tight = ty * TightColumns + tx;
                _tightHeads[tight] = _looseCellNodes.Insert(new LooseCellNode(_tightHeads[tight], cell));
            }
        }

        looseCell.Registered = range;
    }

    private int LooseCellOf(Aabb bounds) =>
        GridCells.Index(bounds.CenterY - Bounds.Top, _looseInverseHeight, LooseRows) * LooseColumns
        + GridCells.Index(bounds.CenterX - Bounds.Left, _looseInverseWidth, LooseColumns);

    // The tight cells a box overlaps; none for an empty box.
    private GridCells.Range TightRangeOf(Aabb bounds) =>
        bounds.IsEmpty
            ? GridCells.Range.None
            : new GridCells.Range(
                GridCells.Index(bounds.Left - Bounds.Left, _tightInverseWidth, TightColumns),
                GridCells.Index(bounds.Top - Bounds.Top, _tightInverseHeight, TightRows),
                GridCells.Index(bounds.Right - Bounds.Left, _tightInverseWidth, TightColumns),
                GridCells.Index(bounds.Bottom - Bounds.Top, _tightInverseHeight, TightRows));

    /// <summary>A loose cell: its element list, its box, and the tight cells it's listed in.</summary>
    private record struct LooseCell(int Head, int Count, Aabb Bounds, GridCells.Range Registered);

    /// <summary>An element, linked into its one loose cell's list through Next.</summary>
    private record struct Element(int Next, int Id, Aabb Bounds);

    /// <summary>One entry in a tight cell's singly linked list of loose cells.</summary>
    private record struct LooseCellNode(int Next, int Cell);
}
