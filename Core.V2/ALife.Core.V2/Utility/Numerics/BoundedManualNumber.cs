using ALife.Core.Utility.Maths;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// A bounded number that can be clampped.
    /// </summary>
    public class BoundedManualNumber
    {
        /// <summary>
        /// The actual value for the number.
        /// </summary>
        private double _value;

        /// <summary>
        /// The minimum value for the number.
        /// </summary>
        private double _minValue;

        /// <summary>
        /// The maximum value for the number.
        /// </summary>
        private double _maxValue;

        /// <summary>
        /// Gets or sets the value for the number.
        /// </summary>
        public double Value
        {
            get => _value;
            set => ExtraMath.Clamp(value, ValueMin, ValueMax);
        }

        /// <summary>
        /// Gets or sets the maximum value for the number.
        /// </summary>
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
        /// Initializes a new instance of the <see cref="BoundedManualNumber"/> class.
        /// </summary>
        /// <param name="value">The starting value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public BoundedManualNumber(double value, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            _value = value;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundedManualNumber"/> class by copying the values from the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public BoundedManualNumber(BoundedManualNumber parent)
        {
            Value = parent.Value;
            MinValue = parent.MinValue;
            MaxValue = parent.MaxValue;
        }

        /// <summary>
        /// Clamps the value to the minimum and maximum values. Does not set the Value property.
        /// </summary>
        /// <returns>Returns the clampped value.</returns>
        public double Clamp()
        {
            _value = ExtraMath.Clamp(_value, _minValue, _maxValue);
            return _value;
        }

        /// <summary>
        /// Clamps the value to the minimum and maximum values.
        /// </summary>
        /// <returns></returns>
        public void ClampAndSet()
        {
            _value = Clamp();
        }
    }
}
