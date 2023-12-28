using ALife.Core.Scenarios;
using ALife.Core.Utility;
using ALife.Core.WorldObjects;
using ALife.Core.WorldObjects.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ALife.Core
{
    /// <summary>
    /// An simulation controller.
    /// TODO: If we want to do things properly, we should probably make Planet not a singleton and have Planet be a field/property of this class (and the Planet passed down as needed to agents, etc.)
    /// </summary>
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
        /// The active genes and their population counts.
        /// </summary>
        private Dictionary<string, int> _activeGenes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationController"/> class.
        /// </summary>
        public SimulationController() : this(string.Empty, null, null, null)
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
            _activeGenes = new();
            ScenarioName = scenarioName;
            StartingSeed = startingSeed;
            if(!string.IsNullOrWhiteSpace(scenarioName))
            {
                PopulateSimulationDetails(width, height);
            }
        }

        /// <summary>
        /// Gets the active agent count.
        /// </summary>
        /// <value>The active agent count.</value>
        public int ActiveAgentCount
        {
            get
            {
                if(Planet.HasWorld)
                {
                    List<WorldObject> objects = new(Planet.World.AllActiveObjects);
                    return objects.Where(wo => wo.Alive && wo is Agent).Count();
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the active gene count.
        /// </summary>
        /// <value>The active gene count.</value>
        public int ActiveGeneCount => _activeGenes.Count;

        /// <summary>
        /// Gets the active genes.
        /// </summary>
        /// <value>The active genes.</value>
        public Dictionary<string, int> ActiveGenes => _activeGenes;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Gets the scenario.
        /// </summary>
        /// <value>The scenario.</value>
        public IScenario Scenario { get; private set; }

        /// <summary>
        /// Gets the scenario details.
        /// </summary>
        /// <value>The scenario details.</value>
        public ScenarioRegistration ScenarioDetails { get; private set; }

        /// <summary>
        /// Gets the simulation description.
        /// </summary>
        /// <value>The simulation description.</value>
        public string SimulationDescription => ScenarioDetails.Description;

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        /// <value>The name of the simulation.</value>
        public string SimulationName => ScenarioDetails.Name;

        /// <summary>
        /// Gets the TPS counter.
        /// </summary>
        /// <value>The TPS counter.</value>
        public PerformanceCounter? TpsCounter
        {
            get
            {
                if(!Planet.HasWorld)
                {
                    return null;
                }

                return Planet.World.SimulationPerformance;
            }
        }

        /// <summary>
        /// Gets the turn count.
        /// </summary>
        /// <value>The turn count.</value>
        public int TurnCount
        {
            get
            {
                if(Planet.HasWorld)
                {
                    return 0;
                }
                return Planet.World.Turns;
            }
        }

        /// <summary>
        /// Gets the zone information.
        /// </summary>
        /// <value>The zone information.</value>
        public string ZoneInfo { get; private set; }

        /// <summary>
        /// Executes a wolrd tick.
        /// </summary>
        /// <param name="updateInfo">if set to <c>true</c> [update information].</param>
        public virtual void ExecuteTick(bool updateInfo = true)
        {
            Planet.World.ExecuteOneTurn();
            if(updateInfo)
            {
                UpdateGeneology();
                UpdateZoneInfo();
            }
        }

        /// <summary>
        /// Executes multiple world ticks.
        /// </summary>
        /// <param name="numberTicks">The number of ticks to execute.</param>
        /// <param name="updateInfo">if set to <c>true</c> [update information].</param>
        public virtual void ExecuteTicks(int numberTicks = 1, bool updateInfo = true)
        {
            for(var i = 0; i < numberTicks; i++)
            {
                Planet.World.ExecuteOneTurn();
            }
            if(updateInfo)
            {
                UpdateGeneology();
                UpdateZoneInfo();
            }
        }

        /// <summary>
        /// Initializes the simulation.
        /// </summary>
        public virtual void InitializeSimulation()
        {
            IsInitialized = false;
            if(string.IsNullOrWhiteSpace(ScenarioName))
            {
                throw new Exception($"{nameof(ScenarioName)} is required.");
            }
            if(Scenario == null)
            {
                PopulateSimulationDetails(null, null);
            }

            Random r = new();
            IScenario newWorld = IScenarioHelpers.FreshInstanceOf(Scenario);

            int seed = StartingSeed ?? r.Next();

            Planet.CreateWorld(seed, newWorld, SimulationHeight, SimulationWidth);
            IsInitialized = true;
        }

        /// <summary>
        /// Populates the simulation details.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void PopulateSimulationDetails(int? width, int? height)
        {
            Scenario = ScenarioRegister.GetScenario(ScenarioName);
            ScenarioDetails = ScenarioRegister.GetScenarioDetails(Scenario.GetType());

            SimulationWidth = width ?? Scenario.WorldWidth;
            SimulationHeight = height ?? Scenario.WorldHeight;
        }

        /// <summary>
        /// Updates the geneology.
        /// </summary>
        private void UpdateGeneology()
        {
            Dictionary<string, int> geneCount = new Dictionary<string, int>();

            List<WorldObject> objects = new(Planet.World.AllActiveObjects);
            for(int i = 0; i < objects.Count; i++)
            {
                WorldObject wo = objects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    string gene = ag.IndividualLabel.Substring(0, 3);
                    if(!geneCount.ContainsKey(gene))
                    {
                        geneCount.Add(gene, 0);
                    }
                    ++geneCount[gene];
                }
            }

            _activeGenes = geneCount;
        }

        /// <summary>
        /// Updates the zone information.
        /// </summary>
        private void UpdateZoneInfo()
        {
            Dictionary<string, int> zoneCount = new Dictionary<string, int>();
            foreach(Zone z in Planet.World.Zones.Values)
            {
                zoneCount.Add(z.Name, 0);
            }
            StringBuilder sb = new StringBuilder();
            List<WorldObject> objects = new(Planet.World.AllActiveObjects);
            for(int i = 0; i < objects.Count; i++)
            {
                WorldObject wo = objects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    zoneCount[ag.HomeZone.Name]++;
                }
            }

            int maxNameLength = zoneCount.Keys.Max(k => k.Length);
            int maxZoneCount = zoneCount.Values.Max();

            foreach(string name in zoneCount.Keys)
            {
                string nameSpaces = new string(' ', maxNameLength - name.Length);

                sb.AppendLine($"{nameSpaces}{name}: {zoneCount[name]}");
            }
            ZoneInfo = sb.ToString();
        }
    }
}
