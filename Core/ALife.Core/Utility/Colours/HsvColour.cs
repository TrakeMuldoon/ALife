﻿using ALife.Core.Utility.Random;
using ALife.Core.Utility.Ranges;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an AHSV colour for the ALife simulation.
    /// See: https://en.wikipedia.org/wiki/HSL_and_HSV
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct HsvColour : IColour
    {
        /// <summary>
        /// A colour representing the absence of colour.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Black = PredefineColour("000000");

        /// <summary>
        /// A colour representing the colour blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Blue = PredefineColour("0000ff");

        /// <summary>
        /// A colour representing the colour cyan.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Cyan = PredefineColour("00FFFF");

        /// <summary>
        /// A colour representing the colour dark blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour DarkBlue = PredefineColour("00008B");

        /// <summary>
        /// A colour representing the colour dark khaki.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour DarkKhaki = PredefineColour("Bdb76b");

        /// <summary>
        /// A colour representing the colour dark red.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour DarkRed = PredefineColour("8B0000");

        /// <summary>
        /// A colour representing the colour dodger blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour DodgerBlue = PredefineColour("1e90ff");

        /// <summary>
        /// A colour representing the colour green.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Green = PredefineColour("00ff00");

        /// <summary>
        /// A colour representing the colour grey.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Grey = PredefineColour("808080");

        /// <summary>
        /// A colour representing the colour hot pink.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour HotPink = PredefineColour("FF69B4");

        /// <summary>
        /// A colour representing the colour indian red.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour IndianRed = PredefineColour("CD5C5C");

        /// <summary>
        /// A colour representing the colour magenta.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Magenta = PredefineColour("FF00FF");

        /// <summary>
        /// A colour representing the colour maroon.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Maroon = PredefineColour("800000");

        /// <summary>
        /// A colour representing the colour lawn green.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour LawnGreen = PredefineColour("7cfc00");

        /// <summary>
        /// A colour representing the colour orange.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Orange = PredefineColour("FFA500");

        /// <summary>
        /// A colour representing the colour papaya whip.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour PapayaWhip = PredefineColour("Ffefd5");

        /// <summary>
        /// A colour representing the colour pink.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Pink = PredefineColour("FFC0CB");

        /// <summary>
        /// A colour representing the colour red.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Red = PredefineColour("Ff0000");

        /// <summary>
        /// A colour representing the colour white.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour White = PredefineColour("FFFFFF");

        /// <summary>
        /// A colour representing the colour white smoke.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour WhiteSmoke = PredefineColour("F5f5f5");

        /// <summary>
        /// A colour representing the colour yellow.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Yellow = PredefineColour("ffff00");

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
        /// The saturation
        /// </summary>
        [JsonIgnore]
        private double _saturation;

        /// <summary>
        /// The value
        /// </summary>
        [JsonIgnore]
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="HsvColour"/> struct.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public HsvColour(IColour colour)
        {
            ColourHelpers.ConvertRgbToHsv(colour.R, colour.G, colour.B, out int hue, out double saturation, out double value);
            _alpha = colour.A;
            _hue = hue;
            _saturation = saturation;
            _value = value;
            R = colour.R;
            G = colour.G;
            B = colour.B;
            WasPredefined = colour.WasPredefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HsvColour"/> struct.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        public HsvColour(int hue, double saturation, double value) : this(255, hue, saturation, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HsvColour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <param name="wasPredefined">if set to <c>true</c> [was predefined].</param>
        [JsonConstructor]
        public HsvColour(byte alpha, int hue, double saturation, double value, bool wasPredefined = false)
        {
            _alpha = alpha;
            _hue = hue;
            _saturation = saturation;
            _value = value;

            ColourHelpers.ConvertHsvToRgb(hue, saturation, value, out byte red, out byte green, out byte blue);
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
                ColourHelpers.ConvertHsvToRgb(_hue, _saturation, _value, out byte red, out byte green, out byte blue);
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
                ColourHelpers.ConvertHsvToRgb(_hue, _saturation, _value, out byte red, out byte green, out byte blue);
                R = red;
                G = green;
                B = blue;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonPropertyName("value")]
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                WasPredefined = false;
                ColourHelpers.ConvertHsvToRgb(_hue, _saturation, _value, out byte red, out byte green, out byte blue);
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
        /// Creates a HsvColour object from the specified AHex values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hex">The red channel.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromAHex(byte alpha, string hex)
        {
            ColourHelpers.ConvertHexToRgb(hex, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            ColourHelpers.ConvertHslToRgb(hue, saturation, lightness, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            ColourHelpers.ConvertHsvToRgb(hue, saturation, value, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            ColourHelpers.ConvertRgbToHsv(red, green, blue, out int hue, out double saturation, out double value);
            return new HsvColour(alpha, hue, saturation, value);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified ATSL values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="tint">The tint.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromATSL(byte alpha, double tint, double saturation, double value)
        {
            ColourHelpers.ConvertTslToRgb(tint, saturation, value, out byte red, out byte green, out byte blue);
            return FromARGB(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified Hex value.
        /// </summary>
        /// <param name="hex">The hex code.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromHex(string hex)
        {
            return FromAHex(255, hex);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified HSL values.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromHSL(int hue, double saturation, double lightness)
        {
            return FromAHSL(255, hue, saturation, lightness);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified HSV values.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromHSV(int hue, double saturation, double value)
        {
            return FromAHSV(255, hue, saturation, value);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified TSL values.
        /// </summary>
        /// <param name="tint"></param>
        /// <param name="saturation"></param>
        /// <param name="value"></param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromTsl(double tint, double saturation, double value)
        {
            return FromATSL(255, tint, saturation, value);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified RGB value.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour FromRGB(byte red, byte green, byte blue)
        {
            return FromARGB(255, red, green, blue);
        }

        /// <summary>
        /// Generates a random Colour.
        /// </summary>
        /// <param name="randomizer">The random number generator. TODO: this should use a Simulation object once those exist.</param>
        /// <param name="alphaRange">The range of valid byte values for the Alpha channel. Defaults to 255-255.</param>
        /// <param name="hueRange">The range of valid int values for the Hue. Defaults to 0-360.</param>
        /// <param name="saturationRange">The range of valid double values for the Saturation. Defaults to 0-1.</param>
        /// <param name="valueRange">The range of valid double values for the Value. Defaults to 0-1.</param>
        /// <returns>The new random colour.</returns>
        public static HsvColour GetRandomColour(IRandom randomizer, Nullable<Range<byte>> alphaRange = null, Nullable<Range<int>> hueRange = null, Nullable<Range<double>> saturationRange = null, Nullable<Range<double>> valueRange = null)
        {
            Range<byte> actualAlphaRange = alphaRange ?? DefaultRanges.RandomDefaultAlphaRange;
            Range<int> actualHueRange = hueRange ?? DefaultRanges.DegreeRange;
            Range<double> actualSaturationRange = saturationRange ?? DefaultRanges.DoublePercentageRange;
            Range<double> actualValueRange = valueRange ?? DefaultRanges.DoublePercentageRange;

            byte alpha = randomizer.NextByte(actualAlphaRange.Minimum, actualAlphaRange.Maximum);
            int hue = randomizer.Next(actualHueRange.Minimum, actualHueRange.Maximum);
            double saturation = randomizer.NextDouble(actualSaturationRange.Minimum, actualSaturationRange.Maximum);
            double value = randomizer.NextDouble(actualValueRange.Minimum, actualValueRange.Maximum);

            return new HsvColour(alpha, hue, saturation, value);
        }

        /// <summary>
        /// Predefines the colour specified by the hex code.
        /// </summary>
        /// <param name="hex">The hex code.</param>
        /// <returns>The HsvColour object.</returns>
        internal static HsvColour PredefineColour(string hex)
        {
            ColourHelpers.ConvertHexToHsv(hex, out int hue, out double saturation, out double value);
            return new HsvColour(255, hue, saturation, value, true);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HsvColour"/> to <see cref="Colour"/>.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Colour(HsvColour colour)
        {
            return new Colour(colour);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(HsvColour left, HsvColour right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(HsvColour left, HsvColour right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
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
            return HashCodeHelper.Combine(A, R, G, B, Hue, Saturation, Value);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            string hex = this.ToHexadecimal();
            string output = $"a{A}#{hex} (h{Hue}, s{Saturation:0.00}, v{Value:0.00})";
            return output;
        }
    }
}
