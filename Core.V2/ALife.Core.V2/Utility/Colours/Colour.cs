using ALife.Core.Utility.Maths;
using ALife.Core.Utility.Ranges;
using System;
using System.Diagnostics;
using System.Drawing;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines an RGBA colour for the ALife simulation.
    /// </summary>
    /// <seealso cref="ALife.Core.BaseObject"/>
    [DebuggerDisplay("r{Red}, g{Green}, b{Blue}, a{Alpha}")]
    public class Colour
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
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
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
        public Colour(Colour parent, byte alpha) : this(alpha, parent.Red, parent.Green, parent.Blue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public Colour(Colour parent) : this(parent.Alpha, parent.Red, parent.Green, parent.Blue)
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
        /// Gets or sets the alpha channel.
        /// </summary>
        /// <value>The alpha.</value>
        public byte Alpha { get; set; }

        /// <summary>
        /// Gets or sets the blue channel.
        /// </summary>
        /// <value>The blue.</value>
        public byte Blue { get; set; }

        /// <summary>
        /// Gets or sets the green channel.
        /// </summary>
        /// <value>The green.</value>
        public byte Green { get; set; }

        /// <summary>
        /// Gets or sets the red channel.
        /// </summary>
        /// <value>The red.</value>
        public byte Red { get; set; }

        /// <summary>
        /// Generates a number from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The colour.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">hue or saturation or lightness</exception>
        public static Colour FromAHSL(byte alpha, int hue, double saturation, double lightness)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            double actualHue = ExtraMath.CircularClamp(hue, 0, 360) / 360d;
            double actualSaturation = ExtraMath.Clamp(saturation, 0, 1);
            double actualLightness = ExtraMath.Clamp(lightness, 0, 1);

            if(actualSaturation == 0)
            {
                r = 1;
                g = 1;
                b = 1;
            }
            else
            {
                double q = 0;
                if(q < 0.5d)
                {
                    q = actualLightness * (1 + actualSaturation);
                }
                else
                {
                    q = actualLightness + actualSaturation - actualLightness * actualSaturation;
                }

                double p = 2 * actualLightness - q;
                double third = 1 / 3d;
                r = GetHslColourComponent(p, q, actualHue + third);
                g = GetHslColourComponent(p, q, actualHue);
                b = GetHslColourComponent(p, q, actualHue - third);
            }

            byte red = DoubleToByte(r);
            byte green = DoubleToByte(g);
            byte blue = DoubleToByte(b);

            return new Colour(alpha, red, green, blue);
        }

        /// <summary>
        /// Generates a colour from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The colour.</returns>
        public static Colour FromAHSV(byte alpha, int hue, double saturation, double value)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            double actualHue = ExtraMath.CircularClamp(hue, 0, 360);
            double actualSaturation = ExtraMath.Clamp(saturation, 0, 1);
            double actualValue = ExtraMath.Clamp(value, 0, 1);

            if(actualSaturation == 0)
            {
                r = actualValue;
                g = actualValue;
                b = actualValue;
            }
            else if(actualValue > 0)
            {
                double hf = actualHue / 60d;
                int i = (int)hf;
                double f = hf - i;
                double pv = actualValue * (1 - actualSaturation);
                double qv = actualValue * (1 - actualSaturation * f);
                double tv = actualValue * (1 - actualSaturation * (1 - f));
                switch(i)
                {
                    // Red is dominant
                    case 0:
                        r = actualValue;
                        g = tv;
                        b = pv;
                        break;

                    case 5:
                        r = actualValue;
                        g = pv;
                        b = qv;
                        break;

                    // Green is dominant
                    case 1:
                        r = qv;
                        g = actualValue;
                        b = pv;
                        break;

                    case 2:
                        r = pv;
                        g = actualValue;
                        b = tv;
                        break;

                    // Blue is dominant
                    case 3:
                        r = pv;
                        g = qv;
                        b = actualValue;
                        break;

                    case 4:
                        r = tv;
                        g = pv;
                        b = actualValue;
                        break;

                    // Boundary Protection
                    case 6:
                        r = actualValue;
                        g = tv;
                        b = pv;
                        break;

                    case -1:
                        r = actualValue;
                        g = pv;
                        b = qv;
                        break;

                    default:
                        r = actualValue;
                        g = actualValue;
                        b = actualValue;
                        break;
                }
            }

            byte red = DoubleToByte(r);
            byte green = DoubleToByte(g);
            byte blue = DoubleToByte(b);

            return new Colour(alpha, red, green, blue);
        }

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
            return Color.FromArgb(colour.Alpha, colour.Red, colour.Green, colour.Blue);
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
                return other.Alpha == Alpha && other.Red == Red && other.Green == Green && other.Blue == Blue;
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
            alpha = Alpha;

            double r = Red / 255d;
            double g = Green / 255d;
            double b = Blue / 255d;

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
            alpha = Alpha;

            double r = Red / 255d;
            double g = Green / 255d;
            double b = Blue / 255d;

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
            return Alpha.GetHashCode() ^ Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"r{Red}, g{Green}, b{Blue}, a{Alpha}";
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
