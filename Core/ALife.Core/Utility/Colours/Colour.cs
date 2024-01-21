using ALife.Core.Utility.Ranges;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an RGBA colour for the ALife simulation.
    /// </summary>
    [DebuggerDisplay("r{Red}, g{Green}, b{Blue}, a{Alpha}")]
    public struct Colour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public Colour(byte alpha, byte red, byte green, byte blue)
        {
            A = alpha;
            R = red;
            G = green;
            B = blue;
            PreDefined = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public Colour(byte red, byte green, byte blue) : this(255, red, green, blue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="alpha">The alpha.</param>
        public Colour(Colour parent, byte alpha) : this(alpha, parent.R, parent.G, parent.B)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public Colour(Colour parent) : this(parent.A, parent.R, parent.G, parent.B)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public Colour(byte alpha, int red, int green, int blue) : this(alpha, IntToByte(red), IntToByte(green), IntToByte(blue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public Colour(byte alpha, double red, double green, double blue) : this(alpha, DoubleToByte(red), DoubleToByte(green), DoubleToByte(blue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <param name="preDefined">if set to <c>true</c> [pre defined].</param>
        [JsonConstructor]
        private Colour(byte alpha, byte red, byte green, byte blue, bool preDefined) : this(alpha, red, green, blue)
        {
            PreDefined = preDefined;
        }

        /// <summary>
        /// Gets or sets the alpha channel.
        /// </summary>
        /// <value>The alpha.</value>
        public byte A { get; }

        /// <summary>
        /// Gets or sets the blue channel.
        /// </summary>
        /// <value>The blue.</value>
        public byte B { get; }

        /// <summary>
        /// Gets or sets the green channel.
        /// </summary>
        /// <value>The green.</value>
        public byte G { get; }

        /// <summary>
        /// Gets a value indicating whether the colour was a [pre defined] colour.
        /// </summary>
        /// <value><c>true</c> if [pre defined]; otherwise, <c>false</c>.</value>
        public bool PreDefined { get; private set; }

        /// <summary>
        /// Gets or sets the red channel.
        /// </summary>
        /// <value>The red.</value>
        public byte R { get; }

        /// <summary>
        /// Generates a colour from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns>The colour.</returns>
        public static Colour FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            return new Colour(alpha, red, green, blue);
        }

        /// <summary>
        /// Froms the HSL.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns></returns>
        public static Colour FromHSL(int hue, double saturation, double lightness)
        {
            return FromAHSL(255, hue, saturation, lightness);
        }

        /// <summary>
        /// Froms the HSV.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Colour FromHSV(int hue, double saturation, double value)
        {
            return FromAHSV(255, hue, saturation, value);
        }

        /// <summary>
        /// Generates a random colour.
        /// </summary>
        /// <param name="sim">The sim.</param>
        /// <returns>The randomized colour.</returns>
        public static Colour GetRandomColour(Simulation sim)
        {
            return GetRandomColour(sim, null, null, null, null);
        }

        /// <summary>
        /// Generates a random colour.
        /// </summary>
        /// <param name="sim">The sim.</param>
        /// <param name="redRange">The red range.</param>
        /// <param name="blueRange">The blue range.</param>
        /// <param name="greenRange">The green range.</param>
        /// <param name="alphaRange">The alpha range.</param>
        /// <returns>The randomized colour.</returns>
        public static Colour GetRandomColour(Simulation sim, Nullable<ByteRange> redRange, Nullable<ByteRange> greenRange, Nullable<ByteRange> blueRange, Nullable<ByteRange> alphaRange)
        {
            ByteRange red = redRange ?? new ByteRange(100, 255);
            ByteRange blue = blueRange ?? new ByteRange(100, 255);
            ByteRange green = greenRange ?? new ByteRange(100, 255);

            byte a = alphaRange == null ? (byte)255 : sim.Random.NextByte(alphaRange.Value.Minimum, alphaRange.Value.Maximum);
            byte r = sim.Random.NextByte(red.Minimum, red.Maximum);
            byte g = sim.Random.NextByte(blue.Minimum, blue.Maximum);
            byte b = sim.Random.NextByte(green.Minimum, green.Maximum);

            Colour colour = new Colour(a, r, g, b);
            return colour;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Colour"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color(Colour colour)
        {
            return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Color"/> to <see cref="Colour"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Colour(Color color)
        {
            return new Colour(color.A, color.R, color.G, color.B);
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
        public Colour Clone()
        {
            return new Colour(this);
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
            if(obj is Colour)
            {
                Colour other = (Colour)obj;
                return other.A == A && other.R == R && other.G == G && other.B == B;
            }
            return false;
        }

        /// <summary>
        /// Gets the AHSL representation of this colour.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public void GetAHSL(out byte alpha, out int hue, out double saturation, out double lightness)
        {
            alpha = A;

            double r = R / 255d;
            double g = G / 255d;
            double b = B / 255d;

            double min = ExtraMath.Minimum(r, g, b);
            double max = ExtraMath.Maximum(r, g, b);
            double delta = max - min;
            double minMaxSum = min + max;
            lightness = minMaxSum / 2d;
            if(lightness <= 0)
            {
                hue = 0;
                saturation = 0;
                lightness = 0;
                return;
            }

            double h = 0;
            if(delta == 0)
            {
                saturation = 0;
            }
            else
            {
                if(lightness < 0.5)
                {
                    saturation = delta / minMaxSum;
                }
                else
                {
                    saturation = delta / (2 - delta);
                }

                if(r == max)
                {
                    h = (g - b) / 6 / delta;
                }
                else if(g == max)
                {
                    h = (1 / 3d) + ((b - r) / 6 / delta);
                }
                else
                {
                    h = (2 / 3d) + ((r - g) / 6 / delta);
                }

                if(h < 0)
                {
                    h += 1;
                }
                if(h > 1)
                {
                    h -= 1;
                }
            }

            hue = (int)Math.Round(h * 360);
        }

        /// <summary>
        /// Gets the AHSV representation of this colour.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        public void GetAHSV(out byte alpha, out int hue, out double saturation, out double value)
        {
            alpha = A;

            double r = R / 255d;
            double g = G / 255d;
            double b = B / 255d;

            double min = ExtraMath.Minimum(r, g, b);
            double max = ExtraMath.Maximum(r, g, b);
            double delta = max - min;
            value = max;

            double h = 0;
            if(max == 0)
            {
                saturation = 0;
            }
            else
            {
                saturation = delta / max;

                if(r == max)
                {
                    h = (g - b) / delta;
                }
                else if(g == max)
                {
                    h = 2 + (b - r) / delta;
                }
                else
                {
                    h = 4 + (r - g) / delta;
                }

                h *= 60;
                if(h < 0)
                {
                    h += 360;
                }
            }

            hue = (int)Math.Round(h);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"r{R}, g{G}, b{B}, a{A}";
        }

        /// <summary>
        /// Generates a colour as a pre-defined colour.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns></returns>
        internal static Colour PredefineColour(byte alpha, byte red, byte green, byte blue)
        {
            return new Colour(alpha, red, green, blue, true);
        }

        /// <summary>
        /// Doubles to byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">value</exception>
        private static byte DoubleToByte(double value)
        {
            if(value < 0)
            {
                return byte.MinValue;
            }
            if(value > 1)
            {
                return byte.MaxValue;
            }
            byte output = (byte)Math.Round(value * byte.MaxValue);
            return output;
        }

        /// <summary>
        /// Gets the HSL colour component.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="q">The q.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        private static double GetHslColourComponent(double p, double q, double t)
        {
            double actualT = t;
            if(actualT < 0)
            {
                actualT += 1;
            }
            if(actualT > 1)
            {
                actualT -= 1;
            }
            if(actualT < 1d / 6)
            {
                return p + (q - p) * 6 * actualT;
            }
            if(actualT < 1d / 2)
            {
                return q;
            }
            if(actualT < 2d / 3)
            {
                return p + (q - p) * (2d / 3 - actualT) * 6;
            }
            return p;
        }

        /// <summary>
        /// Ints to byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The byte representation of the value.</returns>
        private static byte IntToByte(int value)
        {
            if(value < byte.MinValue)
            {
                return byte.MinValue;
            }
            if(value > byte.MaxValue)
            {
                return byte.MaxValue;
            }
            return (byte)value;
        }
    }
}