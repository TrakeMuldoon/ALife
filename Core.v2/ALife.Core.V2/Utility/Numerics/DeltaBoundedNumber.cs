using System.Diagnostics;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded auto-clamping number that can only change by a certain amount.
    /// </summary>
    [DebuggerDisplay("Value = {Value}, DeltaMaximum = {DeltaMaximum}")]
    public class DeltaBoundedNumber
    {
        /// <summary>
        /// The delta maximum
        /// </summary>
        public BoundedNumber DeltaMaximum;

        /// <summary>
        /// The value.
        /// </summary>
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaBoundedNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="deltaMaxValue">The delta maximum value.</param>
        /// <param name="deltaAbsoluteMinValue">The delta absolute minimum value.</param>
        /// <param name="deltaAbsoluteMaxValue">The delta absolute maximum value.</param>
        public DeltaBoundedNumber(double value, double deltaMaxValue, double deltaAbsoluteMinValue, double deltaAbsoluteMaxValue)
        {
            _value = value;
            DeltaMaximum = new BoundedNumber(deltaMaxValue, deltaAbsoluteMinValue, deltaAbsoluteMaxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaBoundedNumber"/> class.
        /// </summary>
        /// <param name="parent">The parent to clone.</param>
        public DeltaBoundedNumber(DeltaBoundedNumber parent)
        {
            _value = parent._value;
            DeltaMaximum = parent.DeltaMaximum.Clone();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get => _value;
            // _value cannot change by more than the deltaMax
            set => _value = ExtraMath.DeltaClamp(value, _value, -DeltaMaximum.Value, DeltaMaximum.Value, DeltaMaximum.MinValue, DeltaMaximum.MaxValue);
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
                   DeltaMaximum.Equals(number.DeltaMaximum);
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
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + DeltaMaximum.GetHashCode();
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
