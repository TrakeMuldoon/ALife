using ALife.Core.Utility.Ranges;
using System.Diagnostics;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an AHSL colour for the ALife simulation.
    /// </summary>
    [DebuggerDisplay("ToString()")]
    public struct HslColour
    {
        public HslColour(byte alpha, int hue, double saturation, double lightness) : this(alpha, hue, saturation, lightness, false)
        {
        }

        public HslColour(int hue, double saturation, double lightness) : this(byte.MaxValue, hue, saturation, lightness, false)
        {
        }

        public HslColour(HslColour parent) : this(parent.A, parent.Hue, parent.Saturation, parent.Lightness, parent.WasPredefinedColour)
        {
        }

        public HslColour(Colour colour)
        {
            WasPredefinedColour = colour.WasPredefinedColour;
            A = colour.A;
            ColourHelpers.ConvertRgbToHsl(colour.R, colour.G, colour.B, out int h, out double s, out double l);
            Hue = h;
            Saturation = s;
            Lightness = l;
        }

        internal HslColour(byte alpha, int hue, double saturation, double lightness, bool preDefined)
        {
            A = alpha;
            Hue = DefaultRanges.DegreeRange.CircularClampValue(hue);
            Saturation = saturation;
            Lightness = lightness;
            WasPredefinedColour = preDefined;
        }

        public byte A { get; set; }

        public int Hue { get; set; }

        public double Saturation { get; set; }

        public double Lightness { get; set; }

        public bool WasPredefinedColour { get; private set; }

        public static bool operator ==(HslColour left, HslColour right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HslColour left, HslColour right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj is HslColour)
            {
                return Equals((HslColour)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(HslColour other)
        {
            return A == other.A && Hue == other.Hue && Saturation == other.Saturation && Lightness == other.Lightness;
        }

        public override string ToString()
        {
            return $"h{Hue}, s{Saturation:#.##}, l{Lightness:#.##}, a{A}";
        }
    }
}