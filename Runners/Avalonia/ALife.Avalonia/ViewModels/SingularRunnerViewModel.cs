using System;
using System.Collections.Generic;
using ALife.Rendering;
using ReactiveUI;

namespace ALife.Avalonia.ViewModels
{
    /// <summary>
    /// The ViewModel for the SingularRunnerView.
    /// TODO: Do this
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ViewModels.ViewModelBase"/>
    public class SingularRunnerViewModel : ViewModelBase
    {
        /// <summary>
        /// The maximum characters for ticks
        /// </summary>
        public const int MAX_CHARACTERS_FOR_TICKS = 10;

        /// <summary>
        /// The simulation speeds
        /// </summary>
        public Dictionary<int, SimulationSpeed> SimulationSpeeds;

        /// <summary>
        /// The agents active
        /// </summary>
        private int _agentsActive;

        /// <summary>
        /// The enabled
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// The fast forward ticks
        /// </summary>
        private int _fastForwardTicks;

        /// <summary>
        /// The FPS
        /// </summary>
        private double _fps;

        /// <summary>
        /// The genes active
        /// </summary>
        private int _genesActive;

        /// <summary>
        /// The performance per tick label
        /// </summary>
        private string _performancePerTickLabel;

        /// <summary>
        /// The scenario name
        /// </summary>
        private string _scenarioName;

        /// <summary>
        /// The seed
        /// </summary>
        private int _seed;

        /// <summary>
        /// The speed
        /// </summary>
        private int _speed;

        /// <summary>
        /// The ticks per second
        /// </summary>
        private double _ticksPerSecond;

        /// <summary>
        /// The turn count
        /// </summary>
        private int _turnCount;

        /// <summary>
        /// The zone info
        /// </summary>
        private string _zoneInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularRunnerViewModel"/> class.
        /// </summary>
        public SingularRunnerViewModel()
        {
            Initialize(string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularRunnerViewModel"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="seed">The seed.</param>
        public SingularRunnerViewModel(string scenarioName, int? seed = null)
        {
            Initialize(scenarioName, seed);
        }

        /// <summary>
        /// Gets or sets the agents active.
        /// </summary>
        /// <value>The agents active.</value>
        public int AgentsActive
        {
            get => _agentsActive;
            set => this.RaiseAndSetIfChanged(ref _agentsActive, value);
        }

        /// <summary>
        /// Gets or sets the fast forward ticks.
        /// </summary>
        /// <value>The fast forward ticks.</value>
        public int FastForwardTicks
        {
            get => _fastForwardTicks;
            set => this.RaiseAndSetIfChanged(ref _fastForwardTicks, value);
        }

        /// <summary>
        /// Gets or sets the frames per second.
        /// </summary>
        /// <value>The frames per second.</value>
        public double FramesPerSecond
        {
            get => _fps;
            set => this.RaiseAndSetIfChanged(ref _fps, value);
        }

        /// <summary>
        /// Gets or sets the genes active.
        /// </summary>
        /// <value>The genes active.</value>
        public int GenesActive
        {
            get => _genesActive;
            set => this.RaiseAndSetIfChanged(ref _genesActive, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled
        {
            get => _enabled;
            set => this.RaiseAndSetIfChanged(ref _enabled, value);
        }

        /// <summary>
        /// Gets or sets the performance per tick label.
        /// </summary>
        /// <value>The performance per tick label.</value>
        public string PerformancePerTickLabel
        {
            get => _performancePerTickLabel;
            set => this.RaiseAndSetIfChanged(ref _performancePerTickLabel, value);
        }

        /// <summary>
        /// Gets the scenario label.
        /// </summary>
        /// <value>The scenario label.</value>
        public string ScenarioLabel => $"Scenario: {StartingScenarioName}";

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>The speed.</value>
        public int Speed
        {
            get => _speed;
            set => this.RaiseAndSetIfChanged(ref _speed, value);
        }

        /// <summary>
        /// Gets or sets the name of the starting scenario.
        /// </summary>
        /// <value>The name of the starting scenario.</value>
        public string StartingScenarioName
        {
            get => _scenarioName;
            set => this.RaiseAndSetIfChanged(ref _scenarioName, value);
        }

        /// <summary>
        /// Gets or sets the starting seed.
        /// </summary>
        /// <value>The starting seed.</value>
        public int StartingSeed
        {
            get => _seed;
            set => this.RaiseAndSetIfChanged(ref _seed, value);
        }

        /// <summary>
        /// Gets or sets the ticks per second.
        /// </summary>
        /// <value>The ticks per second.</value>
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

                string newLabel = $"TPS: {tpsSpaces}{tps.ToString("0.00")} | FPS: {fpsSpaces}{fps.ToString("0.00")}";

                PerformancePerTickLabel = newLabel;
            }
        }

        /// <summary>
        /// Gets or sets the turn count.
        /// </summary>
        /// <value>The turn count.</value>
        public int TurnCount
        {
            get => _turnCount;
            set => this.RaiseAndSetIfChanged(ref _turnCount, value);
        }

        /// <summary>
        /// Gets or sets the zone information.
        /// </summary>
        /// <value>The zone information.</value>
        public string ZoneInfo
        {
            get => _zoneInfo;
            set => this.RaiseAndSetIfChanged(ref _zoneInfo, value);
        }

        /// <summary>
        /// Initializes the specified scenario name.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="seed">The seed.</param>
        private void Initialize(string scenarioName, int? seed = null)
        {
            Random r = new();

            _ticksPerSecond = 0;
            _fps = 0;
            string baseNum = "---";
            string startingSpaces = new string(' ', SingularRunnerViewModel.MAX_CHARACTERS_FOR_TICKS - baseNum.Length);
            string newLabel = $"TPS: {startingSpaces}{baseNum} | FPS: {startingSpaces}{baseNum}";
            _performancePerTickLabel = newLabel;

            _speed = 20;
            _zoneInfo = string.Empty;
            _turnCount = 0;
            _genesActive = 0;
            _agentsActive = 0;
            _fastForwardTicks = 0;
            this._scenarioName = scenarioName;
            this._seed = seed ?? r.Next();
            this._enabled = false;

            SimulationSpeeds = new();
            int sliderValue = 0;
            foreach(SimulationSpeed simSpeed in Enum.GetValues(typeof(SimulationSpeed)))
            {
                SimulationSpeeds.Add(sliderValue, simSpeed);
                sliderValue += 10;
            }
        }
    }
}
