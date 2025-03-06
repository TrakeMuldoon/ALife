using ALife.Core.WorldObjects.Agents;

namespace ALife.Core.Scenarios;

/// <summary>
/// Defines the base abstract class for a scenario.
/// </summary>
public abstract class AScenario
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AScenario"/> class.
    /// </summary>
    /// <param name="defaultPlanetWidth">The default planet width.</param>
    /// <param name="defaultPlanetHeight">The default planet height.</param>
    /// <param name="isFixedPlanetSize">Determines if the planet size is fixed (True) or not (False).</param>
    public AScenario(int defaultPlanetWidth, int defaultPlanetHeight, bool isFixedPlanetSize)
    {
        DefaultPlanetWidth = defaultPlanetWidth;
        DefaultPlanetHeight = defaultPlanetHeight;
        IsFixedPlanetSize = isFixedPlanetSize;
    }
    
    /// <summary>
    /// The default planet width.
    /// </summary>
    public int DefaultPlanetWidth { get; }
    
    /// <summary>
    /// The default planet height.
    /// </summary>
    public int DefaultPlanetHeight { get; }
    
    /// <summary>
    /// Determines if the planet size is fixed (True) or not (False).
    /// </summary>
    public bool IsFixedPlanetSize { get; }

    /// <summary>
    /// Sets up the planet.
    /// </summary>
    /// <param name="planet">The planet to setup.</param>
    public abstract void PlanetSetup(Planet planet);

    /// <summary>
    /// Executes the global end of turn actions against the planet.
    /// </summary>
    /// <param name="planet">The planet.</param>
    public abstract void GlobalEndOfTurnActions(Planet planet);
    
    /// <summary>
    /// Executes the global end of turn actions against an agent.
    /// </summary>
    /// <param name="planet">The planet.</param>
    /// <param name="agent">The agent.</param>
    public abstract void AgentEndOfTurnActions(Planet planet, Agent agent);
}