using ALife.Core.Utility.Angles;
using System;

namespace ALife.Core.Utility.Points
{
    /// <summary>
    /// Various math functions for points (and others).
    /// </summary>
    public static class PointMath
    {
        /// <summary>
        /// Calculates the angle between two points.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns>The angle.</returns>
        public static Angle AngleBetweenPoints(Point target, Point source)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            return Angle.FromRadians(angleBetweenPoints);
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The distance.</returns>
        public static double Distance(Point a, Point b)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double delX = (double)(a.X - b.X);
            double delY = (double)(a.Y - b.Y);

            return Math.Sqrt((delX * delX) + (delY * delY));
        }

        /// <summary>
        /// Calculates the radians between two points.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The radians.</returns>
        public static double RadiansBetweenPoints(Point source, Point target)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angle = Math.Atan2(deltaY, deltaX);
            return angle;
        }

        /// <summary>
        /// Translates the point by the vector.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="radians">The radians.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>The translated point.</returns>
        public static Point TranslateByVector(Point start, double radians, double distance)
        {
            double newX = (distance * Math.Cos(radians)) + start.X;
            double newY = (distance * Math.Sin(radians)) + start.Y;

            return new Point(newX, newY);
        }

        /// <summary>
        /// Translates the point by the vector.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>The translated point.</returns>
        public static Point TranslateByVector(Point start, Angle angle, double distance)
        {
            return TranslateByVector(start, angle.Radians, distance);
        }
    }
}
