using ALife.Core.ScenarioRunners;
using AvaloniaUniv.Core.ALifeImplementations;
using ReactiveUI;
using System;
using System.Threading;

namespace AvaloniaUniv.Core.ViewModels;

public class BatchRunnerViewModel : ViewModelBase
{
    private bool _canRestartRunner;
    private bool _canStartRunner;
    private bool _canStopRunner;
    private int _consoleCaretIndex;
    private string _consoleText;
    private string _executionCount;
    private string _maxTurns;
    private AvaloniaScenarioRunner? _scenarioRunner;
    private string _seedText;
    private string _state;
    private string _turnBatch;
    private string _updateFrequency;

    public BatchRunnerViewModel()
    {
        ScenarioName = string.Empty;
        ScenarioSeed = null;
        AutoStartScenarioRunner = false;
        Init();
    }

    public BatchRunnerViewModel(string scenarioName, int? scenarioSeed, bool autoStart)
    {
        ScenarioName = scenarioName;
        ScenarioSeed = scenarioSeed;
        AutoStartScenarioRunner = autoStart;
        Init();
        if (AutoStartScenarioRunner)
            StartScenarioRunner();
    }

    public bool AutoStartScenarioRunner { get; set; }

    public bool CanRestartRunner
    {
        get => _canRestartRunner;
        set => this.RaiseAndSetIfChanged(ref _canRestartRunner, value);
    }

    public bool CanStartRunner
    {
        get => _canStartRunner;
        set => this.RaiseAndSetIfChanged(ref _canStartRunner, value);
    }

    public bool CanStopRunner
    {
        get => _canStopRunner;
        set => this.RaiseAndSetIfChanged(ref _canStopRunner, value);
    }

    public int ConsoleCaretIndex
    {
        get => _consoleCaretIndex;
        set => this.RaiseAndSetIfChanged(ref _consoleCaretIndex, value);
    }

    public string ConsoleLog
    {
        get => _consoleText;
        set
        {
            _ = this.RaiseAndSetIfChanged(ref _consoleText, value);
            ConsoleCaretIndex = _consoleText.Length;
        }
    }

    public string ExecutionCount
    {
        get => _executionCount;
        set => this.RaiseAndSetIfChanged(ref _executionCount, value);
    }

    public string MaxTurnCount
    {
        get => _maxTurns;
        set => this.RaiseAndSetIfChanged(ref _maxTurns, value);
    }

    public string ScenarioName { get; set; }
    public int? ScenarioSeed { get; set; }

    public string SeedLog
    {
        get => _seedText;
        set => this.RaiseAndSetIfChanged(ref _seedText, value);
    }

    public string State
    {
        get => _state;
        set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public string TurnBatchCount
    {
        get => _turnBatch;
        set => this.RaiseAndSetIfChanged(ref _turnBatch, value);
    }

    public string UpdateFrequencyCount
    {
        get => _updateFrequency;
        set => this.RaiseAndSetIfChanged(ref _updateFrequency, value);
    }

    public void StartRunner()
    {
        if (_scenarioRunner != null && !_scenarioRunner.IsStopped)
        {
            StopRunner();
            string sep = "------------------------------------------------------";
            string nl = Environment.NewLine;
            ConsoleLog += $"{nl}{nl}{sep}{nl}{sep}{nl}{nl}";
        }
        StartScenarioRunner();
    }

    public void StopRunner()
    {
        State = "Stopped";
        CanStartRunner = true;
        CanRestartRunner = false;
        CanStopRunner = false;

        if (_scenarioRunner != null && !_scenarioRunner.IsStopped)
        {
            _scenarioRunner.StopRunner(true);
            Thread.Sleep(50);
        }
    }

    private void Init()
    {
        _state = "Stopped";
        _canStartRunner = true;
        _canStopRunner = false;
        _canRestartRunner = false;
        _consoleCaretIndex = 0;
        _consoleText = string.Empty;
        _seedText = string.Empty;
        _executionCount = Constants.DEFAULT_NUMBER_SEEDS_EXECUTED.ToString();
        _maxTurns = Constants.DEFAULT_TOTAL_TURNS.ToString();
        _turnBatch = Constants.DEFAULT_TURN_BATCH.ToString();
        _updateFrequency = Constants.DEFAULT_UPDATE_FREQUENCY.ToString();
    }

    private (int, int, int, int) GetOrResetScenarioParameters()
    {
        if (!int.TryParse(ExecutionCount, out int seedCount))
        {
            seedCount = Constants.DEFAULT_NUMBER_SEEDS_EXECUTED;
            ExecutionCount = seedCount.ToString();
        }
        if (!int.TryParse(MaxTurnCount, out int maxTurns))
        {
            maxTurns = Constants.DEFAULT_TOTAL_TURNS;
            MaxTurnCount = maxTurns.ToString();
        }
        if (!int.TryParse(TurnBatchCount, out int turnBatch))
        {
            turnBatch = Constants.DEFAULT_TURN_BATCH;
            TurnBatchCount = turnBatch.ToString();
        }
        if (!int.TryParse(UpdateFrequencyCount, out int updateFrequency))
        {
            updateFrequency = Constants.DEFAULT_UPDATE_FREQUENCY;
            UpdateFrequencyCount = updateFrequency.ToString();
        }
        return (seedCount, maxTurns, turnBatch, updateFrequency);
    }

    private void StartScenarioRunner()
    {
        (int seedCount, int maxTurns, int turnBatch, int updateFrequency) = GetOrResetScenarioParameters();
        _scenarioRunner = new AvaloniaScenarioRunner(this, ScenarioName, ScenarioSeed,
            numberSeedsToExecute: seedCount, totalTurns: maxTurns, turnBatch: turnBatch,
            updateFrequency: updateFrequency);
        State = "Running";
        CanStartRunner = false;
        CanRestartRunner = true;
        CanStopRunner = true;
    }
}
