using ALife.Core.Utility.Colours;
using System.Drawing;

namespace ALife.Rendering
{
    /// <summary>
    /// Extensions to Colours.
    /// </summary>
    public static class ColourExtensions
    {
        /// <summary>
        /// Converts an IColour to a System.Drawing.Color.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The Color.</returns>
        public static Color ToColor(this IColour colour)
        {
            return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
        }
    }
}
