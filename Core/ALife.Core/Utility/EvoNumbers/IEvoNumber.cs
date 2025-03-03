using ALife.Core.Interfaces;
using ALife.Core.Utility.Numerics;

namespace ALife.Core.Utility.EvoNumbers
{
    /// <summary>
    /// The interface for a number that can be evolved within a range.
    /// </summary>
    public interface IEvoNumber : IDeepCloneable<IEvoNumber>
    {
        /// <summary>
        /// Gets the original (start) value.
        /// </summary>
        /// <value>The original value.</value>
        double OriginalValue { get; }

        /// <summary>
        /// Gets the original value evolution delta maximum.
        /// </summary>
        /// <value>The original value evolution delta maximum.</value>
        double OriginalValueEvolutionDeltaMax { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        double Value { get; set; }

        /// <summary>
        /// Gets or sets the value absolute minimum. This is the absolute minimum that the Value can grow to be.
        /// </summary>
        /// <value>The value absolute minimum.</value>
        double ValueAbsoluteMaximum { get; set; }

        /// <summary>
        /// Gets or sets the value absolute maximum. This is the absolute maximum that the Value can grow to be.
        /// </summary>
        /// <value>The value absolute maximum.</value>
        double ValueAbsoluteMinimum { get; set; }

        /// <summary>
        /// Gets the value delta. This is the maximum amount that the Value can change when it is updated.
        /// </summary>
        /// <value>The value delta.</value>
        DeltaBoundedNumber ValueDeltaMaximum { get; }

        /// <summary>
        /// Gets or sets the value delta maximum absolute maximum. This is the absolute maximum that the ValueDelta can be.
        /// </summary>
        /// <value>The value delta maximum absolute maximum.</value>
        double ValueDeltaMaximumAbsoluteMaximum { get; set; }

        /// <summary>
        /// Gets or sets the value delta maximum evolution maximum. This is the maximum that the ValueDeltaMaximumValue
        /// value can change during the next evolution.
        /// </summary>
        /// <value>The value delta maximum evolution maximum.</value>
        double ValueDeltaMaximumEvolutionMax { get; set; }

        /// <summary>
        /// Gets or sets the value delta maximum value. This is the maximum that the value can change during the next evolution.
        /// </summary>
        /// <value>The value delta maximum value.</value>
        double ValueDeltaMaximumValue { get; set; }

        /// <summary>
        /// The maximum that the value can be.
        /// </summary>
        BoundedNumber ValueMaximum { get; }

        /// <summary>
        /// Gets or sets the value maximum and minimum evolution delta maximum. This is the maximum amount that the
        /// ValueMaximum and ValueMinimum can change when they are evolved.
        /// </summary>
        /// <value>The value maximum and minimum evolution delta maximum.</value>
        double ValueMaximumAndMinimumEvolutionDeltaMax { get; set; }

        /// <summary>
        /// Gets or sets the value maximum value. This is the current maximum the value could grow to be.
        /// </summary>
        /// <value>The value maximum value.</value>
        double ValueMaximumValue { get; set; }

        /// <summary>
        /// The minimum that the value can be.
        /// </summary>
        BoundedNumber ValueMinimum { get; }

        /// <summary>
        /// Gets or sets the value minimum value. This is the current minimum the value could grow to be.
        /// </summary>
        /// <value>The value minimum value.</value>
        double ValueMinimumValue { get; set; }
    }
}