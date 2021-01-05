using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            int height = 2000;
            int width = 2000;
            IScenario scenario = new ZoneRunnerScenario();

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
            Console.Write(topLine);
            DateTime start = DateTime.Now;
            //Console.Write("Started at: " + start.ToString("HH:mm:ss"));

            Planet.CreateWorld(seedValue, scenario, height, width);

            string error = null;
            try
            {
                for(int i = 0; i < 15; i++)
                {
                    Planet.World.ExecuteManyTurns(1000);
                    Console.Write(".");
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
                int count = Planet.World.AllActiveObjects.Where(wo => wo.Alive).Count();
                if(count > 200)
                {
                    Console.WriteLine(count);
                    string nl = Environment.NewLine;
                    string message = topLine + nl + count + nl;
                    Log.Information(message);
                }
                Console.WriteLine("Boring");
            }
            Console.WriteLine();
        }
    }
}
