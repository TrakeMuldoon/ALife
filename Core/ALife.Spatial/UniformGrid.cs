namespace ALife.Spatial;

/// <summary>
/// A fixed grid for elements that share one upper-bound size, after the "Dirty Trick: Uniform Sizes" answer by user4842163
/// on Stack Overflow. Every element is treated as a box of <see cref="HalfWidth"/> by <see cref="HalfHeight"/> around its
/// center, so only the center is stored and each element sits in exactly one cell; queries widen the area by the half size
/// to find centers that matter.
/// </summary>
/// <remarks>
/// Use it for the many common-sized things in a scene, with the half size set to the largest of them, and keep the rare big
/// ones in a <see cref="LooseQuadTree"/> or <see cref="LooseTightGrid"/>. Results are coarse: they're the elements whose
/// uniform boxes intersect the area, for a finer test of your own.
/// </remarks>
public sealed class UniformGrid : ISpatialIndex
{
    private readonly FreeList<Element> _elements = new();
    private readonly int[] _heads;
    private readonly int[] _counts;
    private readonly float _inverseCellWidth, _inverseCellHeight;

    /// <summary>Creates an empty grid.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed in its edge cells.</param>
    /// <param name="columns">The number of cell columns. Cells work best a few times bigger than the elements.</param>
    /// <param name="rows">The number of cell rows.</param>
    /// <param name="halfWidth">Half the width every element is treated as having.</param>
    /// <param name="halfHeight">Half the height every element is treated as having.</param>
    public UniformGrid(Aabb bounds, int columns, int rows, float halfWidth, float halfHeight)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columns, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(halfWidth);
        ArgumentOutOfRangeException.ThrowIfNegative(halfHeight);
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentException("The bounds must have an area.", nameof(bounds));
        }

        Bounds = bounds;
        Columns = columns;
        Rows = rows;
        HalfWidth = halfWidth;
        HalfHeight = halfHeight;
        _inverseCellWidth = columns / bounds.Width;
        _inverseCellHeight = rows / bounds.Height;
        _heads = new int[columns * rows];
        _counts = new int[columns * rows];
        Clear();
    }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <summary>The number of cell columns.</summary>
    public int Columns { get; }

    /// <summary>The number of cell rows.</summary>
    public int Rows { get; }

    /// <summary>Half the width every element is treated as having.</summary>
    public float HalfWidth { get; }

    /// <summary>Half the height every element is treated as having.</summary>
    public float HalfHeight { get; }

    /// <inheritdoc />
    public int Count => _elements.Count;

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bounds"/> is bigger than the uniform size.</exception>
    public int Insert(int id, Aabb bounds)
    {
        CheckSize(bounds);
        int element = _elements.Insert(new Element(-1, id, bounds.CenterX, bounds.CenterY));
        Link(CellOf(bounds.CenterX, bounds.CenterY), element);
        return element;
    }

    /// <inheritdoc />
    public void Remove(int element)
    {
        Element e = _elements[element];
        Unlink(CellOf(e.X, e.Y), element);
        _elements.Erase(element);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bounds"/> is bigger than the uniform size.</exception>
    public void Move(int element, Aabb bounds)
    {
        CheckSize(bounds);
        Element e = _elements[element];
        float x = bounds.CenterX, y = bounds.CenterY;
        int from = CellOf(e.X, e.Y), to = CellOf(x, y);
        _elements[element].X = x;
        _elements[element].Y = y;
        if (from != to)
        {
            Unlink(from, element);
            Link(to, element);
        }
    }

    /// <inheritdoc />
    public int GetId(int element) => _elements[element].Id;

    /// <inheritdoc />
    /// <remarks>Returns the uniform-size box around the element's center, not the box it was inserted with.</remarks>
    public Aabb GetBounds(int element)
    {
        Element e = _elements[element];
        return Aabb.FromCenter(e.X, e.Y, HalfWidth, HalfHeight);
    }

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (area.IsEmpty)
        {
            return;
        }

        // Any element whose box reaches the area has its center within the area grown by the half size.
        Aabb centers = area.Inflate(HalfWidth, HalfHeight);
        int x1 = GridCells.Index(centers.Left - Bounds.Left, _inverseCellWidth, Columns);
        int x2 = GridCells.Index(centers.Right - Bounds.Left, _inverseCellWidth, Columns);
        int y1 = GridCells.Index(centers.Top - Bounds.Top, _inverseCellHeight, Rows);
        int y2 = GridCells.Index(centers.Bottom - Bounds.Top, _inverseCellHeight, Rows);
        for (int y = y1; y <= y2; y++)
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int element = _heads[y * Columns + x]; element != -1; element = _elements[element].Next)
                {
                    if (element != omitElement && centers.Contains(_elements[element].X, _elements[element].Y))
                    {
                        results.Add(element);
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>Does nothing: the grid never changes shape.</remarks>
    public void Cleanup()
    {
    }

    /// <inheritdoc />
    public void Clear()
    {
        _elements.Clear();
        Array.Fill(_heads, -1);
        Array.Clear(_counts);
    }

    /// <inheritdoc />
    /// <remarks>Reports every cell, at depth 0, as a leaf with its element count.</remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        float cellWidth = Bounds.Width / Columns, cellHeight = Bounds.Height / Rows;
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                float left = Bounds.Left + x * cellWidth, top = Bounds.Top + y * cellHeight;
                visitor(new Aabb(left, top, left + cellWidth, top + cellHeight), 0, true, _counts[y * Columns + x]);
            }
        }
    }

    private void Link(int cell, int element)
    {
        _elements[element].Next = _heads[cell];
        _heads[cell] = element;
        _counts[cell]++;
    }

    private void Unlink(int cell, int element)
    {
        int previous = -1;
        int current = _heads[cell];
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
            _heads[cell] = _elements[current].Next;
        }
        else
        {
            _elements[previous].Next = _elements[current].Next;
        }

        _counts[cell]--;
    }

    private void CheckSize(Aabb bounds)
    {
        // A little slack for rounding in boxes built from a center and the same half size.
        const float Slack = 1.0001f;
        if (bounds.Width > HalfWidth * 2 * Slack + 1e-4f || bounds.Height > HalfHeight * 2 * Slack + 1e-4f)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds), bounds,
                $"The box is bigger than the grid's uniform size of {HalfWidth * 2} by {HalfHeight * 2}.");
        }
    }

    private int CellOf(float x, float y) =>
        GridCells.Index(y - Bounds.Top, _inverseCellHeight, Rows) * Columns + GridCells.Index(x - Bounds.Left, _inverseCellWidth, Columns);

    /// <summary>An element: its center, linked into its one cell's list through Next.</summary>
    private record struct Element(int Next, int Id, float X, float Y);
}
