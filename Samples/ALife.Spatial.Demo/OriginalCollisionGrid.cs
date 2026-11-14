namespace ALife.Spatial.Demo;

/// <summary>
/// ALife's original <c>CollisionGrid&lt;T&gt;</c>, which <c>SpatialCollisionMap</c> replaced, rebuilt behind
/// <see cref="ISpatialIndex"/> so the demo can show it beside the others. It works as the original did, costs included:
/// <list type="bullet">
/// <item>The world is cut into square buckets, 25 units across by default, each with a list of what's in it. An element goes
/// in every bucket its box covers, and a list of those buckets is made for it each time it's added.</item>
/// <item>Removing an element searches each of its buckets' lists, and the list of every element, for it. Moving one removes
/// and adds it again.</item>
/// <item>There are <c>ceil((size + 1) / bucket)</c> buckets across and down, and when the world doesn't divide evenly the last
/// column and row share the lists of the ones before them.</item>
/// <item>Buckets are found by truncating, so anything from a bucket's width left of or above the world falls in the first.
/// Buckets outside the world are skipped, so an element wholly outside it is never found.</item>
/// <item>A query gathers its buckets' elements into a set and keeps those whose boxes overlap the area. Boxes that only touch
/// don't count.</item>
/// </list>
/// </summary>
internal sealed class OriginalCollisionGrid : ISpatialIndex
{
    private readonly List<int>[,] _buckets;
    private readonly int _columns;
    private readonly int _rows;
    private readonly List<int> _tracked = [];
    private readonly HashSet<int> _potential = [];
    private readonly Stack<int> _free = new();
    private Element[] _elements = new Element[16];
    private int _handles;

    /// <summary>Sets up a grid over a world with its corner at the origin, as the original's always was.</summary>
    /// <param name="width">The world's width.</param>
    /// <param name="height">The world's height.</param>
    /// <param name="bucketSize">How wide each bucket is. Like the original, it's cut down to the world's width or height if either is smaller.</param>
    public OriginalCollisionGrid(int width, int height, int bucketSize = 25)
    {
        BucketSize = Math.Min(bucketSize, Math.Min(width, height));
        Bounds = new Aabb(0, 0, width, height);

        double across = (width + 1) / (double)BucketSize;
        double down = (height + 1) / (double)BucketSize;
        _columns = (int)Math.Ceiling(across);
        _rows = (int)Math.Ceiling(down);
        int fullColumns = (int)Math.Floor(across);
        int fullRows = (int)Math.Floor(down);

        _buckets = new List<int>[_columns, _rows];
        for (int x = 0; x < fullColumns; x++)
        {
            for (int y = 0; y < fullRows; y++)
            {
                _buckets[x, y] = [];
            }
        }

        if (_columns != fullColumns)
        {
            for (int y = 0; y < fullRows; y++)
            {
                _buckets[fullColumns, y] = _buckets[fullColumns - 1, y];
            }
        }

        if (_rows != fullRows)
        {
            for (int x = 0; x < _columns; x++)
            {
                _buckets[x, fullRows] = _buckets[x, fullRows - 1];
            }
        }
    }

    /// <summary>How wide each bucket is.</summary>
    public int BucketSize { get; }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <inheritdoc />
    public int Count => _tracked.Count;

    /// <inheritdoc />
    public int Insert(int id, Aabb bounds)
    {
        int handle;
        if (_free.Count > 0)
        {
            handle = _free.Pop();
        }
        else
        {
            if (_handles == _elements.Length)
            {
                Array.Resize(ref _elements, _elements.Length * 2);
            }

            handle = _handles++;
        }

        _elements[handle].Id = id;
        Add(handle, bounds);
        return handle;
    }

    /// <inheritdoc />
    public void Remove(int element)
    {
        Take(element);
        _free.Push(element);
    }

    /// <inheritdoc />
    public void Move(int element, Aabb bounds)
    {
        Take(element);
        Add(element, bounds);
    }

    /// <inheritdoc />
    public int GetId(int element) => _elements[element].Id;

    /// <inheritdoc />
    public Aabb GetBounds(int element) => _elements[element].Bounds;

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        int left = Math.Clamp(Bucket(area.Left), 0, _columns - 1);
        int right = Math.Clamp(Bucket(area.Right), 0, _columns - 1);
        int top = Math.Clamp(Bucket(area.Top), 0, _rows - 1);
        int bottom = Math.Clamp(Bucket(area.Bottom), 0, _rows - 1);

        _potential.Clear();
        for (int x = left; x <= right; x++)
        {
            for (int y = top; y <= bottom; y++)
            {
                foreach (int element in _buckets[x, y])
                {
                    _potential.Add(element);
                }
            }
        }

        foreach (int element in _potential)
        {
            Aabb box = _elements[element].Bounds;
            if (element != omitElement && box.Left < area.Right && box.Right > area.Left && box.Top < area.Bottom && box.Bottom > area.Top)
            {
                results.Add(element);
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>The original had nothing to tidy up, so this does nothing.</remarks>
    public void Cleanup()
    {
    }

    /// <inheritdoc />
    public void Clear()
    {
        foreach (List<int> bucket in _buckets)
        {
            bucket.Clear();
        }

        _tracked.Clear();
        _free.Clear();
        _handles = 0;
    }

    /// <inheritdoc />
    /// <remarks>Each bucket is a leaf at depth 0. The last column and row share their lists, so they show the same counts as the ones before them.</remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                Aabb cell = new(x * BucketSize, y * BucketSize, (x + 1) * BucketSize, (y + 1) * BucketSize);
                visitor(cell, 0, true, _buckets[x, y].Count);
            }
        }
    }

    // As the original's divide: truncating, so negatives round towards zero.
    private int Bucket(float coordinate) => (int)coordinate / BucketSize;

    // Files an element in every bucket its box covers that's in the world, remembering all of them, in the world or not.
    private void Add(int element, Aabb bounds)
    {
        int left = Bucket(bounds.Left), right = Bucket(bounds.Right);
        int top = Bucket(bounds.Top), bottom = Bucket(bounds.Bottom);
        List<(int X, int Y)> covered = [];
        for (int x = left; x <= right; x++)
        {
            for (int y = top; y <= bottom; y++)
            {
                covered.Add((x, y));
            }
        }

        foreach ((int x, int y) in covered)
        {
            if (InGrid(x, y))
            {
                _buckets[x, y].Add(element);
            }
        }

        ref Element entry = ref _elements[element];
        entry.Bounds = bounds;
        entry.Covered = covered;
        _tracked.Add(element);
    }

    private void Take(int element)
    {
        _tracked.Remove(element);
        foreach ((int x, int y) in _elements[element].Covered)
        {
            if (InGrid(x, y))
            {
                _buckets[x, y].Remove(element);
            }
        }

        _elements[element].Covered = [];
    }

    private bool InGrid(int x, int y) => x >= 0 && y >= 0 && x < _columns && y < _rows;

    private struct Element
    {
        public int Id;
        public Aabb Bounds;
        public List<(int X, int Y)> Covered;
    }
}
