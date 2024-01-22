using System.Numerics;

namespace ALife.Core.Geometry
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
        public static Point ToPoint(this Vector2 p)
        {
            return new Point(p.X, p.Y);
        }
    }
}
