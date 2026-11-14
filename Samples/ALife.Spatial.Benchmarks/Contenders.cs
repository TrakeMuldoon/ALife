using ALife.Spatial.Packed;
using ALife.Spatial.Phased;

namespace ALife.Spatial.Benchmarks;

/// <summary>How a scenario's indexes are sized, following each type's own advice.</summary>
internal readonly record struct Sizing(Aabb Bounds, float MinRadius, float CommonRadius, float MaxRadius)
{
    /// <summary>8 levels for the demo's world, one more each time the world doubles.</summary>
    public int TreeDepth => Math.Clamp(8 + (int)MathF.Round(MathF.Log2(Bounds.Width / 1280)), 1, PackedQuadTree.MaxSupportedDepth);

    /// <summary>Enough layers for cells a little over the smallest circles at the bottom and the biggest at the top.</summary>
    public int Layers => 1 + Math.Max(0, (int)MathF.Ceiling(MathF.Log2(MaxRadius / MinRadius)));

    public float TopCell => 2.1f * MinRadius * (1 << (Layers - 1));

    public (int Columns, int Rows) Cells(float size) =>
        (Math.Max(1, (int)(Bounds.Width / size)), Math.Max(1, (int)(Bounds.Height / size)));
}

/// <summary>One way of finding a frame's contacts: an index and the search it's used with.</summary>
internal abstract class Contender
{
    public required string Name { get; init; }

    public required string Search { get; init; }

    public required string Threads { get; init; }

    /// <summary>Creates the index and fills it, untimed.</summary>
    public abstract void Setup(Sizing sizing, Crowd crowd);

    /// <summary>Timed as part of the update, before the agents move.</summary>
    public virtual void BeforeStep(Crowd crowd)
    {
    }

    /// <summary>Brings the index up to date with where the agents are now.</summary>
    public abstract void Update(Crowd crowd);

    /// <summary>Works out every agent's contacts.</summary>
    public abstract void FindContacts(Crowd crowd);

    /// <summary>Every way of using every index, in the order the tables list them.</summary>
    public static IEnumerable<Contender> All()
    {
        string cores = Environment.ProcessorCount.ToString();

        yield return new Incremental { Name = "QuadTree", Search = "Query each", Threads = "1", Create = s => new QuadTree(s.Bounds, 8, s.TreeDepth) };
        yield return new Incremental { Name = "LooseQuadTree", Search = "Query each", Threads = "1", Create = s => new LooseQuadTree(s.Bounds, 8, s.TreeDepth) };
        yield return new Incremental
        {
            // Loose cells a bit over a common circle, tight ones four times bigger.
            Name = "LooseTightGrid", Search = "Query each", Threads = "1",
            Create = s =>
            {
                (int columns, int rows) = s.Cells(3.3f * s.CommonRadius);
                (int tightColumns, int tightRows) = s.Cells(13.2f * s.CommonRadius);
                return new LooseTightGrid(s.Bounds, columns, rows, tightColumns, tightRows);
            },
        };
        yield return new Incremental
        {
            // Cells a few times bigger than a common circle; every element counts as the biggest.
            Name = "UniformGrid", Search = "Query each", Threads = "1",
            Create = s =>
            {
                (int columns, int rows) = s.Cells(6.6f * s.CommonRadius);
                return new UniformGrid(s.Bounds, columns, rows, s.MaxRadius, s.MaxRadius);
            },
        };
        yield return new Layered { Name = "LayeredGrid", Search = "ForEachPair", Threads = "1", ThreadCount = 0 };
        yield return new Layered { Name = "LayeredGrid", Search = "ForEachNeighbor", Threads = "4", ThreadCount = 4 };
        yield return new Layered { Name = "LayeredGrid", Search = "ForEachNeighbor", Threads = cores, ThreadCount = Environment.ProcessorCount };

        Func<Sizing, IPackedSpatialIndex> packedGrid = s =>
        {
            (int columns, int rows) = s.Cells(2.1f * s.CommonRadius);
            return new PackedGrid(s.Bounds, columns, rows);
        };
        Func<Sizing, IPackedSpatialIndex> packedQuadTree = s => new PackedQuadTree(s.Bounds, 8, s.TreeDepth);
        yield return new PackedOne { Name = "PackedGrid", Search = "ForEachPair", Threads = "1", Create = packedGrid };
        yield return new PackedAll { Name = "PackedGrid", Search = "ForEachNeighbor", Threads = $"{cores} (pool)", Create = packedGrid };
        yield return new PackedOne { Name = "PackedQuadTree", Search = "ForEachPair", Threads = "1", Create = packedQuadTree };
        yield return new PackedAll { Name = "PackedQuadTree", Search = "ForEachNeighbor", Threads = $"{cores} (pool)", Create = packedQuadTree };

        yield return new PackedOne { Name = "PackedLayeredGrid", Search = "ForEachPair", Threads = "1", Create = s => PackedLayered(s, 1) };
        yield return new PackedAll { Name = "PackedLayeredGrid", Search = "ForEachNeighbor", Threads = "4", Create = s => PackedLayered(s, 4) };
        yield return new PackedAll { Name = "PackedLayeredGrid", Search = "ForEachNeighbor", Threads = cores, Create = s => PackedLayered(s, Environment.ProcessorCount) };

        yield return new PhasedOne { Name = "PhasedGrid", Search = "ForEachPair", Threads = "1", ThreadCount = 1 };
        yield return new PhasedAll { Name = "PhasedGrid", Search = "ForEachPairInParallel", Threads = "4", ThreadCount = 4 };
        yield return new PhasedAll { Name = "PhasedGrid", Search = "ForEachPairInParallel", Threads = cores, ThreadCount = Environment.ProcessorCount };
        yield return new PhasedAll { Name = "PhasedGrid", Search = "ForEachPairInParallel, reordered", Threads = cores, ThreadCount = Environment.ProcessorCount, Reorder = true };
    }

