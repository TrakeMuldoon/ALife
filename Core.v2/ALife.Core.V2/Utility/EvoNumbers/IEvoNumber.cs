using ALife.Core.Utility.Numerics;

namespace ALife.Core.Utility.EvoNumbers
{
    /// <summary>
    /// The interface for a number that can be evolved within a range.
    /// </summary>
    public interface IEvoNumber
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
        /// Gets the value delta. This is the maximum amount that the Value can change when it is updated.
        /// </summary>
        /// <value>The value delta.</value>
        DeltaBoundedNumber ValueDeltaMaximum { get; }

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
        /// The minimum that the value can be.
        /// </summary>
        BoundedNumber ValueMinimum { get; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        IEvoNumber Clone();
    }
}
