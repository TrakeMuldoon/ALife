using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.CommonInterfaces;
using ALife.Core.Utility.Ranges;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded number that wraps around when it reaches the minimum or maximum.
    /// TODO: We could make a numeric-generic version of this struct once we're on .NET 8 fairly easily...
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct CircularBoundedNumber : IDeepCloneable<CircularBoundedNumber>
    {
        /// <summary>
        /// The range
        /// </summary>
        [JsonIgnore]
        private Range<double> _range;

        /// <summary>
        /// The value
        /// </summary>
        [JsonIgnore]
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBoundedNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        [JsonConstructor]
        public CircularBoundedNumber(double value, double minimum, double maximum)
        {
            _range = new Range<double>(minimum, maximum);
            _value = _range.CircularClampValue(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBoundedNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public CircularBoundedNumber(CircularBoundedNumber parent) : this(parent.Value, parent.Minimum, parent.Maximum)
        {
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        [JsonPropertyName("maximum")]
        public double Maximum
        {
            get => _range.Maximum;
            set
            {
                _range.Maximum = value;
                Value = _value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        [JsonPropertyName("minimum")]
        public double Minimum
        {
            get => _range.Minimum;
            set
            {
                _range.Minimum = value;
                Value = _value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonPropertyName("value")]
        public double Value
        {
            get => _value;
            set => _value = _range.CircularClampValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CircularBoundedNumber"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator double(CircularBoundedNumber number)
        {
            return number.Value;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(CircularBoundedNumber left, CircularBoundedNumber right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(CircularBoundedNumber left, CircularBoundedNumber right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public CircularBoundedNumber Clone()
        {
            return new CircularBoundedNumber(this);
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
            return obj is CircularBoundedNumber number &&
                number.Value == Value &&
                number.Minimum == Minimum &&
                number.Maximum == Maximum;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(_range, _value);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{_range.Minimum} <= {_value} <= {_range.Maximum} (circular)";
        }
    }
}
