using ALife.Core.Utility.Maths;
using ALife.Core.Utility.Random;

namespace ALife.Core.Utility.EvoNumbers
{
    /// <summary>
    /// Helper methods and constants for EvoNumbers
    /// </summary>
    public static class EvoNumberHelpers
    {
        /// <summary>
        /// The mean for the evolution of a value.
        /// TODO: Find out why this is 0. And why it is a constant.
        /// </summary>
        public const double EVOLUTION_MEAN = 0;

        /// <summary>
        /// The standard deviation for the evolution of a value.
        /// Devin: This is a magic number to approximate the distribution I like.
        /// </summary>
        public const double EVOLUTION_STANDARD_DEVIATION = 0.2;

        /// <summary>
        /// Evolves the value.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="current">The current.</param>
        /// <param name="deltaMax">The delta maximum.</param>
        /// <param name="hardMin">The hard minimum.</param>
        /// <param name="hardMax">The hard maximum.</param>
        /// <returns>The evolved number.</returns>
        public static double EvolveValue(IRandom rand, double current, double deltaMax, double hardMin, double hardMax)
        {
            if(deltaMax == 0)
            {
                return current;
            }

            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                                   * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = EVOLUTION_MEAN + EVOLUTION_STANDARD_DEVIATION * randStdNormal;     //random normal(mean,stdDev^2)

            double delta = randNormal * deltaMax;
            //double delta = (Simulation.Random.NextDouble() * deltaMax)
            //               + (Simulation.Random.NextDouble() * deltaMax)
            //               - deltaMax;

            double moddedValue = current + delta;
            double clampedValue = ExtraMath<double>.Clamp(moddedValue, hardMin, hardMax);
            return clampedValue;
        }
    }
}