using ALifeUni.Runners;
using System;

namespace ScenarioRunner
{
    internal class ConsoleScenarioRunner : AbstractScenarioRunner
    {
        protected override bool ShouldStopRunner()
        {
            Console.WriteLine("Done, Hit 'r' to restart, any other key to exit");
            var key = Console.ReadKey();
            if(key.Key != ConsoleKey.R)
            {
                return true;
            }

            return false;
        }

        protected override void StopRunner()
        {
            // blank.
        }

        protected override void Write(string message)
        {
            Console.Write(message);
        }
    }
}
