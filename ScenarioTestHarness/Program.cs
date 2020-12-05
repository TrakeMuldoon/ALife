using ALifeUni.ALife;
using System;

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
            int seedValue = 80;
            int height = 880;
            int width = 1000;

            RunSeed(seedValue, height, width);
            Console.ReadLine();
        }

        private static void RunSeed(int seedValue, int height, int width)
        {
            string topLine = String.Format("Seed:{0}, Height:{1}, Width:{2}", seedValue, height, width);
            Console.WriteLine(topLine);
            DateTime start = DateTime.Now;
            Console.Write("Started at: " + start.ToString("HH:mm:ss"));

            Planet.CreateWorld(seedValue, height, width);

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
            String endString = end.ToString("HH:mm:ss");
            string durationString = (end - start).ToString("mm\\:ss\\.fff");

            Console.WriteLine("Finished at: " + endString + "||Elapsed:" + durationString);

            if(!String.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }
            else
            {
                foreach(Zone z in Planet.World.Zones.Values)
                {
                    Console.Write(z.Name + ":" + z.MyAgents.Count + "\t\t");
                }
                Console.WriteLine();
            }
            Console.Write("--------");
        }
    }
}
