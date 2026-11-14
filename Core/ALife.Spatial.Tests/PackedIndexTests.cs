using System.Collections.Concurrent;
using ALife.Spatial.Packed;
using ALife.Spatial.Phased;

namespace ALife.Spatial.Tests;

/// <summary>Checks every <see cref="IPackedSpatialIndex"/> against brute force, with odd boxes mixed in.</summary>
[TestClass]
public sealed class PackedIndexTests
{
    private const int WorldSize = 1000;

    private const string LayeredPhasedGrid = "PhasedGrid with layers";

    private static readonly Aabb World = new(0, 0, WorldSize, WorldSize);

    public static IEnumerable<object[]> Kinds => new string[] { nameof(PackedGrid), nameof(PackedQuadTree), nameof(PackedLayeredGrid), nameof(PhasedGrid), LayeredPhasedGrid }.Select(kind => new object[] { kind });

    private static IPackedSpatialIndex Create(string kind) => kind switch
    {
        nameof(PackedGrid) => new PackedGrid(World, 40, 40),
        nameof(PackedQuadTree) => new PackedQuadTree(World, maxElements: 4, maxDepth: 8),
        nameof(PackedLayeredGrid) => new PackedLayeredGrid(World, 8, 8, layers: 4),
        nameof(PhasedGrid) => new PhasedGrid(World, 40, 40),
        LayeredPhasedGrid => new PhasedGrid(World, 8, 8, layers: 4),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };

    // Mostly small boxes, some touching exactly, a few bigger than a grid cell, a few empty, some outside the bounds and
    // some stacked on one point.
    private static Aabb[] RandomBoxes(Random random, int count)
    {
        Aabb[] boxes = new Aabb[count];
        for (int i = 0; i < count; i++)
        {
            int roll = random.Next(100);
            if (roll < 3)
            {
                boxes[i] = Aabb.Empty;
            }
            else if (roll < 6)
            {
                boxes[i] = new Aabb(500, 500, 500, 500);
            }
            else
            {
                int maxSize = roll < 10 ? 300 : 25;
                int width = random.Next(0, maxSize), height = random.Next(0, maxSize);
                int left = random.Next(-100, WorldSize + 100), top = random.Next(-100, WorldSize + 100);
                boxes[i] = new Aabb(left, top, left + width, top + height);
            }
        }

        return boxes;
    }

