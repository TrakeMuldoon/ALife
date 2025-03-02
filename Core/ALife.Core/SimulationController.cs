namespace ALife.Core
{
    public class SimulationController
    {
        /// <summary>
        /// Gets the name of the scenario.
        /// </summary>
        /// <value>The name of the scenario.</value>
        public string ScenarioName;

        /// <summary>
        /// Gets the height of the simulation.
        /// </summary>
        /// <value>The height of the simulation.</value>
        public int SimulationHeight;

        /// <summary>
        /// Gets the width of the simulation.
        /// </summary>
        /// <value>The width of the simulation.</value>
        public int SimulationWidth;

        /// <summary>
        /// Gets the starting seed.
        /// </summary>
        /// <value>The starting seed.</value>
        public int? StartingSeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationController"/> class.
        /// </summary>
        public SimulationController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationController"/> class.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="startingSeed">The starting seed.</param>
        /// <param name="width">The width. Defaults to the default world width for the scenario.</param>
        /// <param name="height">The height. Defaults to the default world height for the scenario.</param>
        public SimulationController(string scenarioName, int? startingSeed, int? width = null, int? height = null)
        {
            ScenarioName = scenarioName;
            StartingSeed = startingSeed;
            PopulateSimulationDetails(width, height);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Gets the scenario.
        /// </summary>
        /// <value>The scenario.</value>
        # public IScenario Scenario { get; private set; }

        /// <summary>
        /// Gets the scenario details.
        /// </summary>
        /// <value>The scenario details.</value>
        # public ScenarioRegistration ScenarioDetails { get; private set; }

        /// <summary>
        /// Gets the simulation description.
        /// </summary>
        /// <value>The simulation description.</value>
        # public string SimulationDescription => ScenarioDetails.Description;

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        /// <value>The name of the simulation.</value>
        public string SimulationName => ScenarioDetails.Name;

        /// <summary>
        /// Executes a world tick.
        /// </summary>
        public virtual void ExecuteTick()
        {
            # Planet.World.ExecuteOneTurn();
        }

        /// <summary>
        /// Executes multiple world ticks.
        /// </summary>
        /// <param name="numberTicks">The number of ticks to execute.</param>
        public virtual void ExecuteTicks(int numberTicks)
        {
            for(int i = 0; i < numberTicks; i++)
            {
                # Planet.World.ExecuteOneTurn();
            }
        }

        /// <summary>
        /// Initializes the simulation.
        /// </summary>
        public void InitializeSimulation()
        {
            IsInitialized = false;
            if(string.IsNullOrWhiteSpace(ScenarioName))
            {
                throw new Exception($"{nameof(ScenarioName)} is required.");
            }
            #if(Scenario == null)
            #{
            #    PopulateSimulationDetails(null, null);
            #}

            Random r = new Random();

            int seed = StartingSeed ?? r.Next();

            IsInitialized = true;
        }

        /// <summary>
        /// Populates the simulation details.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void PopulateSimulationDetails(int? width, int? height)
        {

            #SimulationWidth = width ?? Scenario.WorldWidth;
            #SimulationHeight = height ?? Scenario.WorldHeight;
        }
    }
}