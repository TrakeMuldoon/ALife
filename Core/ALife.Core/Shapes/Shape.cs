using ALife.Core.CollisionDetection;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core.Shapes
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
            if(Children.Count == 0)
            {
                return GetBoundingBox();
            }
            else
            {
                BoundingBox[] boxes = Children.Select(child => child.GetCombinedBoundingBox()).ToArray();

                return BoundingBox.FromBoundingBoxes(boxes);
            }
        }

        public bool IsCollision(Shape other)
        {
        }
    }
}
