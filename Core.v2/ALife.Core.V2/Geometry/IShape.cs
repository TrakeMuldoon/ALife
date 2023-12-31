using System.Collections.Generic;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// Defines the basic properties of a shape.
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
        /// Gets or sets the child shapes.
        /// </summary>
        /// <value>The child shapes.</value>
        List<IShape> ChildShapes { get; }

        /// <summary>
        /// Gets or sets the debug fill colour.
        /// </summary>
        /// <value>The debug fill colour.</value>
        Colour DebugFillColour { get; set; }

        /// <summary>
        /// Gets or sets the debug outline colour.
        /// </summary>
        /// <value>The debug outline colour.</value>
        Colour DebugOutlineColour { get; set; }

        /// <summary>
        /// Gets or sets the fill colour.
        /// </summary>
        /// <value>The fill colour.</value>
        Colour FillColour { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a child shape.
        /// </summary>
        /// <value><c>true</c> if this instance is a child shape; otherwise, <c>false</c>.</value>
        bool IsChildShape { get; }

        /// <summary>
        /// Gets or sets the outline colour.
        /// </summary>
        /// <value>The outline colour.</value>
        Colour OutlineColour { get; set; }

        BoundingBox RelativeAxisAlignedBoundingBox { get; }

        BoundingBox RelativeBoundingBox { get; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        IShape Clone();

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        Angle GetOrientation();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        void SetOrientation(Angle orientation);

        /// <summary>
        /// Updates the relative data based on the parent shape.
        /// </summary>
        /// <param name="parentShape">The parent shape.</param>
        void UpdateRelativeData(IShape parentShape);
    }
}
