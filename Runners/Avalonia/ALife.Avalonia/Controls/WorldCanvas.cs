using ALife.Avalonia.ALifeImplementations;
using ALife.Avalonia.ViewModels;
using ALife.Core;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ALPoint = ALife.Core.Geometry.Shapes.Point;
using AvPoint = Avalonia.Point;

namespace ALife.Avalonia.Controls;

public class WorldCanvas : Control
{
    public static readonly DirectProperty<WorldCanvas, bool> EnabledProperty =
        AvaloniaProperty.RegisterDirect<WorldCanvas, bool>(nameof(Enabled), x => x.Enabled, (x, v) => x.Enabled = v);

    public static readonly DirectProperty<WorldCanvas, string> ScenarioNameProperty =
        AvaloniaProperty.RegisterDirect<WorldCanvas, string>(nameof(ScenarioName), x => x.ScenarioName, (x, v) => x.ScenarioName = v);

    public static readonly DirectProperty<WorldCanvas, int?> StartingSeedProperty =
        AvaloniaProperty.RegisterDirect<WorldCanvas, int?>(nameof(StartingSeed), x => x.StartingSeed, (x, v) => x.StartingSeed = v);

    public static readonly DirectProperty<WorldCanvas, int> TurnCountProperty =
        AvaloniaProperty.RegisterDirect<WorldCanvas, int>(nameof(TurnCount), x => x.TurnCount);

    // Protects Planet.World between the sim thread and the UI/render threads.
    private readonly object _worldLock = new();

    // Set by the render thread before it calls lock(_worldLock).
    // The sim thread checks this after each release and yields if set, creating a
    // guaranteed window where the lock is free and the sim isn't competing.
    // Without this, Monitor (which is not fair) lets the sim re-acquire immediately,
    // starving the render thread even on multi-core hardware.
    private volatile bool _renderPending;

    // Sim thread state — all readable from the background thread without UI-thread access.
    private Thread? _simThread;
    private volatile bool _simThreadRunning;
    private volatile bool _isRunning;   // mirrors IsEnabled, updated via OnPropertyChanged

    // volatile double is not legal in C# — store as long bits via Interlocked instead.
    private long TargetSpeedBits = BitConverter.DoubleToInt64Bits((int)SimulationSpeed.Normal);
    private double TargetSpeed
    {
        get => BitConverter.Int64BitsToDouble(Interlocked.Read(ref TargetSpeedBits));
        set => Interlocked.Exchange(ref TargetSpeedBits, BitConverter.DoubleToInt64Bits(value));
    }

    private bool _enabled;
    private AvaloniaRenderer? _renderer;
    private readonly RenderedSimulationController _simulation = new();
    private int _turnCount;
    private SimulatorViewModel? _vm;
    private WorldObject? _special;
    private int _specialCounter;
    private bool _showGeneology;
    private readonly Dictionary<string, int> _agentBirthTurns = new();
    private int _zeroAgentTicks;

    // Single 60 Hz timer: handles init detection, render triggers, and VM updates.
    private readonly DispatcherTimer _uiTimer;

    public event EventHandler? AllAgentsDied;

    public int GetBirthTurn(string individualLabel) =>
        _agentBirthTurns.TryGetValue(individualLabel, out int t) ? t : int.MaxValue;

    static WorldCanvas()
    {
        AffectsRender<WorldCanvas>(TurnCountProperty);
    }

