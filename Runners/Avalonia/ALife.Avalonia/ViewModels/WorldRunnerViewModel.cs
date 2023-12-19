using ALife.Core.Scenarios;
using ReactiveUI;
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
        private string scenarioName;
        public String ScenarioName
        {
            get => scenarioName;
            set => this.RaiseAndSetIfChanged(ref scenarioName, value);
        }
        private int? Seed {  get; set; }

        public WorldRunnerViewModel(string scenarioName, int? seed = null)
        {
            ScenarioName = scenarioName;
            Seed = seed;

            //TODO: This. Seriously. Just the worst.
            WorldCanvas.CanvasScenarioName = scenarioName;
            WorldCanvas.StartingSeed = seed;
        }
    }
}
