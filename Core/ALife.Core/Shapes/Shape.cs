using System.Collections.Generic;

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
    }
}
