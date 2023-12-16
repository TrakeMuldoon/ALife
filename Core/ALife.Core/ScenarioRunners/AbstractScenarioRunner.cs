using ALife.Core.ScenarioRunners.ScenarioLoggers;
using ALife.Core.ScenarioRunners.ScenarioRunnerConfigs;
using ALife.Core.Scenarios;

namespace ALife.Core.ScenarioRunners
{
    /// <summary>
    /// An abstract scenario runner
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public abstract class AbstractScenarioRunner : IDisposable
    {
        /// <summary>
        /// The execution task
        /// </summary>
        protected Task executionTask;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private readonly CancellationTokenSource cancellationTokenSource = new();

        /// <summary>
        /// The scenario
        /// </summary>
        private readonly IScenario scenario;

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Whether we should stop the runner (true) or not (false)
        /// </summary>
        private bool stopRunner = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractScenarioRunner"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="startingSeed">The starting seed.</param>
        /// <param name="numberSeedsToExecute">The number seeds to execute.</param>
        /// <param name="totalTurns">The total turns.</param>
        /// <param name="turnBatch">The turn batch.</param>
        /// <param name="updateFrequency">The update frequency.</param>
        /// <param name="logger">The logger.</param>
        public AbstractScenarioRunner(string scenarioName
                                      , int? startingSeed = null
                                        , int numberSeedsToExecute = Constants.DEFAULT_NUMBER_SEEDS_EXECUTED
                                        , int totalTurns = Constants.DEFAULT_TOTAL_TURNS
                                        , int turnBatch = Constants.DEFAULT_TURN_BATCH
                                        , int updateFrequency = Constants.DEFAULT_UPDATE_FREQUENCY
                                        , Logger? logger = null
                                        , Logger? scenarioSeedLogger = null)
        {
            // instantiate needed variables
            Logger = (Logger)(logger ?? Activator.CreateInstance(LoggerType));
            ScenarioSeedLogger = (Logger)(scenarioSeedLogger ?? Activator.CreateInstance(LoggerType));
            scenario = ScenarioRegister.GetScenario(scenarioName);
            StartingSeed = startingSeed;
            NumberSeedsToExecute = numberSeedsToExecute;
            TotalTurns = totalTurns;
            TurnBatch = turnBatch;
            UpdateFrequency = updateFrequency;
            ExecutionNumber = 1;

            // Execute the runner!
            stopRunner = false;
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = cancellationTokenSource.Token;
            Logger.StartLogger(ct);
            ScenarioSeedLogger.StartLogger(ct);
            executionTask = new Task(() => ExecuteRunner(ct), ct);
            executionTask.Start();
        }

        /// <summary>
        /// Gets the execution number.
        /// </summary>
        /// <value>The execution number.</value>
        public int ExecutionNumber { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is stopped.
        /// </summary>
        /// <value><c>true</c> if this instance is stopped; otherwise, <c>false</c>.</value>
        public bool IsStopped => ((executionTask?.IsCompleted ?? true) || (executionTask?.IsCanceled ?? true)) && stopRunner;

        /// <summary>
        /// Gets a value indicating whether this instance is stopped and logger stopped.
        /// </summary>
        /// <value><c>true</c> if this instance is stopped and logger stopped; otherwise, <c>false</c>.</value>
        public bool IsStoppedAndLoggerStopped => IsStopped && Logger.IsStopped;

        /// <summary>
        /// Gets the number seeds to execute.
        /// </summary>
        /// <value>The number seeds to execute.</value>
        public int NumberSeedsToExecute { get; private set; }

        /// <summary>
        /// The starting seed
        /// </summary>
        public int? StartingSeed { get; private set; }

        /// <summary>
        /// Gets the total turns.
        /// </summary>
        /// <value>The total turns.</value>
        public int TotalTurns { get; private set; }

        /// <summary>
        /// Gets the turn batch.
        /// </summary>
        /// <value>The turn batch.</value>
        public int TurnBatch { get; private set; }

        /// <summary>
        /// Gets the update frequency.
        /// </summary>
        /// <value>The update frequency.</value>
        public int UpdateFrequency { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected Logger Logger { get; }

        /// <summary>
        /// Gets the type of the logger.
        /// </summary>
        /// <value>The type of the logger.</value>
        protected abstract Type LoggerType { get; }

        /// <summary>
        /// Gets the scenario seed logger.
        /// </summary>
        /// <value>The logger.</value>
        protected Logger ScenarioSeedLogger { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops the runner.
        /// </summary>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        public void StopRunner(bool wait = false)
        {
            cancellationTokenSource.Cancel();
            stopRunner = true;
            if(wait)
            {
                while(!IsStopped)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    StopRunner(true);
                }
                cancellationTokenSource.Dispose();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Checks if we should stop the runner or not
        /// </summary>
        /// <returns>True to stop runner, false otherwise</returns>
        protected abstract bool ShouldStopRunner();

        /// <summary>
        /// Executes the runner.
        /// </summary>
        /// <param name="ct">The ct.</param>
        private void ExecuteRunner(CancellationToken ct)
        {
            bool shouldStop;

            do
            {
                if(StartingSeed != null)
                {
                    ScenarioExecutor(StartingSeed.Value, "Executing Single Scenario", ct);
                }
                else
                {
                    Random r = new();
                    for(int i = 0; i < NumberSeedsToExecute; i++)
                    {
                        string message = $"Scenario Execution #{ExecutionNumber++}/{NumberSeedsToExecute} -> ";
                        int seedValue = r.Next();
                        ScenarioExecutor(seedValue, message, ct);

                        if(ct.IsCancellationRequested)
                        {
                            ct.ThrowIfCancellationRequested();
                        }
                    }
                }
                shouldStop = ShouldStopRunner() || stopRunner;
            } while(!shouldStop);
        }

        /// <summary>
        /// Executes a scenario
        /// </summary>
        /// <param name="seedValue">The seed value.</param>
        /// <param name="headerMessage">The header message.</param>
        /// <param name="ct">The ct.</param>
        private void ScenarioExecutor(int seedValue, string headerMessage, CancellationToken ct)
        {
            AbstractScenarionRunnerConfig config = ScenarioRunnerConfigRegister.GetDefaultConfigForScenarioType(scenario.GetType());
            // Display Header Message
            Logger.Write(headerMessage);
            Logger.WriteNewLine(1);
            Logger.WriteLineSeperator(1);
            Logger.WriteNewLine(1);

            ScenarioRegistration scenarioDetails = ScenarioRegister.GetScenarioDetails(scenario.GetType());
            int height = scenario.WorldHeight;
            int width = scenario.WorldWidth;

            //Write Header
            string topLine = $"Seed: {seedValue}, Name: {scenarioDetails.Name}, Height:{height}, Width:{width}, Max Turns: {TotalTurns}";
            Logger.WriteLine($"{topLine}");

            //Get World Ready
            DateTime start = DateTime.Now;
            IScenario newCopy = IScenarioHelpers.FreshInstanceOf(scenario);
            Planet.CreateWorld(seedValue, newCopy, height, width);

            string error = null;
            try
            {
                int turnsWidth = TotalTurns.ToString().Length;
                string turnStringFormat = $"D{turnsWidth}";
                string turnSpaces = new(' ', turnsWidth - 1);
                Logger.WriteLine($"Each . represents {TurnBatch} turns");
                Logger.Write($"{turnSpaces}[0]");
                for(int i = 0; i < TotalTurns / TurnBatch; i++)
                {
                    if(ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                    Planet.World.ExecuteManyTurns(TurnBatch);
                    Logger.Write(".");

                    if(config.ShouldEndSimulation(Logger.Write))
                    {
                        break;
                    }

                    if((i + 1) % (UpdateFrequency / TurnBatch) == 0)
                    {
                        TimeSpan elapsed = DateTime.Now - start;
                        int turnSpacesNeeded = turnsWidth - Planet.World.Turns.ToString().Length;
                        turnSpaces = new string(' ', turnSpacesNeeded);

                        string stats = $"{turnSpaces}[{Planet.World.Turns}]\tElapsed: {elapsed:mm\\:ss\\.ff} TPS: {i * TurnBatch / elapsed.TotalSeconds:0.000} || ";
                        Logger.Write(stats);
                        config.UpdateStatusDetails(Logger.Write);

                        Logger.Write($"{turnSpaces}[{Planet.World.Turns}]");
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

            Logger.WriteLine($"\tTotal Time: {durationString}\tTurns:{Planet.World.Turns}");

            if(!string.IsNullOrEmpty(error))
            {
                Logger.WriteLine($"\tERROR: {error}");
            }
            else
            {
                config.SimulationSuccessInformation(Logger.Write);
                if(config.ScenarioState == ScenarioState.CompleteSuccessful)
                {
                    ScenarioSeedLogger.WriteLine(seedValue);
                }
            }
            Logger.WriteNewLine(1);
        }
    }
}
