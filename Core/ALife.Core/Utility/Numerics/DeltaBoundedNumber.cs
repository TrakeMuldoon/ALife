using ALife.Core.Utility.Maths;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded auto-clamping number that can only change by a certain amount.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct DeltaBoundedNumber
    {
        /// <summary>
        /// The delta maximum
        /// </summary>
        [JsonIgnore]
        private BoundedNumber _deltaMaximum;

        /// <summary>
        /// The value.
        /// </summary>
        [JsonIgnore]
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaBoundedNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="deltaMaxValue">The delta maximum value.</param>
        /// <param name="deltaAbsoluteMinValue">The delta absolute minimum value.</param>
        /// <param name="deltaAbsoluteMaxValue">The delta absolute maximum value.</param>
        [JsonConstructor]
        public DeltaBoundedNumber(double value, double deltaMaxValue, double deltaAbsoluteMinValue, double deltaAbsoluteMaxValue)
        {
            _value = value;
            _deltaMaximum = new BoundedNumber(deltaMaxValue, deltaAbsoluteMinValue, deltaAbsoluteMaxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaBoundedNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent to clone.</param>
        public DeltaBoundedNumber(DeltaBoundedNumber parent) : this(parent.Value, parent.DeltaMaxValue, parent.DeltaAbsoluteMinimumValue, parent.DeltaAbsoluteMaximumValue)
        {
        }

        /// <summary>
        /// Gets or sets the delta absolute maximum value.
        /// </summary>
        /// <value>The delta absolute maximum value.</value>
        [JsonPropertyName("deltaAbsoluteMaxValue")]
        public double DeltaAbsoluteMaximumValue
        {
            get => _deltaMaximum.Maximum;
            set => _deltaMaximum.Maximum = value;
        }

        /// <summary>
        /// Gets or sets the delta absolute minimum value.
        /// </summary>
        /// <value>The delta absolute minimum value.</value>
        [JsonPropertyName("deltaAbsoluteMinValue")]
        public double DeltaAbsoluteMinimumValue
        {
            get => _deltaMaximum.Minimum;
            set => _deltaMaximum.Minimum = value;
        }

        /// <summary>
        /// Gets or sets the delta maximum value.
        /// </summary>
        /// <value>The delta maximum value.</value>
        [JsonPropertyName("deltaMaxValue")]
        public double DeltaMaxValue
        {
            get => _deltaMaximum.Value;
            set => _deltaMaximum.Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonPropertyName("value")]
        public double Value
        {
            get => _value;
            // _value cannot change by more than the deltaMax
            set => _value = ExtraMath.DeltaClamp(value, _value, -DeltaMaxValue, DeltaMaxValue, DeltaAbsoluteMinimumValue, DeltaAbsoluteMaximumValue);
        }

        /// <summary>
        /// Gets or sets the delta maximum.
        /// </summary>
        /// <value>The delta maximum.</value>
        [JsonIgnore]
        private BoundedNumber DeltaMaximum
        {
            get => _deltaMaximum;
            set => _deltaMaximum = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DeltaBoundedNumber"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator double(DeltaBoundedNumber number)
        {
            return number.Value;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DeltaBoundedNumber left, DeltaBoundedNumber right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(DeltaBoundedNumber left, DeltaBoundedNumber right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(DeltaBoundedNumber left, DeltaBoundedNumber right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DeltaBoundedNumber left, DeltaBoundedNumber right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(DeltaBoundedNumber left, DeltaBoundedNumber right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(DeltaBoundedNumber left, DeltaBoundedNumber right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public DeltaBoundedNumber Clone()
        {
            return new DeltaBoundedNumber(this);
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
            return obj is DeltaBoundedNumber number &&
                   Value == number.Value &&
                   _deltaMaximum.Equals(number._deltaMaximum);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = Value.GetHashCode() + DeltaMaximum.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Value = {Value}, DeltaMaximum = {DeltaMaximum}";
        }
    }
}
