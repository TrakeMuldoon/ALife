using System.Numerics;

namespace ALife.Spatial.Shapes;

/// <summary>
/// Where a shape is and which way it's turned: its center of mass's position and its angle, with the angle's cosine and sine
/// worked out once. The space is y-down, as on screen, so a positive angle turns clockwise.
/// </summary>
public readonly struct Pose
{
    /// <summary>Creates a pose.</summary>
    /// <param name="position">Where the center of mass is.</param>
    /// <param name="angle">The angle in radians, clockwise on screen.</param>
    public Pose(Vector2 position, float angle)
    {
        Position = position;
        Angle = angle;
        (Sin, Cos) = MathF.SinCos(angle);
    }

    /// <summary>Where the center of mass is.</summary>
    public Vector2 Position { get; }

    /// <summary>The angle in radians, clockwise on screen.</summary>
    public float Angle { get; }

    /// <summary>The cosine of <see cref="Angle"/>.</summary>
    public float Cos { get; }

    /// <summary>The sine of <see cref="Angle"/>.</summary>
    public float Sin { get; }

    /// <summary>Turns a point relative to the center of mass, then moves it to the position: from the shape's space to the world's.</summary>
    /// <param name="local">The point in the shape's space.</param>
    /// <returns>The point in the world.</returns>
    public Vector2 Apply(Vector2 local) => Position + Rotate(local);

    /// <summary>Turns a vector by the angle, without moving it.</summary>
    /// <param name="vector">The vector in the shape's space.</param>
    /// <returns>The vector in the world.</returns>
    public Vector2 Rotate(Vector2 vector) => new((Cos * vector.X) - (Sin * vector.Y), (Sin * vector.X) + (Cos * vector.Y));

    /// <summary>Turns a vector back by the angle: from the world's directions to the shape's.</summary>
    /// <param name="vector">The vector in the world.</param>
    /// <returns>The vector in the shape's space.</returns>
    public Vector2 InverseRotate(Vector2 vector) => new((Cos * vector.X) + (Sin * vector.Y), (Cos * vector.Y) - (Sin * vector.X));
}
