using System.Text.Json.Serialization;
using ALife.Core.Geometry;
using ALife.Core.Utility;

namespace ALife.Core.CollisionDetection
{
    /// <summary>
    /// Defines a bounding box (axis aligned or not).
    /// </summary>
    public struct BoundingBox
    {
        /// <summary>
        /// The bottom left coordinates.
        /// </summary>
        private Point _bottomLeft;

        /// <summary>
        /// The bottom right coordinates.
        /// </summary>
        private Point _bottomRight;

        /// <summary>
        /// The centre point coordinates.
        /// </summary>
        private Point _centrePoint;

        /// <summary>
        /// The height.
        /// </summary>
        private double _height;

        /// <summary>
        /// The maximum x coordinate.
        /// </summary>
        private double _maxX;

        /// <summary>
        /// The maximum y coordinate.
        /// </summary>
        private double _maxY;

        /// <summary>
        /// The top left coordinates.
        /// </summary>
        private Point _topLeft;

        /// <summary>
        /// The top right coordinates.
        /// </summary>
        private Point _topRight;

        /// <summary>
        /// The width
        /// </summary>
        private double _width;

        /// <summary>
        /// The x coordinate (minx).
        /// </summary>
        private double _x;

        /// <summary>
        /// The y coordinate (miny).
        /// </summary>
        private double _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="maxY">The maximum y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [JsonConstructor]
        public BoundingBox(double x, double y, double maxX, double maxY, double width, double height)
        {
            _x = x;
            _y = y;
            _maxX = maxX;
            _maxY = maxY;
            _width = width;
            _height = height;

            UpdatePoints();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="maxY">The maximum y.</param>
        private BoundingBox(double x, double y, double maxX, double maxY, bool fake)
        {
            _x = x;
            _y = y;
            _maxX = maxX;
            _maxY = maxY;
            _width = maxX - x;
            _height = maxY - y;
            UpdatePoints();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private BoundingBox(double x, double y, double width, double height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _maxX = x + width;
            _maxY = y + height;
            UpdatePoints();
        }

        /// <summary>
        /// Gets the bottom left coordinates.
        /// </summary>
        /// <value>The bottom left.</value>
        [JsonIgnore]
        public Point BottomLeft => _bottomLeft;

        /// <summary>
        /// Gets the bottom right coordinates.
        /// </summary>
        /// <value>The bottom right.</value>
        [JsonIgnore]
        public Point BottomRight => _bottomRight;

        /// <summary>
        /// Gets the centre point of the bounding box.
        /// </summary>
        /// <value>The centre point.</value>
        [JsonIgnore]
        public Point CentrePoint => _centrePoint;

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        [JsonIgnore]
        public double Height => _height;

        /// <summary>
        /// Gets the maximum x coordinate.
        /// </summary>
        /// <value>The maximum x.</value>
        [JsonIgnore]
        public double MaxX
        {
            get => _maxX;
            set
            {
                _maxX = value;
                _width = _maxX - _x;
                UpdatePoints();
            }
        }

        /// <summary>
        /// Gets the maximum y coordinate.
        /// </summary>
        /// <value>The maximum y.</value>
        [JsonIgnore]
        public double MaxY
        {
            get => _maxY;
            set
            {
                _maxY = value;
                _height = _maxY - _y;
                UpdatePoints();
            }
        }

        /// <summary>
        /// Gets the minimum x coordinate.
        /// </summary>
        /// <value>The minimum x.</value>
        [JsonIgnore]
        public double MinX
        {
            get => _x;
            set
            {
                _x = value;
                _width = _maxX - _x;
                UpdatePoints();
            }
        }

        /// <summary>
        /// Gets the minimum y coordinate.
        /// </summary>
        /// <value>The minimum y.</value>
        [JsonIgnore]
        public double MinY
        {
            get => _y;
            set
            {
                _y = value;
                _height = _maxY - _y;
                UpdatePoints();
            }
        }

        /// <summary>
        /// Gets the top left coordinates.
        /// </summary>
        /// <value>The top left.</value>
        [JsonIgnore]
        public Point TopLeft => _topLeft;

        /// <summary>
        /// Gets the top right coordinates.
        /// </summary>
        /// <value>The top right.</value>
        [JsonIgnore]
        public Point TopRight => _topRight;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        [JsonIgnore]
        public double Width => _width;

        /// <summary>
        /// Gets the length of the x.
        /// </summary>
        /// <value>The length of the x.</value>
        [JsonIgnore]
        public double XLength => _width;

        /// <summary>
        /// Gets the height of the y.
        /// </summary>
        /// <value>The height of the y.</value>
        [JsonIgnore]
        public double YHeight => _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct using axis-aligned initializers.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FromAxisAlignedInitializers(double x, double y, double width, double height)
        {
            return new BoundingBox(x, y, width + x, height + y, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct using non-axis-aligned initializers.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FromOrientedInitializers(double x, double y, double width, double height, Angle orientation)
        {
            Matrix transformationMatrix = Matrix.CreateFromAngle(orientation);

            Point topLeft = new Point(x, y);
            Point bottomRight = new Point(x + width, y + height);

            topLeft.Transform(transformationMatrix);
            bottomRight.Transform(transformationMatrix);

            return FromOrientedInitializers(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct using non-axis-aligned initializers
        /// </summary>
        /// <param name="minX">The minimum x.</param>
        /// <param name="minY">The minimum y.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="maxY">The maximum y.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FromOrientedInitializers(double minX, double minY, double maxX, double maxY)
        {
            return new BoundingBox(minX, minY, maxX, maxY, fake: true);
        }

        /// <summary>
        /// Gets the rotated bounding box.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The resulting bounding box.</returns>
        public BoundingBox GetRotatedBoundingBox(Angle angle)
        {
            BoundingBox result = FromOrientedInitializers(_x, _y, _width, _height, angle);
            return result;
        }

        /// <summary>
        /// Gets the transformed bounding box.
        /// </summary>
        /// <param name="transformation">The transformation.</param>
        /// <returns>The resulting bounding box.</returns>
        public BoundingBox GetTransformedBoundingBox(Matrix transformation)
        {
            Point newTopLeft = Point.FromTransformation(TopLeft, transformation);
            Point newBottomRight = Point.FromTransformation(BottomRight, transformation);
            BoundingBox result = FromOrientedInitializers(newTopLeft.X, newTopLeft.Y, newBottomRight.X, newBottomRight.Y);
            return result;
        }

        /// <summary>
        /// Determines whether the specified interloper is collision.
        /// </summary>
        /// <param name="interloper">The interloper.</param>
        /// <returns><c>true</c> if the specified interloper is collision; otherwise, <c>false</c>.</returns>
        public bool IsCollision(BoundingBox interloper)
        {
            bool result = MinX < interloper.MaxX
                && MaxX > interloper.MinX
                && MinY < interloper.MaxY
                && MaxY > interloper.MinY;

            return result;
        }

        /// <summary>
        /// Updates the points.
        /// </summary>
        private void UpdatePoints()
        {
            _topLeft = new Point(_x, _y);
            _topRight = new Point(_maxX, _y);
            _bottomLeft = new Point(_x, _maxY);
            _bottomRight = new Point(_maxX, _maxY);
            _centrePoint = new Point(_x + _width / 2, _y + _height / 2);
        }
    }
}
