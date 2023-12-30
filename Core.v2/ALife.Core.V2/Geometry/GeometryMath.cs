using System;
using System.Numerics;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// Various math functions for points (and others).
    /// </summary>
    public static class GeometryMath
    {
        /// <summary>
        /// A constant representing the mid-way degrees in a circle.
        /// </summary>
        public const double HalfDegrees = 180d;

        /// <summary>
        /// A constant representing half of Pi. 90 degrees.
        /// </summary>
        public const double HalfPi = Math.PI / 2;

        /// <summary>
        /// A constant representing the maximum degrees in a circle.
        /// </summary>
        public const double MaxDegrees = 360d;

        /// <summary>
        /// A constant representing the minimum degrees in a circle.
        /// </summary>
        public const double MinDegrees = 0d;

        /// <summary>
        /// A constant representing 1.5 * Pi. 270 degrees.
        /// </summary>
        public const double OneAndHalfPi = Math.PI * 1.5;

        /// <summary>
        /// A constant representing Pi. 180 degrees.
        /// </summary>
        public const double Pi = Math.PI;

        /// <summary>
        /// A constant representing a quarter of Pi. 45 degrees.
        /// </summary>
        public const double QuarterPi = Math.PI / 4;

        /// <summary>
        /// A constant representing 2 * Pi. 360 degrees.
        /// </summary>
        public const double TwoPi = Math.PI * 2;

        /// <summary>
        /// A constant representing 0 * Pi (which is Zero...but just in case we want to do funky experiments!).
        /// </summary>
        public const double ZeroPi = 0d;

        /// <summary>
        /// Calculates the angle between two points.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns>The angle.</returns>
        public static Angle AngleBetweenPoints(Point source, Point target)
        {
            double deltaX = target.X - source.X;
            double deltaY = target.Y - source.Y;

            double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
            return Angle.FromRadians(angleBetweenPoints);
        }

        /// <summary>
        /// Calculates the degrees between two points.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The degrees.</returns>
        public static double DegreesBetweenPoints(Point source, Point target)
        {
            Angle angle = AngleBetweenPoints(source, target);
            return angle.Degrees;
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
            double delX = a.X - b.X;
            double delY = a.Y - b.Y;

            return Math.Sqrt(delX * delX + delY * delY);
        }

        /// <summary>
        /// Gets the translation vector for the specified angle and distance.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>The translation vector.</returns>
        public static Vector2 GetTranslationVector(Angle angle, double distance)
        {
            double x = distance * Math.Cos(angle.Radians);
            double y = distance * Math.Sin(angle.Radians);
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Gets the translation vector for the specified angle and distance.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="angleIsRadians">if set to <c>true</c> [angle is radians].</param>
        /// <returns>The translation vector.</returns>
        public static Vector2 GetTranslationVector(double radians, double distance, bool angleIsRadians = true)
        {
            Angle angle;
            if(angleIsRadians)
            {
                angle = Angle.FromRadians(radians);
            }
            else
            {
                angle = Angle.FromDegrees(radians);
            }
            return GetTranslationVector(angle, distance);
        }

        /// <summary>
        /// Calculates the radians between two points.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The radians.</returns>
        public static double RadiansBetweenPoints(Point source, Point target)
        {
            Angle angle = AngleBetweenPoints(source, target);
            return angle.Radians;
        }

        /// <summary>
        /// Translates the point by the vector.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="radians">The radians.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="angleIsRadians">
        /// if set to <c>true</c> [radians is measured in radians], else [radians is measured in degrees].
        /// </param>
        /// <returns>The translated point.</returns>
        public static Point TranslateByVector(Point start, double radians, double distance, bool angleIsRadians = true)
        {
            Angle angle;
            if(angleIsRadians)
            {
                angle = Angle.FromRadians(radians);
            }
            else
            {
                angle = Angle.FromDegrees(radians);
            }
            return TranslateByVector(start, angle, distance);
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
            double newX = distance * Math.Cos(angle.Radians) + start.X;
            double newY = distance * Math.Sin(angle.Radians) + start.Y;

            return new Point(newX, newY);
        }

        /// <summary>
        /// Translates the point by the vector.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>The translated point.</returns>
        public static Point TranslateByVector(Point start, Vector2 vector)
        {
            double newX = vector.X + start.X;
            double newY = vector.Y + start.Y;
            return new Point(newX, newY);
        }
    }
}
