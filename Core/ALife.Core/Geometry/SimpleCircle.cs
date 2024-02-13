using System.Text.Json.Serialization;
using ALife.Core.NewGeometry;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// A simple circle, just a point and a radius
    /// NOTE: this is meant to be used purely for collision detection.
    /// </summary>
    public struct SimpleCircle
    {
        /// <summary>
        /// The centre
        /// </summary>
        [JsonPropertyName("centre")]
        public Point Centre;

        /// <summary>
        /// The radius
        /// </summary>
        [JsonPropertyName("radius")]
        public double Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCircle"/> struct.
        /// </summary>
        /// <param name="centre">The centre.</param>
        /// <param name="radius">The radius.</param>
        [JsonConstructor]
        public SimpleCircle(Point centre, double radius)
        {
            Centre = centre;
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCircle"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        public SimpleCircle(double x, double y, double radius)
        {
            Centre = new Point(x, y);
            Radius = radius;
        }
    }
}
