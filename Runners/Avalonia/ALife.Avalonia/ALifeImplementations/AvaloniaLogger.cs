using ALife.Avalonia.ViewModels;
using ALife.Core.ScenarioRunners.ScenarioLoggers;

namespace ALife.Avalonia.ALifeImplementations;

public abstract class AvaloniaLogger(BatchRunnerViewModel vm) : Logger
{
    protected readonly BatchRunnerViewModel _vm = vm;
}
