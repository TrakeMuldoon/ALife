﻿using ALife.Core.Utility.Ranges;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded auto-clamping number.
    /// TODO: We could make a numeric-generic version of this struct once we're on .NET 8 fairly easily...
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct BoundedNumber
    {
        /// <summary>
        /// The range
        /// </summary>
        [JsonIgnore]
        private Range<double> _range;

        /// <summary>
        /// The actual value for the number.
        /// </summary>
        [JsonIgnore]
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedNumber"/> class.
        /// </summary>
        /// <param name="value">The starting value.</param>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        [JsonConstructor]
        public BoundedNumber(double value, double minimum, double maximum)
        {
            _range = new Range<double>(minimum, maximum);
            _value = _range.ClampValue(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedNumber"/> class by copying the values from the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public BoundedNumber(BoundedNumber parent) : this(parent.Value, parent.Minimum, parent.Maximum)
        {
        }

        /// <summary>
        /// Gets or sets the maximum value for the number.
        /// </summary>
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
        /// Gets or sets the minimum value for the number.
        /// </summary>
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
        /// Gets or sets the value for the number.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value
        {
            get => _value;
            set => _value = _range.ClampValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="BoundedNumber"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator double(BoundedNumber number)
        {
            return number.Value;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BoundedNumber left, BoundedNumber right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(BoundedNumber left, BoundedNumber right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(BoundedNumber left, BoundedNumber right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BoundedNumber left, BoundedNumber right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(BoundedNumber left, BoundedNumber right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(BoundedNumber left, BoundedNumber right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A cloned instance.</returns>
        public BoundedNumber Clone()
        {
            return new BoundedNumber(this);
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
            return obj is BoundedNumber number &&
                   Maximum == number.Maximum &&
                   Minimum == number.Minimum &&
                   Value == number.Value;
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
            return $"{_range.Minimum} <= {_value} <= {_range.Maximum}";
        }
    }
}
