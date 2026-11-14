using System.Collections.Concurrent;
using ALife.Spatial.Packed;

namespace ALife.Spatial.Tests;

/// <summary>
/// Checks what the layered grids add: pair searches on the incremental <see cref="LayeredGrid"/>, thread counts, and boxes
/// bigger than a top cell. The rest is covered with the other indexes.
/// </summary>
[TestClass]
public sealed class LayeredGridTests
{
    private const int WorldSize = 1000;

    private static readonly Aabb World = new(0, 0, WorldSize, WorldSize);

    public static IEnumerable<object[]> ThreadCounts => [[1], [4]];

    // Sizes from a dot to a few times a top cell (125 here), so every layer gets some, and some outside the bounds.
    private static Aabb RandomBox(Random random)
    {
        int roll = random.Next(100);
        int maxSize = roll < 3 ? 400 : roll < 15 ? 100 : roll < 40 ? 30 : 8;
        int width = random.Next(0, maxSize), height = random.Next(0, maxSize);
        int left = random.Next(-50, WorldSize + 50), top = random.Next(-50, WorldSize + 50);
        return new Aabb(left, top, left + width, top + height);
    }

    private static HashSet<(int, int)> BrutePairs(IReadOnlyDictionary<int, Aabb> boxes)
    {
        HashSet<(int, int)> pairs = [];
        foreach ((int a, Aabb boxA) in boxes)
        {
            foreach ((int b, Aabb boxB) in boxes)
            {
                if (a < b && boxA.Intersects(boxB))
                {
                    pairs.Add((a, b));
                }
            }
        }

        return pairs;
    }

    private static HashSet<(int, int)> BothWays(HashSet<(int, int)> pairs) => [.. pairs, .. pairs.Select(pair => (pair.Item2, pair.Item1))];

