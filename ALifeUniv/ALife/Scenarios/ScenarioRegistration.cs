using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Scenarios
{
    /// <summary>
    /// A registered Scenario
    /// </summary>
    public class ScenarioRegistration
    {
        /// <summary>
        /// The name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The suggested seeds
        /// </summary>
        public readonly Dictionary<int, string> SuggestedSeeds;

        /// <summary>
        /// The type
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioRegistration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="suggestedSeeds">The suggested seeds.</param>
        /// <param name="type">The type.</param>
        public ScenarioRegistration(string name, Dictionary<int, string> suggestedSeeds, Type type)
        {
            Name = name;
            SuggestedSeeds = suggestedSeeds;
            Type = type;
        }
    }
}