    private static PackedLayeredGrid PackedLayered(Sizing sizing, int threads)
    {
        (int columns, int rows) = sizing.Cells(sizing.TopCell);
        return new PackedLayeredGrid(sizing.Bounds, columns, rows, sizing.Layers, threads);
    }

    /// <summary>An <see cref="ISpatialIndex"/> used as the demo uses it: every agent moved, then queried in turn.</summary>
    private sealed class Incremental : Contender
    {
        private readonly List<int> _candidates = [];
        private ISpatialIndex _index = null!;

        public required Func<Sizing, ISpatialIndex> Create { get; init; }

        public override void Setup(Sizing sizing, Crowd crowd)
        {
            _index = Create(sizing);
            for (int i = 0; i < crowd.Count; i++)
            {
                crowd.Agents[i].Handle = _index.Insert(i, crowd.Boxes[i]);
            }
        }

        public override void Update(Crowd crowd)
        {
            Agent[] agents = crowd.Agents;
            Aabb[] boxes = crowd.Boxes;
            for (int i = 0; i < agents.Length; i++)
            {
                _index.Move(agents[i].Handle, boxes[i]);
            }

            _index.Cleanup();
        }

        public override void FindContacts(Crowd crowd)
        {
            Agent[] agents = crowd.Agents;
            PairContactFinder finder = new(agents, crowd.Contacts);
            for (int i = 0; i < agents.Length; i++)
            {
                _candidates.Clear();
                _index.Query(crowd.Boxes[i], _candidates, agents[i].Handle);
                foreach (int element in _candidates)
                {
                    // Each pair is found from both sides; handle it once.
                    int j = _index.GetId(element);
                    if (j > i)
                    {
                        finder.Visit(i, j);
                    }
                }
            }
        }
    }

    /// <summary>A <see cref="LayeredGrid"/>, moved like the others but searched for every pair at once.</summary>
    private sealed class Layered : Contender
    {
        private LayeredGrid _grid = null!;

        /// <summary>0 searches with ForEachPair on this thread.</summary>
        public required int ThreadCount { get; init; }

        public override void Setup(Sizing sizing, Crowd crowd)
        {
            (int columns, int rows) = sizing.Cells(sizing.TopCell);
            _grid = new LayeredGrid(sizing.Bounds, columns, rows, sizing.Layers, Math.Max(1, ThreadCount));
            for (int i = 0; i < crowd.Count; i++)
            {
                crowd.Agents[i].Handle = _grid.Insert(i, crowd.Boxes[i]);
            }
        }

