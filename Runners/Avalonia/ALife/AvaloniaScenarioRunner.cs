using ALife.Core.ScenarioRunners;
using ALife.Core.ScenarioRunners.ScenarioLoggers;
using ALife.ViewModels;
using System;

namespace ALife
{
    public abstract class AvaloniaLogger : Logger
    {
        protected readonly RunnerViewModel _rvm;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextboxLogger"/> class.
        /// </summary>
        /// <param name="consoleBox">The console box.</param>
        public AvaloniaLogger(RunnerViewModel rvm)
        {
            _rvm = rvm;
        }
    }

    public class ConsoleLogger : AvaloniaLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="rvm"></param>
        public ConsoleLogger(RunnerViewModel rvm) : base(rvm)
        {
        }

        /// <summary>
        /// Writes the message to the actual output.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void WriteInternal(string message)
        {
            _rvm.ConsoleLog += message;
        }
    }

    public class SeedLogger : AvaloniaLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeedLogger"/> class.
        /// </summary>
        /// <param name="rvm"></param>
        public SeedLogger(RunnerViewModel rvm) : base(rvm)
        {
        }

        /// <summary>
        /// Writes the message to the actual output.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void WriteInternal(string message)
        {
            _rvm.SeedLog += message;
        }
    }

    /// <summary>
    /// A ScenarioRunner for Avalonia
    /// </summary>
    /// <seealso cref="ALife.Core.ScenarioRunners.AbstractScenarioRunner" />
    public class AvaloniaScenarioRunner : AbstractScenarioRunner
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
        public AvaloniaScenarioRunner(RunnerViewModel rvm, string scenarioName, int? startingSeed = null, int numberSeedsToExecute = 20, int totalTurns = 50000, int turnBatch = 1000, int updateFrequency = 10000) : base(scenarioName, startingSeed, numberSeedsToExecute, totalTurns, turnBatch, updateFrequency, new ConsoleLogger(rvm), new SeedLogger(rvm))
        {
        }

        /// <summary>
        /// Gets the type of the logger.
        /// </summary>
        /// <value>
        /// The type of the logger.
        /// </value>
        protected override Type LoggerType => typeof(AvaloniaLogger);

        /// <summary>
        /// Checks if we should stop the runner or not
        /// </summary>
        /// <returns>
        /// True to stop runner, false otherwise
        /// </returns>
        protected override bool ShouldStopRunner()
        {
            Logger.WriteNewLine(3);
            Logger.WriteLine("All Scenarios Complete! Hit the [Restart] button to restart, or the [Return to Launcher] button to return to the launcher.");
            return true;
        }
    }
}
