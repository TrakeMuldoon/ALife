namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Various default-defined ranges for use in the ALife project.
    /// </summary>
    public static class DefaultRanges
    {
        /// <summary>
        /// A range representing the full range of a double.
        /// </summary>
        public static readonly Range<double> FullDoubleRange = new Range<double>(double.MinValue, double.MaxValue);

        /// <summary>
        /// A range representing a percentage (0/0% -> 1/100%).
        /// </summary>
        public static readonly Range<double> DoublePercentageRange = new Range<double>(0, 1);

        /// <summary>
        /// A range representing the valid numbers for a neural network.
        /// </summary>
        public static readonly Range<double> NeuralNetworkRange = new Range<double>(-1, 1);

        /// <summary>
        /// A range representing the full range of a byte.
        /// </summary>
        public static readonly Range<byte> FullByteRange = new Range<byte>(byte.MinValue, byte.MaxValue);

        /// <summary>
        /// The default range we use for Alpha in random colours.
        /// </summary>
        public static readonly Range<byte> RandomDefaultAlphaRange = new Range<byte>(255, 255);

        /// <summary>
        /// A range representing the default range we generally want for random colours.
        /// </summary>
        public static readonly Range<byte> RandomDefaultRgbColourRange = new Range<byte>(100, 255);

        /// <summary>
        /// A range representing the degrees in a circle.
        /// </summary>
        public static readonly Range<int> DegreeRange = new Range<int>(0, 360);
    }
}
