using ALife.Core.Utility.Extensions;
using ALife.Core.Utility.Maths;

namespace ALife.Core.WorldInfoObjects.Colours
{
    /// <summary>
    /// Helper methods for dealing with colours.
    /// </summary>
    public static class ColourHelpers
    {
        /// <summary>
        /// The precision for equality operations
        /// </summary>
        private const double EQUALITY_PRECISION = 0.0001;
        
        /// <summary>
        /// Hex code text we should ignore.
        /// </summary>
        private static readonly string[] HEX_REPLACEMENTS = { "#", "0x", "0X", " " };

        /// <summary>
        /// The R modifier for the TSL lightness.
        /// </summary>
        private const double TSL_LIGHTNESS_R_COMPONENT = 0.299d;

        /// <summary>
        /// The R modifier for the TSL lightness when converting back to RGB.
        /// </summary>
        private const double TSL_LIGHTNESS_R_RGB_COMPONENT = 0.185d;

        /// <summary>
        /// The G modifier for the TSL lightness.
        /// </summary>
        private const double TSL_LIGHTNESS_G_COMPONENT = 0.587d;

        /// <summary>
        /// The G modifier for the TSL lightness when converting back to RGB.
        /// </summary>
        private const double TSL_LIGHTNESS_G_RGB_COMPONENT = 0.473d;

        /// <summary>
        /// The B modifier for the TSL lightness.
        /// </summary>
        private const double TSL_LIGHTNESS_B_COMPONENT = 0.114d;

        /// <summary>
        /// Converts the hexadecimal value to RGB values.
        /// </summary>
        /// <param name="hex">The hexidecimal value.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        public static void ConvertHexToRgb(string hex, out byte red, out byte green, out byte blue)
        {
            string cleanedHexCode = hex.ReplaceAny(string.Empty, HEX_REPLACEMENTS);
            int hexColor = Convert.ToInt32(cleanedHexCode, 16);
            int r = (hexColor >> 16) & 0xFF;
            int g = (hexColor >> 8) & 0xFF;
            int b = hexColor & 0xFF;

            red = (byte)ExtraMath<int>.Clamp(r, 0, 255);
            green = (byte)ExtraMath<int>.Clamp(g, 0, 255);
            blue = (byte)ExtraMath<int>.Clamp(b, 0, 255);
        }

        /// <summary>
        /// Converts the hexadecimal value to HSL values.
        /// </summary>
        /// <param name="hex">The hexidecimal value.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public static void ConvertHexToHsl(string hex, out int hue, out double saturation, out double lightness)
        {
            ConvertHexToRgb(hex, out byte red, out byte green, out byte blue);
            ConvertRgbToHsl(red, green, blue, out hue, out saturation, out lightness);
        }

        /// <summary>
        /// Converts the hexadecimal value to HSV values.
        /// </summary>
        /// <param name="hex">The hexidecimal value.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        public static void ConvertHexToHsv(string hex, out int hue, out double saturation, out double value)
        {
            ConvertHexToRgb(hex, out byte red, out byte green, out byte blue);
            ConvertRgbToHsv(red, green, blue, out hue, out saturation, out value);
        }

        /// <summary>
        /// Converts the hexadecimal value to TSL values.
        /// </summary>
        /// <param name="hex">The hexidecimal value.</param>
        /// <param name="tint">The tint.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public static void ConvertHexToTsl(string hex, out double tint, out double saturation, out double lightness)
        {
            ConvertHexToRgb(hex, out byte red, out byte green, out byte blue);
            ConvertRgbToTsl(red, green, blue, out tint, out saturation, out lightness);
        }

        /// <summary>
        /// Converts the HSL values to RGB values.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        public static void ConvertHslToRgb(int hue, double saturation, double lightness, out byte red, out byte green, out byte blue)
        {
            double actualHue = ExtraMath<int>.CircularClamp(hue, 0, 360) / 360d;
            double actualSaturation = ExtraMath<double>.Clamp(saturation, 0, 1);
            double actualLightness = ExtraMath<double>.Clamp(lightness, 0, 1);

            double r = 0;
            double g = 0;
            double b = 0;
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

            red = DoubleToByte(r);
            green = DoubleToByte(g);
            blue = DoubleToByte(b);
        }

        /// <summary>
        /// Converts the HSV values to RGB values.
        /// </summary>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        public static void ConvertHsvToRgb(int hue, double saturation, double value, out byte red, out byte green, out byte blue)
        {
            double actualHue = ExtraMath<int>.CircularClamp(hue, 0, 360);
            double actualSaturation = ExtraMath<double>.Clamp(saturation, 0, 1);
            double actualValue = ExtraMath<double>.Clamp(value, 0, 1);

            double r = 0;
            double g = 0;
            double b = 0;
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

            red = DoubleToByte(r);
            green = DoubleToByte(g);
            blue = DoubleToByte(b);
        }

