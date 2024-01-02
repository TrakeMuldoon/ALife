using System.Diagnostics;
using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Defines a circle.
    /// </summary>
    /// <seealso cref="IShape"/>
    [DebuggerDisplay("{ToString()}")]
    public class Circle : IShape
    {
        private Angle _angle;

        private Point _centrePoint;

        public BoundingBox AxisAlignedBoundingBox => throw new System.NotImplementedException();

        public BoundingBox BoundingBox => throw new System.NotImplementedException();

        public Point CentrePoint
        {
            get => _centrePoint;
            set => _centrePoint = value;
        }

        public Angle Orientation
        {
            get => _angle;
            set => _angle = value;
        }

        public ShapeRenderComponents RenderComponents { get; set; }

        public IShape Clone()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Circle: CentrePoint={CentrePoint}, Radius={Radius}";
        }

        public void UpdateCentrePoint(Point newCentrePoint)
        {
            _centrePoint = newCentrePoint;
        }

        public void UpdateCentrePointX(double newCentrePointX)
        {
            _centrePoint.X = newCentrePointX;
        }

        public void UpdateCentrePointY(double newCentrePointY)
        {
            _centrePoint.Y = newCentrePointY;
        }

        public void UpdateOrientation(Geometry.Angle newOrientation)
        {
            _angle = newOrientation;
        }

        public void UpdateOrientationDegrees(double newOrientationDegrees)
        {
            _angle.Degrees = newOrientationDegrees;
        }

        public void UpdateOrientationRadians(double newOrientationRadians)
        {
            _angle.Radians = newOrientationRadians;
        }
    }
}
