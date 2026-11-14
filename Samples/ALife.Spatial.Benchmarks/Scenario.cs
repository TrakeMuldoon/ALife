namespace ALife.Spatial.Benchmarks;

/// <summary>
/// A kind of crowd to measure at several sizes. The radii say how the indexes are sized: the grids' cells are fitted to
/// <see cref="CommonRadius"/> or, for the layered grids, run from <see cref="MinRadius"/> to <see cref="MaxRadius"/>.
/// </summary>
internal sealed class Scenario
{
    private const float MinSpeed = 30;
    private const float MaxSpeed = 120;

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required int[] Counts { get; init; }

    public required float MinRadius { get; init; }

    public required float CommonRadius { get; init; }

    public required float MaxRadius { get; init; }

    /// <summary>Picks each agent's radius.</summary>
    public required Func<Random, float> Radius { get; init; }

    /// <summary>The world for agents whose discs add up to an area; null keeps the demo's 1280x720.</summary>
    public float? Coverage { get; init; }

    /// <summary>How much of the crowd is bunched into clumps rather than spread evenly.</summary>
    public float Clumped { get; init; }

    public static IReadOnlyList<Scenario> All { get; } =
    [
        new()
        {
            Name = "Demo world",
            Description = "Circles of radius 3 spread evenly over the demo's fixed 1280×720 world, so it gets more crowded as the count rises: "
                + "at 200,000 every circle overlaps about 24 others. The worst case for the narrow phase; it's what the demo shows.",
            Counts = [1_000, 10_000, 50_000, 100_000, 200_000],
            MinRadius = 3, CommonRadius = 3, MaxRadius = 3,
            Radius = _ => 3,
        },
        new()
        {
            Name = "Even spread",
            Description = "Circles of radius 3 spread evenly, with the world grown to keep the crowding the same (half the area covered, "
                + "about 2 overlaps per circle), as in a big level with more units in it.",
            Counts = [1_000, 10_000, 100_000, 250_000, 1_000_000],
            MinRadius = 3, CommonRadius = 3, MaxRadius = 3,
            Radius = _ => 3,
            Coverage = 0.5f,
        },
        new()
        {
            Name = "Mixed sizes",
            Description = "Radii from 3 to 12, mostly small (as with the demo's V key), spread evenly over a world covered half over.",
            Counts = [1_000, 10_000, 100_000, 250_000, 1_000_000],
            MinRadius = 3, CommonRadius = 6, MaxRadius = 12,
            Radius = random => 3 + MathF.Pow(random.NextSingle(), 3) * 9,
            Coverage = 0.5f,
        },
        new()
        {
            Name = "Clustered",
            Description = "Circles of radius 3, four in five of them bunched into 8 dense clumps and the rest spread thin, over a world "
                + "covered a quarter over on average. Most grid cells sit empty while a few are packed: the case trees are meant for.",
            Counts = [1_000, 10_000, 100_000, 250_000, 1_000_000],
            MinRadius = 3, CommonRadius = 3, MaxRadius = 3,
            Radius = _ => 3,
            Coverage = 0.25f,
            Clumped = 0.8f,
        },
        new()
        {
            Name = "A few giants",
            Description = "Circles of radius 3, except one in a thousand of radius 40 to 80, spread evenly over a world covered half over. "
                + "Tests how each index copes with rare elements far bigger than its cells.",
            Counts = [1_000, 10_000, 100_000, 250_000, 1_000_000],
            MinRadius = 3, CommonRadius = 3, MaxRadius = 80,
            Radius = random => random.NextSingle() < 0.001f ? 40 + random.NextSingle() * 40 : 3,
            Coverage = 0.5f,
        },
    ];

    /// <summary>Makes the crowd for a count, the same every time for the same count.</summary>
    public Crowd Create(int count)
    {
        Random random = new(count);
        float[] radii = new float[count];
        double area = 0;
        for (int i = 0; i < count; i++)
        {
            radii[i] = Radius(random);
            area += Math.PI * radii[i] * radii[i];
        }

        Aabb bounds = new(0, 0, 1280, 720);
        if (Coverage is float coverage)
        {
            // 16:9, like the demo, and never smaller than the biggest circle needs.
            float width = MathF.Max((float)Math.Sqrt(area / coverage * 16 / 9), 4 * MaxRadius);
            bounds = new Aabb(0, 0, width, width * 9 / 16);
        }

        // The clumps sit at fixed places, a fixed fraction of the world across.
        (float X, float Y)[] clumps = [(0.2f, 0.25f), (0.5f, 0.2f), (0.8f, 0.3f), (0.3f, 0.55f), (0.65f, 0.5f), (0.15f, 0.8f), (0.45f, 0.8f), (0.85f, 0.75f)];
        float spread = bounds.Width / 30;
        Agent[] agents = new Agent[count];
        for (int i = 0; i < count; i++)
        {
            float x, y;
            if (random.NextSingle() < Clumped)
            {
                (float cx, float cy) = clumps[random.Next(clumps.Length)];
                x = cx * bounds.Width + Gaussian(random) * spread;
                y = cy * bounds.Height + Gaussian(random) * spread;
            }
            else
            {
                x = random.NextSingle() * bounds.Width;
                y = random.NextSingle() * bounds.Height;
            }

            float radius = radii[i];
            float angle = random.NextSingle() * MathF.Tau;
            float speed = MinSpeed + random.NextSingle() * (MaxSpeed - MinSpeed);
            agents[i] = new Agent
            {
                X = Math.Clamp(x, radius, bounds.Width - radius),
                Y = Math.Clamp(y, radius, bounds.Height - radius),
                Vx = MathF.Cos(angle) * speed,
                Vy = MathF.Sin(angle) * speed,
                Radius = radius,
            };
        }

        return new Crowd(bounds, agents);
    }

    private static float Gaussian(Random random)
    {
        float u = 1 - random.NextSingle(), v = random.NextSingle();
        return MathF.Sqrt(-2 * MathF.Log(u)) * MathF.Cos(MathF.Tau * v);
    }
}
