using ALifeUni.ALife;
using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.WorldObjects.Agents;
using System;
using System.Linq;

namespace ALifeUni.ScenarioRunners
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
                if(startingSeed.HasValue)
                {
                    RunSingleSeed(scenarioName, startingSeed.Value);
                }
                else
                {
                    RunSetOfRandomSeeds(scenarioName);
                }
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
                Write($"--------------------------------------------------------------------------{Environment.NewLine}");
            }
        }


        private void RunSingleSeed(string scenarioName, int startingSeed)
        {
            IScenario scenario = ScenarioRegister.GetScenario(scenarioName);

            Write($"Executing Single Scenario");
            RunSeed(startingSeed, scenario);
            WriteLineSeperator(1);
            WriteNewLine(1);
            //TODO: THIS IS A DEBUG THING
            Write($"Executing Single Scenario");
            RunSeed(startingSeed, scenario);
            WriteLineSeperator(1);
            WriteNewLine(1);
        }

        private void RunSetOfRandomSeeds(string scenarioName)
        {
            IScenario scenario = ScenarioRegister.GetScenario(scenarioName);

            Random r = new Random();
            for(int i = 0; i < 20; i++)
            {
                if(CancelRunner)
                {
                    break;
                }
                Write($"Scenario Execution #{i} -> ");
                int seedValue = r.Next();
                RunSeed(seedValue, scenario);
                WriteLineSeperator(1);
                WriteNewLine(1);
            }
        }

        //514029898
        const int TOTAL_TURNS = 50000;
        const int TURN_BATCH = 1000;
        const int UPDATE_FREQUENCY = 10000;
        private void RunSeed(int seedValue, IScenario scenario)
        {
            ScenarioRegistration scenarioDetails = ScenarioRegister.GetScenarioDetails(scenario.GetType());
            int height = scenario.WorldHeight;
            int width = scenario.WorldWidth;

            //Write Header
            string topLine = $"Seed: {seedValue}, Name: {scenarioDetails.Name}, Height:{height}, Width:{width}";
            WriteLine($"\t{topLine}");
            
            //Get World Ready
            DateTime start = DateTime.Now;
            IScenario newCopy = IScenarioHelpers.FreshInstanceOf(scenario);
            Planet.CreateWorld(seedValue, scenario, height, width);

            string error = null;
            try
            {
                WriteLine($"Each . represents {TURN_BATCH} turns");
                Write("    [0]");
                for(int i = 0; i < TOTAL_TURNS/TURN_BATCH; i++)
                {
                    if(CancelRunner)
                    {
                        break;
                    }
                    Planet.World.ExecuteManyTurns(TURN_BATCH);
                    Write(".");

                    if(IsSimulationComplete())
                    {
                        break;
                    }

                    if((i + 1) % (UPDATE_FREQUENCY/TURN_BATCH) == 0)
                    {
                        Planet instance = Planet.World;
                        int population = instance.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
                        TimeSpan elapsed = DateTime.Now - start;
                        string interim = elapsed.ToString("mm\\:ss\\.ff");
                        string stats = $"[{Planet.World.Turns}]\tElapsed: {interim} TPS: {(i * TURN_BATCH) / elapsed.TotalSeconds:0.00000} Pop: {population}";
                        WriteLine(stats);

                        Write($"[{Planet.World.Turns}]");
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

        public bool IsSimulationComplete()
        {
            int population = Planet.World.AllActiveObjects.OfType<Agent>().Where(wo => wo.Alive).Count();
            if(population == 0)
            {
                WriteLine("|> All Dead. Next");
                return true;
            }
            return false;
        }
    }
}
