namespace ALife.Rendering
{
    /// <summary>
    /// Extensions for the SimulationSpeed enum
    /// </summary>
    public static class SimulationSpeedExtensions
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="SimulationSpeed"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns>The result of the conversion.</returns>
        public static double ToDouble(this SimulationSpeed speed)
        {
            return (double)speed;
        }

        /// <summary>
        /// Converts to the target MS the speed represents
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns>The target MS between ticks.</returns>
        public static int ToTargetMS(this SimulationSpeed speed)
        {
            int ms = (int)Math.Round(1000 / (double)speed);
            return ms;
        }
    }
}
