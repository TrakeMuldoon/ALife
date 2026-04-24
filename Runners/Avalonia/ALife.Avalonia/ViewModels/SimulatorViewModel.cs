using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Brains.BehaviourBrains;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ALife.Avalonia.ViewModels;

public class SimulatorViewModel : ViewModelBase
{
    public const int MAX_CHARACTERS_FOR_TICKS = 10;

    private int _agentsActive;
    private bool _enabled;
    private int _fastForwardTicks;
    private double _fps;
    private int _genesActive;
    private bool _isSelectedAgentAlive;
    private string _performancePerTickLabel = string.Empty;
    private string _scenarioName = string.Empty;
    private int _seed;
    private double _ticksPerSecond;
    private int _turnCount;
    private string _zoneInfo = string.Empty;
    private bool _showGeneology;
    private Agent? _selectedAgent;
    private Agent? _selectedDescendant;
    private readonly ObservableCollection<Agent> _aliveAgents = new();
    private readonly ObservableCollection<Agent> _descendantsOfSelected = new();

    public SimulatorViewModel()
    {
        Random r = new();
        InitDefaults();
        _scenarioName = string.Empty;
        _seed = r.Next();
        _enabled = false;
    }

    public SimulatorViewModel(string scenarioName, int? seed = null)
    {
        Random r = new();
        InitDefaults();
        _scenarioName = scenarioName;
        _seed = seed ?? r.Next();
        _enabled = false;
    }

    public int AgentsActive
    {
        get => _agentsActive;
        set => this.RaiseAndSetIfChanged(ref _agentsActive, value);
    }

    public int FastForwardTicks
    {
        get => _fastForwardTicks;
        set => this.RaiseAndSetIfChanged(ref _fastForwardTicks, value);
    }

    public double FramesPerSecond
    {
        get => _fps;
        set => this.RaiseAndSetIfChanged(ref _fps, value);
    }

    public int GenesActive
    {
        get => _genesActive;
        set => this.RaiseAndSetIfChanged(ref _genesActive, value);
    }

    public bool IsEnabled
    {
        get => _enabled;
        set => this.RaiseAndSetIfChanged(ref _enabled, value);
    }

    public string PerformancePerTickLabel
    {
        get => _performancePerTickLabel;
        set => this.RaiseAndSetIfChanged(ref _performancePerTickLabel, value);
    }

    public string ScenarioLabel => $"Scenario: {StartingScenarioName}";

    public string ScenarioHeader => StartingScenarioName;

    public string StartingScenarioName
    {
        get => _scenarioName;
        set => this.RaiseAndSetIfChanged(ref _scenarioName, value);
    }

    public int StartingSeed
    {
        get => _seed;
        set
        {
            this.RaiseAndSetIfChanged(ref _seed, value);
            this.RaisePropertyChanged(nameof(ScenarioHeader));
        }
    }

    public bool ShowGeneology
    {
        get => _showGeneology;
        set => this.RaiseAndSetIfChanged(ref _showGeneology, value);
    }

    public double TicksPerSecond
    {
        get => _ticksPerSecond;
        set
        {
            this.RaiseAndSetIfChanged(ref _ticksPerSecond, value);
            double tps = Math.Round(_ticksPerSecond, 2);
            double fps = Math.Round(_fps, 2);
            string tpsSpaces = new string(' ', MAX_CHARACTERS_FOR_TICKS - tps.ToString("0.00").Length);
            string fpsSpaces = new string(' ', MAX_CHARACTERS_FOR_TICKS - fps.ToString("0.00").Length);
            PerformancePerTickLabel = $"TPS: {tpsSpaces}{tps:0.00} | FPS: {fpsSpaces}{fps:0.00}";
        }
    }

    public int TurnCount
    {
        get => _turnCount;
        set => this.RaiseAndSetIfChanged(ref _turnCount, value);
    }

    public string ZoneInfo
    {
        get => _zoneInfo;
        set
        {
            this.RaiseAndSetIfChanged(ref _zoneInfo, value);
            this.RaisePropertyChanged(nameof(ZoneInfoCompact));
        }
    }

