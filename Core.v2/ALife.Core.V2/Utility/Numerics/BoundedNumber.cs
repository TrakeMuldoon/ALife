﻿using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded auto-clamping number.
    /// </summary>
    [DebuggerDisplay("{_minValue} <= {_value} <= {_maxValue}")]
    public struct BoundedNumber
    {
        /// <summary>
        /// The maximum value for the number.
        /// </summary>
        private double _maxValue;

        /// <summary>
        /// The minimum value for the number.
        /// </summary>
        private double _minValue;

        /// <summary>
        /// The actual value for the number.
        /// </summary>
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedNumber"/> class.
        /// </summary>
        /// <param name="value">The starting value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        [JsonConstructor]
        public BoundedNumber(double value, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            _value = ExtraMath.Clamp(value, minValue, maxValue);
            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedNumber"/> class by copying the values from the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public BoundedNumber(BoundedNumber parent)
        {
            _minValue = parent.MinValue;
            _maxValue = parent.MaxValue;
            Value = parent.Value;
        }

        /// <summary>
        /// Gets or sets the maximum value for the number.
        /// </summary>
        [JsonIgnore]
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                Value = _value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for the number.
        /// </summary>
        [JsonIgnore]
        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                Value = _value;
            }
        }

        /// <summary>
        /// Gets or sets the value for the number.
        /// </summary>
        [JsonIgnore]
        public double Value
        {
            get => _value;
            set => _value = ExtraMath.Clamp(value, MinValue, MaxValue);
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
                   MaxValue == number.MaxValue &&
                   MinValue == number.MinValue &&
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
            var hashCode = -212977889;
            hashCode = hashCode * -1521134295 + _maxValue.GetHashCode();
            hashCode = hashCode * -1521134295 + _minValue.GetHashCode();
            hashCode = hashCode * -1521134295 + _value.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{_minValue} <= {_value} <= {_maxValue}";
        }
    }
}
