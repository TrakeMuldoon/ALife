using System;
using ALife.Core.Geometry;

namespace ALife.Core.Utility.Maths
{
    /// <summary>
    /// THe remaining math functions (related to geometry) from the old ExtraMath class.
    /// TODO: remove this and/or move it to a more appropriate location once one is in place.
    /// </summary>
    public class GeometryMaths
    {
        public static double AngleBetweenPoints(Geometry.Shapes.Point target, Geometry.Shapes.Point source)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            return angleBetweenPoints;
        }

        public static double DistanceBetweenTwoPoints(Geometry.Shapes.Point a, Geometry.Shapes.Point b)
        {
            return Math.Sqrt(SquaredDistanceBetweenTwoPoints(a, b));
        }

        public static double SquaredDistanceBetweenTwoPoints(Geometry.Shapes.Point a, Geometry.Shapes.Point b)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double delX = (double)(a.X - b.X);
            double delY = (double)(a.Y - b.Y);

            return (delX * delX) + (delY * delY);
        }

        public static Geometry.Shapes.Point TranslateByVector(Geometry.Shapes.Point startPoint, double radians, double distance)
        {
            double newX = (distance * Math.Cos(radians)) + startPoint.X;
            double newY = (distance * Math.Sin(radians)) + startPoint.Y;

            return new Geometry.Shapes.Point(newX, newY);
        }

        public static Geometry.Shapes.Point TranslateByVector(Geometry.Shapes.Point startPoint, Angle angle, double distance)
        {
            return TranslateByVector(startPoint, angle.Radians, distance);
        }
    }
}
