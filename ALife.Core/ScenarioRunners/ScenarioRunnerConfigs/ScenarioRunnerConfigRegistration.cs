using ALife.Core.Scenarios;

namespace ALife.Core.ScenarioRunners.ScenarioRunnerConfigs
{
    /// <summary>
    /// An attribute registering a scenario runner config
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScenarioRunnerConfigRegistration : Attribute
    {
        /// <summary>
        /// The name
        /// </summary>
        public readonly string ConfigName;

        /// <summary>
        /// The scenario type this config is tied to
        /// </summary>
        public readonly Type ScenarioType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioRunnerConfigRegistration"/> class.
        /// </summary>
        /// <param name="scenarioType">The type.</param>
        /// <param name="configName">The name. Defaults to "Default".</param>
        public ScenarioRunnerConfigRegistration(Type scenarioType)
        {
            ScenarioType = scenarioType;
            var hasInterface = scenarioType.GetInterfaces().Where(x => x == typeof(IScenario)).FirstOrDefault() != null;

            if (!hasInterface)
            {
                throw new ArgumentException($"ScenarioType must be a subclass of {nameof(IScenario)}!");
            }

            // TODO: Right now, we'll only support one config per scenario type.  We can change this later if we want to allow multiple configs per scenario type.
            ConfigName = Constants.DEFAULT_SCENARIO_RUNNER_CONFIG_NAME;
        }
    }
}
