using ALife.Core.Utility.Maths;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Represents a range of values.
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    [DebuggerDisplay("{ToString()}")]
    public struct ValueRange<T> : IEquatable<ValueRange<T>> where T : INumber<T>
    {
        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        [JsonIgnore]
        private T _maximum;

        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        [JsonIgnore]
        private T _minimum;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ALife.Core.Utility.Ranges.Range`1"/> struct.
        /// </summary>
        /// <param name="parent"></param>
        public ValueRange(ValueRange<T> parent)
        {
            _minimum = parent.Minimum;
            _maximum = parent.Maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ALife.Core.Utility.Ranges.Range`1"/> struct.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        [JsonConstructor]
        public ValueRange(T minimum, T maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
            if(_maximum < _minimum)
            {
                (_minimum, _maximum) = (_maximum, _minimum);
            }
        }

        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        [JsonPropertyName("maximum")]
        public T Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if(_maximum < _minimum)
                {
                    (_minimum, _maximum) = (_maximum, _minimum);
                }
            }
        }

        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        [JsonPropertyName("minimum")]
        public T Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if(_maximum < _minimum)
                {
                    (_minimum, _maximum) = (_maximum, _minimum);
                }
            }
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValueRange<T> left, ValueRange<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValueRange<T> left, ValueRange<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Clamps the value to the range in a circular fashion.
        /// TODO: Generic math would make more performant...
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The clamped value.</returns>
        public T CircularClampValue(T value)
        {
            dynamic output = ExtraMath<T>.CircularClamp(value, Minimum, Maximum);
            return output;
        }

        /// <summary>
        /// Clamps the value to the range.
        /// TODO: Generic math would make more performant...
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The clamped value.</returns>
        public T ClampValue(T value)
        {
            dynamic output = ExtraMath<T>.Clamp(value, Minimum, Maximum);
            return output;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if(obj is ValueRange<T> other)
            {
                return Minimum.Equals(other.Minimum) && Maximum.Equals(other.Maximum);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(_minimum, _maximum);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"({Minimum} -> {Maximum})";
        }

        /// <summary>
        /// Compares two ValueRanges for equality.
        /// </summary>
        /// <param name="other">The range to compare.</param>
        /// <returns>True if equals, False otherwise.</returns>
        public bool Equals(ValueRange<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_maximum, other._maximum) && EqualityComparer<T>.Default.Equals(_minimum, other._minimum);
        }
    }
}