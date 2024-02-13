using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Represents a circle.
    /// </summary>
    /// <seealso cref="ALife.Core.Shapes.Shape"/>
    public class Circle : Shape
    {
        /// <summary>
        /// The radius of the circle.
        /// </summary>
        public double Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        public Circle(double x, double y, double radius) : this(radius, new ShapeArguments(centrePoint: new Point(x, y)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Point coordinates, double radius) : this(radius, new ShapeArguments(centrePoint: coordinates))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="circle">The circle.</param>
        public Circle(Circle circle) : this(circle.Radius, circle.GetShapeArguments())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="shapeArguments">The shape arguments.</param>
        public Circle(double radius, ShapeArguments shapeArguments) : base(shapeArguments)
        {
            Radius = radius;
        }

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public override Shape Clone()
        {
            return new Circle(this);
        }

        /// <summary>
        /// A method that executes when the centre point is updated.
        /// </summary>
        protected override void CentrePointUpdated()
        {
            // We don't have to do anything special for a circle, because the centre point is the only point that matters
        }

        /// <summary>
        /// Gets the bounding box for this instance.
        /// </summary>
        /// <returns>The bounding box for this shape.</returns>
        protected override BoundingBox GetSelfBoundingBox()
        {
            // we don't have to do anything special for a circle, because the orientation doesn't impact the bounding box
            BoundingBox box = new BoundingBox(CentrePoint.X - Radius, CentrePoint.Y - Radius, CentrePoint.X + Radius, CentrePoint.Y + Radius);
            return box;
        }

        /// <summary>
        /// A method that executes when the orientation is updated.
        /// </summary>
        protected override void OrientationUpdated()
        {
            // We don't have to do anything special for a circle, because the orientation doesn't impact the bounding box
        }
    }
}
