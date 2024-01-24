using System.Runtime.CompilerServices;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Various extension methods for the IColour interface.
    /// </summary>
    public static class ColourExtensions
    {
        /// <summary>
        /// Converts the current IColour object to a Colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The Colour object.</returns>
        public static Colour ToColour(this IColour colour)
        {
            return new Colour(colour.A, colour.R, colour.G, colour.B, colour.WasPredefined);
        }

        /// <summary>
        /// Converts the current IColour object to an HslColour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The HslColour object.</returns>
        public static HslColour ToHslColour(this IColour colour)
        {
            ColourHelpers.ConvertRgbToHsl(colour.R, colour.G, colour.B, out var h, out var s, out var l);
            return new HslColour(colour.A, h, s, l, colour.WasPredefined);
        }

        /// <summary>
        /// Converts the current IColour object to an HsvColour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The HsvColour object.</returns>
        public static HsvColour ToHsvColour(this IColour colour)
        {
            ColourHelpers.ConvertRgbToHsv(colour.R, colour.G, colour.B, out var h, out var s, out var v);
            return new HsvColour(colour.A, h, s, v, colour.WasPredefined);
        }

        /// <summary>
        /// Converts the current IColour object to a TslColour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The TslColour object.</returns>
        public static TslColour ToTslColour(this IColour colour)
        {
            ColourHelpers.ConvertRgbToTsl(colour.R, colour.G, colour.B, out var t, out var s, out var l);
            return new TslColour(colour.A, t, s, l, colour.WasPredefined);
        }

        /// <summary>
        /// Converts the current IColour object to the hexadecimal representation of the colour.
        /// </summary>
        /// <param name="colour"></param>
        /// <returns>The hexadecimal representation of the colour.</returns>
        public static string ToHexadecimal(this IColour colour)
        {
            string output = string.Format("{0:X2}{1:X2}{2:X2}", colour.R, colour.G, colour.B);
            return output;
        }
    }
}
