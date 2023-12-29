using System.Drawing;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Extensions for the Colour class.
    /// </summary>
    public static class ColourExtensions
    {
        /// <summary>
        /// Converts to alifecolor.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The colour.</returns>
        public static Colour ToALifeColor(this Color color)
        {
            return new Colour(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts to systemcolor.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The color.</returns>
        public static Color ToSystemColor(this Colour colour)
        {
            return Color.FromArgb(colour.Alpha, colour.Red, colour.Green, colour.Blue);
        }
    }
}
