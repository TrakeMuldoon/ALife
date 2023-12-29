using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// An EvoNumber that where the Value is read-only.
    /// </summary>
    /// <seealso cref="ALife.Core.Utility.Numerics.EvoNumber"/>
    [DebuggerDisplay("ROEvoNumber: {Value} (Original: {OriginalValue})")]
    public class ReadOnlyEvoNumber : EvoNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ReadOnlyEvoNumber(EvoNumber parent) : base(parent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="sim">The sim.</param>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="valueMaximumAndMinimumEvolutionDeltaMax">The value maximum and minimum evolution delta maximum.</param>
        /// <param name="valueDelta">The value delta.</param>
        /// <param name="cloneBoundedNumbers">if set to <c>true</c> [clone bounded numbers].</param>
        public ReadOnlyEvoNumber(Simulation sim, double value, double originalValueEvolutionDeltaMax, BoundedNumber minimumValue, BoundedNumber maximumValue, double valueMaximumAndMinimumEvolutionDeltaMax, DeltaBoundedNumber valueDelta, bool cloneBoundedNumbers = true) : base(sim, value, originalValueEvolutionDeltaMax, minimumValue, maximumValue, valueMaximumAndMinimumEvolutionDeltaMax, valueDelta, cloneBoundedNumbers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="sim">The sim.</param>
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
        public ReadOnlyEvoNumber(Simulation sim, double value, double originalValueEvolutionDeltaMax, double minimumValue, double maximumValue, double absoluteMinimumValue, double absoluteMaximumValue, double valueMaximumAndMinimumEvolutionDeltaMax, double valueDeltaMax, double valueDeltaMaxEvolutionMax, double valueDeltaMaxEvolutionAbsoluteMax) : base(sim, value, originalValueEvolutionDeltaMax, minimumValue, maximumValue, absoluteMinimumValue, absoluteMaximumValue, valueMaximumAndMinimumEvolutionDeltaMax, valueDeltaMax, valueDeltaMaxEvolutionMax, valueDeltaMaxEvolutionAbsoluteMax)
        {
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <exception cref="System.InvalidOperationException">Cannot set the value of a ReadOnlyEvoNumber</exception>
        [JsonIgnore]
        public new double Value
        {
            get => _value;
            set => throw new InvalidOperationException("Cannot set the value of a ReadOnlyEvoNumber");
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ReadOnlyEvoNumber left, ReadOnlyEvoNumber right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(ReadOnlyEvoNumber left, ReadOnlyEvoNumber right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(ReadOnlyEvoNumber left, ReadOnlyEvoNumber right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ReadOnlyEvoNumber left, ReadOnlyEvoNumber right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(ReadOnlyEvoNumber left, ReadOnlyEvoNumber right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(ReadOnlyEvoNumber left, ReadOnlyEvoNumber right)
        {
            return left.Value >= right.Value;
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
            return obj is ReadOnlyEvoNumber number &&
                   OriginalValue == number.OriginalValue &&
                   OriginalValueEvolutionDeltaMax == number.OriginalValueEvolutionDeltaMax &&
                   Value == number.Value &&
                   ValueDeltaMaximum == number.ValueDeltaMaximum &&
                   ValueMaximum == number.ValueMaximum &&
                   ValueMinimum == number.ValueMinimum &&
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
            double hash = 18;
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
            return $"ROEvoNumber: {Value} (Original: {OriginalValue})";
        }
    }
}
