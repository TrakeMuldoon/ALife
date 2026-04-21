using ALife.Core.ScenarioRunners.ScenarioLoggers;
using AvaloniaUniv.Core.ViewModels;

namespace AvaloniaUniv.Core.ALifeImplementations;

public abstract class AvaloniaLogger(BatchRunnerViewModel vm) : Logger
{
    protected readonly BatchRunnerViewModel _vm = vm;
}
