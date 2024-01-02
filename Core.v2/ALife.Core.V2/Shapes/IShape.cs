using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// The interface for a shape.
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// Gets the axis aligned bounding box.
        /// </summary>
        /// <value>The axis aligned bounding box.</value>
        BoundingBox AxisAlignedBoundingBox { get; }

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>The bounding box.</value>
        BoundingBox BoundingBox { get; }

        /// <summary>
        /// Gets or sets the centre point.
        /// </summary>
        /// <value>The centre point.</value>
        Point CentrePoint { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        Angle Orientation { get; set; }

        /// <summary>
        /// Gets the render components.
        /// </summary>
        /// <value>The render components.</value>
        ShapeRenderComponents RenderComponents { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        IShape Clone();

        /// <summary>
        /// Updates the centre point.
        /// </summary>
        /// <param name="newCentrePoint">The new centre point.</param>
        void UpdateCentrePoint(Point newCentrePoint);

        /// <summary>
        /// Updates the centre point x.
        /// </summary>
        /// <param name="newCentrePointX">The new centre point x.</param>
        void UpdateCentrePointX(double newCentrePointX);

        /// <summary>
        /// Updates the centre point y.
        /// </summary>
        /// <param name="newCentrePointY">The new centre point y.</param>
        void UpdateCentrePointY(double newCentrePointY);

        /// <summary>
        /// Updates the orientation.
        /// </summary>
        /// <param name="newOrientation">The new orientation.</param>
        void UpdateOrientation(Angle newOrientation);

        /// <summary>
        /// Updates the orientation to the new degrees.
        /// </summary>
        /// <param name="newOrientationDegrees">The new orientation degrees.</param>
        void UpdateOrientationDegrees(double newOrientationDegrees);

        /// <summary>
        /// Updates the orientation to the new radians.
        /// </summary>
        /// <param name="newOrientationRadians">The new orientation radians.</param>
        void UpdateOrientationRadians(double newOrientationRadians);
    }
}
