using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Random;
using ALife.Core.Utility.Ranges;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an AHSL colour for the ALife simulation.
    /// See: https://en.wikipedia.org/wiki/HSL_and_HSV
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct HslColour : IColour
    {
        /// <summary>
        /// A colour representing the absence of colour.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Black = PredefineColour("000000");

        /// <summary>
        /// A colour representing the colour blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Blue = PredefineColour("0000ff");

        /// <summary>
        /// A colour representing the colour cyan.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Cyan = PredefineColour("00FFFF");

        /// <summary>
        /// A colour representing the colour dark blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour DarkBlue = PredefineColour("00008B");

        /// <summary>
        /// A colour representing the colour dark khaki.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour DarkKhaki = PredefineColour("Bdb76b");

        /// <summary>
        /// A colour representing the colour dark red.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour DarkRed = PredefineColour("8B0000");

        /// <summary>
        /// A colour representing the colour dodger blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour DodgerBlue = PredefineColour("1e90ff");

        /// <summary>
        /// A colour representing the colour green.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Green = PredefineColour("00ff00");

        /// <summary>
        /// A colour representing the colour grey.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Grey = PredefineColour("808080");

        /// <summary>
        /// A colour representing the colour hot pink.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour HotPink = PredefineColour("FF69B4");

        /// <summary>
        /// A colour representing the colour indian red.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour IndianRed = PredefineColour("CD5C5C");

        /// <summary>
        /// A colour representing the colour lawn green.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour LawnGreen = PredefineColour("7cfc00");

        /// <summary>
        /// A colour representing the colour magenta.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Magenta = PredefineColour("FF00FF");

        /// <summary>
        /// A colour representing the colour maroon.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Maroon = PredefineColour("800000");

        /// <summary>
        /// A colour representing the colour orange.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Orange = PredefineColour("FFA500");

        /// <summary>
        /// A colour representing the colour papaya whip.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour PapayaWhip = PredefineColour("Ffefd5");

        /// <summary>
        /// A colour representing the colour pink.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Pink = PredefineColour("FFC0CB");

        /// <summary>
        /// A colour representing the colour red.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Red = PredefineColour("Ff0000");

        /// <summary>
        /// A colour representing the colour white.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour White = PredefineColour("FFFFFF");

        /// <summary>
        /// A colour representing the colour white smoke.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour WhiteSmoke = PredefineColour("F5f5f5");

        /// <summary>
        /// A colour representing the colour yellow.
        /// </summary>
        [JsonIgnore]
        public static readonly HslColour Yellow = PredefineColour("ffff00");

        /// <summary>
        /// The alpha channel
        /// </summary>
        [JsonIgnore]
        private byte _alpha;

        /// <summary>
        /// The hue
        /// </summary>
        [JsonIgnore]
        private int _hue;

        /// <summary>
        /// The lightness
        /// </summary>
        [JsonIgnore]
        private double _lightness;

        /// <summary>
        /// The saturation
        /// </summary>
        [JsonIgnore]
        private double _saturation;

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColour"/> struct.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public HslColour(IColour colour)
        {
            ColourHelpers.ConvertRgbToHsl(colour.R, colour.G, colour.B, out int hue, out double saturation, out double lightness);
            _alpha = colour.A;
            _hue = hue;
            _saturation = saturation;
            _lightness = lightness;
            R = colour.R;
            G = colour.G;
            B = colour.B;
            WasPredefined = colour.WasPredefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColour"/> struct.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public HslColour(int hue, double saturation, double lightness) : this(255, hue, saturation, lightness, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The lightness.</param>
        /// <param name="wasPredefined">if set to <c>true</c> [was predefined].</param>
        [JsonConstructor]
        public HslColour(byte alpha, int hue, double saturation, double lightness, bool wasPredefined = false)
        {
            _alpha = alpha;
            _hue = hue;
            _saturation = saturation;
            _lightness = lightness;

            ColourHelpers.ConvertHslToRgb(hue, saturation, lightness, out byte red, out byte green, out byte blue);
            R = red;
            G = green;
            B = blue;
            WasPredefined = wasPredefined;
        }

        /// <summary>
        /// Gets alpha channel.
        /// </summary>
        /// <value>The alpha channel.</value>
        [JsonPropertyName("alpha")]
        public byte A
        {
            get => _alpha;
            set
            {
                _alpha = value;
                WasPredefined = false;
            }
        }

        /// <summary>
        /// Gets blue channel.
        /// </summary>
        /// <value>The blue channel.</value>
        [JsonIgnore]
        public byte B { get; private set; }

        /// <summary>
        /// Gets green channel.
        /// </summary>
        /// <value>The green channel.</value>
        [JsonIgnore]
        public byte G { get; private set; }

        /// <summary>
        /// Gets the hue.
        /// </summary>
        /// <value>The hue.</value>
        [JsonPropertyName("hue")]
        public int Hue
        {
            get => _hue;
            set
            {
                _hue = value;
                WasPredefined = false;
                ColourHelpers.ConvertHslToRgb(_hue, _saturation, _lightness, out byte red, out byte green, out byte blue);
                R = red;
                G = green;
                B = blue;
            }
        }

        /// <summary>
        /// Gets the lightness.
        /// </summary>
        /// <value>The lightness.</value>
        [JsonPropertyName("lightness")]
        public double Lightness
        {
            get => _lightness;
            set
            {
                _lightness = value;
                WasPredefined = false;
                ColourHelpers.ConvertHslToRgb(_hue, _saturation, _lightness, out byte red, out byte green, out byte blue);
                R = red;
                G = green;
                B = blue;
            }
        }

        /// <summary>
        /// Gets red channel.
        /// </summary>
        /// <value>The red channel.</value>
        [JsonIgnore]
        public byte R { get; private set; }

        /// <summary>
        /// Gets the saturation.
        /// </summary>
        /// <value>The saturation.</value>
        [JsonPropertyName("saturation")]
        public double Saturation
        {
            get => _saturation;
            set
            {
                _saturation = value;
                WasPredefined = false;
                ColourHelpers.ConvertHslToRgb(_hue, _saturation, _lightness, out byte red, out byte green, out byte blue);
                R = red;
                G = green;
                B = blue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this colour [was predefined].
        /// </summary>
        /// <value><c>true</c> if [was predefined]; otherwise, <c>false</c>.</value>
        [JsonPropertyName("wasPredefined")]
        public bool WasPredefined { get; private set; }

        /// <summary>
        /// Creates a HslColour object from the specified AHex values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hex">The red channel.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromAHex(byte alpha, string hex)
        {
            ColourHelpers.ConvertHexToRgb(hex, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HslColour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            ColourHelpers.ConvertHslToRgb(hue, saturation, lightness, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HslColour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            ColourHelpers.ConvertHsvToRgb(hue, saturation, value, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HslColour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            ColourHelpers.ConvertRgbToHsl(red, green, blue, out int hue, out double saturation, out double lightness);
            return new HslColour(alpha, hue, saturation, lightness);
        }

        /// <summary>
        /// Creates a HslColour object from the specified ATSL values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="tint">The tint.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromATSL(byte alpha, double tint, double saturation, double value)
        {
            ColourHelpers.ConvertTslToRgb(tint, saturation, value, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HslColour object from the specified Hex value.
        /// </summary>
        /// <param name="hex">The hex code.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromHex(string hex)
        {
            return FromAHex(255, hex);
        }

        /// <summary>
        /// Creates a HslColour object from the specified HSL values.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromHSL(int hue, double saturation, double lightness)
        {
            return FromAHSL(255, hue, saturation, lightness);
        }

        /// <summary>
        /// Creates a HslColour object from the specified HSV values.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromHSV(int hue, double saturation, double value)
        {
            return FromAHSV(255, hue, saturation, value);
        }

        /// <summary>
        /// Creates a HslColour object from the specified RGB value.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromRGB(byte red, byte green, byte blue)
        {
            return FromARGB(255, red, green, blue);
        }

        /// <summary>
        /// Creates a HslColour object from the specified TSL values.
        /// </summary>
        /// <param name="tint"></param>
        /// <param name="saturation"></param>
        /// <param name="value"></param>
        /// <returns>The HslColour object.</returns>
        public static HslColour FromTsl(double tint, double saturation, double value)
        {
            return FromATSL(255, tint, saturation, value);
        }

        /// <summary>
        /// Generates a random Colour.
        /// </summary>
        /// <param name="randomizer">
        /// The random number generator. TODO: this should use a Simulation object once those exist.
        /// </param>
        /// <param name="alphaRange">The range of valid byte values for the Alpha channel. Defaults to 255-255.</param>
        /// <param name="hueRange">The range of valid int values for the Hue. Defaults to 0-360.</param>
        /// <param name="saturationRange">The range of valid double values for the Saturation. Defaults to 0-1.</param>
        /// <param name="lightnessRange">The range of valid double values for the Lightness. Defaults to 0-1.</param>
        /// <returns>The new random colour.</returns>
        public static HslColour GetRandomColour(IRandom randomizer, Nullable<Range<byte>> alphaRange = null, Nullable<Range<int>> hueRange = null, Nullable<Range<double>> saturationRange = null, Nullable<Range<double>> lightnessRange = null)
        {
            Range<byte> actualAlphaRange = alphaRange ?? DefaultRanges.RandomDefaultAlphaRange;
            Range<int> actualHueRange = hueRange ?? DefaultRanges.DegreeRange;
            Range<double> actualSaturationRange = saturationRange ?? DefaultRanges.DoublePercentageRange;
            Range<double> actualLightnessRange = lightnessRange ?? DefaultRanges.DoublePercentageRange;

            byte alpha = randomizer.NextByte(actualAlphaRange.Minimum, actualAlphaRange.Maximum);
            int hue = randomizer.Next(actualHueRange.Minimum, actualHueRange.Maximum);
            double saturation = randomizer.NextDouble(actualSaturationRange.Minimum, actualSaturationRange.Maximum);
            double lightness = randomizer.NextDouble(actualLightnessRange.Minimum, actualLightnessRange.Maximum);

            return new HslColour(alpha, hue, saturation, lightness);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HslColour"/> to <see cref="Colour"/>.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Colour(HslColour colour)
        {
            return new Colour(colour);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(HslColour left, HslColour right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(HslColour left, HslColour right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public IColour Clone()
        {
            return new Colour(this);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(IColour other)
        {
            return A == other.A && R == other.R && G == other.G && B == other.B;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if(obj is null)
            {
                return false;
            }
            return Equals((IColour)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(A, R, G, B, Hue, Saturation, Lightness);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            string hex = this.ToHexadecimal();
            string output = $"a{A}#{hex} (h{Hue}, s{Saturation:0.00}, l{Lightness:0.00})";
            return output;
        }

        /// <summary>
        /// Predefines the colour specified by the hex code.
        /// </summary>
        /// <param name="hex">The hex code.</param>
        /// <returns>The HslColour object.</returns>
        internal static HslColour PredefineColour(string hex)
        {
            ColourHelpers.ConvertHexToHsl(hex, out int hue, out double saturation, out double lightness);
            return new HslColour(255, hue, saturation, lightness, true);
        }
    }
}
