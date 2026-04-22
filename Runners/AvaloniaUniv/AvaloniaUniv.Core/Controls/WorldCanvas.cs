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
using AvaloniaUniv.Core.ALifeImplementations;
using AvaloniaUniv.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALPoint = ALife.Core.Geometry.Shapes.Point;
using AvPoint = Avalonia.Point;

namespace AvaloniaUniv.Core.Controls;

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

    private bool _enabled;
    private AvaloniaRenderer? _renderer;
    private readonly RenderedSimulationController _simulation = new();
    private int _turnCount;
    private SimulatorViewModel? _vm;
    private WorldObject? _special;
    private int _specialCounter;
    private bool _showGeneology;
    private readonly Dictionary<string, int> _agentBirthTurns = new();
    public DispatcherTimer? Timer { get; private set; }
    public event EventHandler? AllAgentsDied;

    public int GetBirthTurn(string individualLabel) =>
        _agentBirthTurns.TryGetValue(individualLabel, out int t) ? t : int.MaxValue;

    static WorldCanvas()
    {
        AffectsRender<WorldCanvas>(TurnCountProperty);
    }

    public WorldCanvas()
    {
        SetSimulationSpeed((int)SimulationSpeed.Normal);
        Focusable = true;
    }

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

    public void ExecuteTick()
    {
        if (!_simulation.IsInitialized) return;
        Planet.World.ExecuteOneTurn();
        TurnCount = Planet.World.Turns;
    }

    public void SetSimulationSpeed(double speed)
    {
        if (Planet.HasWorld) Planet.World.SimulationPerformance?.ClearBuffer();
        _simulation.FpsCounter?.ClearBuffer();

        Timer?.Stop();
        var interval = double.IsInfinity(speed)
            ? TimeSpan.FromMilliseconds(1)
            : TimeSpan.FromSeconds(1.0 / speed);
        Timer = new DispatcherTimer { Interval = interval };
        Timer.Tick += OnTimerTick;
        Timer.Start();
    }

    public override void Render(DrawingContext drawingContext)
    {
        if (!_simulation.IsInitialized)
        {
            base.Render(drawingContext);
            return;
        }

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

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (!IsEnabled) return;

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
        }
        else
        {
            ExecuteTick();
            UpdateViewModel();
        }
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

        // Zone and gene counts
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
            AllAgentsDied?.Invoke(this, EventArgs.Empty);

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
