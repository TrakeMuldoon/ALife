using System;
using System.Collections.Generic;
using System.Linq;
using ALife.Rendering;

namespace ALife.Avalonia.ALifeImplementations
{
    /// <summary>
    /// Avalonia-specific extensions for the SimulationSpeed enum
    /// </summary>
    public static class SimulationSpeedExtensions
    {
        /// <summary>
        /// The space per slider tick
        /// </summary>
        public const int SPACE_PER_SLIDER_TICK = 10;

        /// <summary>
        /// The maximum slider position
        /// </summary>
        public static int MAX_SLIDER_POSITION = Enum.GetValues(typeof(SimulationSpeed)).Length * SPACE_PER_SLIDER_TICK - SPACE_PER_SLIDER_TICK;

        /// <summary>
        /// The slider positions for speed
        /// </summary>
        public static Dictionary<SimulationSpeed, int> SLIDER_POSITIONS_FOR_SPEED;

        /// <summary>
        /// Initializes the <see cref="SimulationSpeedExtensions"/> class.
        /// </summary>
        static SimulationSpeedExtensions()
        {
            var speeds = Enum.GetValues(typeof(SimulationSpeed)).AsQueryable().Cast<SimulationSpeed>().OrderBy(x => (int)x);

            SLIDER_POSITIONS_FOR_SPEED = new();
            int position = 0;
            foreach(SimulationSpeed speed in speeds)
            {
                SLIDER_POSITIONS_FOR_SPEED.Add(speed, position);
                position += SPACE_PER_SLIDER_TICK;
            }
        }

        /// <summary>
        /// Converts to slider position.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns>The int representing the slider position for this simulation speed.</returns>
        public static int ToSliderPosition(this SimulationSpeed speed)
        {
            return SLIDER_POSITIONS_FOR_SPEED[speed];
        }
    }
}
