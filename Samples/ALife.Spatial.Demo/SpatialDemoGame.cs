using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ALife.Spatial.Collisions;
using ALife.Spatial.Packed;
using ALife.Spatial.Phased;
using ALife.Spatial.Physics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Demo;

/// <summary>
/// Moving circles whose overlaps are found through a spatial index, with the index's nodes drawn underneath and a query box
/// that follows the mouse. Keys 1-4 pick an <see cref="ISpatialIndex"/>, kept up to date as circles move and searched one
/// circle at a time; 5 and 6 pick an <see cref="IPackedSpatialIndex"/>, rebuilt each frame and searched for every circle at
/// once on all cores, which is what takes them to hundreds of thousands. 7 and 8 pick the layered grids, one of each kind,
/// whose pair searches are split by top cell over a set number of threads. 9 picks the <see cref="PhasedGrid"/>, which
/// finds each pair once and lets both circles' contacts be written in the same call, and keeps the circles in the order it
/// sorted them. 0 picks ALife's original collision grid, rebuilt as <see cref="OriginalCollisionGrid"/>, which works like
/// keys 1-4. Tab and Shift+Tab, or . and comma, step forward and back through all ten. Holding the left button sprays circles around the cursor and holding the right button erases them. Left and
/// Right shrink and grow the circles, [ and ] change the trees' maximum depth or the grids' layers, and - and = the layered
/// grids' threads. O switches between running each step while the frame before it is shown, the default, and running it
/// before drawing. S cycles the shapes: circles, then simple shapes, then simple and compound shapes together, which turn
/// and spin off each other and the walls through <see cref="Collide"/> and <see cref="ContactResolver"/>. Each contact
/// between shapes changes both, so in those modes only the <see cref="PhasedGrid"/> resolves them on several threads; the
/// other indexes go through their pairs on one. Stats and controls are in the window title, so no content pipeline is needed.
/// </summary>
public sealed class SpatialDemoGame : Game
{
    private const int WorldWidth = 1280;
    private const int WorldHeight = 720;
    private const float SmallRadius = 3;
    private const float MaxRadius = 12;
    private const int MinAgents = 125;
    private const float BrushRadius = 50;
    private const float SpawnPerSecond = 400;
    private const float RemovePerSecond = 800;
    private const float SizeStep = 1.25f;
    private const float MinSizeScale = 0.25f;
    private const float MaxSizeScale = 8;
    private const int MaxTreeDepth = 16;
    private const int MaxDemoLayers = 8;
    private const int ChunkSize = 4096;
    private const float Restitution = 1;
    private const float Friction = 0.2f;
    private const float WallThickness = 200;

    // What a frame's drawing is split into, for the title and the profile.
    private static readonly string[] DrawPartNames = ["nodes", "agents", "submit", "present"];

    private static readonly string[] Kinds = ["QuadTree", "LooseQuadTree", "LooseTightGrid", "UniformGrid", "PackedGrid", "PackedQuadTree", "LayeredGrid", "PackedLayeredGrid", "PhasedGrid", "CollisionGrid (original)"];

    private static readonly string[] ShapeModes = ["circles", "shapes", "compounds"];

    // Top, bottom, left and right, just outside the world, so the shapes' bounces off them turn them too.
    private static readonly Shape[] WallShapes =
    [
        Shape.Box(WorldWidth + (2 * WallThickness), WallThickness), Shape.Box(WorldWidth + (2 * WallThickness), WallThickness),
        Shape.Box(WallThickness, WorldHeight + (2 * WallThickness)), Shape.Box(WallThickness, WorldHeight + (2 * WallThickness)),
    ];

    private static readonly RigidBody[] Walls =
    [
        RigidBody.Static(new(WorldWidth / 2f, -WallThickness / 2)), RigidBody.Static(new(WorldWidth / 2f, WorldHeight + (WallThickness / 2))),
        RigidBody.Static(new(-WallThickness / 2, WorldHeight / 2f)), RigidBody.Static(new(WorldWidth + (WallThickness / 2), WorldHeight / 2f)),
    ];

    private static readonly Color[] DepthColors =
    [
        new(90, 110, 140), new(70, 140, 200), new(60, 180, 170), new(110, 190, 90),
        new(200, 190, 70), new(230, 140, 60), new(230, 90, 90), new(190, 90, 200),
    ];

    private static readonly Color AgentColor = new(200, 210, 225);
    private static readonly Color CollidingColor = new(255, 120, 90);
    private static readonly Color HighlightColor = new(255, 220, 60);

    private static readonly Aabb World = new(0, 0, WorldWidth, WorldHeight);

    private readonly GraphicsDeviceManager _graphics;
    private readonly Random _random = new(42);
    private readonly List<int> _candidates = [];
    private readonly List<int> _mouseResults = [];
    private readonly List<int> _doomed = [];
    private readonly Stopwatch _stepTimer = new();
    private readonly Stopwatch _drawTimer = new();
    private readonly double[] _drawParts = new double[DrawPartNames.Length];
    private readonly double _profileSeconds;

    private QuadBatch _batch = null!;
    private Texture2D _circle = null!;
    private QuadBatch _shapeBatch = null!;
    private Texture2D _atlas = null!;
    private ShapeSet _shapeSet = new(SmallRadius, MaxRadius);

    // One of these is set, depending on the kind.
    private ISpatialIndex? _index;
    private IPackedSpatialIndex? _packed;

    private Agent[] _agents = [];
    private Agent[] _spareAgents = [];
    private Aabb[] _boxes = [];
    private CircleContact[] _contacts = [];
    private bool _packedStale;
    private int _agentCount;
    private int _kind;
    private bool _variedSizes;
    private float _sizeScale = 1;
    private int _maxDepth = 8;
    private int _layers = 4;
    private int _threads = 4;
    private bool _bounce;
    private int _shapeMode;
    private bool _drawNodes = true;
    private bool _paused;
    private bool _overlap = true;
    private Task? _stepping;
    private float _dt;
    private Aabb _mouseArea = Aabb.Empty;
    private Vector2 _mouse;
    private bool _spawning, _removing;
    private float _brushBudget;
    private KeyboardState _previousKeys;
    private double _stepMilliseconds;
    private double _drawMilliseconds;
    private double _titleTimer;
    private int _frames;
    private long _drawPartStart;
    private double _profileElapsed;
    private int _profileFrames;
    private double _profileStep;
    private double _profileWait;
    private double _profileTitle;
    private int _profileTitles;
    private readonly double[] _profileParts = new double[DrawPartNames.Length];

