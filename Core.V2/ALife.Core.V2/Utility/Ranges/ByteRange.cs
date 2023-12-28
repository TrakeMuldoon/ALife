using System.Diagnostics;

namespace ALife.Core.Utility.Ranges
{
    /// <summary>
    /// Represents a range of bytes.
    /// </summary>
    [DebuggerDisplay("({Minimum} -> {Maximum})")]
    public struct ByteRange
    {
        /// <summary>
        /// The default
        /// </summary>
        public static readonly ByteRange Default = new ByteRange();

        /// <summary>
        /// The default range for a random colour channel
        /// </summary>
        public static readonly ByteRange DefaultForColour = new ByteRange(100, 255);

        /// <summary>
        /// The range
        /// </summary>
        private Range<byte> _range;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteRange"/> struct.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public ByteRange(byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            _range = new Range<byte>(min, max);
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public byte Maximum
        {
            get => _range.Maximum;
            set => _range.Maximum = value;
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public byte Minimum
        {
            get => _range.Minimum;
            set => _range.Minimum = value;
        }
    }
}