    private static HashSet<(int, int)> BrutePairs(Aabb[] boxes)
    {
        HashSet<(int, int)> pairs = [];
        for (int a = 0; a < boxes.Length; a++)
        {
            for (int b = a + 1; b < boxes.Length; b++)
            {
                if (boxes[a].Intersects(boxes[b]))
                {
                    pairs.Add((a, b));
                }
            }
        }

        return pairs;
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void ForEachPair_finds_each_intersecting_pair_once(string kind)
    {
        Random random = new(7);
        IPackedSpatialIndex index = Create(kind);
        for (int round = 0; round < 3; round++)
        {
            Aabb[] boxes = RandomBoxes(random, 1500 + round * 300);
            index.Build(boxes);
            PairCollector collector = new([]);

            index.ForEachPair(ref collector);

            List<(int, int)> found = [.. collector.Pairs.Select(pair => pair.Item1 < pair.Item2 ? pair : (pair.Item2, pair.Item1))];
            Assert.AreEqual(found.Count, found.Distinct().Count());
            Assert.IsTrue(BrutePairs(boxes).SetEquals(found));
        }
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void ForEachNeighbor_finds_each_pair_both_ways_round(string kind)
    {
        Random random = new(8);
        Aabb[] boxes = RandomBoxes(random, 3000);
        IPackedSpatialIndex index = Create(kind);
        index.Build(boxes);
        ConcurrentBag<(int, int)> found = [];

        index.ForEachNeighbor(new BagCollector(found));

        HashSet<(int, int)> expected = BrutePairs(boxes);
        expected.UnionWith([.. expected.Select(pair => (pair.Item2, pair.Item1))]);
        Assert.AreEqual(expected.Count, found.Count);
        Assert.IsTrue(expected.SetEquals(found));
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void ForEachNeighbor_never_runs_one_element_on_two_threads(string kind)
    {
        Aabb[] boxes = RandomBoxes(new Random(9), 5000);
        IPackedSpatialIndex index = Create(kind);
        index.Build(boxes);
        int[] busy = new int[boxes.Length];
        int[] clashes = new int[1];

        index.ForEachNeighbor(new ClashDetector(busy, clashes));

        Assert.AreEqual(0, clashes[0]);
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Query_matches_brute_force(string kind)
    {
        Random random = new(10);
        Aabb[] boxes = RandomBoxes(random, 2000);
        IPackedSpatialIndex index = Create(kind);
        index.Build(boxes);
        List<int> results = [];

        for (int q = 0; q < 200; q++)
        {
            int width = random.Next(0, 300), height = random.Next(0, 300);
            int left = random.Next(-150, WorldSize + 50), top = random.Next(-150, WorldSize + 50);
            Aabb area = new(left, top, left + width, top + height);
            int omit = random.Next(2) == 0 ? random.Next(boxes.Length) : -1;
            results.Clear();

            index.Query(area, results, omit);

            int[] wanted = [.. Enumerable.Range(0, boxes.Length).Where(i => i != omit && boxes[i].Intersects(area))];
            SequenceAssert.AreEqual(wanted, results.Order());
        }
    }

    [TestMethod]
    [DynamicData(nameof(Kinds))]
    public void Build_replaces_everything(string kind)
    {
        IPackedSpatialIndex index = Create(kind);
        index.Build([Aabb.FromCenter(100, 100, 5, 5), Aabb.FromCenter(102, 100, 5, 5)]);
        index.Build([Aabb.FromCenter(800, 800, 5, 5)]);
        List<int> results = [];

        index.Query(World, results);

        Assert.AreEqual(1, index.Count);
        SequenceAssert.AreEqual([0], results);
        Assert.AreEqual(Aabb.FromCenter(800, 800, 5, 5), index.GetBounds(0));
        index.Build([]);
        results.Clear();
        index.Query(World, results);
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void PackedGrid_keeps_boxes_bigger_than_a_cell_aside()
    {
        PackedGrid grid = new(World, 10, 10);

        grid.Build([new Aabb(0, 0, 99, 99), new Aabb(0, 0, 100, 60), Aabb.FromCenter(50, 50, 1, 1)]);

        Assert.AreEqual(1, grid.OversizedCount);
        PairCollector collector = new([]);
        grid.ForEachPair(ref collector);
        Assert.AreEqual(3, collector.Pairs.Count);
    }

    [TestMethod]
    public void PackedQuadTree_stops_at_its_depth_on_a_pile_of_one_point()
    {
        PackedQuadTree tree = new(World, maxElements: 1, maxDepth: 16);
        Aabb[] boxes = [.. Enumerable.Repeat(new Aabb(500, 500, 500, 500), 100)];

        tree.Build(boxes);

        int deepest = 0;
        tree.VisitNodes((_, depth, _, _) => deepest = Math.Max(deepest, depth));
        PairCollector collector = new([]);
        tree.ForEachPair(ref collector);
        Assert.AreEqual(16, deepest);
        Assert.AreEqual(100 * 99 / 2, collector.Pairs.Count);
    }

    private readonly struct PairCollector(List<(int, int)> pairs) : IPairVisitor
    {
        public List<(int, int)> Pairs { get; } = pairs;

        public void Visit(int a, int b) => Pairs.Add((a, b));
    }

    private readonly struct BagCollector(ConcurrentBag<(int, int)> bag) : IPairVisitor
    {
        public void Visit(int a, int b) => bag.Add((a, b));
    }

    // Counts calls for an element that start while another call for it is still going.
    private readonly struct ClashDetector(int[] busy, int[] clashes) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            if (Interlocked.Increment(ref busy[a]) != 1)
            {
                Interlocked.Increment(ref clashes[0]);
            }

            Thread.SpinWait(20);
            Interlocked.Decrement(ref busy[a]);
        }
    }
}
