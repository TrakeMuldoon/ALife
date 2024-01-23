using System.Drawing;
using ALife.Core.Utility.Colours;

namespace ALife.Rendering
{
    /// <summary>
    /// Extensions to Colours.
    /// </summary>
    public class ColourExtensions
    {
        /// <summary>
        /// Converts an IColour to a System.Drawing.Color.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>The Color.</returns>
        public static Color ToColor(IColour colour)
        {
            return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
        }
    }
}
