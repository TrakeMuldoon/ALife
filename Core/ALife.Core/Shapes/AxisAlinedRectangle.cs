using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Represents a rectangle with sides always parallel to the x and y axes.
    /// NOTE: this rectangle ignores the orientation of the shape and the parent shape. NOTE 2: If you want to use a
    /// rectangle with a specific orientation, see the <see cref="ALife.Core.Shapes.Rectangle"/> class.
    /// </summary>
    /// <seealso cref="ALife.Core.Shapes.Shape"/>
    public class AxisAlinedRectangle : Shape
    {
        /// <summary>
        /// The height of the rectangle
        /// </summary>
        private double _height;

        /// <summary>
        /// The top left coordinates of the rectangle
        /// </summary>
        private Point _topLeft;

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        private double _width;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisAlinedRectangle"/> class.
        /// </summary>
        /// <param name="topLeft">The top left.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public AxisAlinedRectangle(Point topLeft, double width, double height) : this(topLeft, width, height, new ShapeArguments(centrePoint: new Point(topLeft.X + width / 2, topLeft.Y + height / 2)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisAlinedRectangle"/> class.
        /// </summary>
        /// <param name="topLeft">The top left.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="arguments">The arguments.</param>
        public AxisAlinedRectangle(Point topLeft, double width, double height, ShapeArguments arguments) : base(arguments)
        {
            _topLeft = topLeft;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisAlinedRectangle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public AxisAlinedRectangle(double x, double y, double width, double height) : this(new Point(x, y), width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisAlinedRectangle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="arguments">The arguments.</param>
        public AxisAlinedRectangle(double x, double y, double width, double height, ShapeArguments arguments) : this(new Point(x, y), width, height, arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisAlinedRectangle"/> class.
        /// </summary>
        /// <param name="axisAlinedRectangle">The axis alined rectangle.</param>
        public AxisAlinedRectangle(AxisAlinedRectangle axisAlinedRectangle) : this(axisAlinedRectangle.TopLeft, axisAlinedRectangle.Width, axisAlinedRectangle.Height, axisAlinedRectangle.GetShapeArguments())
        {
        }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                double newY = _topLeft.Y + _height / 2;
                SetCentreY(newY);
            }
        }

        /// <summary>
        /// Gets the top left coordinates.
        /// </summary>
        /// <value>The top left.</value>
        public Point TopLeft => _topLeft;

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                double newX = _topLeft.X + _width / 2;
                SetCentreX(newX);
            }
        }

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public override Shape Clone()
        {
            return new AxisAlinedRectangle(this);
        }

        /// <summary>
        /// Sets the height of the rectangle.
        /// </summary>
        /// <param name="height">The height.</param>
        public void SetHeight(double height)
        {
            Height = height;
        }

        /// <summary>
        /// Sets the top left coordinates.
        /// </summary>
        /// <param name="newTopLeft">The new top left coordinates.</param>
        public void SetTopLeft(Point newTopLeft)
        {
            _topLeft = newTopLeft;
            double newX = _topLeft.X + _width / 2;
            double newY = _topLeft.Y + _height / 2;
            SetCentrePoint(newX, newY);
        }

        /// <summary>
        /// Sets the top left coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void SetTopLeft(double x, double y)
        {
            _topLeft = new Point(x, y);
            double newX = _topLeft.X + _width / 2;
            double newY = _topLeft.Y + _height / 2;
            SetCentrePoint(newX, newY);
        }

        /// <summary>
        /// Sets the top left x coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        public void SetTopLeftX(double x)
        {
            _topLeft.SetX(x);
            double newX = _topLeft.X + _width / 2;
            SetCentreX(newX);
        }

        /// <summary>
        /// Sets the top left y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        public void SetTopLeftY(double y)
        {
            _topLeft.SetY(y);
            double newY = _topLeft.Y + _height / 2;
            SetCentreY(newY);
        }

        /// <summary>
        /// Sets the width of the rectangle.
        /// </summary>
        /// <param name="width">The width.</param>
        public void SetWidth(double width)
        {
            Width = width;
        }

        protected override void CentrePointUpdated()
        {
            // when the centre point is updated, we need to update the top left coordinates
            double newX = _centrePoint.X - _width / 2;
            double newY = _centrePoint.Y - _height / 2;
            _topLeft.SetXY(newX, newY);
        }

        protected override BoundingBox GetSelfBoundingBox()
        {
            // NOTE: In an axis aligned rectangle, the bounding box is the same as the rectangle itself. NOTE 2: In an
            // axis aligned rectangle, we don't need to worry about the orientation of the shape.
            return new BoundingBox(_topLeft, _width, _height);
        }

        /// <summary>
        /// A method that executes when the orientation is updated.
        /// </summary>
        protected override void OrientationUpdated()
        {
            // NOTE: In an axis aligned rectangle, the orientation is always treated as 0, so we don't need to do
            //       anything here.
        }
    }
}