        public override void Update(Crowd crowd)
        {
            Agent[] agents = crowd.Agents;
            Aabb[] boxes = crowd.Boxes;
            for (int i = 0; i < agents.Length; i++)
            {
                _grid.Move(agents[i].Handle, boxes[i]);
            }

            _grid.Cleanup();
        }

        public override void FindContacts(Crowd crowd)
        {
            if (ThreadCount == 0)
            {
                ByHandle<PairContactFinder> pairs = new(_grid, new PairContactFinder(crowd.Agents, crowd.Contacts));
                _grid.ForEachPair(ref pairs);
            }
            else
            {
                _grid.ForEachNeighbor(new ByHandle<ContactFinder>(_grid, new ContactFinder(crowd.Agents, crowd.Contacts)));
            }
        }
    }

    /// <summary>A packed index rebuilt every frame and searched on this thread, each pair once.</summary>
    private sealed class PackedOne : Contender
    {
        private IPackedSpatialIndex _index = null!;

        public required Func<Sizing, IPackedSpatialIndex> Create { get; init; }

        public override void Setup(Sizing sizing, Crowd crowd) => _index = Create(sizing);

        public override void Update(Crowd crowd) => _index.Build(crowd.Boxes);

        public override void FindContacts(Crowd crowd)
        {
            PairContactFinder finder = new(crowd.Agents, crowd.Contacts);
            _index.ForEachPair(ref finder);
        }
    }

    /// <summary>A packed index rebuilt every frame and searched in parallel, each pair from both sides.</summary>
    private sealed class PackedAll : Contender
    {
        private IPackedSpatialIndex _index = null!;

        public required Func<Sizing, IPackedSpatialIndex> Create { get; init; }

        public override void Setup(Sizing sizing, Crowd crowd) => _index = Create(sizing);

        public override void Update(Crowd crowd) => _index.Build(crowd.Boxes);

        public override void FindContacts(Crowd crowd) => _index.ForEachNeighbor(new ContactFinder(crowd.Agents, crowd.Contacts));
    }

    /// <summary>A <see cref="PhasedGrid"/> searched on this thread, each pair once.</summary>
    private sealed class PhasedOne : Contender
    {
        private PhasedGrid _grid = null!;

        public required int ThreadCount { get; init; }

        public override void Setup(Sizing sizing, Crowd crowd) => _grid = Phased(sizing, ThreadCount);

        public override void Update(Crowd crowd) => _grid.Build(crowd.Boxes);

        public override void FindContacts(Crowd crowd)
        {
            PairContactFinder finder = new(crowd.Agents, crowd.Contacts);
            _grid.ForEachPair(ref finder);
        }
    }

    /// <summary>A <see cref="PhasedGrid"/> searched in parallel, each pair once, optionally keeping the agents in its order.</summary>
    private sealed class PhasedAll : Contender
    {
        private PhasedGrid _grid = null!;

        public required int ThreadCount { get; init; }

        public bool Reorder { get; init; }

        public override void Setup(Sizing sizing, Crowd crowd) => _grid = Phased(sizing, ThreadCount);

        public override void BeforeStep(Crowd crowd)
        {
            if (!Reorder || _grid.Count != crowd.Count)
            {
                return;
            }

            ReadOnlySpan<int> order = _grid.Order;
            Agent[] agents = crowd.Agents, spare = crowd.Spare;
            for (int i = 0; i < order.Length; i++)
            {
                spare[i] = agents[order[i]];
            }

            crowd.Agents = spare;
            crowd.Spare = agents;
        }

        public override void Update(Crowd crowd) => _grid.Build(crowd.Boxes);

        public override void FindContacts(Crowd crowd) => _grid.ForEachPairInParallel(new PairContactFinder(crowd.Agents, crowd.Contacts));
    }

    private static PhasedGrid Phased(Sizing sizing, int threads)
    {
        (int columns, int rows) = sizing.Cells(sizing.TopCell);
        return new PhasedGrid(sizing.Bounds, columns, rows, sizing.Layers, threads);
    }
}
