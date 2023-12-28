using ALife.Core.Utility.Maths;
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
            DeltaMaximum = new BoundedNumber(parent.DeltaMaximum);
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get => _value;
            // _value cannot change by more than the deltaMax
            set => _value = ExtraMath.DeltaClamp(value, _value, -DeltaMaximum.Value, DeltaMaximum.Value, 0, DeltaMaximum.Value);
        }
    }
}
