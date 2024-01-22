using ALife.Core.Utility.Random;
using System.Diagnostics;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an AHSL colour for the ALife simulation.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct HslColour : IColour
    {
        /// <summary>
        /// A HslColour representing the absence of colour.
        /// </summary>
        public static readonly HslColour Black = new HslColour(Colour.Black);

        /// <summary>
        /// A HslColour representing the colour blue.
        /// </summary>
        public static readonly HslColour Blue = new HslColour(Colour.Blue);

        /// <summary>
        /// A HslColour representing the colour cyan.
        /// </summary>
        public static readonly HslColour Cyan = new HslColour(Colour.Cyan);

        /// <summary>
        /// A HslColour representing the colour green.
        /// </summary>
        public static readonly HslColour Green = new HslColour(Colour.Green);

        /// <summary>
        /// A HslColour representing the colour magenta.
        /// </summary>
        public static readonly HslColour Magenta = new HslColour(Colour.Magenta);

        /// <summary>
        /// A HslColour representing the colour red.
        /// </summary>
        public static readonly HslColour Red = new HslColour(Colour.Red);

        /// <summary>
        /// A HslColour representing the colour white.
        /// </summary>
        public static readonly HslColour White = new HslColour(Colour.White);

        /// <summary>
        /// A HslColour representing the colour yellow.
        /// </summary>
        public static readonly HslColour Yellow = new HslColour(Colour.Yellow);

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColour"/> struct.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public HslColour(IColour colour)
        {
            ColourHelpers.ConvertRgbToHsl(colour.R, colour.G, colour.B, out int hue, out double saturation, out double lightness);
            A = colour.A;
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
            R = colour.R;
            G = colour.G;
            B = colour.B;
            WasPredefined = colour.WasPredefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HslColour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public HslColour(byte alpha, int hue, double saturation, double lightness) : this(alpha, hue, saturation, lightness, false)
        {
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
        internal HslColour(byte alpha, int hue, double saturation, double lightness, bool wasPredefined)
        {
            A = alpha;
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;

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
        public byte A { get; }

        /// <summary>
        /// Gets blue channel.
        /// </summary>
        /// <value>The blue channel.</value>
        public byte B { get; }

        /// <summary>
        /// Gets green channel.
        /// </summary>
        /// <value>The green channel.</value>
        public byte G { get; }

        /// <summary>
        /// Gets the hue.
        /// </summary>
        /// <value>The hue.</value>
        public int Hue { get; }

        /// <summary>
        /// Gets the lightness.
        /// </summary>
        /// <value>The lightness.</value>
        public double Lightness { get; }

        /// <summary>
        /// Gets red channel.
        /// </summary>
        /// <value>The red channel.</value>
        public byte R { get; }

        /// <summary>
        /// Gets the saturation.
        /// </summary>
        /// <value>The saturation.</value>
        public double Saturation { get; }

        /// <summary>
        /// Gets a value indicating whether this colour [was predefined].
        /// </summary>
        /// <value><c>true</c> if [was predefined]; otherwise, <c>false</c>.</value>
        public bool WasPredefined { get; }

        /// <summary>
        /// Generates a random HslColour.
        /// </summary>
        /// <param name="randomizer">The randomizer.</param>
        /// <param name="alphaMin">The alpha minimum.</param>
        /// <param name="alphaMax">The alpha maximum.</param>
        /// <param name="hueMin">The hue minimum.</param>
        /// <param name="hueMax">The hue maximum.</param>
        /// <param name="saturationMin">The saturation minimum.</param>
        /// <param name="saturationMax">The saturation maximum.</param>
        /// <param name="lightnessMin">The lightness minimum.</param>
        /// <param name="lightnessMax">The lightness maximum.</param>
        /// <returns>The HslColour.</returns>
        public static HslColour GetRandomColour(IRandom randomizer, byte alphaMin = 0, byte alphaMax = 255, int hueMin = 0, int hueMax = 361, double saturationMin = 0d, double saturationMax = 1d, double lightnessMin = 0d, double lightnessMax = 1d)
        {
            int hue = randomizer.Next(hueMin, hueMax);

            double saturationModifier = randomizer.NextDouble();
            double saturation = saturationModifier * saturationMax + (1 - saturationModifier) * saturationMin;

            double lightnessModifier = randomizer.NextDouble();
            double lightness = lightnessModifier * lightnessMax + (1 - lightnessModifier) * lightnessMin;
            return new HslColour(hue, saturation, lightness);
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
        /// Creates a HslColour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The HslColour object.</returns>
        public IColour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            return new HslColour(alpha, hue, saturation, lightness);
        }

        /// <summary>
        /// Creates a HslColour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The HslColour object.</returns>
        public IColour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            ColourHelpers.ConvertHsvToRgb(hue, saturation, value, out byte red, out byte green, out byte blue);
            ColourHelpers.ConvertRgbToHsl(red, green, blue, out var newHue, out var newSaturation, out var lightness);

            return new HslColour(alpha, newHue, newSaturation, lightness);
        }

        /// <summary>
        /// Creates a HslColour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns>The HslColour object.</returns>
        public IColour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            ColourHelpers.ConvertRgbToHsl(red, green, blue, out int hue, out double saturation, out double lightness);
            return new HslColour(alpha, hue, saturation, lightness);
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
            return $"a{A}, h{Hue}, s{Saturation}, l{Lightness} (r{R}, g{G}, b{B})";
        }
    }
}
