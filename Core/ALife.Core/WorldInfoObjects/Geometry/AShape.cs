using ALife.Core.Collision;
using ALife.Core.Interfaces;
using ALife.Core.Utility.Geometry;

namespace ALife.Core.WorldInfoObjects.Geometry
{
    public abstract class AShape : IDeepCloneable<AShape>
    {
        /// <summary>
        /// The centre point of the shape.
        /// </summary>
        public Point CentrePoint;

        /// <summary>
        /// The orientation of the shape.
        /// </summary>
        public Angle Orientation;
        
        /// <summary>
        /// The children of the shape.
        /// </summary>
        public List<AShape> Children;
        
        /// <summary>
        /// The bounding box of the shape.
        /// </summary>
        public BoundingBox BoundingBox { get; protected set; }

        /// <summary>
        /// The colour information of the shape.
        /// </summary>
        public ColourInformation ColourInformation;
        
        /// <summary>
        /// The type of shape.
        /// </summary>
        public ShapesEnum ShapeType { get; protected set; }

        /// <summary>
        /// Resets the shape.
        /// </summary>
        public abstract void Reset();
        
        /// <summary>
        /// Clones the shape.
        /// </summary>
        /// <returns>A cloned copy of the shape.</returns>
        public abstract AShape Clone();
    }
}