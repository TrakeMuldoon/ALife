using ALife.Core.Geometry;
using ALife.Core.NewGeometry;

namespace ALife.Core.CollisionDetection.Collision
{
    /// <summary>
    /// Various static (i.e. shapes without associated velocities) collision detection methods.
    /// </summary>
    public static class StaticCollisionDetectors
    {
        /// <summary>
        /// Checks for a collision between two circles.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>True on a collision, false otherwise.</returns>
        public static bool CircleToCircleCollision(SimpleCircle a, SimpleCircle b)
        {
            //If the distance between the points is closer or equal to this, then they overlap/collide
            double minimumDistance = a.Radius + b.Radius;
            double minimumSquared = minimumDistance * minimumDistance;

            double xDelta = a.Centre.X - b.Centre.X;
            // Multiplication is generally faster than Math.Pow, so we use it here (note, there might be compiler
            // optimizations that make this not true for this case)
            double xDeltaSquared = xDelta * xDelta;

            double yDelta = a.Centre.Y - b.Centre.Y;
            double yDeltaSquared = yDelta * yDelta;

            double distanceSquared = xDeltaSquared + yDeltaSquared;

            // Note: We never do square roots anywhere, because they are slow. It does mean we're passing around a lot
            // of squared values.
            return distanceSquared <= minimumSquared;
        }

        /// <summary>
        /// Checks for a collision between a circle and a point.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>True on a collision, false otherwise.</returns>
        public static bool CircleToPointCollision(SimpleCircle a, Point b)
        {
            // Note: a point is just a circle with a radius of 0
            return CircleToCircleCollision(a, new SimpleCircle(b.X, b.Y, 0));
        }
    }
}
