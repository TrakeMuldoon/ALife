namespace ALife.Core.Geometry
{
    public static class IShapeExtensions
    {
        /// <summary>
        /// Updates the relative data of this shape based on the parent shape, and then updates the child shapes.
        /// </summary>
        /// <param name="shape">The shape to update.</param>
        public static void UpdateShapeAndChildShapes(this IShape shape)
        {
            shape.UpdateRelativeData(shape);
            if(shape.ChildShapes != null)
            {
                shape.UpdateChildShapes();
            }
        }

        /// <summary>
        /// Updates the relative data of all child shapes for this shape.
        /// </summary>
        /// <param name="shape">The shape to update.</param>
        private static void UpdateChildShapes(this IShape shape)
        {
            for(int i = 0; i < shape.ChildShapes.Count; i++)
            {
                shape.ChildShapes[i].UpdateShapeAndChildShapes();
            }
        }
    }
}
