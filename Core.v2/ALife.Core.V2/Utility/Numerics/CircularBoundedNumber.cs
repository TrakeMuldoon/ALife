using System.Diagnostics;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded number that wraps around when it reaches the minimum or maximum.
    /// </summary>
    [DebuggerDisplay("Value: {Value}, Minimum: {Minimum}, Maximum: {Maximum}")]
    public class CircularBoundedNumber
    {
        /// <summary>
        /// The maximum
        /// </summary>
        private double _maximum;

        /// <summary>
        /// The minimum
        /// </summary>
        private double _minimum;

        /// <summary>
        /// The value
        /// </summary>
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBoundedNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        public CircularBoundedNumber(double value, double minimum, double maximum)
        {
            _value = value;
            _minimum = minimum;
            _maximum = maximum;
        }

        public CircularBoundedNumber(CircularBoundedNumber parent)
        {
            _value = parent.Value;
            _minimum = parent.Minimum;
            _maximum = parent.Maximum;
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                _value = ExtraMath.CircularClamp(_value, _minimum, _maximum);
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public double Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                _value = ExtraMath.CircularClamp(_value, _minimum, _maximum);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get => _value;
            set => _value = ExtraMath.CircularClamp(value, _minimum, _maximum);
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
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
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
            return Value.GetHashCode() ^ Minimum.GetHashCode() ^ Maximum.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Value: {Value}, Minimum: {Minimum}, Maximum: {Maximum}";
        }
    }
}