    public string ZoneInfoCompact
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_zoneInfo)) return "—";
            return string.Join("  ", _zoneInfo
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim().Replace(": ", ":")));
        }
    }

    public bool IsSelectedAgentAlive
    {
        get => _isSelectedAgentAlive;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedAgentAlive, value);
            this.RaisePropertyChanged(nameof(IsSelectedAgentDead));
        }
    }

    public bool IsSelectedAgentDead => HasSelectedAgent && !_isSelectedAgentAlive;

    public bool HasDescendants => _descendantsOfSelected.Count > 0;

    public ObservableCollection<Agent> DescendantsOfSelected => _descendantsOfSelected;

    public Agent? SelectedDescendant
    {
        get => _selectedDescendant;
        set => this.RaiseAndSetIfChanged(ref _selectedDescendant, value);
    }

    public bool FreezeDescendantUpdates { get; set; }

    public void UpdateDescendants(IEnumerable<Agent> allLivingAgents)
    {
        if (_selectedAgent == null || FreezeDescendantUpdates) return;
        _descendantsOfSelected.Clear();
        foreach (var a in allLivingAgents.Where(a => IsDescendantOf(a, _selectedAgent)))
            _descendantsOfSelected.Add(a);
        this.RaisePropertyChanged(nameof(HasDescendants));
    }

    private static bool IsDescendantOf(Agent agent, Agent ancestor)
    {
        Agent? current = agent.Parent;
        while (current != null)
        {
            if (ReferenceEquals(current, ancestor)) return true;
            current = current.Parent;
        }
        return false;
    }

    public ObservableCollection<Agent> AliveAgents => _aliveAgents;

    public string BrainViewerTitle => _selectedAgent != null ? $"Brain: {AgentName}" : "Brain Viewer";

    public void UpdateAliveAgents(IEnumerable<Agent> agents)
    {
        _aliveAgents.Clear();
        foreach (var a in agents)
            _aliveAgents.Add(a);
    }

    public Agent? SelectedAgent
    {
        get => _selectedAgent;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAgent, value);
            _isSelectedAgentAlive = value?.Alive ?? false;
            _descendantsOfSelected.Clear();
            _selectedDescendant = null;
            this.RaisePropertyChanged(nameof(AgentName));
            this.RaisePropertyChanged(nameof(AgentLocation));
            this.RaisePropertyChanged(nameof(AgentSenses));
            this.RaisePropertyChanged(nameof(AgentActions));
            this.RaisePropertyChanged(nameof(AgentBrain));
            this.RaisePropertyChanged(nameof(HasSelectedAgent));
            this.RaisePropertyChanged(nameof(HasNeuralBrain));
            this.RaisePropertyChanged(nameof(HasBehaviourBrain));
            this.RaisePropertyChanged(nameof(BrainViewerTitle));
            this.RaisePropertyChanged(nameof(IsSelectedAgentAlive));
            this.RaisePropertyChanged(nameof(IsSelectedAgentDead));
            this.RaisePropertyChanged(nameof(HasDescendants));
            SnapshotParentState();
            this.RaisePropertyChanged(nameof(HasParent));
            this.RaisePropertyChanged(nameof(IsParentAlive));
            this.RaisePropertyChanged(nameof(ParentAgent));
            this.RaisePropertyChanged(nameof(ParentButtonLabel));
        }
    }

    public bool HasSelectedAgent => _selectedAgent != null;
    public bool HasNeuralBrain => _selectedAgent?.MyBrain is NeuralNetworkBrain;
    public bool HasBehaviourBrain => _selectedAgent?.MyBrain is BehaviourBrain;

    // Cached inside the world lock by RefreshSelectedAgent / SelectedAgent setter so the
    // binding system reads a snapshot rather than a live agent field that the sim thread
    // could flip between the lock release and the UI evaluation.
    private bool _hasParent;
    private bool _isParentAlive;
    private Agent? _parentAgent;
    private string _parentButtonLabel = string.Empty;

    public bool HasParent => _hasParent;
    public bool IsParentAlive => _isParentAlive;
    public Agent? ParentAgent => _parentAgent;
    public string ParentButtonLabel => _parentButtonLabel;

    private void SnapshotParentState()
    {
        var p = _selectedAgent?.Parent;
        _hasParent = p != null;
        _isParentAlive = p?.Alive == true;
        _parentAgent = p;
        _parentButtonLabel = p == null ? string.Empty
            : p.Alive ? $"↑ {p.IndividualLabel}"
            : $"↑ (dead) {p.IndividualLabel}";
    }

    public string AgentName => _selectedAgent?.IndividualLabel ?? string.Empty;

    public string AgentLocation
    {
        get
        {
            if (_selectedAgent == null) return string.Empty;
            var s = _selectedAgent.Shape;
            return $"X:{s.CentrePoint.X:F1}  Y:{s.CentrePoint.Y:F1}\n" +
                   $"Dir:{s.Orientation.Degrees:F1}°\n" +
                   $"Gen:{_selectedAgent.Generation}  Children:{_selectedAgent.NumChildren}";
        }
    }

    public string AgentSenses
    {
        get
        {
            if (_selectedAgent == null) return string.Empty;
            var sb = new System.Text.StringBuilder();
            foreach (var sc in _selectedAgent.Senses)
                foreach (var inp in sc.SubInputs)
                    if (inp is ALife.Core.WorldObjects.Agents.Input baseInp)
                        sb.AppendLine($"{baseInp.Name}: {baseInp.GetValueAsString()}");
            return sb.ToString().TrimEnd();
        }
    }

    public string AgentActions
    {
        get
        {
            if (_selectedAgent == null) return string.Empty;
            var sb = new System.Text.StringBuilder();
            foreach (var kv in _selectedAgent.Actions)
            {
                var ac = kv.Value;
                sb.AppendLine($"[{ac.Name}] {(ac.ActivatedLastTurn ? "✓" : "✗")}");
                foreach (var apKv in ac.SubActions)
                    sb.AppendLine($"  {apKv.Value.Name}: {apKv.Value.IntensityLastTurn:F3}");
            }
            return sb.ToString().TrimEnd();
        }
    }

    public string AgentBrain
    {
        get
        {
            if (_selectedAgent?.MyBrain is not BehaviourBrain bb) return string.Empty;
            var sb = new StringBuilder();
            foreach (var beh in bb.Behaviours)
            {
                sb.Append(beh.PassedThisTurn ? "!!" : "XX");
                string rule = beh.AsEnglish
                    .Replace(" AND", "\n\tAND")
                    .Replace(" THEN", "\n\t\tTHEN");
                sb.AppendLine($" : {rule}");
            }
            return sb.ToString().TrimEnd();
        }
    }

    public void RefreshSelectedAgent()
    {
        if (_selectedAgent == null) return;
        SnapshotParentState();
        this.RaisePropertyChanged(nameof(AgentLocation));
        this.RaisePropertyChanged(nameof(AgentSenses));
        this.RaisePropertyChanged(nameof(AgentActions));
        this.RaisePropertyChanged(nameof(AgentBrain));
        this.RaisePropertyChanged(nameof(IsSelectedAgentAlive));
        this.RaisePropertyChanged(nameof(IsSelectedAgentDead));
        this.RaisePropertyChanged(nameof(HasParent));
        this.RaisePropertyChanged(nameof(IsParentAlive));
        this.RaisePropertyChanged(nameof(ParentAgent));
        this.RaisePropertyChanged(nameof(ParentButtonLabel));
    }

    private void InitDefaults()
    {
        _ticksPerSecond = 0;
        _fps = 0;
        string baseNum = "---";
        string spaces = new string(' ', MAX_CHARACTERS_FOR_TICKS - baseNum.Length);
        _performancePerTickLabel = $"TPS: {spaces}{baseNum} | FPS: {spaces}{baseNum}";
        _zoneInfo = string.Empty;
        _turnCount = 0;
        _genesActive = 0;
        _agentsActive = 0;
        _fastForwardTicks = 200;
        _showGeneology = false;
        _isSelectedAgentAlive = false;
    }
}
