using System.Numerics;

namespace ALife.Core.Utility.Points
{
    /// <summary>
    /// Extensions for the Point struct.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Converts a point to a Vector2.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>The Vector2</returns>
        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2((float)p.X, (float)p.Y);
        }
    }
}
