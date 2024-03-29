﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ALife.Core.Scenarios
{
    /// <summary>
    /// </summary>
    public static class ScenarioRegister
    {
        /// <summary>
        /// The scenario metadata
        /// </summary>
        private static readonly Dictionary<string, RegisteredScenarioMetadata> scenarios;

        /// <summary>
        /// The starting scenario name, if any
        /// </summary>
        private static readonly string startingScenarioName = string.Empty;

        /// <summary>
        /// Initializes the <see cref="ScenarioRegister"/> class.
        /// </summary>
        static ScenarioRegister()
        {
            bool isDebugMode = false;
#if DEBUG
            isDebugMode = true;
#endif

            scenarios = new Dictionary<string, RegisteredScenarioMetadata>();
            string scenarioInterfaceClassName = typeof(IScenario).Name;
            Type[] typesInAssembly = Assembly.GetCallingAssembly().GetTypes();
            List<Type> potentialScenarioTypes = typesInAssembly.Where(x => x.IsClass && !x.IsAbstract && x.GetInterface(scenarioInterfaceClassName) != null && x.IsDefined(typeof(ScenarioRegistration), false)).ToList();

            foreach(Type scenario in potentialScenarioTypes)
            {
                ScenarioRegistration registrationAttribute = (ScenarioRegistration)scenario.GetCustomAttribute(typeof(ScenarioRegistration), false);
                if(registrationAttribute.DebugModeOnly && !isDebugMode)
                {
                    continue;
                }

                List<SuggestedSeed> suggestedSeeds = scenario.GetCustomAttributes(typeof(SuggestedSeed), false).Select(x => (SuggestedSeed)x).ToList();

                RegisteredScenarioMetadata metadata = new RegisteredScenarioMetadata(registrationAttribute, scenario, suggestedSeeds.ToDictionary(x => x.Seed, x => x.Description));

                scenarios.Add(registrationAttribute.Name, metadata);

                if(registrationAttribute.AutoStartScenario != AutoStartMode.None)
                {
                    if(startingScenarioName != string.Empty)
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
        /// Gets the sorted scenarios.
        /// </summary>
        /// <value>
        /// The sorted scenarios.
        /// </value>
        public static List<string> SortedScenarios => new List<string>(scenarios.Keys).OrderBy(x => x).ToList();

        /// <summary>
        /// Gets the scenario.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>The scenario for the specified name.</returns>
        /// <exception cref="System.Exception">Scenario not found</exception>
        public static IScenario GetScenario(string scenarioName)
        {
            if(!scenarios.TryGetValue(scenarioName, out RegisteredScenarioMetadata type))
            {
                throw new Exception("Scenario not found");
            }

            IScenario instance = (IScenario)Activator.CreateInstance(type.Type);
            return instance;
        }

        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>The suggested seeds for the specified scenario.</returns>
        public static Dictionary<int, string> GetSuggestions(string scenarioName)
        {
            if(!scenarios.TryGetValue(scenarioName, out RegisteredScenarioMetadata type))
            {
                return new Dictionary<int, string>();
            }

            return type.SuggestedScenarios;
        }

        /// <summary>
        /// Gets details on the scenario.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <returns>Details on the scenario.</returns>
        public static ScenarioRegistration GetScenarioDetails(string scenarioName)
        {
            if(!scenarios.TryGetValue(scenarioName, out RegisteredScenarioMetadata type))
            {
                return null;
            }

            return type.ScenarioRegistration;
        }

        /// <summary>
        /// Gets details on the scenario.
        /// </summary>
        /// <param name="scenarioType">Type of the scenario.</param>
        /// <returns>Details on the scenario.</returns>
        public static ScenarioRegistration GetScenarioDetails(Type scenarioType)
        {
            RegisteredScenarioMetadata scenarioDetails = scenarios.Values.FirstOrDefault(x => x.Type == scenarioType);
            return scenarioDetails.ScenarioRegistration;
        }

        /// <summary>
        /// Gets the automatic start scenario.
        /// </summary>
        /// <returns>The name and starting seed for the auto start scenario, or null</returns>
        public static (string, int?, AutoStartMode)? GetAutoStartScenario()
        {
            if(string.IsNullOrWhiteSpace(startingScenarioName))
            {
                return null;
            }

            ScenarioRegistration scenario = GetScenarioDetails(startingScenarioName);
            return (startingScenarioName, scenario.AutoStartSeed, scenario.AutoStartScenario);
        }
    }
}
