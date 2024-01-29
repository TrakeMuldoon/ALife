﻿using System;
using ALife.Core.GeometryOld;

namespace ALife.Core.Utility.Maths
{
    /// <summary>
    /// THe remaining math functions (related to geometry) from the old ExtraMath class.
    /// TODO: remove this and/or move it to a more appropriate location once one is in place.
    /// </summary>
    public class GeometryMath
    {
        public static double AngleBetweenPoints(ALife.Core.GeometryOld.Shapes.Point target, ALife.Core.GeometryOld.Shapes.Point source)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            return angleBetweenPoints;
        }

        public static double DistanceBetweenTwoPoints(ALife.Core.GeometryOld.Shapes.Point a, ALife.Core.GeometryOld.Shapes.Point b)
        {
            return Math.Sqrt(SquaredDistanceBetweenTwoPoints(a, b));
        }

        public static double SquaredDistanceBetweenTwoPoints(ALife.Core.GeometryOld.Shapes.Point a, ALife.Core.GeometryOld.Shapes.Point b)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double delX = (double)(a.X - b.X);
            double delY = (double)(a.Y - b.Y);

            return (delX * delX) + (delY * delY);
        }

        public static ALife.Core.GeometryOld.Shapes.Point TranslateByVector(ALife.Core.GeometryOld.Shapes.Point startPoint, double radians, double distance)
        {
            double newX = (distance * Math.Cos(radians)) + startPoint.X;
            double newY = (distance * Math.Sin(radians)) + startPoint.Y;

            return new ALife.Core.GeometryOld.Shapes.Point(newX, newY);
        }

        public static ALife.Core.GeometryOld.Shapes.Point TranslateByVector(ALife.Core.GeometryOld.Shapes.Point startPoint, Angle angle, double distance)
        {
            return TranslateByVector(startPoint, angle.Radians, distance);
        }
    }
}
