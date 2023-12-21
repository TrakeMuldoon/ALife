﻿using ReactiveUI;
using System;

namespace ALife.Avalonia.ViewModels
{
    /// <summary>
    /// The ViewModel for the WorldRunnerView.
    /// TODO: Do this
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ViewModels.ViewModelBase"/>
    public class WorldRunnerViewModel : ViewModelBase
    {
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
        /// The genes active
        /// </summary>
        private int _genesActive;

        /// <summary>
        /// The scenario name
        /// </summary>
        private string _scenarioName;

        /// <summary>
        /// The seed
        /// </summary>
        private int _seed;

        /// <summary>
        /// The ticks per second
        /// </summary>
        private double _ticksPerSecond;

        /// <summary>
        /// The ticks per second
        /// </summary>
        private string _ticksPerSecondLabel;

        /// <summary>
        /// The FPS
        /// </summary>
        private double _fps;

        /// <summary>
        /// The FPS label
        /// </summary>
        private string _fpsLabel;

        /// <summary>
        /// The turn count
        /// </summary>
        private int _turnCount;

        /// <summary>
        /// The zone info
        /// </summary>
        private string _zoneInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldRunnerViewModel"/> class.
        /// </summary>
        public WorldRunnerViewModel()
        {
            Random r = new();

            _ticksPerSecond = 0;
            _ticksPerSecondLabel = "TPS: 0.00";
            _fps = 0;
            _fpsLabel = "FPS: 0.00";
            _zoneInfo = string.Empty;
            _turnCount = 0;
            _genesActive = 0;
            _agentsActive = 0;
            _fastForwardTicks = 0;
            this._scenarioName = string.Empty;
            this._seed = r.Next();
            this._enabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldRunnerViewModel"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="seed">The seed.</param>
        public WorldRunnerViewModel(string scenarioName, int? seed = null)
        {
            Random r = new();

            _ticksPerSecond = 0;
            _ticksPerSecondLabel = "TPS: 0.00";
            _fps = 0;
            _fpsLabel = "FPS: 0.00";
            _zoneInfo = string.Empty;
            _turnCount = 0;
            _genesActive = 0;
            _agentsActive = 0;
            _fastForwardTicks = 0;
            this._scenarioName = scenarioName;
            this._seed = seed ?? r.Next();
            this._enabled = false;
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
        /// Gets the scenario label.
        /// </summary>
        /// <value>The scenario label.</value>
        public string ScenarioLabel => $"Scenario: {StartingScenarioName}";

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
                TicksPerSecondLabel = $"TPS: {Math.Round(_ticksPerSecond, 2)}";
            }
        }

        /// <summary>
        /// Gets or sets the ticks per second.
        /// </summary>
        /// <value>The ticks per second.</value>
        public string TicksPerSecondLabel
        {
            get => _ticksPerSecondLabel;
            set => this.RaiseAndSetIfChanged(ref _ticksPerSecondLabel, value);
        }

        /// <summary>
        /// Gets or sets the frames per second.
        /// </summary>
        /// <value>
        /// The frames per second.
        /// </value>
        public double FramesPerSecond
        {
            get => _fps;
            set
            {
                this.RaiseAndSetIfChanged(ref _fps, value);
                FramesPerSecondLabel = $"FPS: {Math.Round(_fps, 2)}";
            }
        }

        /// <summary>
        /// Gets or sets the frames per second label.
        /// </summary>
        /// <value>
        /// The frames per second label.
        /// </value>
        public string FramesPerSecondLabel
        {
            get => _fpsLabel;
            set => this.RaiseAndSetIfChanged(ref _fpsLabel, value);
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
    }
}
