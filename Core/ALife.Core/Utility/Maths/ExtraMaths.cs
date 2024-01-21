﻿using System;
using System.Linq;

namespace ALife.Core.Utility.Maths
{
	/// <summary>
    /// Provides extra mathematical functions.
    /// TODO: This should use generic math instead of being generated, but we're stuck with .NET Standard 2 for now...
    /// </summary>
    public static class ExtraMaths
    {
                /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Int16 CircularClamp(Int16 value, Int16 min, Int16 max)
        {
            Int16 adder = (Int16)(max - min);
            Int16 remainder = (Int16)(value % adder);
            Int16 actualValue = (Int16)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Int16 Clamp(Int16 value, Int16 min, Int16 max)
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
        public static Int16 DeltaClamp(Int16 value, Int16 currentValue, Int16 deltaMin, Int16 deltamax, Int16 absoluteMin, Int16 absoluteMax)
        {
            Int16 delta = (Int16)(value - currentValue);
            Int16 realDelta = Clamp(delta, deltaMin, deltamax);
            Int16 newValue = (Int16)(currentValue + realDelta);
            Int16 clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Int16 Maximum(params Int16[] numbers)
        {
            Int16 output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Int16 Minimum(params Int16[] numbers)
        {
            Int16 output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Int16 MinMaxDelta(params Int16[] numbers)
        {
            Int16 min = numbers.Min();
            Int16 max = numbers.Max();
            Int16 delta = (Int16)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Int32 CircularClamp(Int32 value, Int32 min, Int32 max)
        {
            Int32 adder = (Int32)(max - min);
            Int32 remainder = (Int32)(value % adder);
            Int32 actualValue = (Int32)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Int32 Clamp(Int32 value, Int32 min, Int32 max)
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
        public static Int32 DeltaClamp(Int32 value, Int32 currentValue, Int32 deltaMin, Int32 deltamax, Int32 absoluteMin, Int32 absoluteMax)
        {
            Int32 delta = (Int32)(value - currentValue);
            Int32 realDelta = Clamp(delta, deltaMin, deltamax);
            Int32 newValue = (Int32)(currentValue + realDelta);
            Int32 clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Int32 Maximum(params Int32[] numbers)
        {
            Int32 output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Int32 Minimum(params Int32[] numbers)
        {
            Int32 output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Int32 MinMaxDelta(params Int32[] numbers)
        {
            Int32 min = numbers.Min();
            Int32 max = numbers.Max();
            Int32 delta = (Int32)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Int64 CircularClamp(Int64 value, Int64 min, Int64 max)
        {
            Int64 adder = (Int64)(max - min);
            Int64 remainder = (Int64)(value % adder);
            Int64 actualValue = (Int64)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Int64 Clamp(Int64 value, Int64 min, Int64 max)
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
        public static Int64 DeltaClamp(Int64 value, Int64 currentValue, Int64 deltaMin, Int64 deltamax, Int64 absoluteMin, Int64 absoluteMax)
        {
            Int64 delta = (Int64)(value - currentValue);
            Int64 realDelta = Clamp(delta, deltaMin, deltamax);
            Int64 newValue = (Int64)(currentValue + realDelta);
            Int64 clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Int64 Maximum(params Int64[] numbers)
        {
            Int64 output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Int64 Minimum(params Int64[] numbers)
        {
            Int64 output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Int64 MinMaxDelta(params Int64[] numbers)
        {
            Int64 min = numbers.Min();
            Int64 max = numbers.Max();
            Int64 delta = (Int64)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static UInt16 CircularClamp(UInt16 value, UInt16 min, UInt16 max)
        {
            UInt16 adder = (UInt16)(max - min);
            UInt16 remainder = (UInt16)(value % adder);
            UInt16 actualValue = (UInt16)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static UInt16 Clamp(UInt16 value, UInt16 min, UInt16 max)
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
        public static UInt16 DeltaClamp(UInt16 value, UInt16 currentValue, UInt16 deltaMin, UInt16 deltamax, UInt16 absoluteMin, UInt16 absoluteMax)
        {
            UInt16 delta = (UInt16)(value - currentValue);
            UInt16 realDelta = Clamp(delta, deltaMin, deltamax);
            UInt16 newValue = (UInt16)(currentValue + realDelta);
            UInt16 clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static UInt16 Maximum(params UInt16[] numbers)
        {
            UInt16 output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static UInt16 Minimum(params UInt16[] numbers)
        {
            UInt16 output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static UInt16 MinMaxDelta(params UInt16[] numbers)
        {
            UInt16 min = numbers.Min();
            UInt16 max = numbers.Max();
            UInt16 delta = (UInt16)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static UInt32 CircularClamp(UInt32 value, UInt32 min, UInt32 max)
        {
            UInt32 adder = (UInt32)(max - min);
            UInt32 remainder = (UInt32)(value % adder);
            UInt32 actualValue = (UInt32)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static UInt32 Clamp(UInt32 value, UInt32 min, UInt32 max)
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
        public static UInt32 DeltaClamp(UInt32 value, UInt32 currentValue, UInt32 deltaMin, UInt32 deltamax, UInt32 absoluteMin, UInt32 absoluteMax)
        {
            UInt32 delta = (UInt32)(value - currentValue);
            UInt32 realDelta = Clamp(delta, deltaMin, deltamax);
            UInt32 newValue = (UInt32)(currentValue + realDelta);
            UInt32 clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static UInt32 Maximum(params UInt32[] numbers)
        {
            UInt32 output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static UInt32 Minimum(params UInt32[] numbers)
        {
            UInt32 output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static UInt32 MinMaxDelta(params UInt32[] numbers)
        {
            UInt32 min = numbers.Min();
            UInt32 max = numbers.Max();
            UInt32 delta = (UInt32)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static UInt64 CircularClamp(UInt64 value, UInt64 min, UInt64 max)
        {
            UInt64 adder = (UInt64)(max - min);
            UInt64 remainder = (UInt64)(value % adder);
            UInt64 actualValue = (UInt64)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static UInt64 Clamp(UInt64 value, UInt64 min, UInt64 max)
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
        public static UInt64 DeltaClamp(UInt64 value, UInt64 currentValue, UInt64 deltaMin, UInt64 deltamax, UInt64 absoluteMin, UInt64 absoluteMax)
        {
            UInt64 delta = (UInt64)(value - currentValue);
            UInt64 realDelta = Clamp(delta, deltaMin, deltamax);
            UInt64 newValue = (UInt64)(currentValue + realDelta);
            UInt64 clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static UInt64 Maximum(params UInt64[] numbers)
        {
            UInt64 output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static UInt64 Minimum(params UInt64[] numbers)
        {
            UInt64 output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static UInt64 MinMaxDelta(params UInt64[] numbers)
        {
            UInt64 min = numbers.Min();
            UInt64 max = numbers.Max();
            UInt64 delta = (UInt64)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Double CircularClamp(Double value, Double min, Double max)
        {
            Double adder = (Double)(max - min);
            Double remainder = (Double)(value % adder);
            Double actualValue = (Double)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Double Clamp(Double value, Double min, Double max)
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
        public static Double DeltaClamp(Double value, Double currentValue, Double deltaMin, Double deltamax, Double absoluteMin, Double absoluteMax)
        {
            Double delta = (Double)(value - currentValue);
            Double realDelta = Clamp(delta, deltaMin, deltamax);
            Double newValue = (Double)(currentValue + realDelta);
            Double clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Double Maximum(params Double[] numbers)
        {
            Double output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Double Minimum(params Double[] numbers)
        {
            Double output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Double MinMaxDelta(params Double[] numbers)
        {
            Double min = numbers.Min();
            Double max = numbers.Max();
            Double delta = (Double)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Single CircularClamp(Single value, Single min, Single max)
        {
            Single adder = (Single)(max - min);
            Single remainder = (Single)(value % adder);
            Single actualValue = (Single)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Single Clamp(Single value, Single min, Single max)
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
        public static Single DeltaClamp(Single value, Single currentValue, Single deltaMin, Single deltamax, Single absoluteMin, Single absoluteMax)
        {
            Single delta = (Single)(value - currentValue);
            Single realDelta = Clamp(delta, deltaMin, deltamax);
            Single newValue = (Single)(currentValue + realDelta);
            Single clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Single Maximum(params Single[] numbers)
        {
            Single output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Single Minimum(params Single[] numbers)
        {
            Single output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Single MinMaxDelta(params Single[] numbers)
        {
            Single min = numbers.Min();
            Single max = numbers.Max();
            Single delta = (Single)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Decimal CircularClamp(Decimal value, Decimal min, Decimal max)
        {
            Decimal adder = (Decimal)(max - min);
            Decimal remainder = (Decimal)(value % adder);
            Decimal actualValue = (Decimal)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Decimal Clamp(Decimal value, Decimal min, Decimal max)
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
        public static Decimal DeltaClamp(Decimal value, Decimal currentValue, Decimal deltaMin, Decimal deltamax, Decimal absoluteMin, Decimal absoluteMax)
        {
            Decimal delta = (Decimal)(value - currentValue);
            Decimal realDelta = Clamp(delta, deltaMin, deltamax);
            Decimal newValue = (Decimal)(currentValue + realDelta);
            Decimal clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Decimal Maximum(params Decimal[] numbers)
        {
            Decimal output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Decimal Minimum(params Decimal[] numbers)
        {
            Decimal output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Decimal MinMaxDelta(params Decimal[] numbers)
        {
            Decimal min = numbers.Min();
            Decimal max = numbers.Max();
            Decimal delta = (Decimal)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static Byte CircularClamp(Byte value, Byte min, Byte max)
        {
            Byte adder = (Byte)(max - min);
            Byte remainder = (Byte)(value % adder);
            Byte actualValue = (Byte)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static Byte Clamp(Byte value, Byte min, Byte max)
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
        public static Byte DeltaClamp(Byte value, Byte currentValue, Byte deltaMin, Byte deltamax, Byte absoluteMin, Byte absoluteMax)
        {
            Byte delta = (Byte)(value - currentValue);
            Byte realDelta = Clamp(delta, deltaMin, deltamax);
            Byte newValue = (Byte)(currentValue + realDelta);
            Byte clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static Byte Maximum(params Byte[] numbers)
        {
            Byte output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static Byte Minimum(params Byte[] numbers)
        {
            Byte output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static Byte MinMaxDelta(params Byte[] numbers)
        {
            Byte min = numbers.Min();
            Byte max = numbers.Max();
            Byte delta = (Byte)(max - min);
            return delta;
        }
		        /// <summary>
        /// Applies a circular clamp to the value (i.e. if the value is outside the range, it wraps around).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum. Non-inclusive.</param>
        /// <returns>The clamped value.</returns>
        public static SByte CircularClamp(SByte value, SByte min, SByte max)
        {
            SByte adder = (SByte)(max - min);
            SByte remainder = (SByte)(value % adder);
            SByte actualValue = (SByte)(remainder + min);
            return actualValue;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The (clamped) value.</returns>
        public static SByte Clamp(SByte value, SByte min, SByte max)
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
        public static SByte DeltaClamp(SByte value, SByte currentValue, SByte deltaMin, SByte deltamax, SByte absoluteMin, SByte absoluteMax)
        {
            SByte delta = (SByte)(value - currentValue);
            SByte realDelta = Clamp(delta, deltaMin, deltamax);
            SByte newValue = (SByte)(currentValue + realDelta);
            SByte clampedValue = Clamp(newValue, absoluteMin, absoluteMax);
            return clampedValue;
        }

        /// <summary>
        /// Gets the largest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The maximum.</returns>
        public static SByte Maximum(params SByte[] numbers)
        {
            SByte output = numbers.Max();
            return output;
        }

        /// <summary>
        /// Gets the smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The minimum.</returns>
        public static SByte Minimum(params SByte[] numbers)
        {
            SByte output = numbers.Min();
            return output;
        }

        /// <summary>
        /// Gets the delta between the largest and smallest of the specified numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns>The delta.</returns>
        public static SByte MinMaxDelta(params SByte[] numbers)
        {
            SByte min = numbers.Min();
            SByte max = numbers.Max();
            SByte delta = (SByte)(max - min);
            return delta;
        }
		    }
}
