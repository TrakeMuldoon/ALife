using System;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Helper methods for dealing with colours.
    /// </summary>
    public static class ColourHelpers
    {
        /// <summary>
        /// Hex code text we should ignore.
        /// </summary>
        private static readonly string[] HEX_REPLACEMENTS = { "#", "0x", "0X", " " };

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

            red = (byte)ExtraMaths.Clamp(r, 0, 255);
            green = (byte)ExtraMaths.Clamp(g, 0, 255);
            blue = (byte)ExtraMaths.Clamp(b, 0, 255);
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
            ConvertHexToRgb(hex, out var red, out var green, out var blue);
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
            ConvertHexToRgb(hex, out var red, out var green, out var blue);
            ConvertRgbToHsv(red, green, blue, out hue, out saturation, out value);
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
            double actualHue = ExtraMaths.CircularClamp(hue, 0, 360) / 360d;
            double actualSaturation = ExtraMaths.Clamp(saturation, 0, 1);
            double actualLightness = ExtraMaths.Clamp(lightness, 0, 1);

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
            double actualHue = ExtraMaths.CircularClamp(hue, 0, 360);
            double actualSaturation = ExtraMaths.Clamp(saturation, 0, 1);
            double actualValue = ExtraMaths.Clamp(value, 0, 1);

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

            double min = ExtraMaths.Minimum(r, g, b);
            double max = ExtraMaths.Maximum(r, g, b);
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

            double min = ExtraMaths.Minimum(r, g, b);
            double max = ExtraMaths.Maximum(r, g, b);
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
    }
}
