using System.Text.Json.Serialization;
using ALife.Core.CollisionDetection.Geometry;
using ALife.Core.NewGeometry;
using ALife.Core.Utility;
using ALife.Core.Utility.Maths;

namespace ALife.Core.CollisionDetection
{
    /// <summary>
    /// Defines an axis-aligned bounding box.
    /// TODO: Investigate introducing caching for MaxX, MaxY, TopLeft, TopRight, BottomLeft, and BottomRight.
    /// </summary>
    public struct BoundingBox
    {
        /// <summary>
        /// The height
        /// </summary>
        [JsonIgnore]
        private double _height;

        /// <summary>
        /// The width
        /// </summary>
        [JsonIgnore]
        private double _width;

        /// <summary>
        /// The x
        /// </summary>
        [JsonIgnore]
        private double _x;

        /// <summary>
        /// The y
        /// </summary>
        [JsonIgnore]
        private double _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [JsonConstructor]
        public BoundingBox(double x, double y, double width, double height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="box">The box to clone.</param>
        public BoundingBox(BoundingBox box)
        {
            _x = box.X;
            _y = box.Y;
            _width = box.Width;
            _height = box.Height;
        }

        /// <summary>
        /// Gets the bottom left coordinates.
        /// </summary>
        /// <value>The bottom left.</value>
        [JsonIgnore]
        public Point BottomLeft => new Point(_x, _y + _height);

        /// <summary>
        /// Gets the bottom right coordinates.
        /// </summary>
        /// <value>The bottom right.</value>
        [JsonIgnore]
        public Point BottomRight => new Point(_x + _width, _y + _height);

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        [JsonPropertyName("height")]
        public double Height => _height;

        /// <summary>
        /// Gets the maximum x coordinate.
        /// </summary>
        /// <value>The maximum x.</value>
        [JsonIgnore]
        public double MaxX => _x + _width;

        /// <summary>
        /// Gets the maximum y coordinate.
        /// </summary>
        /// <value>The maximum y.</value>
        [JsonIgnore]
        public double MaxY => _y + _height;

        /// <summary>
        /// Gets the minimum x coordinate.
        /// </summary>
        /// <value>The minimum x.</value>
        [JsonIgnore]
        public double MinX => _x;

        /// <summary>
        /// Gets the minimum y coordinate.
        /// </summary>
        /// <value>The minimum y.</value>
        [JsonIgnore]
        public double MinY => _y;

        /// <summary>
        /// Gets the top left coordinates.
        /// </summary>
        /// <value>The top left.</value>
        [JsonIgnore]
        public Point TopLeft => new Point(_x, _y);

        /// <summary>
        /// Gets the top right coordinates.
        /// </summary>
        /// <value>The top right.</value>
        [JsonIgnore]
        public Point TopRight => new Point(_x + _width, _y);

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        [JsonPropertyName("width")]
        public double Width => _width;

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x.</value>
        [JsonPropertyName("x")]
        public double X => _x;

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y.</value>
        [JsonPropertyName("y")]
        public double Y => _y;

        /// <summary>
        /// Combines the specified bounding boxes into a single bounding box.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        public static BoundingBox FromBoundingBoxes(params BoundingBox[] boxes)
        {
            double minX = int.MaxValue;
            double minY = int.MaxValue;
            double maxX = int.MinValue;
            double maxY = int.MinValue;

            foreach(BoundingBox box in boxes)
            {
                if(box.MinX < minX)
                {
                    minX = box.MinX;
                }
                if(box.MinY < minY)
                {
                    minY = box.MinY;
                }
                if(box.MaxX > maxX)
                {
                    maxX = box.MaxX;
                }
                if(box.MaxY > maxY)
                {
                    maxY = box.MaxY;
                }
            }

            BoundingBox output = FromMinXMinYMaxXMaxY(minX, minY, maxX, maxY);
            return output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="minX">The minimum x.</param>
        /// <param name="minY">The minimum y.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="maxY">The maximum y.</param>
        /// <returns>The new BoundingBox.</returns>
        public static BoundingBox FromMinXMinYMaxXMaxY(double minX, double minY, double maxX, double maxY)
        {
            if(minX > maxX)
            {
                (minX, maxX) = (maxX, minX);
            }
            if(minY > maxY)
            {
                (minY, maxY) = (maxY, minY);
            }

            double width = maxX - minX;
            double height = maxY - minY;

            return new BoundingBox(minX, minY, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The new BoundingBox.</returns>
        public static BoundingBox FromXYWidthHeight(double x, double y, double width, double height)
        {
            return new BoundingBox(x, y, width, height);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public BoundingBox Clone()
        {
            return new BoundingBox(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is BoundingBox bb && bb.X == _x && bb.Y == _y && bb.Width == _width && bb.Height == _height;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(_x, _y, _width, _height);
        }

        /// <summary>
        /// Gets a transformed bounding box using the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transformed Axis-aligned Bounding Box.</returns>
        public BoundingBox GetTransformedBoundingBox(Matrix matrix)
        {
            BoundingBox newBox = this.Clone();
            newBox.TransformBoundingBox(matrix);
            return newBox;
        }

        /// <summary>
        /// Determines whether the specified interloper is colliding with the current instance.
        /// </summary>
        /// <param name="interloper">The interloper.</param>
        /// <returns><c>true</c> if the specified interloper is collision; otherwise, <c>false</c>.</returns>
        public bool IsCollision(BoundingBox interloper)
        {
            if(MinX < interloper.MaxX
                && MaxX > interloper.MinX
                && MinY < interloper.MaxY
                && MaxY > interloper.MinY)
            {
                return true;
            }
            else //explicit else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            string output = $"X: {_x}, Y: {_y}, Width: {_width}, Height: {_height}";
            return output;
        }

        /// <summary>
        /// Transforms the bounding box.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void TransformBoundingBox(Matrix matrix)
        {
            Point newTopLeft = TopLeft.GetTransformedPoint(matrix);
            Point newBottomRight = BottomRight.GetTransformedPoint(matrix);
            Point newTopRight = TopRight.GetTransformedPoint(matrix);
            Point newBottomLeft = BottomLeft.GetTransformedPoint(matrix);

            double newMinX = ExtraMath.Minimum(newTopLeft.X, newBottomRight.X, newTopRight.X, newBottomLeft.X);
            double newMinY = ExtraMath.Minimum(newTopLeft.Y, newBottomRight.Y, newTopRight.Y, newBottomLeft.Y);
            double newMaxX = ExtraMath.Maximum(newTopLeft.X, newBottomRight.X, newTopRight.X, newBottomLeft.X);
            double newMaxY = ExtraMath.Maximum(newTopLeft.Y, newBottomRight.Y, newTopRight.Y, newBottomLeft.Y);

            _x = newMinX;
            _y = newMinY;
            _width = newMaxX - newMinX;
            _height = newMaxY - newMinY;
        }
    }
}
