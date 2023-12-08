using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ALifeUni.ALife.Scenarios
{
    /// <summary>
    /// </summary>
    public static class ScenarioFactory
    {
        /// <summary>
        /// The scenario metadata
        /// </summary>
        private static readonly Dictionary<string, ScenarioRegistrationMetadata> scenarios;

        /// <summary>
        /// Initializes the <see cref="ScenarioFactory"/> class.
        /// </summary>
        static ScenarioFactory()
        {
            scenarios = new Dictionary<string, ScenarioRegistrationMetadata>();
            string scenarioInterfaceClassName = typeof(IScenario).Name;
            Type[] typesInAssembly = Assembly.GetCallingAssembly().GetTypes();
            List<Type> potentialScenarioTypes = typesInAssembly.Where(x => x.IsClass && !x.IsAbstract && x.GetInterface(scenarioInterfaceClassName) != null && x.IsDefined(typeof(ScenarioRegistration), false)).ToList();

            foreach (Type scenario in potentialScenarioTypes)
            {
                ScenarioRegistration registrationAttribute = (ScenarioRegistration)scenario.GetCustomAttribute(typeof(ScenarioRegistration), false);
                List<SuggestedSeed> suggestedSeeds = scenario.GetCustomAttributes(typeof(SuggestedSeed), false).Select(x => (SuggestedSeed)x).ToList();

                ScenarioRegistrationMetadata metadata = new ScenarioRegistrationMetadata(registrationAttribute, scenario, suggestedSeeds.ToDictionary(x => x.Seed, x => x.Description));

                scenarios.Add(registrationAttribute.Name, metadata);
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
            if (!scenarios.TryGetValue(scenarioName, out var type))
            {
                throw new Exception("Scenario not found");
            }

            var instance = (IScenario)Activator.CreateInstance(type.Type);
            return instance;
        }

        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>The suggested seeds for the specified scenario.</returns>
        public static Dictionary<int, string> GetSuggestions(string scenarioName)
        {
            if (!scenarios.TryGetValue(scenarioName, out var type))
            {
                throw new Exception("Scenario not found");
            }

            return type.SuggestedScenarios;
        }

        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>The suggested seeds for the specified scenario.</returns>
        public static ScenarioRegistration GetRegistrationDetails(string scenarioName)
        {
            if (!scenarios.TryGetValue(scenarioName, out var type))
            {
                throw new Exception("Scenario not found");
            }

            return type.ScenarioRegistration;
        }
    }
}
