using System;
using ALife.Avalonia.ViewModels;
using ALife.Core.ScenarioRunners;

namespace ALife.Avalonia.ALifeImplementations
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaScenarioRunner"/> class.
    /// </summary>
    /// <param name="rvm">The RVM.</param>
    /// <param name="scenarioName">Name of the scenario.</param>
    /// <param name="startingSeed">The starting seed.</param>
    /// <param name="numberSeedsToExecute">The number seeds to execute.</param>
    /// <param name="totalTurns">The total turns.</param>
    /// <param name="turnBatch">The turn batch.</param>
    /// <param name="updateFrequency">The update frequency.</param>
    public class AvaloniaScenarioRunner(ScenarioRunnerViewModel vm, string scenarioName, int? startingSeed, int numberSeedsToExecute, int totalTurns, int turnBatch, int updateFrequency) : AbstractLoggedScenarioRunner(scenarioName, startingSeed, numberSeedsToExecute, totalTurns, turnBatch, updateFrequency, new ConsoleLogger(vm), new SeedLogger(vm))
    {
        /// <summary>
        /// Gets the type of the logger.
        /// </summary>
        /// <value>The type of the logger.</value>
        protected override Type LoggerType => typeof(AvaloniaLogger);

        /// <summary>
        /// Checks if we should stop the runner or not
        /// </summary>
        /// <returns>True to stop runner, false otherwise</returns>
        protected override bool ShouldStopRunner()
        {
            Logger.WriteNewLine(3);
            Logger.WriteLine("All Scenarios Complete! Hit the [Restart] button to restart, or the [Return to Launcher] button to return to the launcher.");
            return true;
        }
    }
}
