using System;

namespace ALife.Core.Scenarios
{
    /// <summary>
    /// An attribute indicating a suggested seed for a scenario
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class SuggestedSeed : Attribute
    {
        /// <summary>
        /// The description of the seed
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The seed
        /// </summary>
        public readonly int Seed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuggestedSeed"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="description">The description.</param>
        public SuggestedSeed(int seed, string description)
        {
            Seed = seed;
            Description = description;
        }
    }
}