    /// <summary>Sets up a 1280x720 window.</summary>
    /// <param name="kind">The index to start with, from 1 to 10: 1 to 9 as on the keys, and 10 for the original grid on 0.</param>
    /// <param name="agentCount">How many circles to start with.</param>
    /// <param name="profileSeconds">If above 0, runs for a second to warm up and then this long, prints the average timings and quits.</param>
    public SpatialDemoGame(int kind = 1, int agentCount = 2000, double profileSeconds = 0)
    {
        _profileSeconds = profileSeconds;
        _kind = Math.Clamp(kind, 1, Kinds.Length) - 1;
        _agentCount = Math.Max(agentCount, MinAgents);
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WorldWidth,
            PreferredBackBufferHeight = WorldHeight,
            SynchronizeWithVerticalRetrace = false,
        };
        IsFixedTimeStep = false;
        IsMouseVisible = true;
    }

    // Debug builds and an attached debugger are several times slower, which is easy to mistake for the index or the drawing.
#if DEBUG
    private static string BuildWarning => " | DEBUG BUILD: run with -c Release";
#else
    private static string BuildWarning => Debugger.IsAttached ? " | DEBUGGER ATTACHED" : "";
#endif

    /// <summary>
    /// Whether each step runs on other threads while the frame before it is sent to the GPU and shown, rather than before
    /// it's drawn. On by default; the frame then takes about as long as the slower of the two rather than both together.
    /// </summary>
    public bool OverlapStep
    {
        get => _overlap;
        set => _overlap = value;
    }

    /// <summary>Whether the index's nodes are drawn under the circles, as the N key toggles.</summary>
    public bool DrawNodes
    {
        get => _drawNodes;
        set => _drawNodes = value;
    }

    /// <summary>Whether overlapping circles or shapes push apart and bounce, as the C key toggles; off by default.</summary>
    public bool Bounce
    {
        get => _bounce;
        set => _bounce = value;
    }

    /// <summary>Which shapes there are, as the S key cycles: 0 for circles, 1 for simple shapes, 2 for those and compounds.</summary>
    public int ShapeMode
    {
        get => _shapeMode;
        set => _shapeMode = Math.Clamp(value, 0, ShapeModes.Length - 1);
    }

    private bool ShapesOn => _shapeMode > 0;

    private bool HasDepth => _kind is 0 or 1 or 5;

    private bool HasLayers => _kind is 6 or 7 or 8;

    /// <inheritdoc />
    protected override void LoadContent()
    {
        _circle = CreateCircleTexture(GraphicsDevice, 64);
        _batch = new QuadBatch(GraphicsDevice, _circle);
        _atlas = ShapeSet.CreateAtlas(GraphicsDevice);
        _shapeBatch = new QuadBatch(GraphicsDevice, _atlas);
        Reset();
    }

    /// <inheritdoc />
    protected override void UnloadContent()
    {
        FinishStep();
        _batch.Dispose();
        _circle.Dispose();
        _shapeBatch.Dispose();
        _atlas.Dispose();
    }

    /// <inheritdoc />
    protected override void Update(GameTime gameTime)
    {
        // Everything below reads or changes the circles or the index, so the step started while the last frame was shown
        // has to be done first.
        long waitStart = Stopwatch.GetTimestamp();
        FinishStep();
        _profileWait += Stopwatch.GetElapsedTime(waitStart).TotalMilliseconds;

        HandleKeys(Keyboard.GetState());

        _dt = MathF.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1 / 30f);
        HandleMouse(Mouse.GetState(), _dt);

        if (!_paused && !_overlap)
        {
            RunStep(_dt);
        }

        _mouseArea = Aabb.FromCenter(_mouse.X, _mouse.Y, 90, 60);
        QueryAgents(_mouseArea, _mouseResults);

        if (_profileSeconds > 0)
        {
            Profile(gameTime.ElapsedGameTime.TotalSeconds);
        }

        _frames++;
        _titleTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_titleTimer >= 0.5)
        {
            long titleStart = Stopwatch.GetTimestamp();
            UpdateTitle(_frames / _titleTimer);
            _profileTitle += Stopwatch.GetElapsedTime(titleStart).TotalMilliseconds;
            _profileTitles++;
            _frames = 0;
            _titleTimer = 0;
        }

        base.Update(gameTime);
    }

    /// <inheritdoc />
    protected override void Draw(GameTime gameTime)
    {
        _drawTimer.Restart();
        _drawPartStart = Stopwatch.GetTimestamp();
        GraphicsDevice.Clear(new Color(18, 20, 26));

        if (_drawNodes)
        {
            VisitNodes(DrawNode);
        }

        EndDrawPart(0);

        if (_spawning || _removing)
        {
            Color brush = _spawning ? new Color(90, 200, 120) : new Color(230, 80, 80);
            DrawCircle(_mouse.X, _mouse.Y, BrushRadius, brush * 0.2f);
        }

        // Every circle or shape is written straight into its batch, a chunk per thread. Shapes are drawn after everything
        // in the other batch, the mouse's box included.
        Agent[] agents = _agents;
        if (ShapesOn)
        {
            int first = _shapeBatch.Reserve(_agentCount);
            QuadBatch.Vertex[] vertices = _shapeBatch.Vertices;
            ForChunks(_agentCount, (start, end) =>
            {
                for (int i = start; i < end; i++)
                {
                    WriteShape(vertices, first + i, agents[i], agents[i].Colliding ? CollidingColor : AgentColor);
                }
            });

            foreach (int i in _mouseResults)
            {
                // Reserved first: it can swap the vertex array for a bigger one.
                int quad = _shapeBatch.Reserve(1);
                WriteShape(_shapeBatch.Vertices, quad, _agents[i], HighlightColor);
            }
        }
        else
        {
            int first = _batch.Reserve(_agentCount);
            QuadBatch.Vertex[] vertices = _batch.Vertices;
            ForChunks(_agentCount, (start, end) =>
            {
                for (int i = start; i < end; i++)
                {
                    ref Agent agent = ref agents[i];
                    QuadBatch.WriteSprite(vertices, first + i, agent.X - agent.Radius, agent.Y - agent.Radius, agent.X + agent.Radius, agent.Y + agent.Radius,
                        agent.Colliding ? CollidingColor : AgentColor);
                }
            });

            foreach (int i in _mouseResults)
            {
                ref Agent agent = ref _agents[i];
                DrawCircle(agent.X, agent.Y, agent.Radius, HighlightColor);
            }
        }

        DrawOutline(_mouseArea, HighlightColor, 2);

        // What's drawn is all in the batch now, so the next step can move the circles while the batch goes to the GPU.
        if (!_paused && _overlap)
        {
            float dt = _dt;
            _stepping = Task.Run(() => RunStep(dt));
        }

        EndDrawPart(1);
        _batch.Flush();
        _shapeBatch.Flush();
        base.Draw(gameTime);
        EndDrawPart(2);
    }

    /// <inheritdoc />
    protected override void EndDraw()
    {
        // Presenting is counted as drawing, since that's where waiting for the GPU shows up.
        base.EndDraw();
        EndDrawPart(3);
        _drawTimer.Stop();
        _drawMilliseconds = _drawMilliseconds * 0.9 + _drawTimer.Elapsed.TotalMilliseconds * 0.1;
    }

    private void RunStep(float dt)
    {
        _stepTimer.Restart();
        if (_packed is PhasedGrid phased)
        {
            StepPhased(phased, dt);
        }
        else if (_packed is not null)
        {
            StepPacked(_packed, dt);
        }
        else if (_index is LayeredGrid layered)
        {
            StepLayered(layered, dt);
        }
        else
        {
            Step(_index!, dt);
        }

        _stepTimer.Stop();
        _stepMilliseconds = _stepMilliseconds * 0.9 + _stepTimer.Elapsed.TotalMilliseconds * 0.1;
    }

    // Waits for a step running alongside the drawing, if there is one, passing on anything it threw.
    private void FinishStep()
    {
        if (_stepping is not null)
        {
            Task stepping = _stepping;
            _stepping = null;
            stepping.GetAwaiter().GetResult();
        }
    }

    // Adds the time since the last part ended to a part of the drawing, smoothed like the other timings.
    private void EndDrawPart(int part)
    {
        long now = Stopwatch.GetTimestamp();
        double milliseconds = Stopwatch.GetElapsedTime(_drawPartStart, now).TotalMilliseconds;
        _drawParts[part] = _drawParts[part] * 0.9 + milliseconds * 0.1;
        _profileParts[part] += milliseconds;
        _drawPartStart = now;
    }

    // Skips the first second, then adds up timings until the profile's time is up, prints their averages and quits.
    private void Profile(double seconds)
    {
        _profileElapsed += seconds;
        if (_profileElapsed < 1)
        {
            Array.Clear(_profileParts);
            _profileStep = 0;
            _profileWait = 0;
            _profileTitle = 0;
            _profileTitles = 0;
            _profileFrames = 0;
            return;
        }

        _profileFrames++;
        _profileStep += _stepTimer.Elapsed.TotalMilliseconds;
        if (_profileElapsed < 1 + _profileSeconds)
        {
            return;
        }

        int nodes = 0;
        VisitNodes((_, _, _, _) => nodes++);
        string parts = string.Join(", ", DrawPartNames.Select((name, i) => $"{name} {_profileParts[i] / _profileFrames:0.00}"));
        Console.WriteLine($"{Kinds[_kind]} | {_agentCount:N0} {ShapeModes[_shapeMode]} | {nodes:N0} nodes {(_drawNodes ? "drawn" : "hidden")} | {_profileFrames / _profileSeconds:0} fps | "
            + $"step {_profileStep / _profileFrames:0.00} ms{(_overlap ? $" (waited for {_profileWait / _profileFrames:0.00})" : "")} | draw {_profileParts.Sum() / _profileFrames:0.00} ms ({parts}) | "
            + $"title {_profileTitle / Math.Max(1, _profileTitles):0.00} ms, {_profileTitles} times");
        Exit();
    }

    // Moves every agent, updates the index, then finds overlaps by querying each agent's box and, with bouncing on, resolves them.
    private void Step(ISpatialIndex index, float dt)
    {
        bool shapes = ShapesOn;
        for (int i = 0; i < _agentCount; i++)
        {
            ref Agent agent = ref _agents[i];
            if (shapes)
            {
                MoveShape(ref agent, dt, _shapeSet);
            }
            else
            {
                Move(ref agent, dt);
                agent.Colliding = false;
            }

            index.Move(agent.Handle, BoundsOf(agent));
        }

        for (int i = 0; i < _agentCount; i++)
        {
            ref Agent a = ref _agents[i];
            _candidates.Clear();
            index.Query(BoundsOf(a), _candidates, a.Handle);
            foreach (int element in _candidates)
            {
                // Each pair is found from both sides; handle it once.
                int j = index.GetId(element);
                if (j > i && shapes)
                {
                    CollideShapes(ref a, ref _agents[j], _shapeSet, _bounce);
                }
                else if (j > i)
                {
                    CollideCircles(ref a, ref _agents[j], _bounce);
                }
            }
        }

        index.Cleanup();
    }

    // Moves every agent and rebuilds the index, then works out every agent's push and bounce from where everything was, in
    // parallel, and applies them all at once. Each agent only ever writes its own contact, so nothing needs locking. Shapes
    // are resolved a pair at a time on this thread instead, since each contact changes both.
    private void StepPacked(IPackedSpatialIndex packed, float dt)
    {
        Agent[] agents = _agents;
        MoveAll(dt);
        packed.Build(_boxes.AsSpan(0, _agentCount));
        _packedStale = false;
        if (ShapesOn)
        {
            ShapeResolver resolver = new(agents, _shapeSet, _bounce);
            packed.ForEachPair(ref resolver);
            return;
        }

        packed.ForEachNeighbor(new ContactFinder(agents, _contacts, _bounce));
        ApplyContacts();
    }

    // Moves every agent and works out its box, in parallel.
    private void MoveAll(float dt)
    {
        Agent[] agents = _agents;
        Aabb[] boxes = _boxes;
        ShapeSet? shapes = ShapesOn ? _shapeSet : null;
        ForChunks(_agentCount, (start, end) =>
        {
            for (int i = start; i < end; i++)
            {
                ref Agent agent = ref agents[i];
                if (shapes is not null)
                {
                    MoveShape(ref agent, dt, shapes);
                }
                else
                {
                    Move(ref agent, dt);
                }

                boxes[i] = BoundsOf(agent);
            }
        });
    }

    // Moves every agent and its element in the grid, which has to be done one at a time, then finds and applies the pushes
    // and bounces as StepPacked does.
    private void StepLayered(LayeredGrid grid, float dt)
    {
        for (int i = 0; i < _agentCount; i++)
        {
            ref Agent agent = ref _agents[i];
            if (ShapesOn)
            {
                MoveShape(ref agent, dt, _shapeSet);
            }
            else
            {
                Move(ref agent, dt);
            }

            grid.Move(agent.Handle, BoundsOf(agent));
        }

        grid.Cleanup();
        if (ShapesOn)
        {
            HandleVisitor<ShapeResolver> resolver = new(grid, new ShapeResolver(_agents, _shapeSet, _bounce));
            grid.ForEachPair(ref resolver);
            return;
        }

        grid.ForEachNeighbor(new HandleVisitor<ContactFinder>(grid, new ContactFinder(_agents, _contacts, _bounce)));
        ApplyContacts();
    }

    // As StepPacked, but each pair comes up once with both its agents' contacts to write. First the agents are put in the
    // order the grid sorted them last step, so that circles near each other sit near each other in memory. They've barely
    // moved since, so it's nearly this step's order too.
    private void StepPhased(PhasedGrid grid, float dt)
    {
        if (!_packedStale && grid.Count == _agentCount)
        {
            ReadOnlySpan<int> order = grid.Order;
            for (int i = 0; i < order.Length; i++)
            {
                _spareAgents[i] = _agents[order[i]];
            }

            (_agents, _spareAgents) = (_spareAgents, _agents);
        }

        Agent[] agents = _agents;
        MoveAll(dt);
        grid.Build(_boxes.AsSpan(0, _agentCount));
        _packedStale = false;
        if (ShapesOn)
        {
            grid.ForEachPairInParallel(new ShapeResolver(agents, _shapeSet, _bounce));
            return;
        }

        grid.ForEachPairInParallel(new PairContactFinder(agents, _contacts, _bounce));
        ApplyContacts();
    }

    // Applies and clears every agent's contact, in parallel.
    private void ApplyContacts()
    {
        Agent[] agents = _agents;
        CircleContact[] contacts = _contacts;
        ForChunks(_agentCount, (start, end) =>
        {
            for (int i = start; i < end; i++)
            {
                ref Agent agent = ref agents[i];
                ref CircleContact contact = ref contacts[i];
                agent.X += contact.Dx;
                agent.Y += contact.Dy;
                agent.Vx += contact.Dvx;
                agent.Vy += contact.Dvy;
                agent.Colliding = contact.Hit;
                contact = default;
            }
        });
    }

    // Splits 0 to count into chunks and runs them on all cores.
    private static void ForChunks(int count, Action<int, int> body) =>
        Parallel.For(0, (count + ChunkSize - 1) / ChunkSize, chunk => body(chunk * ChunkSize, Math.Min(count, (chunk + 1) * ChunkSize)));

    private static void Move(ref Agent agent, float dt)
    {
        agent.X += agent.Vx * dt;
        agent.Y += agent.Vy * dt;
        BounceOffWall(ref agent.X, ref agent.Vx, agent.Radius, WorldWidth);
        BounceOffWall(ref agent.Y, ref agent.Vy, agent.Radius, WorldHeight);
    }

    private static void BounceOffWall(ref float position, ref float velocity, float radius, float size)
    {
        if (position < radius)
        {
            position = radius;
            velocity = MathF.Abs(velocity);
        }
        else if (position > size - radius)
        {
            position = size - radius;
            velocity = -MathF.Abs(velocity);
        }
    }

    // Moves and turns a shape, then bounces it off any wall it's reached, which can set it spinning.
    private static void MoveShape(ref Agent agent, float dt, ShapeSet set)
    {
        agent.X += agent.Vx * dt;
        agent.Y += agent.Vy * dt;
        agent.Angle = MathF.IEEERemainder(agent.Angle + (agent.Spin * dt), MathF.Tau);
        agent.Colliding = false;
        float radius = agent.Radius;
        if (agent.X > radius && agent.X < WorldWidth - radius && agent.Y > radius && agent.Y < WorldHeight - radius)
        {
            return;
        }

        Shape shape = set.Shapes[agent.Shape];
        RigidBody body = BodyOf(agent, set);
        for (int i = 0; i < Walls.Length; i++)
        {
            RigidBody wall = Walls[i];
            if (Collide.Test(WallShapes[i], wall.Pose, shape, body.Pose, out Contact contact))
            {
                ContactResolver.Resolve(ref wall, ref body, contact, Restitution, Friction);
            }
        }

        Store(ref agent, body);
    }

    // Marks overlapping shapes and, if bouncing, pushes them apart and bounces them, spinning them if the hit is off center.
    private static void CollideShapes(ref Agent a, ref Agent b, ShapeSet set, bool bounce)
    {
        RigidBody first = BodyOf(a, set), second = BodyOf(b, set);
        if (!Collide.Test(set.Shapes[a.Shape], first.Pose, set.Shapes[b.Shape], second.Pose, out Contact contact))
        {
            return;
        }

        a.Colliding = true;
        b.Colliding = true;
        if (bounce)
        {
            ContactResolver.Resolve(ref first, ref second, contact, Restitution, Friction);
            Store(ref a, first);
            Store(ref b, second);
        }
    }

    private static RigidBody BodyOf(in Agent agent, ShapeSet set) => new()
    {
        Position = new(agent.X, agent.Y),
        Angle = agent.Angle,
        Velocity = new(agent.Vx, agent.Vy),
        AngularVelocity = agent.Spin,
        InverseMass = set.InverseMass[agent.Shape],
        InverseInertia = set.InverseInertia[agent.Shape],
    };

    private static void Store(ref Agent agent, in RigidBody body)
    {
        agent.X = body.Position.X;
        agent.Y = body.Position.Y;
        agent.Angle = body.Angle;
        agent.Vx = body.Velocity.X;
        agent.Vy = body.Velocity.Y;
        agent.Spin = body.AngularVelocity;
    }

    // Marks overlapping circles and, if bouncing, pushes them apart and bounces them elastically, heavier (bigger) ones
    // moving less.
    private static void CollideCircles(ref Agent a, ref Agent b, bool bounce)
    {
        float dx = b.X - a.X, dy = b.Y - a.Y;
        float reach = a.Radius + b.Radius;
        float distanceSquared = dx * dx + dy * dy;
        if (distanceSquared >= reach * reach)
        {
            return;
        }

        a.Colliding = true;
        b.Colliding = true;
        if (!bounce || distanceSquared == 0)
        {
            return;
        }

        float distance = MathF.Sqrt(distanceSquared);
        float nx = dx / distance, ny = dy / distance;
        float massA = a.Radius * a.Radius, massB = b.Radius * b.Radius;
        float shareA = massB / (massA + massB), shareB = massA / (massA + massB);
        float overlap = reach - distance;
        a.X -= nx * overlap * shareA;
        a.Y -= ny * overlap * shareA;
        b.X += nx * overlap * shareB;
        b.Y += ny * overlap * shareB;

        float closing = (b.Vx - a.Vx) * nx + (b.Vy - a.Vy) * ny;
        if (closing < 0)
        {
            float impulse = 2 * closing;
            a.Vx += impulse * shareA * nx;
            a.Vy += impulse * shareA * ny;
            b.Vx -= impulse * shareB * nx;
            b.Vy -= impulse * shareB * ny;
        }
    }

    private void HandleKeys(KeyboardState keys)
    {
        bool Pressed(Keys key) => keys.IsKeyDown(key) && !_previousKeys.IsKeyDown(key);

        if (keys.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Keys[] numbers = [Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0];
        for (int k = 0; k < numbers.Length; k++)
        {
            if (Pressed(numbers[k]) && k != _kind)
            {
                _kind = k;
                RebuildIndex();
            }
        }

        bool shift = keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift);
        if ((Pressed(Keys.Tab) && !shift) || Pressed(Keys.OemPeriod))
        {
            _kind = (_kind + 1) % Kinds.Length;
            RebuildIndex();
        }

        if ((Pressed(Keys.Tab) && shift) || Pressed(Keys.OemComma))
        {
            _kind = (_kind + Kinds.Length - 1) % Kinds.Length;
            RebuildIndex();
        }

        if (Pressed(Keys.Up))
        {
            _agentCount = Math.Max(_agentCount * 2, MinAgents);
            Reset();
        }

        if (Pressed(Keys.Down) && _agentCount > MinAgents)
        {
            _agentCount = Math.Max(_agentCount / 2, MinAgents);
            Reset();
        }

        if (Pressed(Keys.Left))
        {
            Rescale(_sizeScale / SizeStep);
        }

        if (Pressed(Keys.Right))
        {
            Rescale(_sizeScale * SizeStep);
        }

        // Only the trees have a depth and the layered grids layers; the other grids are one level.
        if (HasDepth && Pressed(Keys.OemOpenBrackets) && _maxDepth > 0)
        {
            _maxDepth--;
            RebuildIndex();
        }

        if (HasDepth && Pressed(Keys.OemCloseBrackets) && _maxDepth < MaxTreeDepth)
        {
            _maxDepth++;
            RebuildIndex();
        }

        if (HasLayers && Pressed(Keys.OemOpenBrackets) && _layers > 1)
        {
            _layers--;
            RebuildIndex();
        }

        if (HasLayers && Pressed(Keys.OemCloseBrackets) && _layers < MaxDemoLayers)
        {
            _layers++;
            RebuildIndex();
        }

        if (HasLayers && Pressed(Keys.OemMinus) && _threads > 1)
        {
            _threads--;
            SetThreads();
        }

        if (HasLayers && Pressed(Keys.OemPlus) && _threads < Environment.ProcessorCount)
        {
            _threads++;
            SetThreads();
        }

        if (Pressed(Keys.V))
        {
            _variedSizes = !_variedSizes;
            Reset();
        }

        if (Pressed(Keys.S))
        {
            _shapeMode = (_shapeMode + 1) % ShapeModes.Length;
            Reset();
        }

        if (Pressed(Keys.C))
        {
            _bounce = !_bounce;
        }

        if (Pressed(Keys.N))
        {
            _drawNodes = !_drawNodes;
        }

        if (Pressed(Keys.O))
        {
            _overlap = !_overlap;
        }

        if (Pressed(Keys.Space))
        {
            _paused = !_paused;
        }

        if (Pressed(Keys.R))
        {
            Reset();
        }

        _previousKeys = keys;
    }

    // While a button is held over the window, sprays circles into (left) or erases them from (right) the brush, at a steady
    // rate, so a longer hold does more.
    private void HandleMouse(MouseState mouse, float dt)
    {
        _mouse = new Vector2(mouse.X, mouse.Y);
        bool inside = IsActive && World.Contains(mouse.X, mouse.Y);
        _spawning = inside && mouse.LeftButton == ButtonState.Pressed;
        _removing = inside && !_spawning && mouse.RightButton == ButtonState.Pressed;
        if (!_spawning && !_removing)
        {
            _brushBudget = 0;
            return;
        }

        _brushBudget += (_spawning ? SpawnPerSecond : RemovePerSecond) * dt;
        int count = (int)_brushBudget;
        _brushBudget -= count;
        if (_spawning)
        {
            Spawn(count);
        }
        else
        {
            Erase(count);
        }
    }

    private void Spawn(int count)
    {
        for (int k = 0; k < count; k++)
        {
            // Uniform over the disc: the square root keeps them from bunching at the middle.
            float angle = _random.NextSingle() * MathF.Tau;
            float distance = BrushRadius * MathF.Sqrt(_random.NextSingle());
            Agent agent = CreateAgent(_mouse.X + MathF.Cos(angle) * distance, _mouse.Y + MathF.Sin(angle) * distance);
            agent.X = Math.Clamp(agent.X, agent.Radius, WorldWidth - agent.Radius);
            agent.Y = Math.Clamp(agent.Y, agent.Radius, WorldHeight - agent.Radius);

            if (_agentCount == _agents.Length)
            {
                Grow(Math.Max(16, _agents.Length * 2));
            }

            if (_index is not null)
            {
                agent.Handle = _index.Insert(_agentCount, BoundsOf(agent));
            }

            _agents[_agentCount++] = agent;
            _packedStale = true;
        }
    }

    // Removes up to count of the circles whose centers are in the brush.
    private void Erase(int count)
    {
        QueryAgents(Aabb.FromCenter(_mouse.X, _mouse.Y, BrushRadius, BrushRadius), _doomed);
        _doomed.RemoveAll(i => Vector2.DistanceSquared(new Vector2(_agents[i].X, _agents[i].Y), _mouse) > BrushRadius * BrushRadius);

        // Highest first: each removal moves the last agent into the gap, and the last is never one still to be removed.
        _doomed.Sort();
        for (int k = _doomed.Count - 1; k >= 0 && k >= _doomed.Count - count; k--)
        {
            RemoveAgent(_doomed[k]);
        }
    }

    private void RemoveAgent(int i)
    {
        _index?.Remove(_agents[i].Handle);
        _packedStale = true;
        int last = --_agentCount;
        if (i == last)
        {
            return;
        }

        // The index knows agents by array position, so the agent moved into the gap is filed again under its new one.
        ref Agent moved = ref _agents[i];
        moved = _agents[last];
        if (_index is not null)
        {
            _index.Remove(moved.Handle);
            moved.Handle = _index.Insert(i, BoundsOf(moved));
        }
    }

    // Fills a list with the agents whose boxes intersect an area.
    private void QueryAgents(Aabb area, List<int> agents)
    {
        agents.Clear();
        if (_index is null)
        {
            // A packed index's elements are the agents' positions in the array it was built from.
            if (_packedStale)
            {
                BuildPacked();
            }

            _packed!.Query(area, agents);
            return;
        }

        _candidates.Clear();
        _index.Query(area, _candidates);
        foreach (int element in _candidates)
        {
            agents.Add(_index.GetId(element));
        }
    }

    private void VisitNodes(SpatialNodeVisitor visitor)
    {
        if (_index is not null)
        {
            _index.VisitNodes(visitor);
        }
        else
        {
            _packed!.VisitNodes(visitor);
        }
    }

    private void Reset()
    {
        _agents = [];
        Grow(Math.Max(16, _agentCount));
        for (int i = 0; i < _agentCount; i++)
        {
            _agents[i] = CreateAgent(_random.NextSingle() * WorldWidth, _random.NextSingle() * WorldHeight);
            ref Agent agent = ref _agents[i];
            agent.X = Math.Clamp(agent.X, agent.Radius, WorldWidth - agent.Radius);
            agent.Y = Math.Clamp(agent.Y, agent.Radius, WorldHeight - agent.Radius);
        }

        RebuildIndex();
    }

    private void Grow(int capacity)
    {
        Array.Resize(ref _agents, capacity);
        Array.Resize(ref _spareAgents, capacity);
        Array.Resize(ref _boxes, capacity);
        Array.Resize(ref _contacts, capacity);
    }

    // Grows or shrinks every circle, keeping them on screen, and rebuilds the index for the new sizes.
    private void Rescale(float scale)
    {
        scale = Math.Clamp(scale, MinSizeScale, MaxSizeScale);
        float factor = scale / _sizeScale;
        if (factor == 1)
        {
            return;
        }

        _sizeScale = scale;
        _shapeSet = new ShapeSet(SmallRadius * scale, MaxRadius * scale);
        for (int i = 0; i < _agentCount; i++)
        {
            ref Agent agent = ref _agents[i];
            agent.Radius = ShapesOn ? _shapeSet.Shapes[agent.Shape].BoundingRadius : agent.Radius * factor;
            agent.X = Math.Clamp(agent.X, agent.Radius, WorldWidth - agent.Radius);
            agent.Y = Math.Clamp(agent.Y, agent.Radius, WorldHeight - agent.Radius);
        }

        RebuildIndex();
    }

    private Agent CreateAgent(float x, float y)
    {
        float radius = _sizeScale * (_variedSizes ? SmallRadius + MathF.Pow(_random.NextSingle(), 3) * (MaxRadius - SmallRadius) : SmallRadius);
        float angle = _random.NextSingle() * MathF.Tau;
        float speed = 30 + _random.NextSingle() * 90;
        Agent agent = new() { X = x, Y = y, Vx = MathF.Cos(angle) * speed, Vy = MathF.Sin(angle) * speed, Radius = radius };
        if (ShapesOn)
        {
            // The shape nearest the size, whose bounding circle is then the agent's radius.
            int kinds = _shapeMode == 1 ? ShapeSet.SimpleKinds : ShapeSet.Kinds;
            agent.Shape = _shapeSet.Index(_random.Next(kinds), radius);
            agent.Radius = _shapeSet.Shapes[agent.Shape].BoundingRadius;
            agent.Angle = _random.NextSingle() * MathF.Tau;
            agent.Spin = (_random.NextSingle() * 4) - 2;
        }

        return agent;
    }

    private void RebuildIndex()
    {
        // Cells are sized for the common (small) agents; the uniform grid has to assume every agent is the biggest one.
        float maxRadius = _sizeScale * (_variedSizes ? MaxRadius : SmallRadius);
        _index = null;
        _packed = null;
        switch (_kind)
        {
            case 0:
                _index = new QuadTree(World, maxElements: 8, maxDepth: _maxDepth);
                break;
            case 1:
                _index = new LooseQuadTree(World, maxElements: 8, maxDepth: _maxDepth);
                break;
            case 2:
                _index = new LooseTightGrid(World, 128, 72, 32, 18);
                break;
            case 3:
                _index = new UniformGrid(World, 64, 36, maxRadius, maxRadius);
                break;
            case 4:
                // Cells a little wider than most circles; with varied sizes, the bigger ones go in the grid's side tree.
                float cell = 2.1f * _sizeScale * (_variedSizes ? 2 * SmallRadius : SmallRadius);
                _packed = new PackedGrid(World, Math.Max(1, (int)(WorldWidth / cell)), Math.Max(1, (int)(WorldHeight / cell)));
                break;
            case 5:
                _packed = new PackedQuadTree(World, maxElements: 8, maxDepth: Math.Min(_maxDepth, PackedQuadTree.MaxSupportedDepth));
                break;
            case 6:
                (int columns, int rows) = LayeredGridSize();
                _index = new LayeredGrid(World, columns, rows, _layers, _threads);
                break;
            case 7:
                (columns, rows) = LayeredGridSize();
                _packed = new PackedLayeredGrid(World, columns, rows, _layers, _threads);
                break;
            case 8:
                (columns, rows) = LayeredGridSize();
                _packed = new PhasedGrid(World, columns, rows, _layers, _threads);
                break;
            default:
                _index = new OriginalCollisionGrid(WorldWidth, WorldHeight);
                break;
        }

        if (_index is not null)
        {
            for (int i = 0; i < _agentCount; i++)
            {
                ref Agent agent = ref _agents[i];
                agent.Handle = _index.Insert(i, BoundsOf(agent));
            }
        }
        else
        {
            BuildPacked();
        }
    }

    // The finest layer's cells are a little wider than the smallest circles, so each layer up holds circles twice as big.
    private (int Columns, int Rows) LayeredGridSize()
    {
        float topCell = 2.1f * SmallRadius * _sizeScale * (1 << (_layers - 1));
        return (Math.Max(1, (int)(WorldWidth / topCell)), Math.Max(1, (int)(WorldHeight / topCell)));
    }

    private void SetThreads()
    {
        switch (_index ?? (object?)_packed)
        {
            case LayeredGrid grid:
                grid.Threads = _threads;
                break;
            case PackedLayeredGrid packed:
                packed.Threads = _threads;
                break;
            case PhasedGrid phased:
                phased.Threads = _threads;
                break;
        }
    }

    private void BuildPacked()
    {
        for (int i = 0; i < _agentCount; i++)
        {
            _boxes[i] = BoundsOf(_agents[i]);
        }

        _packed!.Build(_boxes.AsSpan(0, _agentCount));
        _packedStale = false;
    }

    private static Aabb BoundsOf(in Agent agent) => Aabb.FromCenter(agent.X, agent.Y, agent.Radius, agent.Radius);

    private static void WriteShape(QuadBatch.Vertex[] vertices, int quad, in Agent agent, Color color)
    {
        int kind = ShapeSet.KindOf(agent.Shape);
        (float sin, float cos) = MathF.SinCos(agent.Angle);
        QuadBatch.WriteRotated(vertices, quad, agent.X, agent.Y, agent.Radius * ShapeSet.QuadScale, cos, sin, color,
            ShapeSet.TextureLeft(kind), ShapeSet.TextureRight(kind));
    }

    private void UpdateTitle(double fps)
    {
        int nodes = 0;
        VisitNodes((_, _, _, _) => nodes++);
        string detail = (_index ?? (object?)_packed) switch
        {
            LayeredGrid grid => LayeredDetail(grid.Layers, grid.CellWidth, grid.Threads, grid.OversizedCount),
            PackedLayeredGrid grid => LayeredDetail(grid.Layers, grid.CellWidth, grid.Threads, grid.OversizedCount),
            PhasedGrid grid => LayeredDetail(grid.Layers, grid.CellWidth, grid.Threads, grid.OversizedCount),
            PackedGrid grid => $" ({grid.OversizedCount:N0} oversized)",
            OriginalCollisionGrid grid => $" ({grid.BucketSize}-unit buckets)",
            _ => HasDepth ? $" (depth {_maxDepth})" : "",
        };
        Window.Title =
            $"{(_kind + 1) % 10}: {Kinds[_kind]}{detail} | {_agentCount:N0} {ShapeModes[_shapeMode]}{(_variedSizes ? " (varied)" : "")} x{_sizeScale:0.##} | "
            + $"bounce {(_bounce ? "on" : "off")} | overlap {(_overlap ? "on" : "off")} | {nodes:N0} nodes | step {_stepMilliseconds:0.00} ms | draw {_drawMilliseconds:0.00} ms ({string.Join(", ", DrawPartNames.Select((name, i) => $"{name} {_drawParts[i]:0.0}"))}) | {fps:0} fps{(_paused ? " | paused" : "")}{BuildWarning} | "
            + "Tab/Shift+Tab or ,/. or 0-9 index, Up/Down count, Left/Right size, [/] depth or layers, -/= threads, V sizes, S shapes, C bounce, N nodes, O overlap, Space pause, R reset, LMB/RMB spray/erase";
    }

    private static string LayeredDetail(int layers, float topCell, int threads, int oversized) =>
        $" ({layers} layers, top cell {topCell:0.#}, {threads} threads, {oversized:N0} oversized)";

    private void DrawNode(Aabb bounds, int depth, bool isLeaf, int count)
    {
        if (bounds.IsEmpty)
        {
            return;
        }

        Color color = DepthColors[Math.Min(depth, DepthColors.Length - 1)];
        if (isLeaf && count > 0)
        {
            _batch.AddSolid(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, color * 0.08f);
        }

        DrawOutline(bounds, color * (isLeaf ? 0.6f : 0.3f), 1);
    }

    private void DrawCircle(float x, float y, float radius, Color color) =>
        _batch.AddSprite(x - radius, y - radius, x + radius, y + radius, color);

    private void DrawOutline(Aabb bounds, Color color, float thickness) =>
        _batch.AddOutline(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, thickness, color);

    // A white disc with a soft one-pixel edge, premultiplied for alpha blending. Each smaller mip level averages the one above
    // it, so circles a few pixels across read a few texels from a small level, rather than scattered ones from the big one.
    private static Texture2D CreateCircleTexture(GraphicsDevice device, int size)
    {
        Vector4[] level = new Vector4[size * size];
        float radius = size * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - radius, dy = y + 0.5f - radius;
                level[y * size + x] = new Vector4(Math.Clamp(radius - MathF.Sqrt(dx * dx + dy * dy), 0, 1));
            }
        }

        Texture2D texture = new(device, size, size, mipmap: true, SurfaceFormat.Color);
        for (int mip = 0; mip < texture.LevelCount; mip++)
        {
            int width = size >> mip;
            if (mip > 0)
            {
                Vector4[] smaller = new Vector4[width * width];
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int above = 2 * y * 2 * width + 2 * x;
                        smaller[y * width + x] = (level[above] + level[above + 1] + level[above + 2 * width] + level[above + 2 * width + 1]) / 4;
                    }
                }

                level = smaller;
            }

            texture.SetData(mip, null, level.Select(texel => new Color(texel)).ToArray(), 0, level.Length);
        }

        return texture;
    }

    // In the shape modes, Radius is the shape's bounding radius, and Shape its index in the ShapeSet.
    private struct Agent
    {
        public float X, Y, Vx, Vy, Radius, Angle, Spin;
        public int Handle, Shape;
        public bool Colliding;
    }

    /// <summary>What an agent's collisions this step add up to: a push, a change of velocity, and whether it touched anything.</summary>
    private struct CircleContact
    {
        public float Dx, Dy, Dvx, Dvy;
        public bool Hit;
    }

    /// <summary>
    /// Works out the first agent's half of each overlap, from positions and velocities nobody changes until every pair is
    /// done, so the pairs can be handled in any order on any thread.
    /// </summary>
    private readonly struct ContactFinder(Agent[] agents, CircleContact[] contacts, bool bounce) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            ref readonly Agent self = ref agents[a];
            ref readonly Agent other = ref agents[b];
            float dx = other.X - self.X, dy = other.Y - self.Y;
            float reach = self.Radius + other.Radius;
            float distanceSquared = dx * dx + dy * dy;
            if (distanceSquared >= reach * reach)
            {
                return;
            }

            ref CircleContact contact = ref contacts[a];
            contact.Hit = true;
            if (!bounce || distanceSquared == 0)
            {
                return;
            }

            // The same push and bounce as CollideCircles gives this agent.
            float distance = MathF.Sqrt(distanceSquared);
            float nx = dx / distance, ny = dy / distance;
            float massSelf = self.Radius * self.Radius, massOther = other.Radius * other.Radius;
            float share = massOther / (massSelf + massOther);
            float overlap = reach - distance;
            contact.Dx -= nx * overlap * share;
            contact.Dy -= ny * overlap * share;

            float closing = (other.Vx - self.Vx) * nx + (other.Vy - self.Vy) * ny;
            if (closing < 0)
            {
                contact.Dvx += 2 * closing * share * nx;
                contact.Dvy += 2 * closing * share * ny;
            }
        }
    }

    /// <summary>
    /// Works out both agents' halves of each overlap, as <see cref="ContactFinder"/> does for one. Only for
    /// <see cref="PhasedGrid.ForEachPairInParallel{TVisitor}"/>, which never runs two calls sharing an agent at once.
    /// </summary>
    private readonly struct PairContactFinder(Agent[] agents, CircleContact[] contacts, bool bounce) : IPairVisitor
    {
        public void Visit(int a, int b)
        {
            ref readonly Agent self = ref agents[a];
            ref readonly Agent other = ref agents[b];
            float dx = other.X - self.X, dy = other.Y - self.Y;
            float reach = self.Radius + other.Radius;
            float distanceSquared = dx * dx + dy * dy;
            if (distanceSquared >= reach * reach)
            {
                return;
            }

            ref CircleContact first = ref contacts[a];
            ref CircleContact second = ref contacts[b];
            first.Hit = true;
            second.Hit = true;
            if (!bounce || distanceSquared == 0)
            {
                return;
            }

            // The normal points from a to b, so the pushes and bounces mirror each other, each weighted by the other's mass.
            float distance = MathF.Sqrt(distanceSquared);
            float nx = dx / distance, ny = dy / distance;
            float massSelf = self.Radius * self.Radius, massOther = other.Radius * other.Radius;
            float firstShare = massOther / (massSelf + massOther), secondShare = massSelf / (massSelf + massOther);
            float overlap = reach - distance;
            first.Dx -= nx * overlap * firstShare;
            first.Dy -= ny * overlap * firstShare;
            second.Dx += nx * overlap * secondShare;
            second.Dy += ny * overlap * secondShare;

            float closing = (other.Vx - self.Vx) * nx + (other.Vy - self.Vy) * ny;
            if (closing < 0)
            {
                first.Dvx += 2 * closing * firstShare * nx;
                first.Dvy += 2 * closing * firstShare * ny;
                second.Dvx -= 2 * closing * secondShare * nx;
                second.Dvy -= 2 * closing * secondShare * ny;
            }
        }
    }

    /// <summary>
    /// Resolves each pair of overlapping shapes, changing both. Only for searches that never run two calls sharing an agent
    /// at once: <see cref="PhasedGrid.ForEachPairInParallel{TVisitor}"/>, or any <c>ForEachPair</c>, which runs on one thread.
    /// </summary>
    private readonly struct ShapeResolver(Agent[] agents, ShapeSet set, bool bounce) : IPairVisitor
    {
        public void Visit(int a, int b) => CollideShapes(ref agents[a], ref agents[b], set, bounce);
    }

    /// <summary>Turns a <see cref="LayeredGrid"/>'s handles into agent positions for another visitor.</summary>
    private readonly struct HandleVisitor<TVisitor>(LayeredGrid grid, TVisitor visitor) : IPairVisitor where TVisitor : IPairVisitor
    {
        public void Visit(int a, int b) => visitor.Visit(grid.GetId(a), grid.GetId(b));
    }
}
