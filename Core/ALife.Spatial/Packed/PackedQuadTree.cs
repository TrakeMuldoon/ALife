namespace ALife.Spatial.Packed;

/// <summary>
/// A loose quadtree rebuilt every frame: the elements are radix-sorted by the Morton (Z-order) code of their centers, which
/// lays every node's elements out as one unbroken run, and the nodes are then cut from that order top-down. Each node keeps a
/// box around its elements, as in <see cref="LooseQuadTree"/>, so it copes with sizes that vary wildly, big elements
/// included.
/// </summary>
/// <remarks>
/// Building takes a few milliseconds for hundreds of thousands of elements, and the tree is never out of shape, since it's
/// new each frame. Searches walk the tree, so they're slower than a <see cref="PackedGrid"/>'s when elements are all of a
/// size.
/// </remarks>
public sealed class PackedQuadTree : IPackedSpatialIndex
{
    /// <summary>The deepest a tree can go: centers are placed on a grid of 2^16 by 2^16 to sort them.</summary>
    public const int MaxSupportedDepth = 16;

    // Elements per parallel work item when finding neighbors.
    private const int NeighborBatch = 512;

    private readonly PackedBoxes _sorted = new();
    private readonly ValueList<Node> _nodes = new();
    private readonly ValueList<(int Index, int Depth)> _visitStack = new();
    private readonly int[] _histogram = new int[256];
    private Aabb[] _boxes = [];
    private uint[] _codes = [], _spareCodes = [];
    private int[] _order = [], _spareOrder = [];
    private int _sortedCount;

