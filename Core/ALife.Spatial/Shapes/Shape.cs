using System.Numerics;

namespace ALife.Spatial.Shapes;

/// <summary>
/// A rigid 2D shape: one or more convex <see cref="ConvexPart"/>s around its center of mass, which is the origin of its space.
/// Shapes never change once made, so one can be shared by any number of bodies, each placed by its own <see cref="Pose"/>.
/// The space is y-down, as on screen.
/// </summary>
public sealed class Shape
{
    /// <summary>The most corners a polygon can have.</summary>
    public const int MaxVertices = 16;

    // The array behind Parts, for the collision code.
    internal readonly ConvexPart[] PartArray;

    // The parts have to be centered on their combined center of mass already.
    private Shape(ConvexPart[] parts)
    {
        PartArray = parts;
        foreach (ConvexPart part in parts)
        {
            Area += part.Area;
            Inertia += part.Inertia + (part.Area * part.Center.LengthSquared());
            if (part.Kind == ShapeKind.Circle)
            {
                BoundingRadius = MathF.Max(BoundingRadius, part.Center.Length() + part.Radius);
                continue;
            }

            foreach (Vector2 vertex in part.VertexArray)
            {
                BoundingRadius = MathF.Max(BoundingRadius, vertex.Length());
            }
        }
    }

    /// <summary>The convex pieces, in the shape's space: one for a simple shape, several for a compound.</summary>
    public ReadOnlySpan<ConvexPart> Parts => PartArray;

    /// <summary>The total area: the mass at a density of 1. Parts of a compound that overlap count twice.</summary>
    public float Area { get; }

    /// <summary>
    /// The second moment of the area about the center of mass: the moment of inertia at a density of 1. Multiply by the
    /// density for the real one, as <see cref="Physics.RigidBody.FromShape"/> does.
    /// </summary>
    public float Inertia { get; }

    /// <summary>The distance from the center of mass to the farthest point of the shape: a circle that holds it at any angle.</summary>
    public float BoundingRadius { get; }

