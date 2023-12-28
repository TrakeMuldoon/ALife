using ALife.Core.Utility.Maths;
using System.Diagnostics;

namespace ALife.Core.Utility
{
    /// <summary>
    /// A bounded auto-clamping number.
    /// </summary>
    [DebuggerDisplay("{_minValue} <= {_value} <= {_maxValue}")]
    public class BoundedNumber
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
            Value = parent.Value;
            MinValue = parent.MinValue;
            MaxValue = parent.MaxValue;
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
        /// Gets or sets the value for the number.
        /// </summary>
        public double Value
        {
            get => _value;
            set => ExtraMath.Clamp(value, MinValue, MaxValue);
        }
    }
}
