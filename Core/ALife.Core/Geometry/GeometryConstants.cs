using System;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// Constants for the Geometry namespace.
    /// Note: some of these constants are also defined in System.Math, but are included here for completeness.
    /// </summary>
    public static class GeometryConstants
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
    }
}
