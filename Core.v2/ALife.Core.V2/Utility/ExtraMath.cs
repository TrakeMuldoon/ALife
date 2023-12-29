using System.Linq;

namespace ALife.Core.Utility
{
    /// <summary>
    /// Extra math functions that are not included in the standard library.
    /// </summary>
    public static class ExtraMath
    {
        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The clamped value.</returns>
        public static double CircularClamp(double value, double min, double max)
        {
            double adder = max - min;
            while(value < min)
            {
                value += adder;
            }
            while(value > max)
            {
                value -= adder;
            }
            return value;
        }

        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The clamped value.</returns>
        public static int CircularClamp(int value, int min, int max)
        {
            int adder = max - min;
            while(value < min)
            {
                value += adder;
            }
            while(value > adder)
            {
                value -= max;
            }
            return value;
        }

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

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static double Maximum(params double[] numbers)
        {
            double output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static int Maximum(params int[] numbers)
        {
            int output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static double Minimum(params double[] numbers)
        {
            double output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static int Minimum(params int[] numbers)
        {
            int output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static double MinMaxDelta(params double[] numbers)
        {
            double min = numbers.Min();
            double max = numbers.Max();
            double delta = max - min;
            return delta;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static int MinMaxDelta(params int[] numbers)
        {
            int min = numbers.Min();
            int max = numbers.Max();
            int delta = max - min;
            return delta;
        }
    }
}
