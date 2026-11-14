namespace ALife.Spatial;

/// <summary>
/// A quadtree for many moving elements, after user4842163's "Efficient Quadtrees" answers on Stack Overflow. An element is
/// filed in every leaf its box overlaps, so it suits elements of similar, smallish sizes; for wildly varying sizes use
/// <see cref="LooseQuadTree"/> or <see cref="LooseTightGrid"/>.
/// </summary>
/// <remarks>
/// <para>
/// Nodes are 8 bytes (first child or first element, and an element count that is -1 for branches), the four children of a
/// branch are contiguous, and node boxes aren't stored but worked out while descending. Elements, the per-leaf element lists
/// and groups of four nodes all come from free lists, so a running simulation stops allocating once it reaches its peak size.
/// Coordinates are whole numbers: boxes are rounded down to integers, so queries can return elements up to a unit away.
/// </para>
/// <para>Removing elements doesn't free nodes; <see cref="Cleanup"/> does, one level per call.</para>
/// </remarks>
public sealed class QuadTree : ISpatialIndex
{
    // The largest magnitude a coordinate is clamped to before rounding it to an int.
    private const float CoordinateLimit = 1 << 28;

    private readonly FreeList<Element> _elements = new();
    private readonly FreeList<ElementNode> _elementNodes = new();
    private readonly ValueList<Node> _nodes = new();
    private readonly ValueList<NodeData> _toProcess = new();
    private readonly ValueList<NodeData> _leaves = new();
    private readonly ValueList<NodeData> _visitStack = new();
    private readonly ValueList<int> _cleanupStack = new();
    private readonly NodeData _root;

    // The first of a group of four freed nodes, each group linking to the next through its first node's FirstChild, or -1.
    private int _freeNode = -1;

    // Marks elements already added to a query's results; indexed by element handle and always left all false.
    private bool[] _seen = new bool[16];

    /// <summary>Creates an empty quadtree.</summary>
    /// <param name="bounds">The area to cover. It's rounded out to whole units and an even size.</param>
    /// <param name="maxElements">How many elements a leaf holds before it splits, unless it's at <paramref name="maxDepth"/>.</param>
    /// <param name="maxDepth">How deep the tree can go; the root is depth 0. Aim for the smallest leaves to be a bit bigger than an average element.</param>
    public QuadTree(Aabb bounds, int maxElements = 8, int maxDepth = 8)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxElements, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(maxDepth);
        if (bounds.IsEmpty)
        {
            throw new ArgumentException("The bounds can't be empty.", nameof(bounds));
        }

