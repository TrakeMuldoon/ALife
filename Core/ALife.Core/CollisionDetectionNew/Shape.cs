using ALife.Core.CollisionDetection;
using ALife.Core.CollisionDetection.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.CollisionDetectionNew
{
    /// <summary>
    /// An abstract class that defines a shape.
    /// </summary>
    public abstract class Shape
    {
        /// <summary>
        /// The children of the current shape.
        /// </summary>
        public List<Shape> Children;

        /// <summary>
        /// The colouration of the current shape.
        /// </summary>
        public ShapeColouration Colouration;

        /// <summary>
        /// The parent of the current shape.
        /// </summary>
        public Shape Parent;

        /// <summary>
        /// Whether to allow this shape to combine its bounding box with its children.
        /// </summary>
        public bool AllowChildBoxesToCombine { get; protected set; }

        /// <summary>
        /// Whether to allow this shape to combine its bounding box with its parent.
        /// </summary>
        public bool JoinParentBoundingBox { get; protected set; }

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <returns>The bounding box for the current shape.</returns>
        public abstract BoundingBox GetBoundingBox(bool isAbsolute = true);

        /// <summary>
        /// Gets the combined bounding box.
        /// </summary>
        /// <returns></returns>
        public BoundingBox GetCombinedBoundingBox()
        {
            if(Children.Count == 0 || !AllowChildBoxesToCombine)
            {
                return GetBoundingBox();
            }
            else
            {
                IEnumerable<Shape> combinableChildren = Children.Where(child => child.JoinParentBoundingBox);
                IEnumerable<BoundingBox> childBoxes = combinableChildren.Select(child => child.GetCombinedBoundingBox());
                childBoxes.Append(GetBoundingBox());

                return BoundingBox.FromBoundingBoxes(childBoxes.ToArray());
            }
        }

        public bool IsCollision(Shape other)
        {
        }

        public bool IsCollisionWithBoundingBox(Shape other)
        {
            BoundingBox thisBox = GetCombinedBoundingBox();
            BoundingBox otherBox = other.GetCombinedBoundingBox();

            if(thisBox.IsCollision(otherBox))
            {
                return IsCollision(other);
            }
            return false;
        }
    }
}
