using System;
using System.Collections.Generic;
using ALifeUni.ALife.Scenarios.FieldCrossings;

namespace ALifeUni.ALife.Scenarios
{
    /// <summary>
    /// </summary>
    public static class ScenarioFactory
    {
        /// <summary>
        /// The scenarios
        /// </summary>
        private static readonly Dictionary<string, ScenarioRegistration> scenarios;

        /// <summary>
        /// Initializes the <see cref="ScenarioFactory"/> class.
        /// </summary>
        static ScenarioFactory()
        {
            List<ScenarioRegistration> registeredScenarios = new List<ScenarioRegistration>
            {
                new ScenarioRegistration("Maze", new Dictionary<int, string> {}, typeof(MazeScenario)),
                new ScenarioRegistration("GenerationalMazeScenario", new Dictionary<int, string> {}, typeof(GenerationalMazeScenario)),
                new ScenarioRegistration("DripFeedMaze", new Dictionary<int, string> {}, typeof(DripFeedMaze)),
                new ScenarioRegistration("CarTrackMaze", new Dictionary<int, string> { { 1832460063, "Fun scenario!!!" } }, typeof(CarTrackMaze)),
                new ScenarioRegistration("FieldCrossingScenario", new Dictionary<int, string> {}, typeof(FieldCrossingScenario)),
                new ScenarioRegistration("FieldCrossingLowReproScenario", new Dictionary<int, string> {}, typeof(FieldCrossingLowReproScenario)),
                new ScenarioRegistration("FieldCrossingWallsScenario", new Dictionary<int, string> {}, typeof(FieldCrossingWallsScenario)),
                new ScenarioRegistration("MushroomScenario", new Dictionary<int, string> {}, typeof(MushroomScenario)),
            };

            scenarios = new Dictionary<string, ScenarioRegistration>();
            foreach (ScenarioRegistration scenario in registeredScenarios)
            {
                scenarios.Add(scenario.Name, scenario);
            }
        }

        /// <summary>
        /// Gets a list of scenario names.
        /// </summary>
        /// <value>The scenario names.</value>
        public static List<string> Scenarios => new List<string>(scenarios.Keys);

        /// <summary>
        /// Gets the scenario.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>The scenario for the specified name.</returns>
        /// <exception cref="System.Exception">Scenario not found</exception>
        public static IScenario GetScenario(string scenarioName)
        {
            return scenarios.TryGetValue(scenarioName, out var scenarioType)
                ? (IScenario)Activator.CreateInstance(scenarioType.Type)
                : throw new Exception("Scenario not found");
        }

        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>The suggested seeds for the specified scenario.</returns>
        public static Dictionary<int, string> GetSuggestions(string scenarioName)
        {
            return scenarios.TryGetValue(scenarioName, out var scenarioType)
                ? scenarioType.SuggestedSeeds
                : new Dictionary<int, string>();
        }
    }
}
