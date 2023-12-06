using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using Serilog;
using System;
using System.IO;
using System.Linq;
using Windows.Storage;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace ScenarioRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                RunSetOfSeeds();
                Console.WriteLine("Done, Hit 'r' to restart, any other key to exit");
                var key = Console.ReadKey();
                if(key.Key != ConsoleKey.R)
                {
                    break;
                }
            }
        }

        static void RunSetOfSeeds()
        {
            var logFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "log.txt");
            Log.Logger = new LoggerConfiguration().WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day).CreateLogger();
            Log.Information("<---StartRun");

            IScenario scenario = new MushroomScenario();
            int height = scenario.WorldHeight;
            int width = scenario.WorldWidth;

            Random r = new Random();
            for(int i = 0; i < 20; i++)
            {
                Console.Write(i + "-> ");
                int seedValue = r.Next();
                RunSeed(seedValue, scenario, height, width);
            }
            Log.Information("--->EndRun");
            Log.CloseAndFlush();
        }

        private static void RunSeed(int seedValue, IScenario scenario, int height, int width)
        {
            string topLine = String.Format("Seed:{0}, Name: {1}, Height:{2}, Width:{3}", seedValue, scenario.Name, height, width);
            Console.WriteLine($"\t---------------{topLine}");
            Console.Write("  ");
            DateTime start = DateTime.Now;

            IScenario newCopy = IScenarioHelpers.FreshInstanceOf(scenario);
            Planet.CreateWorld(seedValue, scenario, height, width);

            string error = null;
            try
            {
                for(int i = 0; i < 50; i++)
                {
                    int turnCount = 1000;
                    Planet.World.ExecuteManyTurns(turnCount);
                    Console.Write(".");

                    int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
                    if(population == 0)
                    {
                        Console.WriteLine("|> All Dead. Next");
                        break;
                    }

                    if((i + 1) % 10 == 0)
                    {
                        TimeSpan elapsed = DateTime.Now - start;
                        string interim = elapsed.ToString("mm\\:ss\\.ff");
                        string stats = $"\tElapsed: {interim} TPS: {(i * turnCount) / elapsed.TotalSeconds:0.00000} Pop:{population}";
                        Console.WriteLine(stats);

                        Console.Write(i + 1);
                    }
                }
            }
            catch(Exception ex)
            {
                error = ex.Message;
                string[] stack = ex.StackTrace.Split(Environment.NewLine);
                error += Environment.NewLine + stack[0];
            }
            DateTime end = DateTime.Now;
            string durationString = (end - start).ToString("mm\\:ss\\.fff");

            Console.Write($"\tTotal Time: {durationString}\tTurns:{Planet.World.Turns}");

            if(!String.IsNullOrEmpty(error))
            {
                string nl = Environment.NewLine;
                string message = topLine + nl + error + nl;
                Log.Information(message);

                Console.WriteLine($"\tERROR: {error}");
            }
            else
            {
                int count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();

                string nl = Environment.NewLine;
                string message = topLine + nl + count + nl;
                Log.Information(message);

                Console.WriteLine($"\tSurviving: {count}");
            }
            Console.WriteLine();
        }
    }
}
