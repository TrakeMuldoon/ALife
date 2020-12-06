using ALifeUni.ALife;
using Serilog;
using System;
using System.IO;
using Windows.Foundation.Diagnostics;
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

            int height = 880;
            int width = 1000;
            Random r = new Random();
            for(int i = 0; i < 100; i++)
            {
                Console.Write(i + "-> ");
                int seedValue = r.Next();
                RunSeed(seedValue, height, width);
            }
            Console.WriteLine("Done, Please hit enter key");
            Log.Information("--->EndRun");
            Log.CloseAndFlush();
            Console.ReadLine();
        }

        private static void RunSeed(int seedValue, int height, int width)
        {
            string topLine = String.Format("Seed:{0}, Height:{1}, Width:{2}\t", seedValue, height, width);
            Console.Write(topLine);
            DateTime start = DateTime.Now;
            //Console.Write("Started at: " + start.ToString("HH:mm:ss"));

            Planet.CreateWorld(seedValue, height, width);

            string error = null;
            try
            {
                bool endEarly = false;
                for(int i = 0; i < 5; i++)
                {
                    Planet.World.ExecuteManyTurns(1000);
                    Console.Write(".");
                    foreach(Zone z in Planet.World.Zones.Values)
                    {
                        if(z.MyAgents.Count == 0)
                        {
                            endEarly = true;
                            break;
                        }
                    }
                    if(endEarly)
                    {
                        break;
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
                bool dontBother = false;
                string results = "";
                foreach(Zone z in Planet.World.Zones.Values)
                {
                    results += z.Name + ":" + z.MyAgents.Count + "\t\t";
                    if(z.MyAgents.Count == 0)
                    {
                        dontBother = true;
                    }
                }
                Console.WriteLine(results);

                if(!dontBother)
                {
                    string nl = Environment.NewLine;
                    string message = topLine + nl + results;
                    Log.Information(message);
                }
            }
            Console.WriteLine();
        }
    }
}
