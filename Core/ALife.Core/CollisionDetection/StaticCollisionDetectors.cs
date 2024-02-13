using System.Drawing;

namespace ALife.Core.CollisionDetection
{
    public static class StaticCollisionDetectors
    {
        public static bool CircleToCircle(Point centreA, double radiusA, Point centreB, double radiusB)
        {
            //If the distance between the points is closer or equal to this, then they overlap/collide
            double minimumDistance = radiusA + radiusB;
            double minimumSquared = minimumDistance * minimumDistance;

            double xDelta = centreA.X - centreB.X;
            // Multiplication is generally faster than Math.Pow, so we use it here (note, there might be compiler
            // optimizations that make this not true for this case)
            double xDeltaSquared = xDelta * xDelta;

            double yDelta = centreA.Y - centreB.Y;
            double yDeltaSquared = yDelta * yDelta;

            double distanceSquared = xDeltaSquared + yDeltaSquared;

            // Note: We never do square roots anywhere, because they are slow. It does mean we're passing around a lot
            // of squared values.
            return distanceSquared <= minimumSquared;
        }

        public static bool CircleToPoint(Point centre, double radius, Point point)
        {
            // Note: a point is just a circle with a radius of 0. Note 2: we could probably copy the code from
            // CircleToCircle here and update it to run with the assumption that a point has no radius, but...
            return CircleToCircle(centre, radius, point, 0);
        }
    }
}