        int left = Floor(bounds.Left), top = Floor(bounds.Top);
        int halfWidth = Math.Max(1, (Ceiling(bounds.Right) - left + 1) / 2);
        int halfHeight = Math.Max(1, (Ceiling(bounds.Bottom) - top + 1) / 2);
        _root = new NodeData(0, 0, left + halfWidth, top + halfHeight, halfWidth, halfHeight);
        Bounds = ToAabb(_root);
        MaxElements = maxElements;
        MaxDepth = maxDepth;
        _nodes.Add(new Node(-1, 0));
    }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <summary>How many elements a leaf holds before it splits.</summary>
    public int MaxElements { get; }

    /// <summary>
    /// How deep the tree can go. Coordinates are whole numbers, so a node also stops splitting once its children would be
    /// less than two units across, however deep it is.
    /// </summary>
    public int MaxDepth { get; }

    /// <inheritdoc />
    public int Count => _elements.Count;

    /// <inheritdoc />
    public int Insert(int id, Aabb bounds)
    {
        int element = _elements.Insert(ToElement(id, bounds));
        LinkToLeaves(element);
        return element;
    }

    /// <inheritdoc />
    public void Remove(int element)
    {
        UnlinkFromLeaves(element);
        _elements.Erase(element);
    }

    /// <inheritdoc />
    public void Move(int element, Aabb bounds)
    {
        Element moved = ToElement(_elements[element].Id, bounds);
        if (moved == _elements[element])
        {
            return;
        }

        UnlinkFromLeaves(element);
        _elements[element] = moved;
        LinkToLeaves(element);
    }

    /// <inheritdoc />
    public int GetId(int element) => _elements[element].Id;

    /// <inheritdoc />
    public Aabb GetBounds(int element)
    {
        Element e = _elements[element];
        return new Aabb(e.Left, e.Top, e.Right, e.Bottom);
    }

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        if (area.IsEmpty)
        {
            return;
        }

        int left = Floor(area.Left), top = Floor(area.Top), right = Floor(area.Right), bottom = Floor(area.Bottom);
        FindLeaves(left, top, right, bottom);
        if (_seen.Length < _elements.Range)
        {
            _seen = new bool[Math.Max(_elements.Range, _seen.Length * 2)];
        }

        int start = results.Count;
        for (int i = 0; i < _leaves.Count; i++)
        {
            for (int node = _nodes[_leaves[i].Index].FirstChild; node != -1; node = _elementNodes[node].Next)
            {
                int element = _elementNodes[node].Element;
                if (_seen[element] || element == omitElement)
                {
                    continue;
                }

                Element e = _elements[element];
                if (e.Left <= right && left <= e.Right && e.Top <= bottom && top <= e.Bottom)
                {
                    _seen[element] = true;
                    results.Add(element);
                }
            }
        }

        for (int i = start; i < results.Count; i++)
        {
            _seen[results[i]] = false;
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// A branch whose four children are all empty leaves becomes an empty leaf itself. That leaf is only freed by the next
    /// call, so an area emptied for a moment, such as when one element leaves a node and another enters it in the same
    /// frame, doesn't throw nodes away just to make them again, and the work is spread over frames.
    /// </remarks>
    public void Cleanup()
    {
        _cleanupStack.Clear();
        if (_nodes[0].Count == -1)
        {
            _cleanupStack.Add(0);
        }

        while (_cleanupStack.Count > 0)
        {
            int index = _cleanupStack.Pop();
            int firstChild = _nodes[index].FirstChild;
            int emptyLeaves = 0;
            for (int j = 0; j < 4; j++)
            {
                int count = _nodes[firstChild + j].Count;
                if (count == 0)
                {
                    emptyLeaves++;
                }
                else if (count == -1)
                {
                    _cleanupStack.Add(firstChild + j);
                }
            }

            if (emptyLeaves == 4)
            {
                _nodes[firstChild].FirstChild = _freeNode;
                _freeNode = firstChild;
                _nodes[index] = new Node(-1, 0);
            }
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        _elements.Clear();
        _elementNodes.Clear();
        _nodes.Clear();
        _nodes.Add(new Node(-1, 0));
        _freeNode = -1;
    }

    /// <inheritdoc />
    public void VisitNodes(SpatialNodeVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        _visitStack.Clear();
        _visitStack.Add(_root);
        while (_visitStack.Count > 0)
        {
            NodeData data = _visitStack.Pop();
            Node node = _nodes[data.Index];
            if (node.Count == -1)
            {
                for (int quadrant = 3; quadrant >= 0; quadrant--)
                {
                    _visitStack.Add(Child(data, node.FirstChild, quadrant));
                }
            }

            visitor(ToAabb(data), data.Depth, node.Count != -1, Math.Max(node.Count, 0));
        }
    }

    private void LinkToLeaves(int element)
    {
        Element e = _elements[element];
        FindLeaves(e.Left, e.Top, e.Right, e.Bottom);

        // Splitting a leaf below doesn't touch _leaves, and the other leaves in it are elsewhere in the tree.
        for (int i = 0; i < _leaves.Count; i++)
        {
            InsertIntoLeaf(_leaves[i], _elementNodes.Insert(new ElementNode(-1, element)), element);
        }
    }

    private void UnlinkFromLeaves(int element)
    {
        Element e = _elements[element];
        FindLeaves(e.Left, e.Top, e.Right, e.Bottom);
        for (int i = 0; i < _leaves.Count; i++)
        {
            int leaf = _leaves[i].Index;
            int previous = -1;
            int node = _nodes[leaf].FirstChild;
            while (node != -1 && _elementNodes[node].Element != element)
            {
                previous = node;
                node = _elementNodes[node].Next;
            }

            if (node == -1)
            {
                continue;
            }

            int next = _elementNodes[node].Next;
            if (previous == -1)
            {
                _nodes[leaf].FirstChild = next;
            }
            else
            {
                _elementNodes[previous].Next = next;
            }

            _elementNodes.Erase(node);
            _nodes[leaf].Count--;
        }
    }

    // Collects the leaves a box overlaps into _leaves, descending without recursion.
    private void FindLeaves(int left, int top, int right, int bottom)
    {
        _leaves.Clear();
        _toProcess.Clear();
        _toProcess.Add(_root);
        while (_toProcess.Count > 0)
        {
            NodeData data = _toProcess.Pop();
            Node node = _nodes[data.Index];
            if (node.Count != -1)
            {
                _leaves.Add(data);
                continue;
            }

            int quadrants = Quadrants(data, left, top, right, bottom);
            for (int quadrant = 0; quadrant < 4; quadrant++)
            {
                if ((quadrants & (1 << quadrant)) != 0)
                {
                    _toProcess.Add(Child(data, node.FirstChild, quadrant));
                }
            }
        }
    }

    // Puts an element node at the head of a node's list, descending first if an earlier element split the node.
    private void InsertIntoLeaf(NodeData data, int elementNode, int element)
    {
        if (_nodes[data.Index].Count == -1)
        {
            InsertIntoChildren(data, element, elementNode);
            return;
        }

        _elementNodes[elementNode].Next = _nodes[data.Index].FirstChild;
        _nodes[data.Index].FirstChild = elementNode;
        int count = ++_nodes[data.Index].Count;
        // Children of a node with a half size of 1 would have none: all four would be the same point, and an element on it
        // would be filed in every one of them, level after level.
        if (count > MaxElements && data.Depth < MaxDepth && data.Sx > 1 && data.Sy > 1)
        {
            Split(data);
        }
    }

    private void Split(NodeData data)
    {
        int head = _nodes[data.Index].FirstChild;
        int firstChild = AllocateChildren();
        _nodes[data.Index] = new Node(firstChild, -1);

        // Move the leaf's element nodes into the children, reusing each one for its element's first child.
        while (head != -1)
        {
            ElementNode node = _elementNodes[head];
            InsertIntoChildren(data, node.Element, head);
            head = node.Next;
        }
    }

    // Files an element in the children of a branch it overlaps; reusedNode, if not -1, is used for the first one.
    private void InsertIntoChildren(NodeData data, int element, int reusedNode)
    {
        Element e = _elements[element];
        int firstChild = _nodes[data.Index].FirstChild;
        int quadrants = Quadrants(data, e.Left, e.Top, e.Right, e.Bottom);
        for (int quadrant = 0; quadrant < 4; quadrant++)
        {
            if ((quadrants & (1 << quadrant)) == 0)
            {
                continue;
            }

            int elementNode = reusedNode != -1 ? reusedNode : _elementNodes.Insert(new ElementNode(-1, element));
            reusedNode = -1;
            InsertIntoLeaf(Child(data, firstChild, quadrant), elementNode, element);
        }
    }

    private int AllocateChildren()
    {
        int firstChild;
        if (_freeNode != -1)
        {
            firstChild = _freeNode;
            _freeNode = _nodes[firstChild].FirstChild;
        }
        else
        {
            firstChild = _nodes.Count;
            for (int j = 0; j < 4; j++)
            {
                _nodes.Add(default);
            }
        }

        for (int j = 0; j < 4; j++)
        {
            _nodes[firstChild + j] = new Node(-1, 0);
        }

        return firstChild;
    }

    // The children of a branch a box overlaps, as bits 0 to 3 for top-left, top-right, bottom-left and bottom-right. Inserts,
    // removals and queries all use these same tests, so an element is always found where it was filed.
    private static int Quadrants(NodeData data, int left, int top, int right, int bottom)
    {
        int quadrants = 0;
        if (top <= data.My)
        {
            if (left <= data.Mx)
            {
                quadrants |= 1;
            }

            if (right > data.Mx)
            {
                quadrants |= 2;
            }
        }

        if (bottom > data.My)
        {
            if (left <= data.Mx)
            {
                quadrants |= 4;
            }

            if (right > data.Mx)
            {
                quadrants |= 8;
            }
        }

        return quadrants;
    }

    private static NodeData Child(NodeData parent, int firstChild, int quadrant)
    {
        int halfWidth = parent.Sx >> 1, halfHeight = parent.Sy >> 1;
        int mx = (quadrant & 1) == 0 ? parent.Mx - halfWidth : parent.Mx + halfWidth;
        int my = (quadrant & 2) == 0 ? parent.My - halfHeight : parent.My + halfHeight;
        return new NodeData(firstChild + quadrant, parent.Depth + 1, mx, my, halfWidth, halfHeight);
    }

    private static Element ToElement(int id, Aabb bounds) =>
        new(id, Floor(bounds.Left), Floor(bounds.Top), Floor(bounds.Right), Floor(bounds.Bottom));

    private static Aabb ToAabb(NodeData data) => new(data.Mx - data.Sx, data.My - data.Sy, data.Mx + data.Sx, data.My + data.Sy);

    private static int Floor(float value) => (int)MathF.Floor(Math.Clamp(value, -CoordinateLimit, CoordinateLimit));

    private static int Ceiling(float value) => (int)MathF.Ceiling(Math.Clamp(value, -CoordinateLimit, CoordinateLimit));

    /// <summary>A node: a leaf (Count &gt;= 0, FirstChild the first element node or -1) or a branch (Count -1, FirstChild its first child).</summary>
    private record struct Node(int FirstChild, int Count);

    private record struct Element(int Id, int Left, int Top, int Right, int Bottom);

    /// <summary>One entry in a leaf's singly linked list of elements; an element in several leaves has one per leaf.</summary>
    private record struct ElementNode(int Next, int Element);

    /// <summary>A node with the center and half size worked out on the way down.</summary>
    private readonly record struct NodeData(int Index, int Depth, int Mx, int My, int Sx, int Sy);
}
