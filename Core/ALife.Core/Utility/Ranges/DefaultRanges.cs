namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Various default-defined ranges for use in the ALife project.
    /// </summary>
    public class DefaultRanges
    {
        /// <summary>
        /// A range representing the full range of a double.
        /// </summary>
        public static readonly ValueRange<double> FullDoubleRange = new(double.MinValue, double.MaxValue);

        /// <summary>
        /// A range representing a percentage (0/0% -> 1/100%).
        /// </summary>
        public static readonly ValueRange<double> DoublePercentageRange = new(0, 1);

        /// <summary>
        /// A range representing the valid numbers for a neural network.
        /// </summary>
        public static readonly ValueRange<double> NeuralNetworkRange = new(-1, 1);

        /// <summary>
        /// A range representing the full range of a byte.
        /// </summary>
        public static readonly ValueRange<byte> FullByteRange = new(byte.MinValue, byte.MaxValue);

        /// <summary>
        /// The default range we use for Alpha in random colours.
        /// </summary>
        public static readonly ValueRange<byte> RandomDefaultAlphaRange = new(255, 255);

        /// <summary>
        /// A range representing the default range we generally want for random colours.
        /// </summary>
        public static readonly ValueRange<byte> RandomDefaultRgbColourRange = new(100, 255);

        /// <summary>
        /// A range representing the degrees in a circle.
        /// </summary>
        public static readonly ValueRange<int> DegreeRange = new(0, 360);
    }
}