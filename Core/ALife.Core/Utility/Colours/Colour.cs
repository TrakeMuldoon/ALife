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
        /// A Colour representing the absence of colour.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Black = new Colour(255, 0, 0, 0, true);

        /// <summary>
        /// A Colour representing the colour blue.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Blue = new Colour(255, 0, 0, 255, true);
        
        /// <summary>
        /// A Colour representing the colour cyan.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Cyan = new Colour(255, 0, 255, 255, true);

        /// <summary>
        /// A Colour representing the colour dark blue.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DarkBlue = new Colour(255, 0, 0, 139, true);

        /// <summary>
        /// A Colour representing the colour dark khaki.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DarkKhaki = new Colour(255, 189, 183, 107, true);

        /// <summary>
        /// A Colour representing the colour dodger blue.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour DodgerBlue = new Colour(255, 30, 144, 255, true);

        /// <summary>
        /// A Colour representing the colour green.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Green = new Colour(255, 0, 255, 0, true);

        /// <summary>
        /// A Colour representing the colour grey.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Grey = new Colour(255, 128, 128, 128, true);

        /// <summary>
        /// A Colour representing the colour indian red.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour IndianRed = new Colour(255, 205, 92, 92, true);

        /// <summary>
        /// A Colour representing the colour magenta.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Magenta = new Colour(255, 255, 0, 255, true);

        /// <summary>
        /// A Colour representing the colour maroon.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Maroon = new Colour(255, 128, 0, 0, true);

        /// <summary>
        /// A Colour representing the colour lawn green.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour LawnGreen = new Colour(255, 124, 252, 0, true);

        /// <summary>
        /// A Colour representing the colour orange.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Orange = new Colour(255, 255, 165, 0, true);

        /// <summary>
        /// A Colour representing the colour red.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Red = new Colour(255, 255, 0, 0, true);

        /// <summary>
        /// A Colour representing the colour white.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour White = new Colour(255, 255, 255, 255, true);

        /// <summary>
        /// A Colour representing the colour white smoke.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour WhiteSmoke = new Colour(255, 245, 245, 245, true);

        /// <summary>
        /// A Colour representing the colour yellow.
        /// </summary>
        [JsonIgnore]
        public static readonly Colour Yellow = new Colour(255, 255, 255, 0, true);

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
        /// Creates a Colour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The Colour object.</returns>
        public IColour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            ColourHelpers.ConvertHslToRgb(hue, saturation, lightness, out byte red, out byte green, out byte blue);
            return new Colour(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a Colour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Colour object.</returns>
        public IColour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            ColourHelpers.ConvertHsvToRgb(hue, saturation, value, out byte red, out byte green, out byte blue);
            return new Colour(alpha, red, green, blue);
        }

        /// <summary>
        /// Creates a Colour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns>The Colour object.</returns>
        public IColour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            return new Colour(alpha, red, green, blue);
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
