using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Random;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an RGBA colour for the ALife simulation.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Colour : IColour
    {
        /// <summary>
        /// A colour representing the absence of colour.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Black = PredefineColour("000000");

        /// <summary>
        /// A colour representing the colour blue.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Blue = PredefineColour("0000ff");

        /// <summary>
        /// A colour representing the colour cyan.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Cyan = PredefineColour("00FFFF");

        /// <summary>
        /// A colour representing the colour dark blue.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DarkBlue = PredefineColour("00008B");

        /// <summary>
        /// A colour representing the colour dark khaki.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DarkKhaki = PredefineColour("Bdb76b");

        /// <summary>
        /// A colour representing the colour dark red.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DarkRed = PredefineColour("8B0000");

        /// <summary>
        /// A colour representing the colour dodger blue.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DodgerBlue = PredefineColour("1e90ff");

        /// <summary>
        /// A colour representing the colour green.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Green = PredefineColour("00ff00");

        /// <summary>
        /// A colour representing the colour grey.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Grey = PredefineColour("808080");

        /// <summary>
        /// A colour representing the colour hot pink.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour HotPink = PredefineColour("FF69B4");

        /// <summary>
        /// A colour representing the colour indian red.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour IndianRed = PredefineColour("CD5C5C");

        /// <summary>
        /// A colour representing the colour magenta.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Magenta = PredefineColour("FF00FF");

        /// <summary>
        /// A colour representing the colour maroon.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Maroon = PredefineColour("800000");

        /// <summary>
        /// A colour representing the colour lawn green.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour LawnGreen = PredefineColour("7cfc00");

        /// <summary>
        /// A colour representing the colour orange.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Orange = PredefineColour("FFA500");

        /// <summary>
        /// A colour representing the colour papaya whip.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour PapayaWhip = PredefineColour("Ffefd5");

        /// <summary>
        /// A colour representing the colour pink.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Pink = PredefineColour("FFC0CB");

        /// <summary>
        /// A colour representing the colour red.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Red = PredefineColour("Ff0000");

        /// <summary>
        /// A colour representing the colour white.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour White = PredefineColour("FFFFFF");

        /// <summary>
        /// A colour representing the colour white smoke.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour WhiteSmoke = PredefineColour("F5f5f5");

        /// <summary>
        /// A colour representing the colour yellow.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Yellow = PredefineColour("ffff00");

        /// <summary>
        /// The alpha channel
        /// </summary>
        [JsonIgnore]
        private byte _alpha;

        /// <summary>
        /// The blue channel
        /// </summary>
        [JsonIgnore]
        private byte _blue;

        /// <summary>
        /// The green channel
        /// </summary>
        [JsonIgnore]
        private byte _green;

        /// <summary>
        /// The red channel
        /// </summary>
        [JsonIgnore]
        private byte _red;

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public Colour(byte alpha, byte red, byte green, byte blue) : this(alpha, red, green, blue, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public Colour(byte red, byte green, byte blue) : this(255, red, green, blue, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public Colour(IColour colour) : this(colour.A, colour.R, colour.G, colour.B, colour.WasPredefined)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <param name="wasPredefined">if set to <c>true</c> [was predefined].</param>
        [JsonConstructor]
        internal Colour(byte alpha, byte red, byte green, byte blue, bool wasPredefined)
        {
            _alpha = alpha;
            _red = red;
            _green = green;
            _blue = blue;
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
        [JsonPropertyName("blue")]
        public byte B
        {
            get => _blue;
            set
            {
                _blue = value;
                WasPredefined = false;
            }
        }

        /// <summary>
        /// Gets green channel.
        /// </summary>
        /// <value>The green channel.</value>
        [JsonPropertyName("green")]
        public byte G
        {
            get => _green;
            set
            {
                _green = value;
                WasPredefined = false;
            }
        }

        /// <summary>
        /// Gets red channel.
        /// </summary>
        /// <value>The red channel.</value>
        [JsonPropertyName("red")]
        public byte R
        {
            get => _red;
            set
            {
                _red = value;
                WasPredefined = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this colour [was predefined].
        /// </summary>
        /// <value><c>true</c> if [was predefined]; otherwise, <c>false</c>.</value>
        [JsonPropertyName("wasPredefined")]
        public bool WasPredefined { get; private set; }

        /// <summary>
        /// Creates a Colour object from the specified AHex values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hex">The red channel.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromAHex(byte alpha, string hex)
        {
            ColourHelpers.ConvertHexToRgb(hex, out var r, out var g, out var b);
            return FromARGB(alpha, r, g, b);
        }

        /// <summary>
        /// Creates a Colour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            ColourHelpers.ConvertHslToRgb(hue, saturation, lightness, out var r, out var g, out var b);
            return FromARGB(alpha, r, g, b);
        }

        /// <summary>
        /// Creates a Colour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            ColourHelpers.ConvertHsvToRgb(hue, saturation, value, out var r, out var g, out var b);
            return FromARGB(alpha, r, g, b);
        }

        /// <summary>
        /// Creates a Colour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha channel.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            return new Colour(alpha, red, green, blue, false);
        }

        /// <summary>
        /// Creates a Colour object from the specified Hex value.
        /// </summary>
        /// <param name="hex">The hex code.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromHex(string hex)
        {
            return FromAHex(255, hex);
        }

        /// <summary>
        /// Creates a Colour object from the specified HSL value.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromHSL(int hue, double saturation, double lightness)
        {
            return FromAHSL(255, hue, saturation, lightness);
        }

        /// <summary>
        /// Creates a Colour object from the specified HSV value.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromHSV(int hue, double saturation, double value)
        {
            return FromAHSV(255, hue, saturation, value);
        }

        /// <summary>
        /// Creates a Colour object from the specified RGB value.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <returns>The Colour object.</returns>
        public static Colour FromRGB(byte red, byte green, byte blue)
        {
            return FromARGB(255, red, green, blue);
        }

        /// <summary>
        /// Generates a random Colour.
        /// </summary>
        /// <param name="randomizer">The randomizer.</param>
        /// <param name="alphaMin">The alpha minimum.</param>
        /// <param name="alphaMax">The alpha maximum.</param>
        /// <param name="redMin">The red minimum.</param>
        /// <param name="redMax">The red maximum.</param>
        /// <param name="greenMin">The green minimum.</param>
        /// <param name="greenMax">The green maximum.</param>
        /// <param name="blueMin">The blue minimum.</param>
        /// <param name="blueMax">The blue maximum.</param>
        /// <returns>The Colour.</returns>
        public static Colour GetRandomColour(IRandom randomizer, byte alphaMin = 0, byte alphaMax = 255, byte redMin = 0, byte redMax = 255, byte greenMin = 0, byte greenMax = 255, byte blueMin = 0, byte blueMax = 255)
        {
            byte alpha = randomizer.NextByte(alphaMin, alphaMax);
            byte red = randomizer.NextByte(redMin, redMax);
            byte green = randomizer.NextByte(greenMin, greenMax);
            byte blue = randomizer.NextByte(blueMin, blueMax);
            return new Colour(alpha, red, green, blue);
        }

        /// <summary>
        /// Predefines the colour specified by the hex code.
        /// </summary>
        /// <param name="hex">The hex code.</param>
        /// <returns>The Colour object.</returns>
        internal static Colour PredefineColour(string hex)
        {
            ColourHelpers.ConvertHexToRgb(hex, out var r, out var g, out var b);
            return new Colour(255, r, g, b, true);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Colour left, Colour right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Colour left, Colour right)
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
            int hash = A + B + G + R;
            return hash.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"r{R}, g{G}, b{B}, a{A}";
        }
    }
}
