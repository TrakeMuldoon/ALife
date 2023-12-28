namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Represents a range of values.
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    public struct Range<T>
    {
        /// <summary>
        /// The minimum value of the range.
        /// </summary>
        public T Minimum;

        /// <summary>
        /// The maximum value of the range.
        /// </summary>
        public T Maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ALife.Core.Utility.Ranges.Range`1"/> struct.
        /// </summary>
        /// <param name="value"></param>
        public Range(Range<T> parent)
        {
            Minimum = parent.Minimum;
            Maximum = parent.Maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ALife.Core.Utility.Ranges.Range`1"/> struct.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public Range(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
