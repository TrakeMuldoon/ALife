using System.Text.Json.Serialization;
using ALife.Core.Geometry;
using ALife.Core.Utility;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Shapes
{
    public class Rectangle : Shape
    {
        /// <summary>
        /// The bottom left
        /// </summary>
        private Point _bottomLeft;

        /// <summary>
        /// The bottom right
        /// </summary>
        private Point _bottomRight;

        /// <summary>
        /// The height
        /// </summary>
        private double _height;

        /// <summary>
        /// The top left
        /// </summary>
        private Point _topLeft;

        /// <summary>
        /// The top right
        /// </summary>
        private Point _topRight;

        /// <summary>
        /// The width
        /// </summary>
        private double _width;

        public Rectangle(double x, double y, double width, double height, Angle orientation, Colour fillColour, Colour fillDebugColour, Colour outlineColour, Colour outlineDebugColour) : base(x, y, orientation, new ShapeRenderComponent(fillColour, fillDebugColour), new ShapeRenderComponent(outlineColour, outlineDebugColour))
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Gets the bottom left.
        /// </summary>
        /// <value>The bottom left.</value>
        [JsonIgnore]
        public Point BottomLeft => _bottomLeft;

        /// <summary>
        /// Gets the bottom right.
        /// </summary>
        /// <value>The bottom right.</value>
        [JsonIgnore]
        public Point BottomRight => _bottomRight;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [JsonIgnore]
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                RecalculateOwnShapeData();
            }
        }

        /// <summary>
        /// Gets the top left.
        /// </summary>
        /// <value>The top left.</value>
        [JsonIgnore]
        public Point TopLeft => _topLeft;

        /// <summary>
        /// Gets the top right.
        /// </summary>
        /// <value>The top right.</value>
        [JsonIgnore]
        public Point TopRight => _topRight;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [JsonIgnore]
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                RecalculateOwnShapeData();
            }
        }

        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Shape CloneInstance()
        {
            Rectangle newInstance = new Rectangle(CentrePoint.X, CentrePoint.Y, Width, Height, Orientation, RenderComponents.FillComponent.Colour, RenderComponents.FillComponent.DebugColour, RenderComponents.OutlineComponent.Colour, RenderComponents.OutlineComponent.DebugColour);
            return newInstance;
        }

        /// <summary>
        /// Triggers the recalculations of shape data for the current instance.
        /// </summary>
        public override void RecalculateOwnShapeData()
        {
            Matrix transformationMatrix = GetTransformationMatrix();

            double halfWidth = Width / 2;
            double halfHeight = Height / 2;
            _topLeft = new Point(-halfWidth, -halfHeight);
            _topRight = new Point(halfWidth, -halfHeight);
            _bottomLeft = new Point(-halfWidth, halfHeight);
            _bottomRight = new Point(halfWidth, halfHeight);

            double maxX = ExtraMath.Maximum(_topLeft.X, _topRight.X, _bottomLeft.X, _bottomRight.X);
            double maxY = ExtraMath.Maximum(_topLeft.Y, _topRight.Y, _bottomLeft.Y, _bottomRight.Y);
            double minX = ExtraMath.Minimum(_topLeft.X, _topRight.X, _bottomLeft.X, _bottomRight.X);
            double minY = ExtraMath.Minimum(_topLeft.Y, _topRight.Y, _bottomLeft.Y, _bottomRight.Y);

            _boundingBox = new BoundingBox(minX, minY, maxX, maxY);
        }
    }
}