    /// <summary>A circle.</summary>
    /// <param name="radius">The radius; above zero.</param>
    /// <returns>The shape.</returns>
    public static Shape Circle(float radius)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);
        return new Shape([ConvexPart.Circle(Vector2.Zero, radius)]);
    }

    /// <summary>A rectangle, unturned with its edges along the axes.</summary>
    /// <param name="width">The width; above zero.</param>
    /// <param name="height">The height; above zero.</param>
    /// <returns>The shape.</returns>
    public static Shape Box(float width, float height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        float x = width * 0.5f, y = height * 0.5f;
        return new Shape([ConvexPart.Polygon([new(-x, -y), new(x, -y), new(x, y), new(-x, y)])]);
    }

    /// <summary>A square, unturned with its edges along the axes.</summary>
    /// <param name="size">The length of a side; above zero.</param>
    /// <returns>The shape.</returns>
    public static Shape Square(float size) => Box(size, size);

    /// <summary>An equilateral triangle, unturned with a corner pointing up the screen.</summary>
    /// <param name="side">The length of a side; above zero.</param>
    /// <returns>The shape.</returns>
    public static Shape Triangle(float side) => RegularPolygon(3, side / MathF.Sqrt(3));

    /// <summary>A regular polygon, unturned with a corner pointing up the screen.</summary>
    /// <param name="sides">How many sides; from 3 to <see cref="MaxVertices"/>.</param>
    /// <param name="radius">The distance from the center to each corner; above zero.</param>
    /// <returns>The shape.</returns>
    public static Shape RegularPolygon(int sides, float radius)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 3);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sides, MaxVertices);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);
        Vector2[] vertices = new Vector2[sides];
        for (int i = 0; i < sides; i++)
        {
            // Up is -y, and a growing angle turns clockwise on screen.
            (float sin, float cos) = MathF.SinCos((-MathF.PI / 2) + (2 * MathF.PI * i / sides));
            vertices[i] = new Vector2(cos, sin) * radius;
        }

        return new Shape([ConvexPart.Polygon(vertices)]);
    }

    /// <summary>
    /// The convex hull of some points: a polygon with the outermost of them as corners, moved so its centroid is the origin.
    /// Points inside the hull or on its edges are dropped, so they needn't be convex or in order.
    /// </summary>
    /// <param name="points">The points; at least three of them not all on one line.</param>
    /// <returns>The shape.</returns>
    /// <exception cref="ArgumentException">The hull has no area, or more than <see cref="MaxVertices"/> corners.</exception>
    public static Shape Polygon(params ReadOnlySpan<Vector2> points)
    {
        Vector2[] hull = ConvexHull(points);
        if (hull.Length < 3)
        {
            throw new ArgumentException("The points need at least three corners not on one line.", nameof(points));
        }

        if (hull.Length > MaxVertices)
        {
            throw new ArgumentException($"The hull has {hull.Length} corners; the most is {MaxVertices}.", nameof(points));
        }

        return new Shape([ConvexPart.Polygon(hull)]);
    }

    /// <summary>
    /// A shape made of others, fixed together: each one's parts are turned and moved into place, and the whole is moved so its
    /// combined center of mass is the origin. Parts that overlap count twice towards <see cref="Area"/> and
    /// <see cref="Inertia"/>, so keep them apart, or touching, for an even density.
    /// </summary>
    /// <param name="parts">The shapes and where they go; at least one.</param>
    /// <returns>The shape.</returns>
    public static Shape Compound(params ReadOnlySpan<ShapePart> parts)
    {
        if (parts.IsEmpty)
        {
            throw new ArgumentException("A compound needs at least one part.", nameof(parts));
        }

        List<ConvexPart> placed = [];
        float area = 0;
        Vector2 moment = Vector2.Zero;
        foreach (ShapePart part in parts)
        {
            ArgumentNullException.ThrowIfNull(part.Shape, nameof(parts));
            (float sin, float cos) = MathF.SinCos(part.Rotation);
            foreach (ConvexPart convex in part.Shape.PartArray)
            {
                ConvexPart moved = convex.Transform(part.Offset, cos, sin);
                placed.Add(moved);
                area += moved.Area;
                moment += moved.Area * moved.Center;
            }
        }

        Vector2 center = moment / area;
        ConvexPart[] centered = new ConvexPart[placed.Count];
        for (int i = 0; i < centered.Length; i++)
        {
            centered[i] = placed[i].Transform(-center, 1, 0);
        }

        return new Shape(centered);
    }

    /// <summary>The smallest box around the shape where a pose puts it.</summary>
    /// <param name="pose">Where the shape is and how it's turned.</param>
    /// <returns>The box.</returns>
    public Aabb GetBounds(in Pose pose)
    {
        float left = float.PositiveInfinity, top = float.PositiveInfinity;
        float right = float.NegativeInfinity, bottom = float.NegativeInfinity;
        foreach (ConvexPart part in PartArray)
        {
            if (part.Kind == ShapeKind.Circle)
            {
                Vector2 center = pose.Apply(part.Center);
                left = MathF.Min(left, center.X - part.Radius);
                top = MathF.Min(top, center.Y - part.Radius);
                right = MathF.Max(right, center.X + part.Radius);
                bottom = MathF.Max(bottom, center.Y + part.Radius);
                continue;
            }

            foreach (Vector2 vertex in part.VertexArray)
            {
                Vector2 corner = pose.Apply(vertex);
                left = MathF.Min(left, corner.X);
                top = MathF.Min(top, corner.Y);
                right = MathF.Max(right, corner.X);
                bottom = MathF.Max(bottom, corner.Y);
            }
        }

        return new Aabb(left, top, right, bottom);
    }

    // Andrew's monotone chain, which leaves out repeated points and ones on an edge. The corners come out with positive
    // signed area, which is clockwise on screen.
    private static Vector2[] ConvexHull(ReadOnlySpan<Vector2> points)
    {
        Vector2[] sorted = points.ToArray();
        Array.Sort(sorted, static (a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));
        if (sorted.Length < 3)
        {
            return sorted;
        }

        Vector2[] hull = new Vector2[sorted.Length * 2];
        int count = 0;
        for (int i = 0; i < sorted.Length; i++)
        {
            while (count >= 2 && ConvexPart.Cross(hull[count - 1] - hull[count - 2], sorted[i] - hull[count - 2]) <= 0)
            {
                count--;
            }

            hull[count++] = sorted[i];
        }

        for (int i = sorted.Length - 2, lower = count + 1; i >= 0; i--)
        {
            while (count >= lower && ConvexPart.Cross(hull[count - 1] - hull[count - 2], sorted[i] - hull[count - 2]) <= 0)
            {
                count--;
            }

            hull[count++] = sorted[i];
        }

        // The last point is the first again.
        return hull[..(count - 1)];
    }
}
