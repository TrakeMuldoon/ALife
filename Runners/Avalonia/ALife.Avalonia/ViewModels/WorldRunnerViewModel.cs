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
        private string scenarioName;
        private int? seed;

        public WorldRunnerViewModel(string scenarioName, int? seed = null)
        {
            this.scenarioName = scenarioName;
            this.seed = seed;
        }

        public string StartingScenarioName
        {
            get => scenarioName;
            set => this.RaiseAndSetIfChanged(ref scenarioName, value);
        }

        public int? StartingSeed
        {
            get => seed;
            set => this.RaiseAndSetIfChanged(ref seed, value);
        }
    }
}
