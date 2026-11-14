using System.Collections.Concurrent;
using ALife.Spatial.Packed;
using ALife.Spatial.Phased;

namespace ALife.Spatial.Tests;

/// <summary>
/// Checks what <see cref="PhasedGrid"/> adds to the packed indexes: the parallel pair search that finds each pair once, the
/// parallel build and <see cref="PhasedGrid.Order"/>. The rest is covered with the other packed indexes.
/// </summary>
[TestClass]
public sealed class PhasedGridTests
{
    private const int WorldSize = 1000;

    private static readonly Aabb World = new(0, 0, WorldSize, WorldSize);

    public static IEnumerable<object[]> LayersAndThreads => [[1, 1], [1, 4], [4, 1], [4, 4]];

    // Sizes from a dot to a few times a top cell (125 across with 8 columns), a few empty, some outside the bounds.
    private static Aabb[] RandomBoxes(Random random, int count)
    {
        Aabb[] boxes = new Aabb[count];
        for (int i = 0; i < count; i++)
        {
            int roll = random.Next(100);
            if (roll < 2)
            {
                boxes[i] = Aabb.Empty;
                continue;
            }

            int maxSize = roll < 4 ? 400 : roll < 15 ? 100 : roll < 40 ? 30 : 8;
            int width = random.Next(0, maxSize), height = random.Next(0, maxSize);
            int left = random.Next(-50, WorldSize + 50), top = random.Next(-50, WorldSize + 50);
            boxes[i] = new Aabb(left, top, left + width, top + height);
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

    // Each pair with its lower element first.
    private static HashSet<(int, int)> Ordered(IEnumerable<(int, int)> pairs) =>
        [.. pairs.Select(pair => pair.Item1 < pair.Item2 ? pair : (pair.Item2, pair.Item1))];

    [TestMethod]
    [DynamicData(nameof(LayersAndThreads))]
    public void ForEachPairInParallel_finds_each_intersecting_pair_once(int layers, int threads)
    {
        Random random = new(31);
        Aabb[] boxes = RandomBoxes(random, 3000);
        PhasedGrid grid = new(World, 8, 8, layers, threads);
        grid.Build(boxes);
        ConcurrentBag<(int, int)> found = [];

        grid.ForEachPairInParallel(new BagCollector(found));

        List<(int, int)> ordered = [.. found.Select(pair => pair.Item1 < pair.Item2 ? pair : (pair.Item2, pair.Item1))];
        Assert.AreEqual(ordered.Count, ordered.Distinct().Count());
        Assert.IsTrue(BrutePairs(boxes).SetEquals(ordered));
        Assert.IsTrue(grid.OversizedCount > 0);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(4)]
    public void ForEachPairInParallel_never_runs_two_calls_sharing_an_element_at_once(int layers)
    {
        Random random = new(32);
        Aabb[] boxes = RandomBoxes(random, 6000);
        PhasedGrid grid = new(World, 20, 20, layers, threads: 8);
        grid.Build(boxes);
        int[] busy = new int[boxes.Length];
        int[] clashes = new int[1];

        grid.ForEachPairInParallel(new ClashDetector(busy, clashes));

        Assert.AreEqual(0, clashes[0]);
    }

    [TestMethod]
    public void Building_on_several_threads_sorts_the_same_as_on_one()
    {
        Random random = new(33);
        Aabb[] boxes = RandomBoxes(random, 40000);
        PhasedGrid single = new(World, 8, 8, layers: 4, threads: 1);
        PhasedGrid several = new(World, 8, 8, layers: 4, threads: 6);

        single.Build(boxes);
        several.Build(boxes);

        SequenceAssert.AreEqual(single.Order.ToArray(), several.Order.ToArray());
        Assert.AreEqual(single.OversizedCount, several.OversizedCount);
        SequenceAssert.AreEqual(Enumerable.Range(0, boxes.Length), single.Order.ToArray().Order());
    }

    [TestMethod]
    public void Building_with_more_counters_than_boxes_sorts_the_same()
    {
        // On one thread every cell is counted at once. On several, that would take more counters than there are boxes, so the
        // boxes go into buckets of cells first and are then sorted within them. Both have to come out the same.
        Random random = new(34);
        Aabb[] boxes = RandomBoxes(random, 20000);
        PhasedGrid single = new(World, 50, 50, layers: 2, threads: 1);
        PhasedGrid several = new(World, 50, 50, layers: 2, threads: 6);
        ConcurrentBag<(int, int)> singlePairs = [], severalPairs = [];

        single.Build(boxes);
        several.Build(boxes);
        single.ForEachPairInParallel(new BagCollector(singlePairs));
        several.ForEachPairInParallel(new BagCollector(severalPairs));

        SequenceAssert.AreEqual(single.Order.ToArray(), several.Order.ToArray());
        Assert.AreEqual(single.OversizedCount, several.OversizedCount);
        Assert.IsTrue(Ordered(singlePairs).SetEquals(Ordered(severalPairs)));
    }

    [TestMethod]
    public void Order_keeps_a_cells_elements_in_the_order_they_came()
    {
        // Two cells, top-left and bottom-right, given alternately; then an empty box and an oversized one.
        Aabb[] boxes =
        [
            Aabb.FromCenter(10, 10, 1, 1), Aabb.FromCenter(990, 990, 1, 1), Aabb.FromCenter(20, 20, 1, 1),
            Aabb.Empty, Aabb.FromCenter(500, 500, 600, 600), Aabb.FromCenter(980, 980, 1, 1),
        ];
        PhasedGrid grid = new(World, 2, 2);

        grid.Build(boxes);

        SequenceAssert.AreEqual([0, 2, 1, 5, 4, 3], grid.Order.ToArray());
    }

    [TestMethod]
    public void Constructor_and_Threads_check_their_arguments()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PhasedGrid(World, 0, 10));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PhasedGrid(World, 10, 10, layers: PhasedGrid.MaxLayers + 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PhasedGrid(World, 10, 10, threads: 0));
        Assert.ThrowsExactly<ArgumentException>(() => new PhasedGrid(new Aabb(0, 0, 10, 0), 10, 10));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PhasedGrid(World, 10, 10) { Threads = 0 });
    }

    private readonly struct BagCollector(ConcurrentBag<(int, int)> bag) : IPairVisitor
    {
        public void Visit(int a, int b) => bag.Add((a, b));
    }

    // Counts calls that start while another call for either of their elements is still going.
    private readonly struct ClashDetector(int[] busy, int[] clashes) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            if (Interlocked.Increment(ref busy[a]) != 1)
            {
                Interlocked.Increment(ref clashes[0]);
            }

            if (Interlocked.Increment(ref busy[b]) != 1)
            {
                Interlocked.Increment(ref clashes[0]);
            }

            Thread.SpinWait(20);
            Interlocked.Decrement(ref busy[b]);
            Interlocked.Decrement(ref busy[a]);
        }
    }
}
