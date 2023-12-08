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
        /// The starting scenario name, if any
        /// </summary>
        private static readonly string startingScenarioName = string.Empty;

        /// <summary>
        /// Initializes the <see cref="ScenarioFactory"/> class.
        /// </summary>
        static ScenarioFactory()
        {
            bool isDebugMode = false;
#if DEBUG
            isDebugMode = true;
#endif

            scenarios = new Dictionary<string, ScenarioRegistrationMetadata>();
            string scenarioInterfaceClassName = typeof(IScenario).Name;
            Type[] typesInAssembly = Assembly.GetCallingAssembly().GetTypes();
            List<Type> potentialScenarioTypes = typesInAssembly.Where(x => x.IsClass && !x.IsAbstract && x.GetInterface(scenarioInterfaceClassName) != null && x.IsDefined(typeof(ScenarioRegistration), false)).ToList();

            foreach (Type scenario in potentialScenarioTypes)
            {
                ScenarioRegistration registrationAttribute = (ScenarioRegistration)scenario.GetCustomAttribute(typeof(ScenarioRegistration), false);
                if (registrationAttribute.DebugModeOnly && !isDebugMode)
                {
                    continue;
                }

                List<SuggestedSeed> suggestedSeeds = scenario.GetCustomAttributes(typeof(SuggestedSeed), false).Select(x => (SuggestedSeed)x).ToList();

                ScenarioRegistrationMetadata metadata = new ScenarioRegistrationMetadata(registrationAttribute, scenario, suggestedSeeds.ToDictionary(x => x.Seed, x => x.Description));

                scenarios.Add(registrationAttribute.Name, metadata);

                if (registrationAttribute.AutoStartScenario)
                {
                    if (startingScenarioName != string.Empty)
                    {
                        throw new Exception("Multiple scenarios have been marked as the auto-start scenario");
                    }

                    startingScenarioName = registrationAttribute.Name;
                }
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

        /// <summary>
        /// Gets the automatic start scenario.
        /// </summary>
        /// <returns>The name and starting seed for the auto start scenario, or null</returns>
        public static Nullable<(string, Nullable<int>)> GetAutoStartScenario()
        {
            if (string.IsNullOrWhiteSpace(startingScenarioName))
            {
                return null;
            }

            ScenarioRegistration scenario = GetRegistrationDetails(startingScenarioName);
            return (startingScenarioName, scenario.AutoStartSeed);
        }
    }
}
