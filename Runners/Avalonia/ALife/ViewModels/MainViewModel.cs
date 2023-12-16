using ALife.Core.Scenarios;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace ALife.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _selectedScenario = string.Empty;
    private string _selectedScenarioDescription = string.Empty;
    private List<string> _suggestedSeeds = new();
    private Dictionary<string, (int, string)> _suggestedSeedCache = new();
    private string _currentSeedText = string.Empty;
    private const string CurrentSeedTextDefault = "Enter a Numerical Seed Here";

    public MainViewModel()
    {
        CurrentSeedText = CurrentSeedTextDefault;
    }

    /// <summary>
    /// Gets the available scenarios.
    /// </summary>
    /// <value>
    /// The available scenarios.
    /// </value>
    public List<string> AvailableScenarios => ScenarioRegister.SortedScenarios;

    /// <summary>
    /// Selects the scenario.
    /// </summary>
    /// <param name="scenarioName">Name of the scenario.</param>
    public void SelectScenario(string scenarioName)
    {
        SelectedScenario = scenarioName;
        var scenarioDetails = ScenarioRegister.GetScenarioDetails(scenarioName);

        SelectedScenarioDescription = scenarioDetails?.Description.Trim() ?? string.Empty;
        var suggestions = ScenarioRegister.GetSuggestions(scenarioName);


        _suggestedSeedCache.Clear();
        var seeds = new List<string>();
        if(suggestions?.Count > 0)
        {
            var maxSeedLength = suggestions.Select(x => x.Key).Max().ToString().Length;

            foreach(var suggestion in suggestions)
            {
                var seedDescription = $"{suggestion.Key.ToString($"D{maxSeedLength}")} : {suggestion.Value}";
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

        CurrentSeedText = _suggestedSeedCache[key].Item1.ToString();
    }

    /// <summary>
    /// Gets or sets the suggested seeds.
    /// </summary>
    /// <value>
    /// The suggested seeds.
    /// </value>
    public List<string> SuggestedSeeds
    {
        get => _suggestedSeeds;
        set => this.RaiseAndSetIfChanged(ref _suggestedSeeds, value);
    }

    /// <summary>
    /// Gets or sets the selected scenario.
    /// </summary>
    /// <value>
    /// The selected scenario.
    /// </value>
    public string SelectedScenario
    {
        get => _selectedScenario;
        set => this.RaiseAndSetIfChanged(ref _selectedScenario, value);
    }

    /// <summary>
    /// Gets or sets the current seed text.
    /// </summary>
    /// <value>
    /// The current seed text.
    /// </value>
    public string CurrentSeedText
    {
        get => _currentSeedText;
        set => this.RaiseAndSetIfChanged(ref _currentSeedText, value);
    }

    /// <summary>
    /// Gets or sets the selected scenario description.
    /// </summary>
    /// <value>
    /// The selected scenario description.
    /// </value>
    public string SelectedScenarioDescription
    {
        get => _selectedScenarioDescription;
        set => this.RaiseAndSetIfChanged(ref _selectedScenarioDescription, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic start scenario runner].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic start scenario runner]; otherwise, <c>false</c>.
    /// </value>
    public bool AutoStartScenarioRunner { get; set; } = true;
}