    [TestMethod]
    [DynamicData(nameof(ThreadCounts))]
    public void LayeredGrid_pair_searches_match_brute_force_through_inserts_moves_and_removes(int threads)
    {
        Random random = new(21);
        LayeredGrid grid = new(World, 8, 8, layers: 4, threads: threads);
        Dictionary<int, Aabb> expected = [];

        for (int round = 0; round < 6; round++)
        {
            for (int step = 0; step < 600; step++)
            {
                int roll = random.Next(100);
                if (roll < 50 || expected.Count == 0)
                {
                    Aabb box = RandomBox(random);
                    expected[grid.Insert(0, box)] = box;
                }
                else if (roll < 80)
                {
                    int handle = expected.Keys.ElementAt(random.Next(expected.Count));
                    Aabb box = RandomBox(random);
                    grid.Move(handle, box);
                    expected[handle] = box;
                }
                else
                {
                    int handle = expected.Keys.ElementAt(random.Next(expected.Count));
                    grid.Remove(handle);
                    expected.Remove(handle);
                }
            }

            if (round % 2 == 1)
            {
                grid.Cleanup();
            }

            HashSet<(int, int)> wanted = BrutePairs(expected);
            List<(int, int)> once = [];
            PairCollector collector = new(once);
            grid.ForEachPair(ref collector);
            List<(int, int)> ordered = [.. once.Select(pair => pair.Item1 < pair.Item2 ? pair : (pair.Item2, pair.Item1))];
            Assert.AreEqual(ordered.Count, ordered.Distinct().Count());
            Assert.IsTrue(wanted.SetEquals(ordered));

            ConcurrentBag<(int, int)> both = [];
            grid.ForEachNeighbor(new BagCollector(both));
            HashSet<(int, int)> wantedBoth = BothWays(wanted);
            Assert.AreEqual(wantedBoth.Count, both.Count);
            Assert.IsTrue(wantedBoth.SetEquals(both));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ThreadCounts))]
    public void PackedLayeredGrid_finds_each_pair_both_ways_round_on_any_thread_count(int threads)
    {
        Random random = new(22);
        Aabb[] boxes = [.. Enumerable.Range(0, 3000).Select(_ => RandomBox(random))];
        PackedLayeredGrid grid = new(World, 8, 8, layers: 4, threads: threads);
        grid.Build(boxes);
        ConcurrentBag<(int, int)> found = [];

        grid.ForEachNeighbor(new BagCollector(found));

        HashSet<(int, int)> wanted = BothWays(BrutePairs(boxes.Index().ToDictionary(pair => pair.Index, pair => pair.Item)));
        Assert.AreEqual(wanted.Count, found.Count);
        Assert.IsTrue(wanted.SetEquals(found));
    }

    [TestMethod]
    public void LayeredGrid_ForEachNeighbor_never_runs_one_element_on_two_threads()
    {
        Random random = new(23);
        LayeredGrid grid = new(World, 8, 8, layers: 4, threads: 8);
        for (int i = 0; i < 5000; i++)
        {
            grid.Insert(i, RandomBox(random));
        }

        int[] busy = new int[5000];
        int[] clashes = new int[1];

        grid.ForEachNeighbor(new ClashDetector(busy, clashes));

        Assert.AreEqual(0, clashes[0]);
    }

    [TestMethod]
    public void Elements_go_in_the_finest_layer_they_fit()
    {
        LayeredGrid grid = new(World, 10, 10, layers: 3);
        PackedLayeredGrid packed = new(World, 10, 10, layers: 3);
        Aabb[] boxes = [Aabb.FromCenter(50, 50, 2, 2), Aabb.FromCenter(250, 250, 20, 20), Aabb.FromCenter(550, 550, 40, 40), Aabb.FromCenter(850, 850, 80, 80)];
        foreach (Aabb box in boxes)
        {
            grid.Insert(0, box);
        }

        packed.Build(boxes);

        // Cells are 100, 50 and 25: the 4-wide box goes in the finest layer, the 40 in the middle one, and the 160 (too big
        // for any) and the 80 in the top one, in two cells.
        List<(int, int)> wanted = [(0, 1), (0, 1), (1, 1), (2, 1)];
        SequenceAssert.AreEqual(wanted, OccupiedCells(grid.VisitNodes));
        SequenceAssert.AreEqual(wanted, OccupiedCells(packed.VisitNodes));
        Assert.AreEqual(1, grid.OversizedCount);
        Assert.AreEqual(1, packed.OversizedCount);
    }

    [TestMethod]
    public void LayeredGrid_tracks_oversized_elements_through_moves_removes_and_cleanup()
    {
        LayeredGrid grid = new(World, 10, 10, layers: 3);
        int big = grid.Insert(0, Aabb.FromCenter(500, 500, 300, 300));
        int small = grid.Insert(1, Aabb.FromCenter(100, 100, 2, 2));
        List<int> results = [];

        grid.Query(Aabb.FromCenter(250, 250, 1, 1), results);
        SequenceAssert.AreEqual([big], results);
        Assert.AreEqual(1, grid.OversizedCount);

        grid.Move(big, Aabb.FromCenter(500, 500, 10, 10));
        Assert.AreEqual(0, grid.OversizedCount);
        grid.Cleanup();
        grid.Move(small, Aabb.FromCenter(900, 900, 200, 200));
        Assert.AreEqual(1, grid.OversizedCount);

        // The far corner of the moved box is found, though its center is four top cells away.
        results.Clear();
        grid.Query(Aabb.FromCenter(720, 720, 1, 1), results);
        SequenceAssert.AreEqual([small], results);

        grid.Remove(small);
        grid.Cleanup();
        Assert.AreEqual(0, grid.OversizedCount);
        results.Clear();
        grid.Query(World, results);
        SequenceAssert.AreEqual([big], results);
    }

    [TestMethod]
    public void Constructors_check_their_arguments()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new LayeredGrid(World, 0, 10));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new LayeredGrid(World, 10, 10, layers: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new LayeredGrid(World, 10, 10, layers: LayeredGrid.MaxLayers + 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new LayeredGrid(World, 10, 10, threads: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PackedLayeredGrid(World, 1000, 1000, layers: 6));
        Assert.ThrowsExactly<ArgumentException>(() => new PackedLayeredGrid(new Aabb(0, 0, 0, 10), 10, 10));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PackedLayeredGrid(World, 10, 10) { Threads = 0 });
    }

    // The (depth, count) of each finer-layer cell holding anything, in the order reported. Top cells are always reported.
    private static List<(int, int)> OccupiedCells(Action<SpatialNodeVisitor> visitNodes)
    {
        List<(int, int)> cells = [];
        visitNodes((_, depth, _, count) =>
        {
            if (count > 0)
            {
                cells.Add((depth, count));
            }
        });
        return cells;
    }

    private readonly struct PairCollector(List<(int, int)> pairs) : IPairVisitor
    {
        public void Visit(int a, int b) => pairs.Add((a, b));
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
