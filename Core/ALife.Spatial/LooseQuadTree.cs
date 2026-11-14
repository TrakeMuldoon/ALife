namespace ALife.Spatial;

/// <summary>
/// A loose quadtree, after user4842163's "Loose Quadtree" answer on Stack Overflow. Each element is filed in exactly one leaf,
/// by the center of its box, and every node keeps a box around everything below it, so huge elements cost no more than tiny
/// ones. It suits scenes whose element sizes vary wildly; queries are slower than <see cref="QuadTree"/>'s, since they test
/// each child's box instead of comparing against a center.
/// </summary>
/// <remarks>
/// Node boxes only grow as elements are added; <see cref="Cleanup"/> shrinks them to fit again and frees branches whose
/// children are all empty.
/// </remarks>
public sealed class LooseQuadTree : ISpatialIndex
{
    private readonly FreeList<Element> _elements = new();
    private readonly ValueList<Node> _nodes = new();
    private readonly ValueList<int> _stack = new();
    private readonly ValueList<int> _order = new();
    private readonly ValueList<(int Index, int Depth)> _visitStack = new();
    private readonly Cell _root;

    // The first of a group of four freed nodes, each group linking to the next through its first node's FirstChild, or -1.
    private int _freeNode = -1;

    /// <summary>Creates an empty loose quadtree.</summary>
    /// <param name="bounds">The area to cover. Element centers outside it are filed at its edges.</param>
    /// <param name="maxElements">How many elements a leaf holds before it splits, unless it's at <paramref name="maxDepth"/>.</param>
    /// <param name="maxDepth">How deep the tree can go; the root is depth 0.</param>
    public LooseQuadTree(Aabb bounds, int maxElements = 8, int maxDepth = 8)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxElements, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(maxDepth);
        if (bounds.IsEmpty)
        {
            throw new ArgumentException("The bounds can't be empty.", nameof(bounds));
        }

