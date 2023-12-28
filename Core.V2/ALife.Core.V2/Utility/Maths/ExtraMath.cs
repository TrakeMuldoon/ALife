namespace ALife.Core.Utility.Maths
{
    /// <summary>
    /// Extra math functions that are not included in the standard library.
    /// </summary>
    public static class ExtraMath
    {
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static double Clamp(double value, double min, double max)
        {
            if(value < min)
            {
                return min;
            }
            else if(value > max)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Applies a delta to a value, clamping the result between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="currentValue">The current value.</param>
        /// <param name="deltaMin">The delta minimum.</param>
        /// <param name="deltamax">The deltamax.</param>
        /// <param name="absoluteMin">The absolute minimum.</param>
        /// <param name="absoluteMax">The absolute maximum.</param>
        /// <returns>The delta-clampped value.</returns>
        public static double DeltaClamp(double value, double currentValue, double deltaMin, double deltamax, double absoluteMin, double absoluteMax)
        {
            double delta = value - currentValue;
            double realDelta = Clamp(delta, deltaMin, deltamax);
            double newValue = currentValue + realDelta;
            double clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }
    }
}
