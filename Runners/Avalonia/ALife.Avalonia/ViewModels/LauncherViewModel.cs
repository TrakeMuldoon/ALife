using ALife.Core.Scenarios;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Avalonia.ViewModels;

public class LauncherViewModel : ViewModelBase
{
    private const string SeedTextDefault = "Enter a Numerical Seed Here";

    private readonly Dictionary<string, (int, string)> _suggestedSeedCache = [];

    private string _currentSeedText = string.Empty;
    private string _selectedScenario = string.Empty;
    private string _selectedScenarioDescription = string.Empty;
    private List<string> _suggestedSeeds = [];

    public LauncherViewModel()
    {
        CurrentSeedText = SeedTextDefault;
    }

    public bool AutoStartScenarioRunner { get; set; } = true;

    public List<string> AvailableScenarios => ScenarioRegister.SortedScenarios;

    public string CurrentSeedText
    {
        get => _currentSeedText;
        set => this.RaiseAndSetIfChanged(ref _currentSeedText, value);
    }

    public string SelectedScenario
    {
        get => _selectedScenario;
        set => this.RaiseAndSetIfChanged(ref _selectedScenario, value);
    }

    public string SelectedScenarioDescription
    {
        get => _selectedScenarioDescription;
        set => this.RaiseAndSetIfChanged(ref _selectedScenarioDescription, value);
    }

    public List<string> SuggestedSeeds
    {
        get => _suggestedSeeds;
        set => this.RaiseAndSetIfChanged(ref _suggestedSeeds, value);
    }

    public void SelectScenario(string scenarioName)
    {
        SelectedScenario = scenarioName;
        ScenarioRegistration? details = ScenarioRegister.GetScenarioDetails(scenarioName);
        SelectedScenarioDescription = details?.Description.Trim() ?? string.Empty;

        Dictionary<int, string>? suggestions = ScenarioRegister.GetSuggestions(scenarioName);
        _suggestedSeedCache.Clear();
        List<string> seeds = [];

        if (suggestions?.Count > 0)
        {
            int maxLen = suggestions.Keys.Max().ToString().Length;
            foreach (var kv in suggestions)
            {
                string entry = $"{kv.Key.ToString($"D{maxLen}")} : {kv.Value}";
                seeds.Add(entry);
                _suggestedSeedCache[entry] = (kv.Key, kv.Value);
            }
        }

        CurrentSeedText = SeedTextDefault;
        SuggestedSeeds = seeds;
    }

    public void SelectSeed(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            CurrentSeedText = SeedTextDefault;
            return;
        }
        CurrentSeedText = _suggestedSeedCache.TryGetValue(key, out var val)
            ? val.Item1.ToString()
            : SeedTextDefault;
    }
}
