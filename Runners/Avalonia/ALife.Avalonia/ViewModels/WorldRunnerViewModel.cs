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
        private string _scenarioName;
        private int? _seed;

        private bool _enabled;

        public WorldRunnerViewModel(string scenarioName, int? seed = null)
        {
            this._scenarioName = scenarioName;
            this._seed = seed;
            this._enabled = false;
        }

        public string StartingScenarioName
        {
            get => _scenarioName;
            set => this.RaiseAndSetIfChanged(ref _scenarioName, value);
        }

        public bool IsEnabled
        {
            get => _enabled;
            set => this.RaiseAndSetIfChanged(ref _enabled, value);
        }

        public int? StartingSeed
        {
            get => _seed;
            set => this.RaiseAndSetIfChanged(ref _seed, value);
        }
    }
}