    public WorldCanvas()
    {
        TargetSpeed = (int)SimulationSpeed.Normal;

        _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.0 / 60.0) };
        _uiTimer.Tick += OnUiTick;
        _uiTimer.Start();

        Focusable = true;
    }

    // ── Public API ───────────────────────────────────────────────────────────

    public bool Enabled
    {
        get => _enabled;
        set => SetAndRaise(EnabledProperty, ref _enabled, value);
    }

    public string ScenarioName
    {
        get => _simulation.ScenarioName;
        set => SetAndRaise(ScenarioNameProperty, ref _simulation.ScenarioName, value);
    }

    public RenderedSimulationController Simulation => _simulation;

    public SimulationSpeed SimulationSpeedValue { get; private set; }

    public int? StartingSeed
    {
        get => _simulation.StartingSeed;
        set => SetAndRaise(StartingSeedProperty, ref _simulation.StartingSeed, value);
    }

    public int TurnCount
    {
        get => _turnCount;
        set => SetAndRaise(TurnCountProperty, ref _turnCount, value);
    }

    public WorldObject? SelectedObject => _special;

    public void SetSelectedAgent(Agent? agent)
    {
        _special = agent;
        _specialCounter = 0;
        _simulation.SpecialObject = agent;
        InvalidateVisual();
    }

    public void SetGeneologyVisible(bool show)
    {
        _showGeneology = show;
        InvalidateVisual();
    }

    // Manual single-step (used when paused).
    public void ExecuteTick()
    {
        if (!_simulation.IsInitialized) return;
        lock (_worldLock)
            Planet.World.ExecuteOneTurn();
        TurnCount = Planet.World.Turns;
    }

    // Speed change: just update the target — the sim thread picks it up on the next cycle.
    public void SetSimulationSpeed(double speed)
    {
        TargetSpeed = speed;
        if (Planet.HasWorld)
        {
            lock (_worldLock)
                Planet.World.SimulationPerformance?.ClearBuffer();
        }
        _simulation.FpsCounter?.ClearBuffer();
    }

    // Called by SimulatorView.Dispose.
    public void StopAll()
    {
        _simThreadRunning = false;
        _isRunning = false;
        _uiTimer.Stop();
        _simThread?.Join(500);
        _simThread = null;
    }

    public override void Render(DrawingContext drawingContext)
    {
        if (!_simulation.IsInitialized)
        {
            base.Render(drawingContext);
            return;
        }

        // Signal to the sim thread that we want the lock. The sim checks this flag
        // after each release and yields (Sleep(0)) if set, giving us a guaranteed
        // window where the lock is free and no thread is competing for it.
        _renderPending = true;
        lock (_worldLock)
        {
            _renderPending = false;
            RenderCore(drawingContext);
        }
    }

    // ── Avalonia overrides ───────────────────────────────────────────────────

    // Sync _isRunning (sim-thread-readable) with IsEnabled (UI-thread-only).
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsEnabledProperty && change.NewValue is bool enabled)
            _isRunning = enabled;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.X)
        {
            _simulation.ViewPast = true;
            InvalidateVisual();
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Key == Key.X)
        {
            _simulation.ViewPast = false;
            InvalidateVisual();
        }
    }

    // ── Private ─────────────────────────────────────────────────────────────

    // 60 Hz UI-thread callback: initializes on first call, then drives render + VM.
    private void OnUiTick(object? sender, EventArgs e)
    {
        if (!_simulation.IsInitialized)
        {
            var vm = DataContext as SimulatorViewModel;
            if (vm == null) return;

            _vm = vm;
            _simulation.InitializeSimulation();
            Width = _simulation.SimulationWidth;
            Height = _simulation.SimulationHeight;
            _renderer = new AvaloniaRenderer();
            PointerPressed += OnPointerPressed;

            _isRunning = IsEnabled;
            _simThreadRunning = true;
            _simThread = new Thread(SimLoop) { IsBackground = true, Name = "ALife-Sim" };
            _simThread.Start();
            return;
        }

        lock (_worldLock)
        {
            TurnCount = Planet.World.Turns; // triggers render via AffectsRender
            UpdateViewModel();
        }
    }

    // Sim loop: runs entirely on the background thread.
    // Reads only volatile fields and _worldLock — never touches UI-thread-owned objects.
    private void SimLoop()
    {
        var sw = Stopwatch.StartNew();
        while (_simThreadRunning)
        {
            if (!_isRunning)
            {
                Thread.Sleep(5);
                sw.Restart();
                continue;
            }

            lock (_worldLock)
                Planet.World.ExecuteOneTurn();

            double speed = TargetSpeed;
            if (double.IsInfinity(speed))
            {
                // In unlimited mode, Monitor is unfair: the sim can re-acquire the lock
                // before the render thread gets scheduled, even after many releases.
                // If a render is pending, yield here — the lock is already free and the
                // sim is not competing, so the render thread wins the next acquire race.
                if (_renderPending)
                    Thread.Sleep(0);
            }
            else
            {
                // Throttled: sleep for the bulk of the remaining interval (coarse),
                // then spin for the last ≤15 ms (precision). We subtract elapsed tick
                // time so the target interval is measured from the start of the tick.
                // The lock is NOT held during sleep/spin, so render is free to acquire.
                double targetMs = 1000.0 / speed;
                double remaining = targetMs - sw.Elapsed.TotalMilliseconds;
                if (remaining > 15.0)
                    Thread.Sleep((int)(remaining - 15));
                while (sw.Elapsed.TotalMilliseconds < targetMs)
                    Thread.SpinWait(10);
            }

            sw.Restart();
        }
    }

    private void RenderCore(DrawingContext drawingContext)
    {
        _renderer!.SetContext(drawingContext);
        Planet p = Planet.World;
        _renderer.FillAARectangle(new ALPoint(0, 0), new ALPoint(p.WorldWidth, p.WorldHeight), Colour.PapayaWhip);
        _simulation.Render(_renderer);

        if (_showGeneology)
            RenderLogic.DrawAncestry(_renderer);

        if (_special != null)
            RenderSelected(drawingContext);

        if (_vm != null)
            _vm.FramesPerSecond = _simulation.FpsCounter.AverageFramesPerTicks;

        base.Render(drawingContext);
    }

    private void RenderSelected(DrawingContext ctx)
    {
        if (_special is not Agent ag) return;
        if (ag.Shape is not Circle cir) return;
        var pen = new Pen(Brushes.HotPink, 2);
        ctx.DrawEllipse(null, pen, new AvPoint(cir.CentrePoint.X, cir.CentrePoint.Y),
            cir.Radius + 6, cir.Radius + 6);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Focus();
        if (!_simulation.IsInitialized) return;
        var pos = e.GetPosition(this);
        var bb = new BoundingBox(pos.X - 5, pos.Y - 5, pos.X + 5, pos.Y + 5);
        var hits = Planet.World.CollisionLevels[ReferenceValues.CollisionLevelPhysical]
            .QueryForBoundingBoxCollisions(bb);

        if (hits.Count == 0)
        {
            _special = null;
            _specialCounter = 0;
        }
        else
        {
            if (_specialCounter >= hits.Count) _specialCounter = 0;
            _special = hits[_specialCounter++];
        }

        UpdateSelectedInViewModel();
        InvalidateVisual();
    }

    private void UpdateViewModel()
    {
        if (_vm == null) return;
        _vm.TicksPerSecond = Planet.World.SimulationPerformance.AverageFramesPerTicks;
        _vm.TurnCount = TurnCount;

        var zoneCount = new Dictionary<string, int>();
        foreach (var z in Planet.World.Zones.Values)
            zoneCount[z.Name] = 0;

        int agentCount = 0;
        var geneCount = new Dictionary<string, int>();
        var livingAgents = new List<Agent>();

        foreach (var wo in Planet.World.AllActiveObjects)
        {
            if (wo is Agent ag && ag.Alive)
            {
                agentCount++;
                zoneCount[ag.HomeZone.Name]++;
                string gene = ag.IndividualLabel[..Math.Min(3, ag.IndividualLabel.Length)];
                geneCount.TryAdd(gene, 0);
                geneCount[gene]++;
                _agentBirthTurns.TryAdd(ag.IndividualLabel, TurnCount);
                livingAgents.Add(ag);
            }
        }

        _vm.AgentsActive = agentCount;
        _vm.GenesActive = geneCount.Count;

        if (agentCount == 0 && IsEnabled)
        {
            _zeroAgentTicks++;
            if (_zeroAgentTicks >= 5)
                AllAgentsDied?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _zeroAgentTicks = 0;
        }

        if (_vm.SelectedAgent != null)
        {
            _vm.IsSelectedAgentAlive = _vm.SelectedAgent.Alive;
            _vm.UpdateDescendants(livingAgents.OrderBy(a => GetBirthTurn(a.IndividualLabel)));
        }

        if (zoneCount.Count > 0)
        {
            int maxNameLen = zoneCount.Keys.Max(k => k.Length);
            var sb = new StringBuilder();
            foreach (var kv in zoneCount)
                sb.AppendLine($"{kv.Key.PadLeft(maxNameLen)}: {kv.Value}");
            _vm.ZoneInfo = sb.ToString();
        }

        _vm.RefreshSelectedAgent();
    }

    private void UpdateSelectedInViewModel()
    {
        if (_vm == null) return;
        _vm.SelectedAgent = _special as Agent;
        _simulation.SpecialObject = _special;
    }
}
