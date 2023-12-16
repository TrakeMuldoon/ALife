namespace ALife.ViewModels;

public class RunnerViewModel : ViewModelBase
{
    public bool AutoStartScenarioRunner { get; set; } = false;

    public string ScenarioName { get; set; } = string.Empty;

    public int? ScenarioSeed { get; set; } = null;

    public RunnerViewModel()
    {
    }
}
