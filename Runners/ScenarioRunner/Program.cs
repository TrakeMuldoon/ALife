// This example code shows how you could implement the required main function for a Console UWP Application. You can
// replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the Package.appxmanifest to a value that
// you define. To edit this file manually, right-click it in Solution Explorer and select View Code, or open it with the
// XML Editor.

using ALife.Core.Scenarios;

namespace ScenarioRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string scenario = ScenarioSelect();

            ConsoleScenarioRunner runner = new(scenario);

            while(!runner.IsStoppedAndLoggerStopped)
            {
                Thread.Sleep(1000);
            }
        }

        private static string ScenarioSelect()
        {
            Console.WriteLine("Which Scenario to Start??");
            for(int i = 1; i < ScenarioRegister.SortedScenarios.Count; i++)
            {
                Console.WriteLine($"{i}. {ScenarioRegister.SortedScenarios[i]}");
            }
            Console.WriteLine("Type the number and hit enter: ");

            string selected = String.Empty;
            try
            {
                selected = Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Try Again.");
                Thread.Sleep(1000);
                Console.Clear();
                return ScenarioSelect();
            }

            int selectedInt;
            bool success = int.TryParse(selected, out selectedInt);
            if(!success)
            {
                Console.WriteLine("How Dare You Mock Me?!?");
                Console.WriteLine("Try Again.");
                Thread.Sleep(500);
                Console.Clear();
                return ScenarioSelect();
            }
            if(selectedInt > ScenarioRegister.SortedScenarios.Count)
            {
                Console.WriteLine("Invalid Request. Try Again.");
                Thread.Sleep(500);
                Console.Clear();
                return ScenarioSelect();
            }
            if(selectedInt < 1)
            {
                Console.WriteLine("You don't deserve my application.");
                Thread.Sleep(1000);
                Environment.Exit(0);
            }

            return ScenarioRegister.SortedScenarios[selectedInt];
        }
    }
}
