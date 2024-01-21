using System;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Internal Colour helper methods.
    /// </summary>
    internal static class ColourHelpers
    {
        /// <summary>
        /// Converts a double on a 0 to 1 range to the byte representation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The byte representation of the value.</returns>
        internal static byte DoubleToByte(double value)
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
        /// Converts an int on a 0 to 255 range to the byte representation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The byte representation of the value.</returns>
        internal static byte IntToByte(int value)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        internal static void ConvertHslToRgb(int hue, double saturation, double lightness, out byte r, out byte g, out byte b)
        {
            double red = 0;
            double green = 0;
            double blue = 0;

            double actualHue = ExtraMaths.CircularClamp(hue, 0, 360) / 360d;
            double actualSaturation = ExtraMaths.Clamp(saturation, 0, 1);
            double actualLightness = ExtraMaths.Clamp(lightness, 0, 1);

            if(actualSaturation == 0)
            {
                red = 1;
                green = 1;
                blue = 1;
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
                red = GetHslColourComponent(p, q, actualHue + third);
                green = GetHslColourComponent(p, q, actualHue);
                blue = GetHslColourComponent(p, q, actualHue - third);
            }

            r = DoubleToByte(red);
            g = DoubleToByte(green);
            b = DoubleToByte(blue);
        }

        internal static void ConvertHsvToRgb(int hue, double saturation, double value, out byte r, out byte g, out byte v)
        {
            double red = 0;
            double green = 0;
            double blue = 0;

            double actualHue = ExtraMaths.CircularClamp(hue, 0, 360);
            double actualSaturation = ExtraMaths.Clamp(saturation, 0, 1);
            double actualValue = ExtraMaths.Clamp(value, 0, 1);

            if(actualSaturation == 0)
            {
                red = actualValue;
                green = actualValue;
                blue = actualValue;
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
                        red = actualValue;
                        green = tv;
                        blue = pv;
                        break;

                    case 5:
                        red = actualValue;
                        green = pv;
                        blue = qv;
                        break;

                    // Green is dominant
                    case 1:
                        red = qv;
                        green = actualValue;
                        blue = pv;
                        break;

                    case 2:
                        red = pv;
                        green = actualValue;
                        blue = tv;
                        break;

                    // Blue is dominant
                    case 3:
                        red = pv;
                        green = qv;
                        blue = actualValue;
                        break;

                    case 4:
                        red = tv;
                        green = pv;
                        blue = actualValue;
                        break;

                    // Boundary Protection
                    case 6:
                        red = actualValue;
                        green = tv;
                        blue = pv;
                        break;

                    case -1:
                        red = actualValue;
                        green = pv;
                        blue = qv;
                        break;

                    default:
                        red = actualValue;
                        green = actualValue;
                        blue = actualValue;
                        break;
                }
            }

            r = DoubleToByte(red);
            g = DoubleToByte(green);
            b = DoubleToByte(blue);
        }

        /// <summary>
        /// Converts RGB colour values to HSL colour values.
        /// </summary>
        /// <param name="r">Red Channel.</param>
        /// <param name="g">Green Channel.</param>
        /// <param name="b">Blue Channel.</param>
        /// <param name="h">Hue.</param>
        /// <param name="s">Saturation.</param>
        /// <param name="l">Lightness.</param>
        internal static void ConvertRgbToHsl(byte r, byte g, byte b, out int h, out double s, out double l)
        {
            double rd = r / 255d;
            double gd = g / 255d;
            double bd = b / 255d;

            double min = ExtraMaths.Minimum(rd, gd, bd);
            double max = ExtraMaths.Maximum(rd, gd, bd);
            double delta = max - min;
            double minMaxSum = min + max;
            l = minMaxSum / 2d;
            if(l <= 0)
            {
                h = 0;
                s = 0;
                l = 0;
                return;
            }

            double hD = 0;
            if(delta == 0)
            {
                s = 0;
            }
            else
            {
                if(l < 0.5)
                {
                    s = delta / minMaxSum;
                }
                else
                {
                    s = delta / (2 - delta);
                }

                double deltaR = (((max - rd) / 6d) + (delta / 2d)) / delta;
                double deltaG = (((max - gd) / 6d) + (delta / 2d)) / delta;
                double deltaB = (((max - bd) / 6d) + (delta / 2d)) / delta;

                if(rd == max)
                {
                    hD = deltaB - deltaG;
                }
                else if(gd == max)
                {
                    hD = (1d / 3d) + deltaR - deltaB;
                }
                else if(bd == max)
                {
                    hD = (2d / 3d) + deltaG - deltaR;
                }

                if(hD < 0)
                {
                    hD += 1;
                }
                if(hD > 1)
                {
                    hD -= 1;
                }
            }

            h = (int)Math.Round(hD * 360d);
            s = Math.Round(s, 3);
            l = Math.Round(l, 3);
        }

        internal static void ConvertRgbToHsv(byte r, byte g, byte b, out int h, out double s, out double v)
        {

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
    }
}
