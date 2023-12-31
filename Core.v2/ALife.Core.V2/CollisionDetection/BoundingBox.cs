using System.Text.Json.Serialization;

namespace ALife.Core.Geometry
{
    public struct BoundingBox
    {
        /// <summary>
        /// The height
        /// </summary>
        public double Height;

        /// <summary>
        /// The width
        /// </summary>
        public double Width;

        /// <summary>
        /// The x
        /// </summary>
        public double X;

        /// <summary>
        /// The y
        /// </summary>
        public double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="angle">The angle. Defaults to 0 degrees.</param>
        [JsonConstructor]
        public BoundingBox(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the centre point of the bounding box.
        /// </summary>
        /// <value>The centre point.</value>
        [JsonIgnore]
        public Point CentrePoint => new Point(X + Width / 2, Y + Height / 2);

        /// <summary>
        /// Gets the maximum x.
        /// </summary>
        /// <value>The maximum x.</value>
        [JsonIgnore]
        public double MaxX => X + Width;

        /// <summary>
        /// Gets the maximum y.
        /// </summary>
        /// <value>The maximum y.</value>
        [JsonIgnore]
        public double MaxY => Y + Height;

        /// <summary>
        /// Gets the minimum x.
        /// </summary>
        /// <value>The minimum x.</value>
        [JsonIgnore]
        public double MinX => X;

        /// <summary>
        /// Gets the minimum y.
        /// </summary>
        /// <value>The minimum y.</value>
        [JsonIgnore]
        public double MinY => Y;

        /// <summary>
        /// Gets the length of the x.
        /// </summary>
        /// <value>The length of the x.</value>
        [JsonIgnore]
        public double XLength => Width;

        /// <summary>
        /// Gets the height of the y.
        /// </summary>
        /// <value>The height of the y.</value>
        [JsonIgnore]
        public double YHeight => Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct using classic initializers.
        /// </summary>
        /// <param name="minX">The minimum x.</param>
        /// <param name="minY">The minimum y.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="maxY">The maximum y.</param>
        /// <returns>A new BoundingBox.</returns>
        public static BoundingBox FromClassicInitializers(double minX, double minY, double maxX, double maxY)
        {
            double width = maxX - minX;
            double height = maxY - minY;

            return new BoundingBox(minX, minY, width, height);
        }
    }
}
