namespace ALife.Spatial.Tests;

/// <summary>
/// Checks every <see cref="ISpatialIndex"/> against a brute-force list. Coordinates are whole numbers, so the
/// <see cref="QuadTree"/>'s flooring changes nothing, and the <see cref="UniformGrid"/> only gets boxes of its one size.
/// </summary>
[TestClass]
public sealed class SpatialIndexTests
{
    private const int WorldSize = 1000;
    private const int UniformHalfSize = 6;

    private static readonly Aabb World = new(0, 0, WorldSize, WorldSize);

    public static IEnumerable<object[]> Kinds => new string[] { nameof(QuadTree), nameof(LooseQuadTree), nameof(LooseTightGrid), nameof(UniformGrid), nameof(LayeredGrid) }.Select(kind => new object[] { kind });

    private static ISpatialIndex Create(string kind) => kind switch
    {
        nameof(QuadTree) => new QuadTree(World, maxElements: 4, maxDepth: 6),
        nameof(LooseQuadTree) => new LooseQuadTree(World, maxElements: 4, maxDepth: 6),
        nameof(LooseTightGrid) => new LooseTightGrid(World, 40, 40, 8, 8),
        nameof(UniformGrid) => new UniformGrid(World, 25, 25, UniformHalfSize, UniformHalfSize),
        nameof(LayeredGrid) => new LayeredGrid(World, 8, 8, layers: 4),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };

    private static Aabb RandomBox(Random random, string kind)
    {
        if (kind == nameof(UniformGrid))
        {
            return Aabb.FromCenter(random.Next(0, WorldSize + 1), random.Next(0, WorldSize + 1), UniformHalfSize, UniformHalfSize);
        }

        // Mostly small boxes, with the odd huge one to stretch the loose structures.
        int maxSize = random.Next(20) == 0 ? 400 : 30;
        int width = random.Next(0, maxSize), height = random.Next(0, maxSize);
        int left = random.Next(0, WorldSize - width + 1), top = random.Next(0, WorldSize - height + 1);
        return new Aabb(left, top, left + width, top + height);
    }

