using System;
using System.Threading;
using ALife.Avalonia.ALifeImplementations;
using ALife.Core.ScenarioRunners;
using ReactiveUI;

namespace ALife.Avalonia.ViewModels
{
    /// <summary>
    /// The View Model for the Scenario Runner view
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ViewModels.ViewModelBase"/>
    public class ScenarioRunnerViewModel : ViewModelBase
    {
        /// <summary>
        /// The can restart runner
        /// </summary>
        private bool _canRestartRunner = false;

        /// <summary>
        /// The can start runner
        /// </summary>
        private bool _canStartRunner = false;

        /// <summary>
        /// The can stop runner
        /// </summary>
        private bool _canStopRunner = false;

        /// <summary>
        /// The console caret index
        /// </summary>
        private int _consoleCaretIndex = 0;

        /// <summary>
        /// The console text
        /// </summary>
        private string _consoleText = string.Empty;

        /// <summary>
        /// The execution count
        /// </summary>
        private string _executionCount = string.Empty;

        /// <summary>
        /// The maximum turns
        /// </summary>
        private string _maxTurns = string.Empty;

        /// <summary>
        /// The scenario runner
        /// </summary>
        private AvaloniaScenarioRunner? _scenarioRunner = null;

        /// <summary>
        /// The seed text
        /// </summary>
        private string _seedText = string.Empty;

        /// <summary>
        /// The state
        /// </summary>
        private string _state = string.Empty;

        /// <summary>
        /// The turn batch
        /// </summary>
        private string _turnBatch = string.Empty;

