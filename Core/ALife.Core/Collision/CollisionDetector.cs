using ALife.Core.Geometry.Shapes;
using System.Collections.Generic;

namespace ALife.Core.Collision
{
    /// <summary>
    /// The narrow phase: whether ALife shapes actually overlap, once a collision map has found the ones whose boxes do. Every
    /// pairing of circle, rectangle, axis-aligned rectangle and sector works, child shapes included, using the exact tests in
    /// <see cref="ALife.Spatial.Collisions.Overlap"/>. Shapes that only touch overlap. It reads each shape's centre, orientation
    /// and size as they are now, so a shape that moved since its last <see cref="IShape.Reset"/> is tested where it is.
    /// </summary>
    public static class CollisionDetector
    {
        /// <summary>Whether two shapes overlap or touch.</summary>
        /// <param name="a">The first shape.</param>
        /// <param name="b">The second shape.</param>
        /// <returns>True if they overlap or touch.</returns>
        public static bool Collides(IShape a, IShape b)
        {
            Point origin = a.CentrePoint;
            return ShapeGeometry.From(a, origin).Overlaps(ShapeGeometry.From(b, origin));
        }

        /// <summary>The objects whose shapes overlap or touch a shape, in the order given.</summary>
        /// <param name="toCollide">The objects to test, usually the ones a collision map found near <paramref name="me"/>.</param>
        /// <param name="me">The shape to test them against.</param>
        /// <typeparam name="T">The objects' type.</typeparam>
        /// <returns>A new list of the ones that overlap.</returns>
        public static List<T> FineGrainedCollisionDetection<T>(IEnumerable<T> toCollide, IShape me) where T : IHasShape
        {
            Point origin = me.CentrePoint;
            ShapeGeometry mine = ShapeGeometry.From(me, origin);
            List<T> collisions = new List<T>();
            foreach(T candidate in toCollide)
            {
                if(mine.Overlaps(ShapeGeometry.From(candidate.Shape, origin)))
                {
                    collisions.Add(candidate);
                }
            }

            return collisions;
        }
    }
}
