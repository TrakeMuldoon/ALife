using ALife.Core.Utility.Ranges;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALife.Core.Utility.Colour
{
    [DebuggerDisplay("r{Red}, g{Green}, b{Blue}, a{Alpha}")]
    public class Colour
    {
        public byte Alpha { get; set; }

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }

        public Colour(byte alpha, byte red, byte green, byte blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Colour(Colour parent, byte alpha)
        {
            Alpha = alpha;
            Red = parent.Red;
            Green = parent.Green;
            Blue = parent.Blue;
        }

        public Colour(Colour parent)
        {
            Alpha = parent.Alpha;
            Red = parent.Red;
            Green = parent.Green;
            Blue = parent.Blue;
        }

        public static Colour GetRandomColour(Nullable<ByteRange> redRange = null, Nullable<ByteRange> blueRange = null, Nullable<ByteRange> greenRange = null, Nullable<ByteRange> alphaRange = null)
        {
            ByteRange
        }
    }
}
