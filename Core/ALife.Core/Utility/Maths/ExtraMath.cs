using System.Numerics;

namespace ALife.Core.Utility.Maths
{
    /// <summary>
    /// Provides extra mathematical functions.
    /// </summary>
    public static class ExtraMath<T> where T : INumber<T>
    {
        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static T CircularClamp(T value, T min, T max)
        {
            T actualValue = value;

            (T actualMin, T actualMax) = (min, max);
            if(min > max)
            {
                (actualMin, actualMax) = (max, min);
            }
            if (value >= actualMin && value < actualMax)
            {
                return value;
            }

            T negativeCorrection = T.Zero;
            if (min < T.Zero)
            {
                negativeCorrection = min;
                actualMin = T.Zero;
                actualMax = max - negativeCorrection;
                actualValue = value - negativeCorrection;
            }

            T difference = actualMax - actualMin;
            T remainder = actualValue % difference;
        
            T actualRemainder = remainder;
            if (remainder < T.Zero)
            {
                actualRemainder = remainder + difference;
            }

            T output = actualRemainder + actualMin + negativeCorrection;
            return output;
        }
        
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static T Clamp(T value, T min, T max)
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }
        
        /// <summary>
        /// Clamps a value between a minimum and maximum value, and returns the difference between the value and the clamped value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value, and the amount that was clamped</returns>
        public static (T, T) ClampWithDifference(T value, T min, T max)
        {
            if (value.CompareTo(min) < 0)
            {
                return (min, min - value);
            }
            if (value.CompareTo(max) > 0)
            {
                return (max, value - max);
            }
            return (value, T.Zero);
        }

        /// <summary>
        /// Applies a delta to a value, clamping the result between a minimum and maximum value.
        /// </summary>
        /// <param name="newValue">The new value we want to set.</param>
        /// <param name="currentValue">The current value.</param>
        /// <param name="deltaMin">The delta minimum.</param>
        /// <param name="deltamax">The deltamax.</param>
        /// <param name="absoluteMin">The absolute minimum.</param>
        /// <param name="absoluteMax">The absolute maximum.</param>
        /// <returns>The delta-clampped value.</returns>
        public static T DeltaClamp(T newValue, T currentValue, T deltaMin, T deltamax, T absoluteMin, T absoluteMax)
        {
            T delta = newValue - currentValue;
            T realDelta = Clamp(delta, deltaMin, deltamax);
            T newActualValue = currentValue + realDelta;
            T clampedValue = Clamp(newActualValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static T Maximum(params T[] values)
        {
            if (values.Length == 0)
            {
                return T.Zero;
            }
            return values.Max();
        }
        
        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static T Minimum(params T[] values)
        {
            if (values.Length == 0)
            {
                return T.Zero;
            }
            return values.Min();
        }
        
        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The delta.</returns>
        public static T MinMaxDelta(params T[] values)
        {
            if (values.Length == 0)
            {
                return T.Zero;
            }
            return Maximum(values) - Minimum(values);
        }
    }
}