namespace ALife.Core;

/// <summary>
/// The levels of the simulation.
/// </summary>
public enum SimulationLevels
{
    Dead = 1,
    Zone = 2,
    Physical = 4,
    Sight = 8,
    Scent = 16,
    Sound = 32,
}