        /// <summary>
        /// The update frequency
        /// </summary>
        private string _updateFrequency = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioRunnerViewModel"/> class.
        /// </summary>
        public ScenarioRunnerViewModel(string scenarioName, int? scenarioSeed, bool autoStartScenarioRunner)
        {
            ScenarioName = scenarioName;
            ScenarioSeed = scenarioSeed;
            AutoStartScenarioRunner = autoStartScenarioRunner;

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

            if(AutoStartScenarioRunner)
            {
                StartScenarioRunner();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic start scenario runner].
        /// </summary>
        /// <value><c>true</c> if [automatic start scenario runner]; otherwise, <c>false</c>.</value>
        public bool AutoStartScenarioRunner { get; set; } = false;

        /// <summary>
        /// Gets or sets the can restart runner.
        /// </summary>
        /// <value>The can restart runner.</value>
        public bool CanRestartRunner
        {
            get => _canRestartRunner;
            set => this.RaiseAndSetIfChanged(ref _canRestartRunner, value);
        }

        /// <summary>
        /// Gets or sets the can start runner.
        /// </summary>
        /// <value>The can start runner.</value>
        public bool CanStartRunner
        {
            get => _canStartRunner;
            set => this.RaiseAndSetIfChanged(ref _canStartRunner, value);
        }

        /// <summary>
        /// Gets or sets the can stop runner.
        /// </summary>
        /// <value>The can stop runner.</value>
        public bool CanStopRunner
        {
            get => _canStopRunner;
            set => this.RaiseAndSetIfChanged(ref _canStopRunner, value);
        }

        /// <summary>
        /// Gets or sets the index of the console caret.
        /// </summary>
        /// <value>The index of the console caret.</value>
        public int ConsoleCaretIndex
        {
            get => _consoleCaretIndex;
            set => this.RaiseAndSetIfChanged(ref _consoleCaretIndex, value);
        }

        /// <summary>
        /// Gets or sets the console log.
        /// </summary>
        /// <value>The console log.</value>
        public string ConsoleLog
        {
            get => _consoleText;
            set
            {
                _ = this.RaiseAndSetIfChanged(ref _consoleText, value);
                ConsoleCaretIndex = _consoleText.Length;
            }
        }

        /// <summary>
        /// Gets or sets the execution count.
        /// </summary>
        /// <value>The execution count.</value>
        public string ExecutionCount
        {
            get => _executionCount;
            set => this.RaiseAndSetIfChanged(ref _executionCount, value);
        }

        /// <summary>
        /// Gets or sets the maximum turn count.
        /// </summary>
        /// <value>The maximum turn count.</value>
        public string MaxTurnCount
        {
            get => _maxTurns;
            set => this.RaiseAndSetIfChanged(ref _maxTurns, value);
        }

        /// <summary>
        /// Gets or sets the name of the scenario.
        /// </summary>
        /// <value>The name of the scenario.</value>
        public string ScenarioName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scenario seed.
        /// </summary>
        /// <value>The scenario seed.</value>
        public int? ScenarioSeed { get; set; } = null;

        /// <summary>
        /// Gets or sets the seed log.
        /// </summary>
        /// <value>The seed log.</value>
        public string SeedLog
        {
            get => _seedText;
            set => this.RaiseAndSetIfChanged(ref _seedText, value);
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State
        {
            get => _state;
            set => this.RaiseAndSetIfChanged(ref _state, value);
        }

        /// <summary>
        /// Gets or sets the turn batch count.
        /// </summary>
        /// <value>The turn batch count.</value>
        public string TurnBatchCount
        {
            get => _turnBatch;
            set => this.RaiseAndSetIfChanged(ref _turnBatch, value);
        }

        /// <summary>
        /// Gets or sets the update frequency count.
        /// </summary>
        /// <value>The update frequency count.</value>
        public string UpdateFrequencyCount
        {
            get => _updateFrequency;
            set => this.RaiseAndSetIfChanged(ref _updateFrequency, value);
        }

        /// <summary>
        /// Starts the runner.
        /// </summary>
        public void StartRunner()
        {
            if(_scenarioRunner != null && !_scenarioRunner.IsStopped)
            {
                StopRunner();

                String LINEBLOCK = "------------------------------------------------------";
                String nl = Environment.NewLine;

                ConsoleLog += $"{nl}{nl}{nl}{nl}{LINEBLOCK}{nl}{LINEBLOCK}{nl}{nl}{nl}";
            }

            StartScenarioRunner();
        }

        /// <summary>
        /// Stops the runner.
        /// </summary>
        public void StopRunner()
        {
            State = "Simulation Stopped";
            CanStartRunner = true;
            CanRestartRunner = false;
            CanStopRunner = false;

            if(_scenarioRunner != null && !_scenarioRunner.IsStopped)
            {
                _scenarioRunner.StopRunner(true);
                // add some extra sleep time to make sure we're done writing to the console
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Gets the or reset scenario parameters.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        /// <returns></returns>
        private (int, int, int, int) GetOrResetScenarioParameters(bool reset = false)
        {
            // get the number of scenarios we want to execute
            if(!int.TryParse(ExecutionCount, out int seedCount) || reset)
            {
                seedCount = Constants.DEFAULT_NUMBER_SEEDS_EXECUTED;
                ExecutionCount = seedCount.ToString();
            }

            // get the number of turns we want per scenario
            if(!int.TryParse(MaxTurnCount, out int maxTurns) || reset)
            {
                maxTurns = Constants.DEFAULT_TOTAL_TURNS;
                MaxTurnCount = maxTurns.ToString();
            }

            // get the number of turns we want per scenario
            if(!int.TryParse(TurnBatchCount, out int turnBatch) || reset)
            {
                turnBatch = Constants.DEFAULT_TURN_BATCH;
                TurnBatchCount = turnBatch.ToString();
            }

            // get the number of turns we want per scenario
            if(!int.TryParse(UpdateFrequencyCount, out int updateFrequency) || reset)
            {
                updateFrequency = Constants.DEFAULT_UPDATE_FREQUENCY;
                UpdateFrequencyCount = updateFrequency.ToString();
            }

            return (seedCount, maxTurns, turnBatch, updateFrequency);
        }

        /// <summary>
        /// Starts the scenario runner.
        /// </summary>
        private void StartScenarioRunner()
        {
            (int seedCount, int maxTurns, int turnBatch, int updateFrequency) = GetOrResetScenarioParameters();

            _scenarioRunner = new AvaloniaScenarioRunner(this, ScenarioName, ScenarioSeed, numberSeedsToExecute: seedCount, totalTurns: maxTurns, turnBatch: turnBatch, updateFrequency: updateFrequency);

            State = "Simulation Running (or ran)";
            CanStartRunner = false;
            CanRestartRunner = true;
            CanStopRunner = true;
        }
    }
}
