using ALife.Core.ScenarioRunners;
using AvaloniaUniv.Core.ViewModels;
using System;

namespace AvaloniaUniv.Core.ALifeImplementations;

public class AvaloniaScenarioRunner(
    BatchRunnerViewModel vm,
    string scenarioName,
    int? startingSeed,
    int numberSeedsToExecute,
    int totalTurns,
    int turnBatch,
    int updateFrequency)
    : AbstractLoggedScenarioRunner(scenarioName, startingSeed, numberSeedsToExecute, totalTurns, turnBatch, updateFrequency,
        new ConsoleLogger(vm), new SeedLogger(vm))
{
    protected override Type LoggerType => typeof(AvaloniaLogger);

    protected override bool ShouldStopRunner()
    {
        Logger.WriteNewLine(3);
        Logger.WriteLine("All Scenarios Complete! Hit [Restart] to run again, or [Return to Launcher] to go back.");
        return true;
    }
}
