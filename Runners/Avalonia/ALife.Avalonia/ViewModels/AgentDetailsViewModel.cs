using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Brains.BehaviourBrains;
using ReactiveUI;
using System.Text;
using Input = ALife.Core.WorldObjects.Agents.Input;

namespace ALife.Avalonia.ViewModels;

public class AgentDetailsViewModel : ViewModelBase
{
    private readonly Agent _agent;
    private bool _isAlive;
    private int _turnCount;
    private string _agentLocation = string.Empty;
    private string _agentSenses = string.Empty;
    private string _agentActions = string.Empty;
    private string _agentBrain = string.Empty;

    public AgentDetailsViewModel(Agent agent)
    {
        _agent = agent;
        _isAlive = agent.Alive;
        Refresh(0);
    }

    public Agent Agent => _agent;
    public string AgentName => _agent.IndividualLabel;
    public bool HasNeuralBrain => _agent.MyBrain is ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrain;
    public bool HasBehaviourBrain => _agent.MyBrain is BehaviourBrain;

    public bool IsAlive
    {
        get => _isAlive;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isAlive, value);
            this.RaisePropertyChanged(nameof(IsDead));
        }
    }

    public bool IsDead => !_isAlive;

    public int TurnCount
    {
        get => _turnCount;
        private set => this.RaiseAndSetIfChanged(ref _turnCount, value);
    }

    public string AgentLocation
    {
        get => _agentLocation;
        private set => this.RaiseAndSetIfChanged(ref _agentLocation, value);
    }

    public string AgentSenses
    {
        get => _agentSenses;
        private set => this.RaiseAndSetIfChanged(ref _agentSenses, value);
    }

    public string AgentActions
    {
        get => _agentActions;
        private set => this.RaiseAndSetIfChanged(ref _agentActions, value);
    }

    public string AgentBrain
    {
        get => _agentBrain;
        private set => this.RaiseAndSetIfChanged(ref _agentBrain, value);
    }

    // Call from the UI thread. Returns false once the agent has died (caller may stop the timer).
    public bool Refresh(int currentTurn)
    {
        bool alive = _agent.Alive;
        IsAlive = alive;
        TurnCount = currentTurn;

        // Update display while alive. On death, do one final snapshot then freeze.
        if (alive || _agentLocation.Length == 0)
        {
            AgentLocation = ComputeLocation();
            AgentSenses = ComputeSenses();
            AgentActions = ComputeActions();
            AgentBrain = ComputeBrain();
        }

        return alive;
    }

    private string ComputeLocation()
    {
        try
        {
            var s = _agent.Shape;
            return $"X:{s.CentrePoint.X:F1}  Y:{s.CentrePoint.Y:F1}\n" +
                   $"Dir:{s.Orientation.Degrees:F1}°\n" +
                   $"Gen:{_agent.Generation}  Children:{_agent.NumChildren}";
        }
        catch { return _agentLocation; }
    }

    private string ComputeSenses()
    {
        try
        {
            var sb = new StringBuilder();
            foreach (var sc in _agent.Senses)
                foreach (var inp in sc.SubInputs)
                    if (inp is Input baseInp)
                        sb.AppendLine($"{baseInp.Name}: {baseInp.GetValueAsString()}");
            return sb.ToString().TrimEnd();
        }
        catch { return _agentSenses; }
    }

    private string ComputeActions()
    {
        try
        {
            var sb = new StringBuilder();
            foreach (var kv in _agent.Actions)
            {
                var ac = kv.Value;
                sb.AppendLine($"[{ac.Name}] {(ac.ActivatedLastTurn ? "✓" : "✗")}");
                foreach (var apKv in ac.SubActions)
                    sb.AppendLine($"  {apKv.Value.Name}: {apKv.Value.IntensityLastTurn:F3}");
            }
            return sb.ToString().TrimEnd();
        }
        catch { return _agentActions; }
    }

    private string ComputeBrain()
    {
        try
        {
            if (_agent.MyBrain is not BehaviourBrain bb) return string.Empty;
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
        catch { return _agentBrain; }
    }
}
