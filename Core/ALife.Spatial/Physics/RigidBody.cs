using System.Numerics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Physics;

/// <summary>
/// The moving state of a rigid body: where it is, how it's turned, how fast it moves and spins, and how hard it is to push.
/// A plain mutable struct for keeping in arrays, with no reference to its <see cref="Shape"/>; keep that alongside. The space
/// is y-down, as on screen, so a positive angle or angular velocity turns clockwise.
/// </summary>
public struct RigidBody
{
    /// <summary>Where its center of mass is.</summary>
    public Vector2 Position;

    /// <summary>Its angle in radians, clockwise on screen.</summary>
    public float Angle;

    /// <summary>How fast its center of mass moves, in units a second.</summary>
    public Vector2 Velocity;

    /// <summary>How fast it spins, in radians a second, clockwise on screen.</summary>
    public float AngularVelocity;

    /// <summary>One over its mass; zero for a body nothing can move.</summary>
    public float InverseMass;

    /// <summary>One over its moment of inertia; zero for a body nothing can turn.</summary>
    public float InverseInertia;

    /// <summary>A body with a shape's mass and inertia at a density, at rest.</summary>
    /// <param name="shape">The shape, for its <see cref="Shape.Area"/> and <see cref="Shape.Inertia"/>.</param>
    /// <param name="density">The mass per unit of area; above zero.</param>
    /// <param name="position">Where its center of mass is.</param>
    /// <param name="angle">Its angle in radians, clockwise on screen.</param>
    /// <returns>The body.</returns>
    public static RigidBody FromShape(Shape shape, float density, Vector2 position, float angle = 0)
    {
        ArgumentNullException.ThrowIfNull(shape);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(density);
        return new RigidBody
        {
            Position = position,
            Angle = angle,
            InverseMass = 1 / (shape.Area * density),
            InverseInertia = 1 / (shape.Inertia * density),
        };
    }

    /// <summary>A body nothing can move or turn, such as a wall: its inverse mass and inertia are zero.</summary>
    /// <param name="position">Where it is.</param>
    /// <param name="angle">Its angle in radians, clockwise on screen.</param>
    /// <returns>The body.</returns>
    public static RigidBody Static(Vector2 position, float angle = 0) => new() { Position = position, Angle = angle };

    /// <summary>Its position and angle, for placing its shape.</summary>
    public readonly Pose Pose => new(Position, Angle);

    /// <summary>Moves and turns it by its velocities over a time step.</summary>
    /// <param name="dt">The time step in seconds.</param>
    public void Integrate(float dt)
    {
        Position += Velocity * dt;
        Angle += AngularVelocity * dt;
    }

    /// <summary>How fast a point fixed to the body moves, counting its spin.</summary>
    /// <param name="point">The point, in the world.</param>
    /// <returns>Its velocity.</returns>
    public readonly Vector2 VelocityAt(Vector2 point)
    {
        Vector2 arm = point - Position;
        return Velocity + new Vector2(-AngularVelocity * arm.Y, AngularVelocity * arm.X);
    }

    /// <summary>Applies an impulse at a point, changing its velocity and, if the point is off its center, its spin.</summary>
    /// <param name="impulse">The impulse: a force times the time it acts for.</param>
    /// <param name="point">Where it acts, in the world.</param>
    public void ApplyImpulse(Vector2 impulse, Vector2 point)
    {
        Velocity += impulse * InverseMass;
        AngularVelocity += InverseInertia * Cross(point - Position, impulse);
    }

    internal static float Cross(Vector2 a, Vector2 b) => (a.X * b.Y) - (a.Y * b.X);
}