        Bounds = bounds;
        MaxElements = maxElements;
        MaxDepth = maxDepth;
        _root = new Cell(0, 0, bounds.CenterX, bounds.CenterY, bounds.Width * 0.5f, bounds.Height * 0.5f);
        _nodes.Add(new Node(Aabb.Empty, -1, 0));
    }

    /// <inheritdoc />
    public Aabb Bounds { get; }

    /// <summary>How many elements a leaf holds before it splits.</summary>
    public int MaxElements { get; }

    /// <summary>How deep the tree can go.</summary>
    public int MaxDepth { get; }

    /// <inheritdoc />
    public int Count => _elements.Count;

    /// <inheritdoc />
    public int Insert(int id, Aabb bounds)
    {
        int element = _elements.Insert(new Element(-1, id, bounds));
        InsertFrom(_root, element);
        return element;
    }

    /// <inheritdoc />
    public void Remove(int element)
    {
        Unlink(element);
        _elements.Erase(element);
    }

    /// <inheritdoc />
    public void Move(int element, Aabb bounds)
    {
        Unlink(element);
        _elements[element].Bounds = bounds;
        InsertFrom(_root, element);
    }

    /// <inheritdoc />
    public int GetId(int element) => _elements[element].Id;

    /// <inheritdoc />
    public Aabb GetBounds(int element) => _elements[element].Bounds;

    /// <inheritdoc />
    public void Query(Aabb area, List<int> results, int omitElement = -1)
    {
        ArgumentNullException.ThrowIfNull(results);
        _stack.Clear();
        _stack.Add(0);
        while (_stack.Count > 0)
        {
            Node node = _nodes[_stack.Pop()];
            if (!node.Bounds.Intersects(area))
            {
                continue;
            }

            if (node.Count == -1)
            {
                for (int j = 0; j < 4; j++)
                {
                    _stack.Add(node.FirstChild + j);
                }

                continue;
            }

            for (int element = node.FirstChild; element != -1; element = _elements[element].Next)
            {
                if (element != omitElement && _elements[element].Bounds.Intersects(area))
                {
                    results.Add(element);
                }
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Works bottom-up, so a branch whose children all empty out in this call is freed in the same call.
    /// </remarks>
    public void Cleanup()
    {
        // Every node in the tree, parents before children, so walking it backwards visits children first.
        _order.Clear();
        _stack.Clear();
        _stack.Add(0);
        while (_stack.Count > 0)
        {
            int index = _stack.Pop();
            _order.Add(index);
            if (_nodes[index].Count == -1)
            {
                for (int j = 0; j < 4; j++)
                {
                    _stack.Add(_nodes[index].FirstChild + j);
                }
            }
        }

        for (int i = _order.Count - 1; i >= 0; i--)
        {
            int index = _order[i];
            Node node = _nodes[index];
            Aabb bounds = Aabb.Empty;
            if (node.Count == -1)
            {
                int emptyLeaves = 0;
                for (int j = 0; j < 4; j++)
                {
                    Node child = _nodes[node.FirstChild + j];
                    emptyLeaves += child.Count == 0 ? 1 : 0;
                    bounds = bounds.Union(child.Bounds);
                }

                if (emptyLeaves == 4)
                {
                    _nodes[node.FirstChild].FirstChild = _freeNode;
                    _freeNode = node.FirstChild;
                    _nodes[index] = new Node(Aabb.Empty, -1, 0);
                    continue;
                }
            }
            else
            {
                for (int element = node.FirstChild; element != -1; element = _elements[element].Next)
                {
                    bounds = bounds.Union(_elements[element].Bounds);
                }
            }

            _nodes[index].Bounds = bounds;
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        _elements.Clear();
        _nodes.Clear();
        _nodes.Add(new Node(Aabb.Empty, -1, 0));
        _freeNode = -1;
    }

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
            if (node.Count == -1)
            {
                for (int j = 3; j >= 0; j--)
                {
                    _visitStack.Add((node.FirstChild + j, depth + 1));
                }
            }

            visitor(node.Bounds, depth, node.Count != -1, Math.Max(node.Count, 0));
        }
    }

    // Descends from a node by the element's center, growing each node's box on the way, and files it in the leaf it reaches.
    private void InsertFrom(Cell cell, int element)
    {
        Aabb bounds = _elements[element].Bounds;
        float x = bounds.CenterX, y = bounds.CenterY;
        while (true)
        {
            _nodes[cell.Index].Bounds = _nodes[cell.Index].Bounds.Union(bounds);
            if (_nodes[cell.Index].Count != -1)
            {
                break;
            }

            cell = Child(cell, _nodes[cell.Index].FirstChild, Quadrant(cell, x, y));
        }

        _elements[element].Next = _nodes[cell.Index].FirstChild;
        _nodes[cell.Index].FirstChild = element;
        int count = ++_nodes[cell.Index].Count;
        if (count > MaxElements && cell.Depth < MaxDepth)
        {
            Split(cell);
        }
    }

    private void Split(Cell cell)
    {
        int head = _nodes[cell.Index].FirstChild;
        int firstChild = AllocateChildren();
        _nodes[cell.Index].FirstChild = firstChild;
        _nodes[cell.Index].Count = -1;

        // The node's box already covers its elements, so they can go straight into the children.
        while (head != -1)
        {
            int next = _elements[head].Next;
            Aabb bounds = _elements[head].Bounds;
            InsertFrom(Child(cell, firstChild, Quadrant(cell, bounds.CenterX, bounds.CenterY)), head);
            head = next;
        }
    }

    // Takes an element out of its leaf's list, found by the same descent InsertFrom made. Node boxes stay as they are.
    private void Unlink(int element)
    {
        Aabb bounds = _elements[element].Bounds;
        float x = bounds.CenterX, y = bounds.CenterY;
        Cell cell = _root;
        while (_nodes[cell.Index].Count == -1)
        {
            cell = Child(cell, _nodes[cell.Index].FirstChild, Quadrant(cell, x, y));
        }

        int previous = -1;
        int current = _nodes[cell.Index].FirstChild;
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
            _nodes[cell.Index].FirstChild = _elements[current].Next;
        }
        else
        {
            _elements[previous].Next = _elements[current].Next;
        }

        _nodes[cell.Index].Count--;
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
            _nodes[firstChild + j] = new Node(Aabb.Empty, -1, 0);
        }

        return firstChild;
    }

    // The quadrant a point is in: bit 0 for the right half, bit 1 for the bottom half.
    private static int Quadrant(Cell cell, float x, float y) => (x < cell.Mx ? 0 : 1) | (y < cell.My ? 0 : 2);

    private static Cell Child(Cell parent, int firstChild, int quadrant)
    {
        float halfWidth = parent.Sx * 0.5f, halfHeight = parent.Sy * 0.5f;
        float mx = (quadrant & 1) == 0 ? parent.Mx - halfWidth : parent.Mx + halfWidth;
        float my = (quadrant & 2) == 0 ? parent.My - halfHeight : parent.My + halfHeight;
        return new Cell(firstChild + quadrant, parent.Depth + 1, mx, my, halfWidth, halfHeight);
    }

    /// <summary>A node: a leaf (Count &gt;= 0, FirstChild its first element or -1) or a branch (Count -1, FirstChild its first child).</summary>
    private record struct Node(Aabb Bounds, int FirstChild, int Count);

    /// <summary>An element, linked into its one leaf's list through Next.</summary>
    private record struct Element(int Next, int Id, Aabb Bounds);

    /// <summary>
    /// A node with the fixed (tight) center and half size worked out on the way down; elements are filed by these, while
    /// queries use the node's loose box.
    /// </summary>
    private readonly record struct Cell(int Index, int Depth, float Mx, float My, float Sx, float Sy);
}
