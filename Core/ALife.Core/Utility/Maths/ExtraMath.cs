using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ALife.Core.Utility.Maths
{
    /// <summary>
    /// Provides extra mathematical functions.
    /// </summary>
    public static class ExtraMath
    {
        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static T CircularClamp<T>(T value, T min, T max) where T : INumber<T>
        {
            (T actualMin, T actualMax) = min > max ? (max, min) : (min, max);
            if(value >= actualMin && value < actualMax)
            {
                return value;
            }

            T negativeCorrection = T.Zero;
            if(min < T.Zero)
            {
                negativeCorrection = min;
                actualMin = T.Zero;
                actualMax = max - negativeCorrection;
                value -= negativeCorrection;
            }

            T minMaxDifference = actualMax - actualMin;
            T remainder = value % minMaxDifference;
            if(remainder < T.Zero)
            {
                remainder += minMaxDifference;
            }

            T output = remainder + actualMin + negativeCorrection;
            return output;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static T Clamp<T>(T value, T min, T max) where T : INumber<T>
        {
            if(value < min)
            {
                return min;
            }

            if(value > max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Applies a delta to a value, clamping the result between a minimum and maximum value.
        /// </summary>
        /// <param name="newValue">The new value we want to set.</param>
        /// <param name="currentValue">The current value.</param>
        /// <param name="deltaMin">The delta minimum.</param>
        /// <param name="deltaMax">The delta maximum.</param>
        /// <param name="absoluteMin">The absolute minimum.</param>
        /// <param name="absoluteMax">The absolute maximum.</param>
        /// <returns>The delta-clamped value.</returns>
        public static T DeltaClamp<T>(T newValue, T currentValue, T deltaMin, T deltaMax, T absoluteMin, T absoluteMax)
            where T : INumber<T>
        {
            T delta = newValue - currentValue;
            T realDelta = Clamp(delta, deltaMin, deltaMax);
            T newActualValue = currentValue + realDelta;
            T clampedValue = Clamp(newActualValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The maximum.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Maximum<T>(params T[] values) where T : INumber<T>
        {
            T output = values.Max()!;
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The minimum.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Minimum<T>(params T[] values) where T : INumber<T>
        {
            T output = values.Min()!;
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="values">The numbers.</param>
        /// <returns>The delta.</returns>
        public static T MinMaxDelta<T>(params T[] values)  where T : INumber<T>
        {
            T min = values.Min()!;
            T max = values.Max()!;
            T delta = max - min;
            return delta;
        }
    }
}