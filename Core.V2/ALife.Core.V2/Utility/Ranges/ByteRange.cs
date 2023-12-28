using System.Diagnostics;

namespace ALife.Core.Utility.Ranges
{
    [DebuggerDisplay("({Minimum} -> {Maximum})")]
    public struct ByteRange
    {
        private Range<byte> _range;

        public byte Minimum {
            get => _range.Minimum;
            set => _range.Minimum = value;
        }

        public byte Maximum {
            get => _range.Maximum;
            set => _range.Maximum = value;
        }

        public ByteRange(byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            _range = new Range<byte>(min, max);
        }

        public ByteRange()
        {
            _range = new Range<byte>(0, 255);
        }

        public static readonly ByteRange Default = new ByteRange();

        public static readonly ByteRange DefaultForColour = new ByteRange(100, 255);
    }
}
