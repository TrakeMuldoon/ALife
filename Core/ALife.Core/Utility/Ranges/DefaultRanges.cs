using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALife.Core.Utility.Ranges
{
    public static class DefaultRanges
    {
        public static readonly Range<double> FullDoubleRange = new Range<double>(double.MinValue, double.MaxValue);

        public static readonly Range<double> DoublePercentageRange = new Range<double>(0, 1);

        public static readonly Range<byte> FullByteRange = new Range<byte>(byte.MinValue, byte.MaxValue);

        public static readonly Range<byte> RandomColourRange = new Range<byte>(100, 255);

        public static readonly Range<int> DegreeRange = new Range<int>(0, 360);
    }
}
