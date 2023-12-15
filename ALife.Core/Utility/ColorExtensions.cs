using Windows.UI;
using ALife.Core;

namespace ALife.Core.Utility
{
    static class ColorExtensions
    {
        public static Color Clone(this Color c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        public static Color GetRandomColor()
        {
            Color color = new Color()
            {
                R = (byte)Planet.World.NumberGen.Next(100, 255),
                G = (byte)Planet.World.NumberGen.Next(100, 255),
                B = (byte)Planet.World.NumberGen.Next(100, 255),
                A = 255
            };
            return color;
        }
    }
}
