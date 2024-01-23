using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.EvoNumbersV2;
using ALife.Core.Utility.Maths;
using ALife.Core.Utility.Numerics;

namespace ALife.Core.Utility.EvoNumbersV2
{
    /// <summary>
    /// A number that can be evolved within a range.
    /// </summary>
    /// <seealso cref="ALife.Core.BaseObject"/>
    [DebuggerDisplay("{ToString()}")]
    public struct EvoNumber : IEvoNumber
    {
        /// <summary>
        /// The actual value
        /// </summary>
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="valueMaximumAndMinimumEvolutionDeltaMax">The value maximum and minimum evolution delta maximum.</param>
        /// <param name="valueDelta">The value delta.</param>
        /// <param name="cloneBoundedNumbers">if set to <c>true</c> [clone bounded numbers].</param>
        [JsonConstructor]
        public EvoNumber(double value, double originalValueEvolutionDeltaMax, BoundedNumber minimumValue, BoundedNumber maximumValue, double valueMaximumAndMinimumEvolutionDeltaMax, DeltaBoundedNumber valueDelta, bool cloneBoundedNumbers = true)
        {
            OriginalValue = value;
            _value = value;
            OriginalValueEvolutionDeltaMax = originalValueEvolutionDeltaMax;

            ValueDeltaMaximum = cloneBoundedNumbers ? valueDelta.Clone() : valueDelta;
            ValueMinimum = cloneBoundedNumbers ? minimumValue.Clone() : minimumValue;
            ValueMaximum = cloneBoundedNumbers ? maximumValue.Clone() : maximumValue;
            ValueMaximumAndMinimumEvolutionDeltaMax = valueMaximumAndMinimumEvolutionDeltaMax;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="absoluteMinimumValue">The absolute minimum value.</param>
        /// <param name="absoluteMaximumValue">The absolute maximum value.</param>
        /// <param name="valueMaximumAndMinimumEvolutionDeltaMax">The value maximum and minimum evolution delta maximum.</param>
        /// <param name="valueDeltaMax">The value delta maximum.</param>
        /// <param name="valueDeltaMaxEvolutionMax">The value delta maximum evolution maximum.</param>
        /// <param name="valueDeltaMaxEvolutionAbsoluteMax">The value delta maximum evolution absolute maximum.</param>
        public EvoNumber(double value, double originalValueEvolutionDeltaMax, double minimumValue, double maximumValue, double absoluteMinimumValue, double absoluteMaximumValue, double valueMaximumAndMinimumEvolutionDeltaMax, double valueDeltaMax, double valueDeltaMaxEvolutionMax, double valueDeltaMaxEvolutionAbsoluteMax) : this(value, originalValueEvolutionDeltaMax, new BoundedNumber(minimumValue, minValue: absoluteMinimumValue), new BoundedNumber(maximumValue, maxValue: absoluteMaximumValue), valueMaximumAndMinimumEvolutionDeltaMax, new DeltaBoundedNumber(valueDeltaMax, valueDeltaMaxEvolutionMax, 0, valueDeltaMaxEvolutionAbsoluteMax), cloneBoundedNumbers: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvoNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public EvoNumber(EvoNumber parent) : this(parent.Value, parent.OriginalValueEvolutionDeltaMax, parent.ValueMinimum, parent.ValueMaximum, parent.ValueMaximumAndMinimumEvolutionDeltaMax, parent.ValueDeltaMaximum)
        {
        }

        /// <summary>
        /// Gets the original (start) value.
        /// </summary>
        /// <value>The original value.</value>
        public double OriginalValue { get; }

        /// <summary>
        /// Gets the original value evolution delta maximum.
        /// </summary>
        /// <value>The original value evolution delta maximum.</value>
        public double OriginalValueEvolutionDeltaMax { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonIgnore]
        public double Value
        {
            get => _value;
            set => _value = ExtraMaths.DeltaClamp(value, _value, -ValueDeltaMaximum, ValueDeltaMaximum, ValueMinimum.Value, ValueMaximum.Value);
        }

        /// <summary>
        /// Gets the value delta. This is the maximum amount that the Value can change when it is updated.
        /// </summary>
        /// <value>The value delta.</value>
        public DeltaBoundedNumber ValueDeltaMaximum { get; }

        /// <summary>
        /// The maximum that the value can be.
        /// </summary>
        public BoundedNumber ValueMaximum { get; }

        /// <summary>
        /// Gets or sets the value maximum and minimum evolution delta maximum. This is the maximum amount that the
        /// ValueMaximum and ValueMinimum can change when they are evolved.
        /// </summary>
        /// <value>The value maximum and minimum evolution delta maximum.</value>
        public double ValueMaximumAndMinimumEvolutionDeltaMax { get; set; }

        /// <summary>
        /// The minimum that the value can be.
        /// </summary>
        public BoundedNumber ValueMinimum { get; }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EvoNumber left, EvoNumber right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(EvoNumber left, EvoNumber right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(EvoNumber left, EvoNumber right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EvoNumber left, EvoNumber right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(EvoNumber left, EvoNumber right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(EvoNumber left, EvoNumber right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEvoNumber Clone()
        {
            return new EvoNumber(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is EvoNumber number &&
                   OriginalValue == number.OriginalValue &&
                   OriginalValueEvolutionDeltaMax == number.OriginalValueEvolutionDeltaMax &&
                   Value == number.Value &&
                   ValueDeltaMaximum.Equals(number.ValueDeltaMaximum) &&
                   ValueMaximum.Equals(number.ValueMaximum) &&
                   ValueMinimum.Equals(number.ValueMinimum) &&
                   ValueMaximumAndMinimumEvolutionDeltaMax == number.ValueMaximumAndMinimumEvolutionDeltaMax;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            double hash = 17;
            hash = hash * 23 + OriginalValue.GetHashCode();
            hash = hash * 23 + OriginalValueEvolutionDeltaMax.GetHashCode();
            hash = hash * 23 + Value.GetHashCode();
            hash = hash * 23 + ValueDeltaMaximum.GetHashCode();
            hash = hash * 23 + ValueMaximum.GetHashCode();
            hash = hash * 23 + ValueMinimum.GetHashCode();
            hash = hash * 23 + ValueMaximumAndMinimumEvolutionDeltaMax.GetHashCode();
            return (int)hash;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"EvoNumber: {Value} (Original: {OriginalValue})";
        }
    }
}
