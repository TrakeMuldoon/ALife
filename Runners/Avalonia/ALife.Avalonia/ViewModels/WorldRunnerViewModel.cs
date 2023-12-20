using ReactiveUI;

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
        /// The enabled
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// The scenario name
        /// </summary>
        private string _scenarioName;

        /// <summary>
        /// The seed
        /// </summary>
        private int? _seed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldRunnerViewModel"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="seed">The seed.</param>
        public WorldRunnerViewModel(string scenarioName, int? seed = null)
        {
            this._scenarioName = scenarioName;
            this._seed = seed;
            this._enabled = false;
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
        public int? StartingSeed
        {
            get => _seed;
            set => this.RaiseAndSetIfChanged(ref _seed, value);
        }
    }
}
