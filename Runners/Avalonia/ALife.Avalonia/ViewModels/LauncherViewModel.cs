using System.Collections.Generic;
using System.Linq;
using ALife.Core.Scenarios;
using ReactiveUI;

namespace ALife.Avalonia.ViewModels
{
    /// <summary>
    /// A view model for the launcher view.
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ViewModels.ViewModelBase"/>
    public class LauncherViewModel : ViewModelBase
    {
        /// <summary>
        /// The current seed text default
        /// </summary>
        private const string CurrentSeedTextDefault = "Enter a Numerical Seed Here";

        /// <summary>
        /// The suggested seed cache
        /// </summary>
        private readonly Dictionary<string, (int, string)> _suggestedSeedCache = [];

        /// <summary>
        /// The current seed text
        /// </summary>
        private string _currentSeedText = string.Empty;

        /// <summary>
        /// The selected scenario
        /// </summary>
        private string _selectedScenario = string.Empty;

        /// <summary>
        /// The selected scenario description
        /// </summary>
        private string _selectedScenarioDescription = string.Empty;

        /// <summary>
        /// The suggested seeds
        /// </summary>
        private List<string> _suggestedSeeds = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherViewModel"/> class.
        /// </summary>
        public LauncherViewModel()
        {
            CurrentSeedText = CurrentSeedTextDefault;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic start scenario runner].
        /// </summary>
        /// <value><c>true</c> if [automatic start scenario runner]; otherwise, <c>false</c>.</value>
        public bool AutoStartScenarioRunner { get; set; } = true;

        /// <summary>
        /// Gets the available scenarios.
        /// </summary>
        /// <value>The available scenarios.</value>
        public List<string> AvailableScenarios => ScenarioRegister.SortedScenarios;

        /// <summary>
        /// Gets or sets the current seed text.
        /// </summary>
        /// <value>The current seed text.</value>
        public string CurrentSeedText
        {
            get => _currentSeedText;
            set => this.RaiseAndSetIfChanged(ref _currentSeedText, value);
        }

        /// <summary>
        /// Gets or sets the selected scenario.
        /// </summary>
        /// <value>The selected scenario.</value>
        public string SelectedScenario
        {
            get => _selectedScenario;
            set => this.RaiseAndSetIfChanged(ref _selectedScenario, value);
        }

        /// <summary>
        /// Gets or sets the selected scenario description.
        /// </summary>
        /// <value>The selected scenario description.</value>
        public string SelectedScenarioDescription
        {
            get => _selectedScenarioDescription;
            set => this.RaiseAndSetIfChanged(ref _selectedScenarioDescription, value);
        }

        /// <summary>
        /// Gets or sets the suggested seeds.
        /// </summary>
        /// <value>The suggested seeds.</value>
        public List<string> SuggestedSeeds
        {
            get => _suggestedSeeds;
            set => this.RaiseAndSetIfChanged(ref _suggestedSeeds, value);
        }

        /// <summary>
        /// Selects the scenario.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public void SelectScenario(string scenarioName)
        {
            SelectedScenario = scenarioName;
            ScenarioRegistration? scenarioDetails = ScenarioRegister.GetScenarioDetails(scenarioName);

            SelectedScenarioDescription = scenarioDetails?.Description.Trim() ?? string.Empty;
            Dictionary<int, string>? suggestions = ScenarioRegister.GetSuggestions(scenarioName);

            _suggestedSeedCache.Clear();
            List<string> seeds = [];
            if(suggestions?.Count > 0)
            {
                int maxSeedLength = suggestions.Select(x => x.Key).Max().ToString().Length;

                foreach(KeyValuePair<int, string> suggestion in suggestions)
                {
                    string seedDescription = $"{suggestion.Key.ToString($"D{maxSeedLength}")} : {suggestion.Value}";
                    seeds.Add(seedDescription);
                    _suggestedSeedCache.Add(seedDescription, (suggestion.Key, suggestion.Value));
                }
            }

            CurrentSeedText = CurrentSeedTextDefault;
            SuggestedSeeds = seeds;
        }

        /// <summary>
        /// Selects the seed.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SelectSeed(string key)
        {
            if(string.IsNullOrWhiteSpace(key))
            {
                CurrentSeedText = CurrentSeedTextDefault;
            }

            CurrentSeedText = _suggestedSeedCache.ContainsKey(key) ? _suggestedSeedCache[key].Item1.ToString() : CurrentSeedTextDefault;
        }
    }
}
