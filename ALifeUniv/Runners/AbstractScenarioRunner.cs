using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.WorldObjects.Agents;
using System;
using System.Linq;

namespace ALifeUni.Runners
{
    public abstract class AbstractScenarioRunner
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public void ExecuteRunner(string scenarioName, int? startingSeed)
        {
            CancelRunner = false;
            IsStopped = false;
            while(true)
            {
                RunSetOfSeeds(scenarioName, startingSeed);
                if(CancelRunner || ShouldStopRunner())
                {
                    StopRunner();
                    break;
                }
            }

            IsStopped = true;
        }

        /// <summary>
        /// Checks if we should stop the runner or not
        /// </summary>
        /// <returns>True to stop runner, false otherwise</returns>
        protected abstract bool ShouldStopRunner();

        /// <summary>
        /// Gets or sets a value indicating whether [cancel runner].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cancel runner]; otherwise, <c>false</c>.
        /// </value>
        public bool CancelRunner { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is stopped.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is stopped; otherwise, <c>false</c>.
        /// </value>
        public bool IsStopped { get; private set; } = false;

        /// <summary>
        /// Stops the runner.
        /// </summary>
        protected abstract void StopRunner();

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void Write(string message);

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Write(int message)
        {
            Write(message.ToString());
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void WriteLine(string message = " ")
        {
            Write($"{message}{Environment.NewLine}");
        }

        /// <summary>
        /// Writes new lines.
        /// </summary>
        /// <param name="numLines">The number of new lines.</param>
        protected void WriteNewLine(int numLines)
        {
            for(var i = 0; i < numLines; i++)
            {
                Write($" {Environment.NewLine}");
            }
        }

        /// <summary>
        /// Writes line seperators.
        /// </summary>
        /// <param name="numLines">The number of line seperators.</param>
        protected void WriteLineSeperator(int numLines)
        {
            for(var i = 0; i < numLines; i++)
            {
                Write($"-------------------------------------{Environment.NewLine}");
            }
        }


        private void RunSetOfSeeds(string scenarioName, int? startingSeed)
        {
            // TODO: Restore logging functionality. Had been using Serilog previously
            /*
            var logFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "log.txt");
            Log.Logger = new LoggerConfiguration().WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day).CreateLogger();
            Log.Information("<---StartRun");

            // IScenario scenario = new MushroomScenario();
            IScenario scenario = ScenarioRegister.GetScenario(scenarioName);

            int height = scenario.WorldHeight;
            int width = scenario.WorldWidth;

            Random r = new Random();
            for (int i = 0; i < 20; i++)
            {
                Console.Write(i + "-> ");
                int seedValue = r.Next();
                RunSeed(seedValue, scenario, height, width);
            }
            Log.Information("--->EndRun");
            Log.CloseAndFlush();
             */

            IScenario scenario = ScenarioRegister.GetScenario(scenarioName);

            int height = scenario.WorldHeight;
            int width = scenario.WorldWidth;

            Random r = new Random();
            for(int i = 0; i < 20; i++)
            {
                if(CancelRunner)
                {
                    break;
                }
                Write($"Scenario Execution #{i} -> ");
                int seedValue = r.Next();
                if(startingSeed.HasValue)
                {
                    seedValue = startingSeed.Value;
                }
                RunSeed(seedValue, scenario, height, width);
                WriteNewLine(1);
                WriteLineSeperator(3);
                WriteNewLine(1);
            }
        }

        private void RunSeed(int seedValue, IScenario scenario, int height, int width)
        {
            var scenarioDetails = ScenarioRegister.GetScenarioDetails(scenario.GetType());
            string topLine = $"Seed: {seedValue}, Name: {scenarioDetails.Name}, Height:{height}, Width:{width}";
            WriteLine($"\t{topLine}");
            WriteLineSeperator(1);
            DateTime start = DateTime.Now;

            IScenario newCopy = IScenarioHelpers.FreshInstanceOf(scenario);
            Planet.CreateWorld(seedValue, scenario, height, width);

            string error = null;
            try
            {
                Write(" 0");
                for(int i = 0; i < 50; i++)
                {
                    if(CancelRunner)
                    {
                        break;
                    }
                    int turnCount = 1000;
                    Planet.World.ExecuteManyTurns(turnCount);
                    Write(".");

                    int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
                    if(population == 0)
                    {
                        WriteLine("|> All Dead. Next");
                        break;
                    }

                    if((i + 1) % 10 == 0)
                    {
                        TimeSpan elapsed = DateTime.Now - start;
                        string interim = elapsed.ToString("mm\\:ss\\.ff");
                        string stats = $"\tElapsed: {interim} TPS: {(i * turnCount) / elapsed.TotalSeconds:0.00000} Pop: {population}";
                        WriteLine(stats);

                        Write(i + 1);
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

            Write($"\tTotal Time: {durationString}\tTurns:{Planet.World.Turns}");

            if(!String.IsNullOrEmpty(error))
            {
                string nl = Environment.NewLine;
                string message = topLine + nl + error + nl;
                //Log.Information(message);

                WriteNewLine(1);
                WriteLine($"\tERROR: {error}");
            }
            else
            {
                int count = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();

                string nl = Environment.NewLine;
                string message = topLine + nl + count + nl;
                //Log.Information(message);

                WriteNewLine(1);
                WriteLine($"\tSurviving: {count}");
            }
            WriteLine();
        }
    }
}
