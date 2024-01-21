using ALife.Core.Utility.Ranges;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an AHSV colour for the ALife simulation.
    /// </summary>
    [DebuggerDisplay("ToString()")]
    public struct HsvColour
    {
        public HsvColour(byte alpha, int hue, double saturation, double value) : this(alpha, hue, saturation, value, false)
        {
        }

        public HsvColour(int hue, double saturation, double value) : this(byte.MaxValue, hue, saturation, value, false)
        {
        }

        public HsvColour(HsvColour parent) : this(parent.A, parent.Hue, parent.Saturation, parent.Value, parent.WasPredefinedColour)
        {
        }

        public HsvColour(Colour colour)
        {
            WasPredefinedColour = colour.WasPredefinedColour;
            A = colour.A;
            ColourHelpers.ConvertRgbToHsl(colour.R, colour.G, colour.B, out int h, out double s, out double v);
            Hue = h;
            Saturation = s;
            Value = v;
        }

        internal HsvColour(byte alpha, int hue, double saturation, double value, bool preDefined)
        {
            A = alpha;
            Hue = DefaultRanges.DegreeRange.CircularClampValue(hue);
            Saturation = saturation;
            Value = value;
            WasPredefinedColour = preDefined;
        }

        public byte A { get; set; }

        public int Hue { get; set; }

        public double Saturation { get; set; }

        public double Value { get; set; }

        public bool WasPredefinedColour { get; private set; }

        public static bool operator ==(HsvColour left, HsvColour right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HsvColour left, HsvColour right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj is HsvColour)
            {
                return Equals((HsvColour)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(HsvColour other)
        {
            return A == other.A && Hue == other.Hue && Saturation == other.Saturation && Lightness == other.Lightness;
        }

        public override string ToString()
        {
            return $"h{Hue}, s{Saturation:#.##}, v{Value:#.##}, a{A}";
        }
    }
}