        /// <summary>
        /// Converts the RGB values to HSL values.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public static void ConvertRgbToHsl(byte red, byte green, byte blue, out int hue, out double saturation, out double lightness)
        {
            double r = red / 255d;
            double g = green / 255d;
            double b = blue / 255d;

            double min = ExtraMath<double>.Minimum(r, g, b);
            double max = ExtraMath<double>.Maximum(r, g, b);
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

                if(Math.Abs(r - max) < EQUALITY_PRECISION)
                {
                    h = (g - b) / 6 / delta;
                }
                else if(Math.Abs(g - max) < EQUALITY_PRECISION)
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
        /// Converts the RGB values to HSV values.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        public static void ConvertRgbToHsv(byte red, byte green, byte blue, out int hue, out double saturation, out double value)
        {
            double r = red / 255d;
            double g = green / 255d;
            double b = blue / 255d;

            double min = ExtraMath<double>.Minimum(r, g, b);
            double max = ExtraMath<double>.Maximum(r, g, b);
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

                if(Math.Abs(r - max) < EQUALITY_PRECISION)
                {
                    h = (g - b) / delta;
                }
                else if(Math.Abs(g - max) < EQUALITY_PRECISION)
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
        /// Converts RGB values to TSL values.
        /// </summary>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        /// <param name="tint">The tint.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        public static void ConvertRgbToTsl(byte red, byte green, byte blue, out double tint, out double saturation, out double lightness)
        {
            if(red + green + blue == 0)
            {
                tint = 0;
                saturation = 0;
                lightness = 0;
                return;
            }
            double redPercentage = red / 255d;
            double greenPercentage = green / 255d;
            double bluePercentage = blue / 255d;
            double totalPercentage = redPercentage + greenPercentage + bluePercentage;

            double r = redPercentage / totalPercentage;
            double g = greenPercentage / totalPercentage;
            double rPrime = r - (1 / 3d);
            double rPrimeSquared = rPrime * rPrime;
            double gPrime = g - (1 / 3d);
            double gPrimeSquared = gPrime * gPrime;

            // Calculate the tint
            if(ApproximatelyEqual(gPrime, 0, EQUALITY_PRECISION))
            {
                tint = 0;
            }
            else
            {
                tint = 0.5 - (Math.Atan2(gPrime, rPrime) / (2 * Math.PI));
            }

            // Calculate the saturation
            saturation = Math.Sqrt((9 / 5d) * (rPrimeSquared + gPrimeSquared));

            // Calculate the lightness
            double redComponent = TSL_LIGHTNESS_R_COMPONENT * redPercentage;
            double greenComponent = TSL_LIGHTNESS_G_COMPONENT * greenPercentage;
            double blueComponent = TSL_LIGHTNESS_B_COMPONENT * bluePercentage;
            lightness = redComponent + greenComponent + blueComponent;
        }

        /// <summary>
        /// Converts TSL values to RGB values.
        /// Note: https://stackoverflow.com/a/43712296 (the formula on Wikipedia is wrong)
        /// </summary>
        /// <param name="tint">The tint.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <param name="red">The red channel.</param>
        /// <param name="green">The green channel.</param>
        /// <param name="blue">The blue channel.</param>
        public static void ConvertTslToRgb(double tint, double saturation, double lightness, out byte red, out byte green, out byte blue)
        {
            if(ApproximatelyEqual(lightness, 0, EQUALITY_PRECISION))
            {
                red = 0;
                green = 0;
                blue = 0;
                return;
            }

            double rPrime = 0;
            double gPrime = 0;
            if(IsNegativeZero(tint))
            {
                rPrime = -Math.Sqrt(5d) / 3d * saturation;
            }
            else if(ApproximatelyEqual(tint, 0, EQUALITY_PRECISION))
            {
                rPrime = Math.Sqrt(5d) / 3d * saturation;
            }
            else
            {
                double x = -1d / Math.Tan(2 * Math.PI * tint);
                gPrime = Math.Sqrt(5d / (1 + x * x)) / 3d * saturation;
                if(tint > 0.5)
                {
                    gPrime = -gPrime;
                }
                rPrime = x * gPrime;
            }

            double r = rPrime + 1 / 3d;
            double g = gPrime + 1 / 3d;
            double b = 1 - r - g;
            double kRed = TSL_LIGHTNESS_R_RGB_COMPONENT * r;
            double kGreen = TSL_LIGHTNESS_G_RGB_COMPONENT * g;
            double k = lightness / (kRed + kGreen + TSL_LIGHTNESS_B_COMPONENT);

            double redDouble = k * r;
            double greenDouble = k * g;
            double blueDouble = k * b;

            red = DoubleToByte(redDouble);
            green = DoubleToByte(greenDouble);
            blue = DoubleToByte(blueDouble);
        }

        /// <summary>
        /// Determines if the two numbers are approximately equal.
        /// </summary>
        /// <param name="a">Number a.</param>
        /// <param name="b">Number b.</param>
        /// <param name="maxDifference">The maximum difference.</param>
        /// <returns>True if approximately equal, False otherwise.</returns>
        private static bool ApproximatelyEqual(double a, double b, double maxDifference)
        {
            double difference = Math.Abs(a - b);
            return difference <= maxDifference;
        }

        /// <summary>
        /// Converts a double on a 0 to 1 range to the byte representation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The byte representation of the value.</returns>
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
        /// <returns>The value of the HSL component.</returns>
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
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool IsNegativeZero(double x)
        {
            return ApproximatelyEqual(x, 0, EQUALITY_PRECISION) && double.IsNegativeInfinity(1d / x);
        }
    }
}
