using System.Numerics;

namespace ALife.Spatial.Shapes;

/// <summary>
/// A slice of a disc in the world: the points no further than <see cref="Radius"/> from <see cref="Apex"/> whose direction
/// from it is within <see cref="Sweep"/> of <see cref="Start"/>, turning clockwise on screen (y-down). Any sweep from none to
/// a full turn works, including ones over half a turn, which aren't convex. It's for areas that only need to know what they
/// overlap, such as an agent's eye, through <see cref="Collisions.Overlap"/>: it has no mass and makes no contacts.
/// </summary>
public readonly struct Sector
{
    /// <summary>Creates a sector.</summary>
    /// <param name="apex">The point of the slice: the disc's center.</param>
    /// <param name="start">The direction one straight edge leaves the apex in, in radians, clockwise on screen from +x.</param>
    /// <param name="sweep">How far round from <paramref name="start"/> the slice reaches, in radians; zero or more. A full turn or more is the whole disc.</param>
    /// <param name="radius">How far from the apex the slice reaches; zero or more.</param>
    public Sector(Vector2 apex, float start, float sweep, float radius)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sweep);
        ArgumentOutOfRangeException.ThrowIfNegative(radius);
        Apex = apex;
        Start = start;
        Sweep = MathF.Min(sweep, MathF.Tau);
        Radius = radius;
        (float sin, float cos) = MathF.SinCos(start);
        StartDirection = new Vector2(cos, sin);
        (sin, cos) = MathF.SinCos(start + Sweep);
        EndDirection = new Vector2(cos, sin);
    }

    /// <summary>The point of the slice: the disc's center.</summary>
    public Vector2 Apex { get; }

    /// <summary>The direction the first straight edge leaves the apex in, in radians, clockwise on screen from +x.</summary>
    public float Start { get; }

    /// <summary>How far round from <see cref="Start"/> the slice reaches, in radians, from zero to a full turn.</summary>
    public float Sweep { get; }

    /// <summary>How far from the apex the slice reaches.</summary>
    public float Radius { get; }

    /// <summary>The unit direction of the first straight edge, at <see cref="Start"/>.</summary>
    public Vector2 StartDirection { get; }

    /// <summary>The unit direction of the second straight edge, at <see cref="Start"/> plus <see cref="Sweep"/>.</summary>
    public Vector2 EndDirection { get; }

    /// <summary>Where the first straight edge meets the arc.</summary>
    public Vector2 StartPoint => Apex + (StartDirection * Radius);

    /// <summary>Where the second straight edge meets the arc.</summary>
    public Vector2 EndPoint => Apex + (EndDirection * Radius);

    /// <summary>Whether the sweep is a full turn, so the sector is the whole disc.</summary>
    public bool IsFull => Sweep >= MathF.Tau;

    /// <summary>Whether a direction from the apex is within the sweep, edges included. The zero vector is.</summary>
    /// <param name="direction">The direction, of any length.</param>
    /// <returns>True if it's within the sweep.</returns>
    public bool InSweep(Vector2 direction)
    {
        if (IsFull)
        {
            return true;
        }

        float fromStart = ConvexPart.Cross(StartDirection, direction);
        float toEnd = ConvexPart.Cross(direction, EndDirection);
        if (Sweep > MathF.PI)
        {
            // Over half a turn, the sweep is everything but the convex wedge between the end and the start.
            return fromStart >= 0 || toEnd >= 0;
        }

        // A narrow sweep also has to face the start's way, or the direction straight back would count when the edges meet.
        return fromStart >= 0 && toEnd >= 0 && (Sweep > MathF.PI / 2 || Vector2.Dot(direction, StartDirection) >= 0);
    }

    /// <summary>Whether a point is in the sector, edges included.</summary>
    /// <param name="point">The point in the world.</param>
    /// <returns>True if it's in the sector.</returns>
    public bool Contains(Vector2 point)
    {
        Vector2 offset = point - Apex;
        return offset.LengthSquared() <= Radius * Radius && InSweep(offset);
    }

    /// <summary>The smallest box around the sector: the apex, the ends of the arc, and wherever the arc passes due right, down, left or up.</summary>
    /// <returns>The box.</returns>
    public Aabb GetBounds()
    {
        Vector2 startPoint = StartPoint, endPoint = EndPoint;
        float left = MathF.Min(Apex.X, MathF.Min(startPoint.X, endPoint.X));
        float top = MathF.Min(Apex.Y, MathF.Min(startPoint.Y, endPoint.Y));
        float right = MathF.Max(Apex.X, MathF.Max(startPoint.X, endPoint.X));
        float bottom = MathF.Max(Apex.Y, MathF.Max(startPoint.Y, endPoint.Y));
        if (InSweep(Vector2.UnitX))
        {
            right = Apex.X + Radius;
        }

        if (InSweep(Vector2.UnitY))
        {
            bottom = Apex.Y + Radius;
        }

        if (InSweep(-Vector2.UnitX))
        {
            left = Apex.X - Radius;
        }

        if (InSweep(-Vector2.UnitY))
        {
            top = Apex.Y - Radius;
        }

        return new Aabb(left, top, right, bottom);
    }
}