    private static Aabb RandomArea(Random random)
    {
        int width = random.Next(0, 300), height = random.Next(0, 300);
        int left = random.Next(-50, WorldSize), top = random.Next(-50, WorldSize);
        return new Aabb(left, top, left + width, top + height);
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Matches_brute_force_through_inserts_moves_and_removes(string kind)
    {
        Random random = new(1234);
        ISpatialIndex index = Create(kind);
        Dictionary<int, (int Id, Aabb Bounds)> expected = [];
        List<int> results = [];
        int nextId = 0;

        for (int step = 0; step < 4000; step++)
        {
            int roll = random.Next(100);
            if (roll < 45 || expected.Count == 0)
            {
                Aabb bounds = RandomBox(random, kind);
                int id = nextId++;
                int handle = index.Insert(id, bounds);
                Assert.IsFalse(expected.ContainsKey(handle), "A handle in use was handed out again.");
                expected[handle] = (id, bounds);
            }
            else if (roll < 75)
            {
                int handle = expected.Keys.ElementAt(random.Next(expected.Count));
                Aabb bounds = RandomBox(random, kind);
                index.Move(handle, bounds);
                expected[handle] = (expected[handle].Id, bounds);
            }
            else if (roll < 95)
            {
                int handle = expected.Keys.ElementAt(random.Next(expected.Count));
                index.Remove(handle);
                expected.Remove(handle);
            }
            else
            {
                index.Cleanup();
            }

            Assert.AreEqual(expected.Count, index.Count);
            if (step % 10 == 0)
            {
                AssertQueriesMatch(index, expected, random, results);
            }
        }

        foreach ((int handle, (int id, Aabb bounds)) in expected)
        {
            Assert.AreEqual(id, index.GetId(handle));
            Assert.AreEqual(bounds, index.GetBounds(handle));
        }
    }

    private static void AssertQueriesMatch(ISpatialIndex index, Dictionary<int, (int Id, Aabb Bounds)> expected, Random random, List<int> results)
    {
        for (int q = 0; q < 5; q++)
        {
            Aabb area = RandomArea(random);
            int omit = expected.Count > 0 && random.Next(2) == 0 ? expected.Keys.ElementAt(random.Next(expected.Count)) : -1;
            results.Clear();
            index.Query(area, results, omit);

            int[] actual = [.. results.Order()];
            int[] wanted = [.. expected.Where(pair => pair.Key != omit && pair.Value.Bounds.Intersects(area)).Select(pair => pair.Key).Order()];
            SequenceAssert.AreEqual(wanted, actual);
        }
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Query_appends_without_clearing(string kind)
    {
        ISpatialIndex index = Create(kind);
        int handle = index.Insert(7, Aabb.FromCenter(100, 100, UniformHalfSize, UniformHalfSize));
        List<int> results = [-42];

        index.Query(new Aabb(90, 90, 110, 110), results);

        SequenceAssert.AreEqual([-42, handle], results);
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Query_finds_nothing_for_an_empty_area(string kind)
    {
        ISpatialIndex index = Create(kind);
        index.Insert(1, Aabb.FromCenter(100, 100, UniformHalfSize, UniformHalfSize));
        List<int> results = [];

        index.Query(Aabb.Empty, results);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Move_keeps_the_handle_and_id(string kind)
    {
        ISpatialIndex index = Create(kind);
        int handle = index.Insert(99, Aabb.FromCenter(100, 100, UniformHalfSize, UniformHalfSize));
        Aabb moved = Aabb.FromCenter(800, 700, UniformHalfSize, UniformHalfSize);
        List<int> results = [];

        index.Move(handle, moved);
        index.Query(moved, results);

        SequenceAssert.AreEqual([handle], results);
        Assert.AreEqual(99, index.GetId(handle));
        Assert.AreEqual(moved, index.GetBounds(handle));
        results.Clear();
        index.Query(new Aabb(90, 90, 110, 110), results);
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Cleanup_after_removing_everything_leaves_nothing_behind(string kind)
    {
        Random random = new(99);
        ISpatialIndex index = Create(kind);
        List<int> handles = [];
        for (int i = 0; i < 500; i++)
        {
            handles.Add(index.Insert(i, RandomBox(random, kind)));
        }

        foreach (int handle in handles)
        {
            index.Remove(handle);
        }

        // The tight quadtree collapses one level per call, as in the answer.
        for (int i = 0; i < 10; i++)
        {
            index.Cleanup();
        }

        Assert.AreEqual(0, index.Count);
        List<(int Depth, bool IsLeaf, int Count)> nodes = [];
        index.VisitNodes((_, depth, isLeaf, count) => nodes.Add((depth, isLeaf, count)));
        foreach ((int Depth, bool IsLeaf, int Count) node in nodes)
        {
            Assert.AreEqual(0, node.Count);
        }
        if (index is QuadTree or LooseQuadTree)
        {
            SequenceAssert.AreEqual([(0, true, 0)], nodes);
        }
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Clear_empties_the_index_and_it_stays_usable(string kind)
    {
        ISpatialIndex index = Create(kind);
        for (int i = 0; i < 100; i++)
        {
            index.Insert(i, Aabb.FromCenter(10 + i * 9, 500, UniformHalfSize, UniformHalfSize));
        }

        index.Clear();
        List<int> results = [];
        index.Query(World, results);

        Assert.AreEqual(0, index.Count);
        Assert.AreEqual(0, results.Count);

        int handle = index.Insert(5, Aabb.FromCenter(50, 50, UniformHalfSize, UniformHalfSize));
        index.Query(World, results);
        SequenceAssert.AreEqual([handle], results);
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void VisitNodes_counts_add_up_to_the_elements(string kind)
    {
        Random random = new(5);
        ISpatialIndex index = Create(kind);
        for (int i = 0; i < 300; i++)
        {
            index.Insert(i, RandomBox(random, kind));
        }

        int leafCount = 0;
        index.VisitNodes((_, _, isLeaf, count) => leafCount += isLeaf ? count : 0);

        // The tight quadtree lists an element in every leaf it overlaps, so it can count some twice.
        if (index is QuadTree)
        {
            Assert.IsTrue(leafCount >= 300);
        }
        else
        {
            Assert.AreEqual(300, leafCount);
        }
    }

    [TestMethod]
    public void QuadTree_stops_splitting_at_two_units_however_deep_it_may_go()
    {
        QuadTree tree = new(new Aabb(0, 0, 64, 64), maxElements: 1, maxDepth: 30);
        List<int> handles = [];
        for (int i = 0; i < 50; i++)
        {
            // All on one point, which would otherwise be filed in four children per level, down to depth 30.
            handles.Add(tree.Insert(i, new Aabb(32, 32, 32, 32)));
        }

        int deepest = 0, smallest = int.MaxValue;
        tree.VisitNodes((bounds, depth, _, _) =>
        {
            deepest = Math.Max(deepest, depth);
            smallest = Math.Min(smallest, (int)bounds.Width);
        });
        List<int> results = [];
        tree.Query(new Aabb(32, 32, 32, 32), results);

        Assert.IsTrue(smallest >= 2, $"A node {smallest} units across.");
        Assert.IsTrue(deepest <= 5, $"Depth {deepest}.");
        SequenceAssert.AreEqual(handles, results.Order());
    }

    [TestMethod]
    public void UniformGrid_rejects_boxes_bigger_than_its_size()
    {
        UniformGrid grid = new(World, 10, 10, 5, 5);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => grid.Insert(1, new Aabb(0, 0, 11, 4)));
        int handle = grid.Insert(1, new Aabb(0, 0, 4, 4));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => grid.Move(handle, new Aabb(0, 0, 4, 20)));
    }

    [TestMethod]
    public void UniformGrid_reports_the_uniform_box()
    {
        UniformGrid grid = new(World, 10, 10, 5, 5);

        int handle = grid.Insert(1, new Aabb(10, 10, 12, 12));

        Assert.AreEqual(new Aabb(6, 6, 16, 16), grid.GetBounds(handle));
    }

    [TestMethod]
    public void LooseQuadTree_holds_elements_far_bigger_than_its_cells()
    {
        LooseQuadTree tree = new(World, maxElements: 1, maxDepth: 8);
        for (int i = 0; i < 50; i++)
        {
            tree.Insert(i, Aabb.FromCenter(10 + i * 19, 10 + i * 19, 1, 1));
        }

        int big = tree.Insert(100, new Aabb(0, 0, 1000, 1000));
        List<int> results = [];
        tree.Query(new Aabb(990, 5, 995, 8), results);

        SequenceAssert.AreEqual([big], results);
    }
}
