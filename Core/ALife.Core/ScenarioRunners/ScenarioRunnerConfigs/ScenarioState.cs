namespace ALife.Core.ScenarioRunners.ScenarioRunnerConfigs
{
    /// <summary>
    /// An enum representing different states for a scenario
    /// TODO: If scenario configs are expanded, etc., we can add more potential states here
    /// </summary>
    public enum ScenarioState
    {
        /// <summary>
        /// Was successful and is complete
        /// </summary>
        CompleteSuccessful,

        /// <summary>
        /// Was not successful, but is complete
        /// </summary>
        Complete,

        /// <summary>
        /// The scenario is still in progress
        /// </summary>
        InProgress,
    }
}
