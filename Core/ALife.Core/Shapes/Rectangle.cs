using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Represents a non-axis aligned rectangle.
    /// NOTE: If you want an axis aligned rectangle, use a <see cref="AxisAlinedRectangle"/> instead.
    /// </summary>
    /// <seealso cref="ALife.Core.Shapes.Shape"/>
    public class Rectangle : Shape
    {
        /// <summary>
        /// The bottom left coordinates
        /// </summary>
        private Point _bottomLeft;

        /// <summary>
        /// The bottom right coordinates
        /// </summary>
        private Point _bottomRight;

        /// <summary>
        /// The top left coordinates
        /// </summary>
        private Point _topLeft;

        /// <summary>
        /// The top right coordinates
        /// </summary>
        private Point _topRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="centrePoint">The centre point.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rectangle(Point centrePoint, double width, double height) : this(width, height, new ShapeArguments(centrePoint))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="arguments">The arguments.</param>
        public Rectangle(double width, double height, ShapeArguments arguments) : base(arguments)
        {
            _topLeft = new Point(-width / 2, -height / 2) + arguments.CentrePoint;
            _topRight = new Point(width / 2, -height / 2) + arguments.CentrePoint;
            _bottomLeft = new Point(-width / 2, height / 2) + arguments.CentrePoint;
            _bottomRight = new Point(width / 2, height / 2) + arguments.CentrePoint;
        }

        public Rectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight) : base(arguments)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
        }

        public Rectangle(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight, ShapeArguments arguments) : base(arguments)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
        }

        public Rectangle(Rectangle rectangle)
        {
        }

        /// <summary>
        /// Gets the bottom left coordinates.
        /// </summary>
        /// <value>The bottom left.</value>
        public Point BottomLeft => _bottomLeft;

        /// <summary>
        /// Gets the bottom right coordinates.
        /// </summary>
        /// <value>The bottom right.</value>
        public Point BottomRight => _bottomRight;

        /// <summary>
        /// Gets the top left coordinates.
        /// </summary>
        /// <value>The top left.</value>
        public Point TopLeft => _topLeft;

        /// <summary>
        /// Gets the top right coordinates.
        /// </summary>
        /// <value>The top right.</value>
        public Point TopRight => _topRight;

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public override Shape Clone()
        {
            return new Rectangle(this);
        }

        /// <summary>
        /// Updates the bottom left coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        public void UpdateBottomLeft(Point point)
        {
            _bottomLeft = point;
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom left coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void UpdateBottomLeft(double x, double y)
        {
            _bottomLeft.SetXY(x, y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom left x coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        public void UpdateBottomLeftX(double x)
        {
            _bottomLeft.SetX(x);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom left y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        public void UpdateBottomLeftY(double y)
        {
            _bottomLeft.SetY(y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom right coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        public void UpdateBottomRight(Point point)
        {
            _bottomRight = point;
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom right coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void UpdateBottomRight(double x, double y)
        {
            _bottomRight.SetXY(x, y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom right x coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        public void UpdateBottomRightX(double x)
        {
            _bottomRight.SetX(x);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the bottom right y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        public void UpdateBottomRightY(double y)
        {
            _bottomRight.SetY(y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top left coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        public void UpdateTopLeft(Point point)
        {
            _topLeft = point;
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top left coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void UpdateTopLeft(double x, double y)
        {
            _topLeft.SetXY(x, y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top left x coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        public void UpdateTopLeftX(double x)
        {
            _topLeft.SetX(x);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top left y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        public void UpdateTopLeftY(double y)
        {
            _topLeft.SetY(y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top right coordinates.
        /// </summary>
        /// <param name="point">The point.</param>
        public void UpdateTopRight(Point point)
        {
            _topRight = point;
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top right coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void UpdateTopRight(double x, double y)
        {
            _topRight.SetXY(x, y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top right x coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        public void UpdateTopRightX(double x)
        {
            _topRight.SetX(x);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// Updates the top right y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        public void UpdateTopRightY(double y)
        {
            _topRight.SetY(y);
            _centrePoint = RecalculateCentrePoint();
        }

        /// <summary>
        /// A method that executes when the centre point is updated.
        /// </summary>
        protected override void CentrePointUpdated()
        {
            // translate the points to the new centre point
            Point difference = CentrePoint - _previousCentrePoint;
            Matrix translation = Matrix.CreateFromTranslation(difference);

            _topLeft.TransformPoint(translation);
            _topRight.TransformPoint(translation);
            _bottomLeft.TransformPoint(translation);
            _bottomRight.TransformPoint(translation);
        }

        /// <summary>
        /// Gets the bounding box for this instance.
        /// </summary>
        /// <returns>The bounding box for this shape.</returns>
        protected override BoundingBox GetSelfBoundingBox()
        {
            GetMinimumsAndMaximums(out double minX, out double maxX, out _, out double minY, out double maxY, out _);

            Point boxTopLeft = new Point(minX, minY);
            Point boxBottomRight = new Point(maxX, maxY);

            Angle orientation = GetAbsoluteOrientation();
            Matrix rotation = Matrix.CreateFromAngle(orientation);

            boxTopLeft.TransformPoint(rotation);
            boxBottomRight.TransformPoint(rotation);

            double newXMin = ExtraMath.Minimum(boxTopLeft.X, boxBottomRight.X);
            double newXMax = ExtraMath.Maximum(boxTopLeft.X, boxBottomRight.X);
            double newWidth = newXMax - newXMin;

            double newYMin = ExtraMath.Minimum(boxTopLeft.Y, boxBottomRight.Y);
            double newYMax = ExtraMath.Maximum(boxTopLeft.Y, boxBottomRight.Y);
            double newHeight = newYMax - newYMin;

            return new BoundingBox(new Point(newXMin, newYMin), newWidth, newHeight);
        }

        /// <summary>
        /// A method that executes when the orientation is updated.
        /// </summary>
        protected override void OrientationUpdated()
        {
            Angle difference = Orientation - _previousOrientation;
            Matrix rotation = Matrix.CreateFromAngle(difference);

            _topLeft.TransformPoint(rotation);
            _topRight.TransformPoint(rotation);
            _bottomLeft.TransformPoint(rotation);
            _bottomRight.TransformPoint(rotation);
        }

        /// <summary>
        /// Calculates the centre point.
        /// </summary>
        /// <param name="topLeft">The top left.</param>
        /// <param name="topRight">The top right.</param>
        /// <param name="bottomLeft">The bottom left.</param>
        /// <param name="bottomRight">The bottom right.</param>
        /// <returns>What the centre point should be.</returns>
        private Point CalculateCentrePoint(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight)
        {
            double minX = ExtraMath.Minimum(_topLeft.X, _topRight.X, _bottomLeft.X, _bottomRight.X);
            double maxX = ExtraMath.Maximum(_topLeft.X, _topRight.X, _bottomLeft.X, _bottomRight.X);
            double width = maxX - minX;

            double minY = ExtraMath.Minimum(_topLeft.Y, _topRight.Y, _bottomLeft.Y, _bottomRight.Y);
            double maxY = ExtraMath.Maximum(_topLeft.Y, _topRight.Y, _bottomLeft.Y, _bottomRight.Y);
            double height = maxY - minY;

            double x = minX + width / 2;
            double y = minY + height / 2;

            Point point = new Point(x, y);
            return point;
        }

        /// <summary>
        /// Recalculates the centre point.
        /// </summary>
        /// <returns>The new centre point.</returns>
        private Point RecalculateCentrePoint()
        {
            Point point = CalculateCentrePoint(_topLeft, _topRight, _bottomLeft, _bottomRight);
            return point;
        }
    }
}
