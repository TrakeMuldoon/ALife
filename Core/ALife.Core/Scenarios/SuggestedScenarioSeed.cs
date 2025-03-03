namespace ALife.Core.Scenarios
{
    /// <summary>
    /// An attribute indicating a suggested seed for a scenario
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SuggestedScenarioSeed
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
        /// Initializes a new instance of the <see cref="SuggestedScenarioSeed"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="description">The description.</param>
        public SuggestedScenarioSeed(int seed, string description)
        {
            Seed = seed;
            Description = description;
        }
    }
}