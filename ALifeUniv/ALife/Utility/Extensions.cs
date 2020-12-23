using Windows.UI;

namespace ALifeUni.ALife.Utility
{
    static class Extensions
    {
        public static Color Clone(this Color c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}
