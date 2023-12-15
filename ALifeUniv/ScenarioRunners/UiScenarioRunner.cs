using System;
using ALifeUni.ScenarioRunners.ScenarioLoggers;
using Windows.UI.Xaml.Controls;

namespace ALifeUni.ScenarioRunners
{
    /// <summary>
    /// A ScenarioRunner that writes to a UI textbox
    /// </summary>
    /// <seealso cref="ALifeUni.ScenarioRunners.AbstractScenarioRunner"/>
    public class UiScenarioRunner : AbstractScenarioRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UiScenarioRunner"/> class.
        /// </summary>
        /// <param name="consoleBox">The console box.</param>
        /// <param name="seedBox">The seed box.</param>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="startingSeed">The starting seed.</param>
        /// <param name="numberSeedsToExecute">The number seeds to execute.</param>
        /// <param name="totalTurns">The total turns.</param>
        /// <param name="turnBatch">The turn batch.</param>
        /// <param name="updateFrequency">The update frequency.</param>
        public UiScenarioRunner(TextBox consoleBox, TextBox seedBox, string scenarioName, int? startingSeed = null, int numberSeedsToExecute = 20, int totalTurns = 50000, int turnBatch = 1000, int updateFrequency = 10000) : base(scenarioName, startingSeed, numberSeedsToExecute, totalTurns, turnBatch, updateFrequency, new TextboxLogger(consoleBox), new TextboxLogger(seedBox))
        {
        }

        /// <summary>
        /// Gets the type of the logger.
        /// </summary>
        /// <value>The type of the logger.</value>
        protected override Type LoggerType => typeof(TextboxLogger);

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
