namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// Extensions for numeric utility classes.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Converts a ManualBoundedNumber to a BoundedAutoNumber.
        /// </summary>
        /// <param name="ManualBoundedNumber">The ManualBoundedNumber to convert</param>
        /// <returns></returns>
        public static BoundedNumber ToAutoBoundedNumber(this ManualBoundedNumber number)
        {
            return new BoundedNumber(number.Value, number.MinValue, number.MaxValue);
        }

        /// <summary>
        /// Converts a BoundedNumber to a ManualBoundedNumber.
        /// </summary>
        /// <param name="BoundedAutoNumber">The BoundedNumber to convert</param>
        /// <returns></returns>
        public static ManualBoundedNumber ToManuallyBoundedNumber(this BoundedNumber number)
        {
            return new ManualBoundedNumber(number.Value, number.MinValue, number.MaxValue);
        }
    }
}
