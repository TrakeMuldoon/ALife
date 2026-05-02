using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Maths;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;


namespace ALife.Core.Geometry;

/// <summary>
/// Defines an angle in degrees or radians.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public struct Angle : IEquatable<Angle>
{
    /// <summary>
    /// An angle representing zero degrees.
    /// </summary>
    [JsonIgnore]
    public static readonly Angle Zero = new(0d);

    /// <summary>
    /// The degrees, always in [0, 360).
    /// </summary>
    [JsonIgnore]
    private double _degrees;

    /// <summary>
    /// The radians, always in [0, 2π).
    /// </summary>
    [JsonIgnore]
    private double _radians;

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle"/> class.
    /// </summary>
    /// <param name="degrees">The degrees.</param>
    [JsonConstructor]
    public Angle(double degrees)
    {
        _degrees = WrapDegrees(degrees);
        _radians = DegreesToRadians(_degrees);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Angle"/> class.
    /// </summary>
    /// <param name="angle">The angle.</param>
    public Angle(Angle angle)
    {
        _degrees = angle._degrees;
        _radians = angle._radians;
    }

    /// <summary>
    /// Represents an angular measurement, with utility methods and properties for operations involving angles.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isRads">if set to <c>true</c> [is rads].</param>
    public Angle(double value, bool isRads)
    {
        if(isRads)
        {
            _radians = WrapRadians(value);
            _degrees = RadiansToDegrees(_radians);
        }
        else
        {
            _degrees = WrapDegrees(value);
            _radians = DegreesToRadians(_degrees);
        }
    }

    /// <summary>
    /// Gets or sets the degrees.
    /// </summary>
    /// <value>The degrees.</value>
    [JsonPropertyName("degrees")]
    public double Degrees
    {
        get => _degrees;
        set
        {
            _degrees = WrapDegrees(value);
            _radians = DegreesToRadians(_degrees);
        }
    }

    /// <summary>
    /// Gets the inverse degrees.
    /// </summary>
    /// <value>The inverse degrees.</value>
    [JsonIgnore]
    public double InverseDegrees => -(GeometryConstants.MaxDegrees - _degrees);

    /// <summary>
    /// Gets the inverse radians.
    /// </summary>
    /// <value>The inverse radians.</value>
    [JsonIgnore]
    public double InverseRadians => -(GeometryConstants.TwoPi - _radians);

    /// <summary>
    /// Gets or sets the radians.
    /// </summary>
    /// <value>The radians.</value>
    [JsonIgnore]
    public double Radians
    {
        get => _radians;
        set
        {
            _radians = WrapRadians(value);
            _degrees = RadiansToDegrees(_radians);
        }
    }

    public void SetDegrees(double degrees)
    {
        _degrees = WrapDegrees(degrees);
        _radians = DegreesToRadians(_degrees);
    }

    /// <summary>
    /// Froms the degrees.
    /// </summary>
    /// <param name="degrees">The degrees.</param>
    /// <returns>A new.</returns>
    public static Angle FromDegrees(double degrees)
    {
        return new(degrees);
    }

    /// <summary>
    /// Froms the radians.
    /// </summary>
    /// <param name="radians">The radians.</param>
    /// <returns>A new.</returns>
    public static Angle FromRadians(double radians)
    {
        return new(radians, true);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator -(Angle a, Angle b)
    {
        return new(a._degrees - b._degrees);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator -(Angle a, double b)
    {
        return new(a._degrees - b);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator -(double a, Angle b)
    {
        return new(a - b._degrees);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(Angle left, Angle right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator *(Angle a, Angle b)
    {
        return new(a._degrees * b._degrees);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator *(Angle a, double b)
    {
        return new(a._degrees * b);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator *(double a, Angle b)
    {
        return new(a * b._degrees);
    }

    /// <summary>
    /// Implements the operator /.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator /(Angle a, Angle b)
    {
        return new(a._degrees / b._degrees);
    }

    /// <summary>
    /// Implements the operator /.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator /(Angle a, double b)
    {
        return new(a._degrees / b);
    }

    /// <summary>
    /// Implements the operator /.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator /(double a, Angle b)
    {
        return new(a / b._degrees);
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator +(Angle a, double b)
    {
        return new(a._degrees + b);
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator +(double a, Angle b)
    {
        return new(a + b._degrees);
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>The result of the operator.</returns>
    public static Angle operator +(Angle a, Angle b)
    {
        return new(a._degrees + b._degrees);
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(Angle left, Angle right)
    {
        return left._degrees < right._degrees;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <=(Angle left, Angle right)
    {
        return left._degrees <= right._degrees;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(Angle left, Angle right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(Angle left, Angle right)
    {
        return left._degrees > right._degrees;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >=(Angle left, Angle right)
    {
        return left._degrees >= right._degrees;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public Angle Clone()
    {
        return new(this);
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/>, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return obj is Angle angle &&
            angle._degrees == _degrees;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return _degrees.GetHashCode();
    }

    /// <summary>
    /// Gets the transformation matrix representing this angle.
    /// </summary>
    /// <returns>The transformation matrix.</returns>
    public Matrix GetTransformationMatrix()
    {
        Matrix output = Matrix.CreateFromAngle(this);
        return output;
    }

    /// <summary>
    /// Gets the transformation matrix representing this angle and a specified translation.
    /// </summary>
    /// <param name="translation">The translation.</param>
    /// <returns>The transformation matrix.</returns>
    public Matrix GetTransformationMatrix(Point translation)
    {
        Matrix output = Matrix.CreateFromTranslationAndAngle(this, translation);
        return output;
    }

    /// <summary>
    /// Gets the transformation matrix representing this angle and a specified translation.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>The transformation matrix.</returns>
    public Matrix GetTransformationMatrix(double x, double y)
    {
        Matrix output = Matrix.CreateFromTranslationAndAngle(this, x, y);
        return output;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        return $"Deg:{_degrees}, Rads:{_radians}";
    }

    /// <summary>
    /// Converts the degrees to radians.
    /// </summary>
    /// <param name="degrees">The degrees.</param>
    /// <returns>The radians.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double DegreesToRadians(double degrees)
    {
        return degrees * GeometryConstants.Pi / GeometryConstants.HalfDegrees;
    }

    /// <summary>
    /// Converts the radians to degrees.
    /// </summary>
    /// <param name="radians">The radians.</param>
    /// <returns>The degrees.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double RadiansToDegrees(double radians)
    {
        return radians * GeometryConstants.HalfDegrees / GeometryConstants.Pi;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double WrapDegrees(double d)
    {
        d %= 360.0;
        return d < 0 ? d + 360.0 : d;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double WrapRadians(double r)
    {
        r %= GeometryConstants.TwoPi;
        return r < 0 ? r + GeometryConstants.TwoPi : r;
    }

    /// <summary>
    /// Determines whether this instance and another specified <see cref="Angle"/> object have the same value.
    /// </summary>
    /// <param name="other">The other <see cref="Angle"/> instance to compare with this instance.</param>
    /// <returns><c>true</c> if the current instance is equal to the <paramref name="other"/> instance; otherwise, <c>false</c>.</returns>
    public bool Equals(Angle other)
    {
        return _degrees == other._degrees;
    }
}
