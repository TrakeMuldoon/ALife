using ALife.Core.Scenarios;
using ALifeUni.ScenarioRunners.ScenarioRunnerConfigs.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ALifeUni.ScenarioRunners.ScenarioRunnerConfigs
{
    /// <summary>
    /// A register of all scenario runner configs
    /// </summary>
    public static class ScenarioRunnerConfigRegister
    {
        /// <summary>
        /// The registered scenario configs
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<string, Type>> scenarioConfigs;

        /// <summary>
        /// Initializes the <see cref="ScenarioRunnerConfigRegister"/> class.
        /// </summary>
        static ScenarioRunnerConfigRegister()
        {
            scenarioConfigs = new Dictionary<Type, Dictionary<string, Type>>();

            // Find all the types in the assembly that are subclasses of AbstractScenarioRunnerConfig and have the
            // ScenarioRunnerConfigRegistration attribute
            Type[] typesInAssembly = Assembly.GetCallingAssembly().GetTypes();
            List<Type> potentialConfigs = typesInAssembly.Where(x => x.IsClass 
                                                                && !x.IsAbstract 
                                                                && x.IsSubclassOf(typeof(AbstractScenarionRunnerConfig)) 
                                                                && x.IsDefined(typeof(ScenarioRunnerConfigRegistration), false)
                                                                ).ToList();

            // Loop through and register them
            foreach(Type config in potentialConfigs)
            {
                ScenarioRunnerConfigRegistration registrationAttribute = (ScenarioRunnerConfigRegistration)config.GetCustomAttribute(typeof(ScenarioRunnerConfigRegistration), false);

                if(!scenarioConfigs.ContainsKey(registrationAttribute.ScenarioType))
                {
                    scenarioConfigs.Add(registrationAttribute.ScenarioType, new Dictionary<string, Type>());
                }
                scenarioConfigs[registrationAttribute.ScenarioType].Add(registrationAttribute.ConfigName, config);
            }
        }

        /// <summary>
        /// Gets the default type of the scenario configuration for.
        /// </summary>
        /// <param name="scenarioType">Type of the scenario.</param>
        /// <returns>The default scenario config, or null.</returns>
        public static AbstractScenarionRunnerConfig GetDefaultConfigForScenarioType(Type scenarioType)
        {
            if(!IsInstanceOfInterface(scenarioType, typeof(IScenario)))
            {
                throw new ArgumentException($"ScenarioType must be a subclass of {nameof(IScenario)}!");
            }

            Dictionary<string, Type> configs = GetConfigsForScenarioType(scenarioType);

            if(configs.Count == 0 || !configs.ContainsKey(Constants.DEFAULT_SCENARIO_RUNNER_CONFIG_NAME))
            {
                return new DefaultScenarioRunnerConfig();
            }

            AbstractScenarionRunnerConfig instance = (AbstractScenarionRunnerConfig)Activator.CreateInstance(configs[Constants.DEFAULT_SCENARIO_RUNNER_CONFIG_NAME]);
            return instance;
        }

        /// <summary>
        /// Gets the type of the configs for scenario.
        /// </summary>
        /// <param name="scenarioType">Type of the scenario.</param>
        /// <returns>A dictioanry of the registered configs.</returns>
        private static Dictionary<string, Type> GetConfigsForScenarioType(Type scenarioType)
        {
            if(!IsInstanceOfInterface(scenarioType, typeof(IScenario)))
            {
                throw new ArgumentException($"ScenarioType must be a subclass of {nameof(IScenario)}!");
            }

            Dictionary<string, Type> configs = new Dictionary<string, Type>();

            if(scenarioConfigs.ContainsKey(scenarioType))
            {
                configs = scenarioConfigs[scenarioType];
            }
            else
            {
                configs[Constants.DEFAULT_SCENARIO_RUNNER_CONFIG_NAME] = typeof(DefaultScenarioRunnerConfig);
            }

            return configs;
        }

        /// <summary>
        /// Determines whether [is instance of interface] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns><c>true</c> if [is instance of interface] [the specified type]; otherwise, <c>false</c>.</returns>
        private static bool IsInstanceOfInterface(Type type, Type interfaceType)
        {
            return type.GetInterfaces().Where(x => x == interfaceType).FirstOrDefault() != null;
        }
    }
}
