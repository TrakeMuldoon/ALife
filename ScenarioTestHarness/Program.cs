using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace ScenarioTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var logFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "log.txt");
            Log.Logger = new LoggerConfiguration().WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day).CreateLogger();
            Log.Information("<---StartRun");

            IScenario scenario = new GenerationalMazeScenario();
            //int height = 2000;
            //int width = 2000;
            int height = scenario.WorldHeight;
            int width = scenario.WorldWidth;

            Random r = new Random();
            for(int i = 0; i < 10; i++)
            {
                Console.Write(i + "-> ");
                int seedValue = r.Next();
                RunSeed(seedValue, scenario, height, width);
            }
            Console.WriteLine("Done, Please hit enter key");
            Log.Information("--->EndRun");
            Log.CloseAndFlush();
            Console.ReadLine();
        }

        private static void RunSeed(int seedValue, IScenario scenario, int height, int width)
        {
            string topLine = String.Format("Seed:{0}, Name: {1}, Height:{2}, Width:{3}\t", seedValue, scenario.Name, height, width);
            Console.WriteLine(topLine);
            Console.Write("  ");
            DateTime start = DateTime.Now;
            //Console.Write("Started at: " + start.ToString("HH:mm:ss"));

            Planet.CreateWorld(seedValue, scenario, height, width);

            string error = null;
            try
            {
                int generationIndex = 0;
                for(int i = 0; i < 200; i++)
                {
                    Planet.World.ExecuteManyTurns(1000);
                    Console.Write(".");

                    if((i + 1) % 10 == 0)
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan elapsed = DateTime.Now - start;
                        string interim = elapsed.ToString("mm\\:ss\\.ff");
                        string stats = String.Format("\tElapsed: {0} TPS: {1:0.00000}"
                                                        , interim, (elapsed.TotalSeconds / i * 1000));
                        Console.WriteLine(stats);
                        while(generationIndex < Planet.World.MessagePump.Count)
                        {
                            Console.WriteLine(Planet.World.MessagePump[generationIndex]);
                            generationIndex += 5;
                        }

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

            Console.WriteLine("\tElapsed: " + durationString);

            if(!String.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);

                string nl = Environment.NewLine;
                string message = topLine + nl + error + nl;
                Log.Information(message);
            }
            else
            {
                int count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
                Console.WriteLine(count);
                string nl = Environment.NewLine;
                string message = topLine + nl + count + nl;
                Log.Information(message);

                Console.WriteLine("Boring");
            }
            Console.WriteLine();
        }
    }
}
