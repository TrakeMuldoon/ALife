using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Random;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an AHSV colour for the ALife simulation.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct HsvColour : IColour
    {
        /// <summary>
        /// A HsvColour representing the absence of colour.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Black = new HsvColour(Colour.Black);

        /// <summary>
        /// A HsvColour representing the colour blue.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Blue = new HsvColour(Colour.Blue);

        /// <summary>
        /// A HsvColour representing the colour cyan.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Cyan = new HsvColour(Colour.Cyan);

        /// <summary>
        /// A HsvColour representing the colour green.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Green = new HsvColour(Colour.Green);

        /// <summary>
        /// A HsvColour representing the colour magenta.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Magenta = new HsvColour(Colour.Magenta);

        /// <summary>
        /// A HsvColour representing the colour red.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Red = new HsvColour(Colour.Red);

        /// <summary>
        /// A HsvColour representing the colour white.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour White = new HsvColour(Colour.White);

        /// <summary>
        /// A HsvColour representing the colour yellow.
        /// </summary>
        [JsonIgnore]
        public static readonly HsvColour Yellow = new HsvColour(Colour.Yellow);

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
        /// Generates a random HsvColour.
        /// </summary>
        /// <param name="randomizer">The randomizer.</param>
        /// <param name="alphaMin">The alpha minimum.</param>
        /// <param name="alphaMax">The alpha maximum.</param>
        /// <param name="hueMin">The hue minimum.</param>
        /// <param name="hueMax">The hue maximum.</param>
        /// <param name="saturationMin">The saturation minimum.</param>
        /// <param name="saturationMax">The saturation maximum.</param>
        /// <param name="valueMin">The value minimum.</param>
        /// <param name="valueMax">The value maximum.</param>
        /// <returns>The HsvColour.</returns>
        public static HsvColour GetRandomColour(IRandom randomizer, byte alphaMin = 0, byte alphaMax = 255, int hueMin = 0, int hueMax = 361, double saturationMin = 0d, double saturationMax = 1d, double valueMin = 0d, double valueMax = 1d)
        {
            int hue = randomizer.Next(hueMin, hueMax);

            double saturationModifier = randomizer.NextDouble();
            double saturation = saturationModifier * saturationMax + (1 - saturationModifier) * saturationMin;

            double valueModifier = randomizer.NextDouble();
            double value = valueModifier * valueMax + (1 - valueModifier) * valueMin;
            return new HsvColour(hue, saturation, value);
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
        /// Creates a HsvColour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HsvColour object.</returns>
        public IColour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            ColourHelpers.ConvertHslToRgb(hue, saturation, lightness, out byte red, out byte green, out byte blue);
            return new HsvColour(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The HsvColour object.</returns>
        public IColour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            return new HsvColour(alpha, hue, saturation, value);
        }

        /// <summary>
        /// Creates a HsvColour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns>The HsvColour object.</returns>
        public IColour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            ColourHelpers.ConvertRgbToHsv(red, green, blue, out int hue, out double saturation, out double value);
            return new HsvColour(alpha, hue, saturation, value);
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
            return $"a{A}, h{Hue}, s{Saturation:0.00}, v{Value:0.00} (r{R}, g{G}, b{B})";
        }
    }
}
