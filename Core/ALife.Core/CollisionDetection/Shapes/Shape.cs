using System.Collections.Generic;

namespace ALife.Core.CollisionDetection.Shapes
{
    /// <summary>
    /// An abstract class that defines a shape.
    /// </summary>
    public abstract class Shape
    {
        /// <summary>
        /// Any child shapes of this shape. Children will be transformed relative to their parent.
        /// </summary>
        public List<Shape> Children;

        /// <summary>
        /// The parent shape of the current shape...if one exists.
        /// </summary>
        public Shape Parent;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has collision detection.
        /// </summary>
        /// <value><c>true</c> if this instance has collision detection; otherwise, <c>false</c>.</value>
        public bool HasCollisionDetection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [merge bounding box with children].
        /// </summary>
        /// <value><c>true</c> if [merge bounding box with children]; otherwise, <c>false</c>.</value>
        public bool MergeBoundingBoxWithChildren { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Shape"/> is renderable.
        /// </summary>
        /// <value><c>true</c> if renderable; otherwise, <c>false</c>.</value>
        public bool Renderable { get; set; }

        public abstract BoundingBox GetBoundingBox();

        public abstract bool IsFineGrainedCollision(Shape other);
    }
}
