using System;
using System.Collections.Generic;
using ALife.Core.Scenarios;

namespace ALife.Core.Scenarios
{
    /// <summary>
    /// Stores metadata about a scenario registration
    /// </summary>
    public class RegisteredScenarioMetadata
    {
        /// <summary>
        /// The scenario registration
        /// </summary>
        public readonly ScenarioRegistration ScenarioRegistration;

        /// <summary>
        /// The suggested scenarios
        /// </summary>
        public readonly Dictionary<int, string> SuggestedScenarios;

        /// <summary>
        /// The type
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredScenarioMetadata"/> class.
        /// </summary>
        /// <param name="scenarioRegistration">The scenario registration.</param>
        /// <param name="type">The type.</param>
        /// <param name="suggestedScenarios">The suggested scenarios.</param>
        public RegisteredScenarioMetadata(ScenarioRegistration scenarioRegistration, Type type, Dictionary<int, string> suggestedScenarios)
        {
            ScenarioRegistration = scenarioRegistration;
            Type = type;
            SuggestedScenarios = suggestedScenarios;
        }
    }
}
