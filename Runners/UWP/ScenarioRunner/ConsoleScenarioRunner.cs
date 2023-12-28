using System;
using ALifeUni.ScenarioRunners;
using ALifeUni.ScenarioRunners.ScenarioLoggers;

namespace ScenarioRunner
{
    internal class ConsoleLogger : Logger
    {
        protected override void WriteInternal(string message)
        {
            Console.Write(message);
        }
    }

    internal class ConsoleScenarioRunner : AbstractScenarioRunner
    {
        public ConsoleScenarioRunner(string scenarioName, int? startingSeed = null, int numberSeedsToExecute = 20, int totalTurns = 50000, int turnBatch = 1000, int updateFrequency = 10000) : base(scenarioName, startingSeed, numberSeedsToExecute, totalTurns, turnBatch, updateFrequency)
        {
        }

        protected override Type LoggerType => typeof(ConsoleLogger);

        protected override bool ShouldStopRunner()
        {
            Logger.WriteNewLine(3);
            Console.WriteLine("Done, Hit 'r' to restart, any other key to exit");
            var key = Console.ReadKey();
            return key.Key != ConsoleKey.R;
        }
    }
}
