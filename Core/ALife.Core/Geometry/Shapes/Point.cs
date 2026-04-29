using ALife.Core.Utility;
using ALife.Core.Utility.Maths;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ALife.Core.Geometry.Shapes;

/// <summary>
/// Defines a Point in space
/// </summary>
[DebuggerDisplay("{ToString()}")]
public struct Point
{
    /// <summary>
    /// A point representing 0, 0
    /// </summary>
    public static Point Zero = new Point(0, 0);

    /// <summary>
    /// The x coordinate
    /// </summary>
    private double _x;

    /// <summary>
    /// The y coordinate
    /// </summary>
    private double _y;

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> struct.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    [JsonConstructor]
    public Point(double x, double y)
    {
        _x = x;
        _y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> struct.
    /// </summary>
    /// <param name="Geometry.Shapes.Point">The Geometry.Shapes.Point.</param>
    public Point(Point Point)
    {
        _x = Point.X;
        _y = Point.Y;
    }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    /// <value>The x int.</value>
    [JsonIgnore]
    public double X => _x;

    /// <summary>
    /// Gets the x int.
    /// </summary>
    /// <value>The x int.</value>
    [JsonIgnore]
    public int XInt => (int)Math.Round(_x);

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    /// <value>The x int.</value>
    [JsonIgnore]
    public double Y => _y;

    /// <summary>
    /// Gets the y int.
    /// </summary>
    /// <value>The y int.</value>
    [JsonIgnore]
    public int YInt => (int)Math.Round(_y);

    /// <summary>
    /// Transforms the specified point by the matrix.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed point.</returns>
    public static Point FromTransformation(Point point, Matrix matrix)
    {
        double newX = point._x * matrix.M11 + point._y * matrix.M21 + matrix.M41;
        double newY = point._x * matrix.M12 + point._y * matrix.M22 + matrix.M42;

        return new Point(newX, newY);
    }

    /// <summary>
    /// Implements the operator op_Inequality.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(Point left, Point right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(Point left, Point right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public Point Clone()
    {
        return new Point(_x, _y);
    }

    /// <summary>
    /// Equalses the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>True if equals, False otherwise.</returns>
    public override bool Equals(object obj)
    {
        return obj is Point && Equals((Point)obj);
    }

    /// <summary>
    /// Equalses the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>True if equals, False otherwise.</returns>
    public bool Equals(Point value)
    {
        return _x.Equals(value.X) && _y.Equals(value.Y);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return HashCodeHelper.Combine(this._x, this._y);
    }

    /// <summary>
    /// Gets the transformation matrix representing this point.
    /// </summary>
    /// <returns>The transformation matrix.</returns>
    public Matrix GetTransformationMatrix()
    {
        Matrix output = Matrix.CreateFromTranslation(this);
        return output;
    }

    /// <summary>
    /// Gets the transformation matrix representing this point and a specified angle.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <returns>The transformation matrix.</returns>
    public Matrix GetTransformationMatrix(Angle angle)
    {
        Matrix output = Matrix.CreateFromTranslationAndAngle(angle, this);
        return output;
    }

    /// <summary>
    /// Gets a transformed point using the current point against the specified matrix.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    public Point GetTransformedPoint(Matrix matrix)
    {
        return FromTransformation(this, matrix);
    }

    /// <summary>
    /// Updates the x coord.
    /// </summary>
    /// <param name="newX">The new x.</param>
    public void SetX(double newX)
    {
        _x = newX;
    }

    /// <summary>
    /// Updates the x and y coords.
    /// </summary>
    /// <param name="newX">The new x.</param>
    /// <param name="newY">The new y.</param>
    public void SetXY(double newX, double newY)
    {
        _x = newX;
        _y = newY;
    }

    /// <summary>
    /// Updates the y coord.
    /// </summary>
    /// <param name="newY">The new y.</param>
    public void SetY(double newY)
    {
        _y = newY;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>The string representation of the Geometry.Shapes.Point.</returns>
    public override string ToString()
    {
        return $"({_x}, {_y})";
    }

    /// <summary>
    /// Converts a point to a Vector2.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <returns>The Vector2</returns>
    public Vector2 ToVector2()
    {
        return new Vector2((float)_x, (float)_y);
    }

    /// <summary>
    /// Calculates the angle between the current point and the specified point.
    /// </summary>
    /// <param name="other">The point to calculate the angle to.</param>
    /// <returns>An <see cref="Angle"/> representing the angle between the two points.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle GetAngleBetween(Point other)
    {
        double deltaX = other.X - this.X;
        double deltaY = other.Y - this.Y;

        double angleBetweenPoints = Math.Atan2(deltaY, deltaX);
        Angle angle = new(angleBetweenPoints, true);
        return angle;
    }

    /// <summary>
    /// Calculates the angle between the current point and another specified point.
    /// </summary>
    /// <param name="other">The other point to calculate the angle to.</param>
    /// <returns>The angle in radians between the current point and the specified point.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetRadiansBetween(Point other)
    {
        Angle angle = GetAngleBetween(other);
        return angle.Radians;
    }

    /// <summary>
    /// Calculates the angle between the current point and another specified point.
    /// </summary>
    /// <param name="other">The other point to calculate the angle to.</param>
    /// <returns>The angle in degrees between the current point and the specified point.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetDegreesBetween(Point other)
    {
        Angle angle = GetAngleBetween(other);
        return angle.Degrees;
    }

    /// <summary>
    /// Calculates the distance from the current <see cref="Point"/> to another <see cref="Point"/>.
    /// </summary>
    /// <param name="other">The other <see cref="Point"/> to calculate the distance from.</param>
    /// <returns>The distance between the two points as a double.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double DistanceFrom(Point other)
    {
        double squaredDistance = SquaredDistanceFrom(other);
        double distance = Math.Sqrt(squaredDistance);
        return distance;
    }

    /// <summary>
    /// Computes the squared distance from the current point to another point.
    /// </summary>
    /// <param name="other">The other <see cref="Point"/> to compute the squared distance from.</param>
    /// <returns>The squared distance as a double.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double SquaredDistanceFrom(Point other)
    {
        //pythagorean theorem c^2 = a^2 + b^2
        //thus c = square root(a^2 + b^2)
        double delX = this.X - other.X;
        double delY = this.Y - other.Y;
        
        double squaredDistance = (delX * delX) + (delY * delY);
        return squaredDistance;
    }

    /// <summary>
    /// Transforms the current point using the specified matrix.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    public void Transform(Matrix matrix)
    {
        Point transformed = FromTransformation(this, matrix);
        _x = transformed._x;
        _y = transformed._y;
    }

    /// <summary>
    /// Transforms the current point using the specified translation.
    /// </summary>
    /// <param name="translation">The translation.</param>
    public void Transform(Point translation)
    {
        Matrix matrix = Matrix.CreateFromTranslation(translation);
        Transform(matrix);
    }

    /// <summary>
    /// Transforms the current point using the specified angle.
    /// </summary>
    /// <param name="angle">The angle.</param>
    public void Transform(Angle angle)
    {
        Matrix matrix = Matrix.CreateFromAngle(angle);
        Transform(matrix);
    }

    /// <summary>
    /// Transforms the current point using the specified translation.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    public void Transform(double x, double y)
    {
        Matrix matrix = Matrix.CreateFromTranslation(x, y);
        Transform(matrix);
    }

    /// <summary>
    /// Transforms the current point using the specified angle in radians.
    /// </summary>
    /// <param name="radians">The radians.</param>
    public void Transform(double radians)
    {
        Matrix matrix = Matrix.CreateFromAngle(radians);
        Transform(matrix);
    }

    /// <summary>
    /// Transforms the current point using the specified translation and angle.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="angle">The angle.</param>
    public void Transform(double x, double y, Angle angle)
    {
        Matrix matrix = Matrix.CreateFromTranslationAndAngle(angle, x, y);
        Transform(matrix);
    }

    /// <summary>
    /// Transforms the current point using the specified translation and angle in radians.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="radians">The radians.</param>
    public void Transform(double x, double y, double radians)
    {
        Matrix matrix = Matrix.CreateFromTranslationAndAngle(radians, x, y);
        Transform(matrix);
    }

    /// <summary>
    /// Transforms the current point using the specified translation and angle.
    /// </summary>
    /// <param name="translation">The translation.</param>
    /// <param name="angle">The angle.</param>
    public void Transform(Point translation, Angle angle)
    {
        Matrix matrix = Matrix.CreateFromTranslationAndAngle(angle, translation);
        Transform(matrix);
    }

    /// <summary>
    /// Applies a transformation to the current point based on the specified radians and distance.
    /// </summary>
    /// <param name="radians">The angle in radians to rotate the point.</param>
    /// <param name="distance">The distance to move the point in the direction of the specified angle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TransformByVector(double radians, double distance)
    {
        this._x = (distance * Math.Cos(radians)) + this.X;
        this._y = (distance * Math.Sin(radians)) + this.Y;
    }

    /// <summary>
    /// Transforms the current point by a vector defined by an angle and a distance.
    /// </summary>
    /// <param name="angle">The angle of the vector in radians.</param>
    /// <param name="distance">The distance of the vector to transform by.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TransformByVector(Angle angle, double distance)
    {
        TransformByVector(angle.Radians, distance);
    }
}

public static partial class Extensions
{
    public static Vector2 ToVector2(this Point p) => new((float)p.X, (float)p.Y);
}
