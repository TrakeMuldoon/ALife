using System;
using System.Collections.Generic;
using ALife.Core;
using ALife.Rendering;
using ReactiveUI;

namespace ALife.Avalonia.ViewModels
{
    /// <summary>
    /// The ViewModel for the SingularRunnerView.
    /// TODO: Do this
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ViewModels.ViewModelBase"/>
    public class SingularRunnerViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// The maximum characters for ticks
        /// </summary>
        public const int MAX_CHARACTERS_FOR_TICKS = 10;

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
        /// The simulation
        /// </summary>
        private RenderedSimulationController _simulation;

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
        /// Whether the simulation is enabled or not.
        /// </summary>
        public bool _isSimulationEnabled;

        /// <summary>
        /// Whether the play button should be enabled or not.
        /// </summary>
        private bool _playButtonEnabled;

        /// <summary>
        /// Whether the pause button should be enabled or not.
        /// </summary>
        private bool _pauseButtonEnabled;

        /// <summary>
        /// Whether the ExecuteTurnButton should be enabled or not.
        /// </summary>
        private bool _oneTurnButtonEnabled;

        /// <summary>
        /// Whether the object has been disposed or not.
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularRunnerViewModel"/> class.
        /// </summary>
        public SingularRunnerViewModel() : this(string.Empty, true, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularRunnerViewModel"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="isSimulationEnabled">Whether the simulation is enabled or not.</param>
        /// <param name="seed">The seed.</param>
        public SingularRunnerViewModel(string scenarioName, bool isSimulationEnabled, int? seed = null)
        {
            Random r = new();

            _ticksPerSecond = 0;
            _fps = 0;
            string baseNum = "---";
            string startingSpaces = new string(' ', SingularRunnerViewModel.MAX_CHARACTERS_FOR_TICKS - baseNum.Length);

            string newLabel = $"TPS: {startingSpaces}{baseNum} | FPS: {startingSpaces}{baseNum}";
            _performancePerTickLabel = newLabel;
            _zoneInfo = string.Empty;
            _turnCount = 0;
            _genesActive = 0;
            _agentsActive = 0;
            _fastForwardTicks = 0;
            _isSimulationEnabled = isSimulationEnabled;
            this._scenarioName = scenarioName;
            this._seed = seed ?? r.Next();

            _playButtonEnabled = !_isSimulationEnabled;
            _pauseButtonEnabled = _isSimulationEnabled;
            _oneTurnButtonEnabled = !_isSimulationEnabled;

            _simulation = new RenderedSimulationController(scenarioName, seed);
            _simulation.OnSimulationTickEvent += Simulation_OnSimulationTickEvent;
            _simulation.InitializeSimulation();
            if (_isSimulationEnabled)
            {
                _simulation.StartSimulation();
            }
        }

        private void Simulation_OnSimulationTickEvent(object? sender, RenderedSimulationTickEventArgs e)
        {
            GenesActive = _simulation.ActiveGeneCount;
            AgentsActive = _simulation.ActiveAgentCount;
            ZoneInfo = _simulation.ZoneInfo;
            TurnCount = Planet.World.Turns;
            FramesPerSecond = _simulation.FpsCounter.AverageFramesPerTicks;
            TicksPerSecond = _simulation.TpsCounter.AverageFramesPerTicks;
        }

        public bool IsPlayButtonEnabled
        {
            get => _playButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _playButtonEnabled, value);
        }

        public bool IsPauseButtonEnabled
        {
            get => _pauseButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _pauseButtonEnabled, value);
        }

        public bool IsSingleTurnButtonEnabled
        {
            get => _oneTurnButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _oneTurnButtonEnabled, value);
        }

        public bool IsSimulationEnabled
        {
            get => _isSimulationEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _isSimulationEnabled, value);
                IsPlayButtonEnabled = !_isSimulationEnabled;
                IsPauseButtonEnabled = _isSimulationEnabled;
                IsSingleTurnButtonEnabled = !_isSimulationEnabled;
            }
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
        /// Gets or sets the simulation.
        /// </summary>
        /// <value>The simulation.</value>
        public RenderedSimulationController Simulation
        {
            get => _simulation;
            set => this.RaiseAndSetIfChanged(ref _simulation, value);
        }

        /// <summary>
        /// The agent UI settings
        /// </summary>
        public AgentUISettings SimulationAgentUiSettings => Simulation.AgentUiSettings;

        /// <summary>
        /// Gets the simulation layers.
        /// </summary>
        /// <value>The simulation layers.</value>
        public List<LayerUISettings> SimulationLayers => Simulation.Layers;

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

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    _simulation.OnSimulationTickEvent -= Simulation_OnSimulationTickEvent;
                    _simulation.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SingularRunnerViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
