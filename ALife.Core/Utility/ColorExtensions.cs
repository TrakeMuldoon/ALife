using System.Drawing;

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
            byte r = Planet.World.NumberGen.NextByte(100, 255);
            byte g = Planet.World.NumberGen.NextByte(100, 255);
            byte b = Planet.World.NumberGen.NextByte(100, 255);

            Color color = Color.FromArgb(255, r, g, b);
            return color;
        }
    }
}
