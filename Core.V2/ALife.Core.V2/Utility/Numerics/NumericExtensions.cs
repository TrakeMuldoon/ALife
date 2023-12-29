namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// Extensions for numeric utility classes.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Converts a BoundedManualNumber to a BoundedAutoNumber.
        /// </summary>
        /// <param name="BoundedManualNumber">The BoundedManualNumber to convert</param>
        /// <returns></returns>
        public static BoundedNumber ToAutoBoundedNumber(this BoundedManualNumber number)
        {
            return new BoundedNumber(number.Simulation, number.Value, number.MinValue, number.MaxValue);
        }

        /// <summary>
        /// Converts the ROEvoNumber to a EvoNumber.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The EvoNumber.</returns>
        public static EvoNumber ToEvoNumber(this ReadOnlyEvoNumber number)
        {
            return new EvoNumber(number);
        }

        /// <summary>
        /// Converts a BoundedAutoNumber to a BoundedManualNumber.
        /// </summary>
        /// <param name="BoundedAutoNumber">The BoundedManualNumber to convert</param>
        /// <returns></returns>
        public static BoundedManualNumber ToManuallyBoundedNumber(this BoundedNumber number)
        {
            return new BoundedManualNumber(number.Simulation, number.Value, number.MinValue, number.MaxValue);
        }

        /// <summary>
        /// Converts the EvoNumber to a ROEvoNumber.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The RPEvoNumber.</returns>
        public static ReadOnlyEvoNumber ToReadOnlyEvoNumber(this EvoNumber number)
        {
            return new ReadOnlyEvoNumber(number);
        }
    }
}
