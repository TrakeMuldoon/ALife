namespace ALife.Spatial.Benchmarks;

/// <summary>A circle moving in a straight line, bouncing off the edges of the world.</summary>
internal struct Agent
{
    public float X, Y, Vx, Vy, Radius;
    public int Handle;
}

/// <summary>What an agent's collisions in one frame add up to: a push, a change of velocity, and whether it touched anything.</summary>
internal struct Contact
{
    public float Dx, Dy, Dvx, Dvy;
    public bool Hit;
}

/// <summary>
/// The agents of one run, with the arrays an index works from. Moving them isn't timed. Contacts are worked out every frame
/// but never applied, so the agents pass through each other and the crowd keeps its shape however long a run takes.
/// </summary>
internal sealed class Crowd
{
    private const float FrameSeconds = 1 / 60f;
    private const int ChunkSize = 8192;

    public Crowd(Aabb bounds, Agent[] agents)
    {
        Bounds = bounds;
        Agents = agents;
        Spare = new Agent[agents.Length];
        Boxes = new Aabb[agents.Length];
        Contacts = new Contact[agents.Length];
        FillBoxes();
    }

    public Aabb Bounds { get; }

    public int Count => Agents.Length;

    public Agent[] Agents { get; set; }

    /// <summary>A second array of agents, for indexes that reorder them.</summary>
    public Agent[] Spare { get; set; }

    public Aabb[] Boxes { get; }

    public Contact[] Contacts { get; }

    public static Aabb BoundsOf(in Agent agent) => Aabb.FromCenter(agent.X, agent.Y, agent.Radius, agent.Radius);

    /// <summary>Moves every agent one frame on, then works out its box and clears its contact.</summary>
    public void Step()
    {
        Agent[] agents = Agents;
        Aabb[] boxes = Boxes;
        Contact[] contacts = Contacts;
        Aabb bounds = Bounds;
        Parallel.For(0, (Count + ChunkSize - 1) / ChunkSize, chunk =>
        {
            int end = Math.Min((chunk + 1) * ChunkSize, agents.Length);
            for (int i = chunk * ChunkSize; i < end; i++)
            {
                ref Agent agent = ref agents[i];
                agent.X += agent.Vx * FrameSeconds;
                agent.Y += agent.Vy * FrameSeconds;
                if (agent.X < bounds.Left + agent.Radius || agent.X > bounds.Right - agent.Radius)
                {
                    agent.Vx = -agent.Vx;
                    agent.X = Math.Clamp(agent.X, bounds.Left + agent.Radius, bounds.Right - agent.Radius);
                }

                if (agent.Y < bounds.Top + agent.Radius || agent.Y > bounds.Bottom - agent.Radius)
                {
                    agent.Vy = -agent.Vy;
                    agent.Y = Math.Clamp(agent.Y, bounds.Top + agent.Radius, bounds.Bottom - agent.Radius);
                }

                boxes[i] = BoundsOf(agent);
                contacts[i] = default;
            }
        });
    }

    public void FillBoxes()
    {
        for (int i = 0; i < Count; i++)
        {
            Boxes[i] = BoundsOf(Agents[i]);
        }
    }

    /// <summary>How many pairs of circles (not just boxes) overlap.</summary>
    public long CountTouchingPairs()
    {
        Phased.PhasedGrid grid = new(Bounds, Math.Max(1, (int)(Bounds.Width / 64)), Math.Max(1, (int)(Bounds.Height / 64)), layers: 4, threads: 1);
        grid.Build(Boxes);
        TouchCounter counter = new(Agents);
        grid.ForEachPair(ref counter);
        return counter.Count;
    }

    private struct TouchCounter(Agent[] agents) : Packed.IPairVisitor
    {
        public long Count;

        public void Visit(int a, int b)
        {
            ref readonly Agent self = ref agents[a];
            ref readonly Agent other = ref agents[b];
            float dx = other.X - self.X, dy = other.Y - self.Y, reach = self.Radius + other.Radius;
            if (dx * dx + dy * dy < reach * reach)
            {
                Count++;
            }
        }
    }
}
