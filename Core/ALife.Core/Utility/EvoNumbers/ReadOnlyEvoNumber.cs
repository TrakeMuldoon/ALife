using ALife.Core.Utility.Numerics;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.EvoNumbers
{
    /// <summary>
    /// A number that can be evolved within a range. This version can only be read, not written to.
    /// </summary>
    /// <seealso cref="ALife.Core.Utility.EvoNumbers.IEvoNumber"/>
    [DebuggerDisplay("{ToString()}")]
    public struct ReadOnlyEvoNumber : IEvoNumber
    {
        /// <summary>
        /// The actual value
        /// </summary>
        [JsonIgnore]
        private double _value;

        /// <summary>
        /// The value delta maximum
        /// </summary>
        [JsonIgnore]
        private DeltaBoundedNumber _valueDeltaMaximum;

        /// <summary>
        /// The value maximum
        /// </summary>
        [JsonIgnore]
        private Numerics.BoundedNumber _valueMaximum;

        /// <summary>
        /// The value minimum
        /// </summary>
        [JsonIgnore]
        private Numerics.BoundedNumber _valueMinimum;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="valueMinimumAndMaximumEvolutionDeltaMax">The value minimum and maximum evolution delta maximum.</param>
        /// <param name="valueDelta">The value delta.</param>
        [JsonConstructor]
        public ReadOnlyEvoNumber(double value, double originalValueEvolutionDeltaMax, Numerics.BoundedNumber minimumValue, Numerics.BoundedNumber maximumValue, double valueMinimumAndMaximumEvolutionDeltaMax, DeltaBoundedNumber valueDelta)
        {
            OriginalValue = value;
            _value = value;
            OriginalValueEvolutionDeltaMax = originalValueEvolutionDeltaMax;
            _valueMinimum = minimumValue;
            _valueMaximum = maximumValue;
            ValueMaximumAndMinimumEvolutionDeltaMax = valueMinimumAndMaximumEvolutionDeltaMax;
            _valueDeltaMaximum = valueDelta;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="startValue">The value.</param>
        /// <param name="evoDeltaMax">The evolution delta maximum.</param>
        /// <param name="absoluteMinimum">The absolute minimum.</param>
        /// <param name="hardMax">The absolute maximum.</param>
        public ReadOnlyEvoNumber(double startValue, double evoDeltaMax, double hardMin, double hardMax) : this(startValue, evoDeltaMax, new BoundedNumber(hardMin, hardMin, hardMax), new BoundedNumber(hardMax, hardMin, hardMax), evoDeltaMax, new DeltaBoundedNumber(evoDeltaMax, evoDeltaMax, evoDeltaMax - 1, evoDeltaMax + 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="originalValueEvolutionDeltaMax">The original value evolution delta maximum.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="absoluteMinimumValue">The absolute minimum value.</param>
        /// <param name="absoluteMaximumValue">The absolute maximum value.</param>
        /// <param name="valueMinimumAndMaximumEvolutionDeltaMax">The value minimum and maximum evolution delta maximum.</param>
        /// <param name="valueDeltaMax">The value delta maximum.</param>
        /// <param name="valueDeltaMaxEvolutionMax">The value delta maximum evolution maximum.</param>
        /// <param name="valueDeltaMaxEvolutionAbsoluteMax">The value delta maximum evolution absolute maximum.</param>
        public ReadOnlyEvoNumber(double value, double originalValueEvolutionDeltaMax, double minimumValue, double maximumValue, double absoluteMinimumValue, double absoluteMaximumValue, double valueMinimumAndMaximumEvolutionDeltaMax, double valueDeltaMax, double valueDeltaMaxEvolutionMax, double valueDeltaMaxEvolutionAbsoluteMax) : this(value, originalValueEvolutionDeltaMax, new Numerics.BoundedNumber(minimumValue, absoluteMinimumValue, absoluteMaximumValue), new Numerics.BoundedNumber(maximumValue, absoluteMinimumValue, absoluteMaximumValue), valueMinimumAndMaximumEvolutionDeltaMax, new DeltaBoundedNumber(valueDeltaMax, valueDeltaMaxEvolutionMax, 0, valueDeltaMaxEvolutionAbsoluteMax))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEvoNumber"/> class.
        /// </summary>
        /// <param name="evoNumber">The evo number to clone.</param>
        public ReadOnlyEvoNumber(IEvoNumber evoNumber) : this(evoNumber.Value, evoNumber.OriginalValueEvolutionDeltaMax, evoNumber.ValueMinimum, evoNumber.ValueMaximum, evoNumber.ValueMaximumAndMinimumEvolutionDeltaMax, evoNumber.ValueDeltaMaximum)
        {
        }

        /// <summary>
        /// Gets the original (start) value.
        /// </summary>
        /// <value>The original value.</value>
        [JsonIgnore]
        public double OriginalValue { get; }

        /// <summary>
        /// Gets the original value evolution delta maximum.
        /// </summary>
        /// <value>The original value evolution delta maximum.</value>
        [JsonPropertyName("originalValueEvolutionDeltaMax")]
        public double OriginalValueEvolutionDeltaMax { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <exception cref="System.InvalidOperationException">Cannot set the value of a ReadOnlyEvoNumber</exception>
        [JsonPropertyName("value")]
        public double Value
        {
            get => _value;
            set => throw new InvalidOperationException("Cannot set the value of a ReadOnlyEvoNumber");
        }

        /// <summary>
        /// Gets or sets the value absolute minimum. This is the absolute minimum that the Value can grow to be.
        /// </summary>
        /// <value>The value absolute minimum.</value>
        [JsonIgnore]
        public double ValueAbsoluteMaximum
        {
            get => _valueMaximum.Maximum;
            set
            {
                _valueMinimum.Maximum = value;
                _valueMaximum.Maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the value absolute maximum. This is the absolute maximum that the Value can grow to be.
        /// </summary>
        /// <value>The value absolute maximum.</value>
        [JsonIgnore]
        public double ValueAbsoluteMinimum
        {
            get => _valueMinimum.Minimum;
            set
            {
                _valueMinimum.Minimum = value;
                _valueMaximum.Minimum = value;
            }
        }

        /// <summary>
        /// Gets the value delta. This is the maximum amount that the Value can change when it is updated.
        /// </summary>
        /// <value>The value delta.</value>
        [JsonPropertyName("valueDelta")]
        public DeltaBoundedNumber ValueDeltaMaximum => _valueDeltaMaximum;

        /// <summary>
        /// Gets or sets the value delta maximum absolute maximum. This is the absolute maximum that the ValueDelta can be.
        /// </summary>
        /// <value>The value delta maximum absolute maximum.</value>
        [JsonIgnore]
        public double ValueDeltaMaximumAbsoluteMaximum
        {
            get => _valueDeltaMaximum.DeltaAbsoluteMaximumValue;
            set => _valueDeltaMaximum.DeltaAbsoluteMaximumValue = value;
        }

        /// <summary>
        /// Gets or sets the value delta maximum evolution maximum. This is the maximum that the ValueDeltaMaximumValue
        /// value can change during the next evolution.
        /// </summary>
        /// <value>The value delta maximum evolution maximum.</value>
        [JsonIgnore]
        public double ValueDeltaMaximumEvolutionMax
        {
            get => _valueDeltaMaximum.DeltaMaxValue;
            set => _valueDeltaMaximum.DeltaMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the value delta maximum value. This is the maximum that the value can change during the next evolution.
        /// </summary>
        /// <value>The value delta maximum value.</value>
        [JsonIgnore]
        public double ValueDeltaMaximumValue
        {
            get => _valueDeltaMaximum.Value;
            set => _valueDeltaMaximum.Value = value;
        }

        /// <summary>
        /// The maximum that the value can be.
        /// </summary>
        [JsonPropertyName("maximumValue")]
        public Numerics.BoundedNumber ValueMaximum => _valueMaximum;

        /// <summary>
        /// Gets or sets the value maximum and minimum evolution delta maximum. This is the maximum amount that the
        /// ValueMaximum and ValueMinimum can change when they are evolved.
        /// </summary>
        /// <value>The value maximum and minimum evolution delta maximum.</value>
        [JsonPropertyName("valueMinimumAndMaximumEvolutionDeltaMax")]
        public double ValueMaximumAndMinimumEvolutionDeltaMax { get; set; }

        /// <summary>
        /// Gets or sets the value maximum value. This is the current maximum the value could grow to be.
        /// </summary>
        /// <value>The value maximum value.</value>
        [JsonIgnore]
        public double ValueMaximumValue
        {
            get => _valueMaximum.Value;
            set => _valueMaximum.Value = value;
        }

        /// <summary>
        /// The minimum that the value can be.
        /// </summary>
        [JsonPropertyName("minimumValue")]
        public Numerics.BoundedNumber ValueMinimum => _valueMinimum;

        /// <summary>
        /// Gets or sets the value minimum value. This is the current minimum the value could grow to be.
        /// </summary>
        /// <value>The value minimum value.</value>
        [JsonIgnore]
        public double ValueMinimumValue
        {
            get => _valueMinimum.Value;
            set => _valueMinimum.Value = value;
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
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public IEvoNumber Clone()
        {
            return new ReadOnlyEvoNumber(this);
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
            return HashCodeHelper.Combine(Value, ValueDeltaMaximum, ValueMaximum, ValueMinimum, OriginalValue, OriginalValueEvolutionDeltaMax, ValueMaximumAndMinimumEvolutionDeltaMax);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"ReadOnlyEvoNumber: {Value} (Original: {OriginalValue})";
        }
    }
}
