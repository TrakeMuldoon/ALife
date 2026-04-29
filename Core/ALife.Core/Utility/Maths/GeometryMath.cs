using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using System;
using System.Runtime.CompilerServices;

namespace ALife.Core.Utility.Maths;

/// <summary>
/// The remaining math functions (related to geometry) from the old ExtraMath class.
/// </summary>
public static class GeometryMath
{
    /// <summary>
    /// Translates a point by a specified distance along a direction given by an angle in radians.
    /// </summary>
    /// <param name="startPoint">The starting point to be translated.</param>
    /// <param name="radians">The angle in radians indicating the direction of translation.</param>
    /// <param name="distance">The distance to translate the point.</param>
    /// <returns>The new point resulting from the translation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point TranslateByVector(Point startPoint, double radians, double distance)
    {
        double newX = (distance * Math.Cos(radians)) + startPoint.X;
        double newY = (distance * Math.Sin(radians)) + startPoint.Y;

        return new Point(newX, newY);
    }

    /// <summary>
    /// Translates a point by a specified distance and angle.
    /// </summary>
    /// <param name="startPoint">The starting point to be translated.</param>
    /// <param name="angle">The angle indicating the direction of the translation.</param>
    /// <param name="distance">The distance by which the point should be translated.</param>
    /// <returns>The translated point after applying the vector transformation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point TranslateByVector(Point startPoint, Angle angle, double distance)
    {
        return TranslateByVector(startPoint, angle.Radians, distance);
    }
}