    /// <summary>Creates an empty tree.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed at its edges.</param>
    /// <param name="maxElements">How many elements a leaf holds before it splits, unless it's at <paramref name="maxDepth"/>.</param>
    /// <param name="maxDepth">How deep the tree can go, from 0 (just the root) to <see cref="MaxSupportedDepth"/>.</param>
    public PackedQuadTree(Aabb bounds, int maxElements = 8, int maxDepth = 8)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxElements, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(maxDepth);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxDepth, MaxSupportedDepth);
        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentException("The bounds must have an area.", nameof(bounds));
        }

        Bounds = bounds;
        MaxElements = maxElements;
        MaxDepth = maxDepth;
        _nodes.Add(new Node(Aabb.Empty, -1, 0, 0));
    }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <summary>How many elements a leaf holds before it splits.</summary>
    public int MaxElements { get; }

    /// <summary>How deep the tree can go.</summary>
    public int MaxDepth { get; }

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <inheritdoc />
    public void Build(ReadOnlySpan<Aabb> boxes)
    {
        int count = boxes.Length;
        if (count > _boxes.Length)
        {
            int capacity = Math.Max(count, _boxes.Length * 2);
            _boxes = new Aabb[capacity];
            _codes = new uint[capacity];
            _spareCodes = new uint[capacity];
            _order = new int[capacity];
            _spareOrder = new int[capacity];
        }

        boxes.CopyTo(_boxes);
        Count = count;

        // Empty boxes are left out: they'd only stretch the nodes they landed in.
        float scaleX = 65536 / Bounds.Width, scaleY = 65536 / Bounds.Height;
        _sortedCount = 0;
        for (int i = 0; i < count; i++)
        {
            Aabb box = _boxes[i];
            if (!box.IsEmpty)
            {
                _codes[_sortedCount] = Interleave(Quantize((box.CenterX - Bounds.Left) * scaleX)) | (Interleave(Quantize((box.CenterY - Bounds.Top) * scaleY)) << 1);
                _order[_sortedCount++] = i;
            }
        }

        if (_sortedCount > 1)
        {
            RadixSort();
        }

        _sorted.EnsureCapacity(_sortedCount);
        for (int p = 0; p < _sortedCount; p++)
        {
            _sorted.Set(p, _order[p], _boxes[_order[p]]);
        }

        _nodes.Clear();
        _nodes.Add(default);
        BuildNode(0, 0, _sortedCount, 0);
    }

    /// <inheritdoc />
    public Aabb GetBounds(int element) => _boxes[element];

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        Span<int> stack = stackalloc int[4 * (MaxSupportedDepth + 1)];
        int depth = 0;
        stack[depth++] = 0;
        while (depth > 0)
        {
            Node node = _nodes[stack[--depth]];
            if (!node.Bounds.Intersects(area))
            {
                continue;
            }

            if (node.FirstChild == -1)
            {
                _sorted.AddRun(results, area, node.Start, node.End, omitElement);
                continue;
            }

            for (int j = 0; j < 4; j++)
            {
                stack[depth++] = node.FirstChild + j;
            }
        }
    }

    /// <inheritdoc />
    public void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor
    {
        for (int p = 0; p < _sortedCount; p++)
        {
            VisitOverlaps(ref visitor, p, onlyLater: true);
        }
    }

    /// <inheritdoc />
    public void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor
    {
        int batches = (_sortedCount + NeighborBatch - 1) / NeighborBatch;
        Parallel.For(0, batches, batch =>
        {
            TVisitor local = visitor;
            int end = Math.Min((batch + 1) * NeighborBatch, _sortedCount);
            for (int p = batch * NeighborBatch; p < end; p++)
            {
                VisitOverlaps(ref local, p, onlyLater: false);
            }
        });
    }

    /// <summary>Calls the visitor with (<paramref name="element"/>, e) for each element e whose box intersects <paramref name="area"/>.</summary>
    internal void VisitArea<TVisitor>(ref TVisitor visitor, int element, Aabb area) where TVisitor : IPairVisitor =>
        VisitOverlaps(ref visitor, element, area, -1, onlyLater: false);

    /// <inheritdoc />
    /// <remarks>Reports each node's loose box, which is <see cref="Aabb.Empty"/> for an empty leaf.</remarks>
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        _visitStack.Clear();
        _visitStack.Add((0, 0));
        while (_visitStack.Count > 0)
        {
            (int index, int depth) = _visitStack.Pop();
            Node node = _nodes[index];
            if (node.FirstChild != -1)
            {
                for (int j = 3; j >= 0; j--)
                {
                    _visitStack.Add((node.FirstChild + j, depth + 1));
                }
            }

            visitor(node.Bounds, depth, node.FirstChild == -1, node.FirstChild == -1 ? node.End - node.Start : 0);
        }
    }

    // Visits (element at p, e) for each other element e whose box intersects it.
    private void VisitOverlaps<TVisitor>(ref TVisitor visitor, int p, bool onlyLater) where TVisitor : IPairVisitor =>
        VisitOverlaps(ref visitor, _sorted.ElementAt(p), _sorted.BoxAt(p), p, onlyLater);

    // Visits (element, e) for each element e whose box intersects the box, other than the one at position p; with onlyLater,
    // just those sorted after p, which skips whole subtrees that end before it.
    private void VisitOverlaps<TVisitor>(ref TVisitor visitor, int element, Aabb box, int p, bool onlyLater) where TVisitor : IPairVisitor
    {
        Span<int> stack = stackalloc int[4 * (MaxSupportedDepth + 1)];
        int depth = 0;
        stack[depth++] = 0;
        while (depth > 0)
        {
            ref Node node = ref _nodes[stack[--depth]];
            if ((onlyLater && node.End <= p + 1) || !node.Bounds.Intersects(box))
            {
                continue;
            }

            if (node.FirstChild == -1)
            {
                _sorted.VisitRun(ref visitor, element, box, onlyLater ? Math.Max(node.Start, p + 1) : node.Start, node.End, p);
                continue;
            }

            for (int j = 0; j < 4; j++)
            {
                stack[depth++] = node.FirstChild + j;
            }
        }
    }

    // Fills in a node over sorted positions start to end at a depth, splitting it while it's too full, and returns its box.
    private Aabb BuildNode(int index, int start, int end, int depth)
    {
        if (end - start <= MaxElements || depth >= MaxDepth)
        {
            Aabb bounds = Aabb.Empty;
            for (int p = start; p < end; p++)
            {
                bounds = bounds.Union(_sorted.BoxAt(p));
            }

            _nodes[index] = new Node(bounds, -1, start, end);
            return bounds;
        }

        int firstChild = _nodes.Count;
        for (int j = 0; j < 4; j++)
        {
            _nodes.Add(default);
        }

        // The node's elements share the code's top 2 * depth bits; the next two pick the child, in sorted order.
        int shift = 30 - 2 * depth;
        uint prefix = depth == 0 ? 0 : _codes[start] & (~0u << (shift + 2));
        Aabb union = Aabb.Empty;
        int childStart = start;
        for (int j = 0; j < 4; j++)
        {
            int childEnd = j == 3 ? end : LowerBound(childStart, end, prefix | ((uint)(j + 1) << shift));
            union = union.Union(BuildNode(firstChild + j, childStart, childEnd, depth + 1));
            childStart = childEnd;
        }

        _nodes[index] = new Node(union, firstChild, start, end);
        return union;
    }

    // The first sorted position from start to end whose code is at least the key.
    private int LowerBound(int start, int end, uint key)
    {
        while (start < end)
        {
            int middle = (start + end) >>> 1;
            if (_codes[middle] < key)
            {
                start = middle + 1;
            }
            else
            {
                end = middle;
            }
        }

        return start;
    }

    // A least-significant-digit radix sort of the codes, a byte at a time, carrying the element order along; a byte every
    // code shares is skipped.
    private void RadixSort()
    {
        for (int shift = 0; shift < 32; shift += 8)
        {
            Array.Clear(_histogram);
            for (int p = 0; p < _sortedCount; p++)
            {
                _histogram[(_codes[p] >> shift) & 0xFF]++;
            }

            if (_histogram[(_codes[0] >> shift) & 0xFF] == _sortedCount)
            {
                continue;
            }

            for (int digit = 0, total = 0; digit < 256; digit++)
            {
                int digitCount = _histogram[digit];
                _histogram[digit] = total;
                total += digitCount;
            }

            for (int p = 0; p < _sortedCount; p++)
            {
                int target = _histogram[(_codes[p] >> shift) & 0xFF]++;
                _spareCodes[target] = _codes[p];
                _spareOrder[target] = _order[p];
            }

            (_codes, _spareCodes) = (_spareCodes, _codes);
            (_order, _spareOrder) = (_spareOrder, _order);
        }
    }

    // A coordinate already scaled to 0..65536, clamped to 16 bits; NaN goes to 0.
    private static uint Quantize(float scaled) => scaled >= 0 ? scaled < 65535 ? (uint)scaled : 65535 : 0;

    // Spreads 16 bits out to the even bits of 32.
    private static uint Interleave(uint value)
    {
        value = (value | (value << 8)) & 0x00FF00FF;
        value = (value | (value << 4)) & 0x0F0F0F0F;
        value = (value | (value << 2)) & 0x33333333;
        return (value | (value << 1)) & 0x55555555;
    }

    /// <summary>
    /// A node: its loose box, its first of four children (-1 for a leaf), and the run of sorted positions its elements are
    /// in, which a branch has too, so pair searches can skip whole branches.
    /// </summary>
    private record struct Node(Aabb Bounds, int FirstChild, int Start, int End);
}
