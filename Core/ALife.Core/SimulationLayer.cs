using System;

namespace ALife.Core
{
    /// <summary>
    /// The simulation layers available in the simulation
    /// </summary>
    [Flags]
    public enum SimulationLayer
    {
        /// <summary>
        /// The Physical layer
        /// </summary>
        PHYSICAL = 1,

        /// <summary>
        /// The Sound layer
        /// </summary>
        SOUND = 2,

        /// <summary>
        /// All
        /// </summary>
        ALL = PHYSICAL | SOUND,

        /// <summary>
        /// No layers applicable
        /// </summary>
        NONE = 0,
    }
}
