using System.Numerics;

namespace ALife.Spatial.Shapes;

/// <summary>
/// One convex piece of a <see cref="Shape"/>: a circle or a convex polygon, placed in the shape's space, whose origin is the
/// whole shape's center of mass. Parts never change once made. A struct, so a shape's parts sit side by side in one array
/// rather than each in an object of its own. The default is an empty circle at the origin; the only real parts are the ones
/// a shape's <see cref="Shape.Parts"/> holds.
/// </summary>
public readonly struct ConvexPart
{
    private static readonly Vector2[] None = [];

    // The arrays behind Vertices and Normals, for the collision code; null in a default part.
    internal readonly Vector2[] VertexArray;
    internal readonly Vector2[] NormalArray;

    private ConvexPart(ShapeKind kind, Vector2 center, float radius, float area, float inertia, Vector2[] vertices, Vector2[] normals)
    {
        Kind = kind;
        Center = center;
        Radius = radius;
        Area = area;
        Inertia = inertia;
        VertexArray = vertices;
        NormalArray = normals;
    }

    /// <summary>Whether it's a circle or a polygon.</summary>
    public ShapeKind Kind { get; }

    /// <summary>Its own center of mass, in the shape's space.</summary>
    public Vector2 Center { get; }

    /// <summary>A circle's radius, or for a polygon the distance from <see cref="Center"/> to its farthest corner.</summary>
    public float Radius { get; }

    /// <summary>Its area.</summary>
    public float Area { get; }

    /// <summary>The second moment of its area about <see cref="Center"/>: its moment of inertia at a density of 1.</summary>
    public float Inertia { get; }

    /// <summary>A polygon's corners in the shape's space, clockwise on screen (y-down); empty for a circle.</summary>
    public ReadOnlySpan<Vector2> Vertices => VertexArray;

    /// <summary>
    /// A polygon's outward unit normals in the shape's space: <c>Normals[i]</c> is the edge from <c>Vertices[i]</c> to the
    /// next corner. Empty for a circle.
    /// </summary>
    public ReadOnlySpan<Vector2> Normals => NormalArray;

    internal static ConvexPart Circle(Vector2 center, float radius)
    {
        float area = MathF.PI * radius * radius;
        return new ConvexPart(ShapeKind.Circle, center, radius, area, area * radius * radius * 0.5f, None, None);
    }

    // The corners have to be convex, with positive signed area (clockwise on screen) and no repeats; Shape.Polygon sees to
    // that. They're moved so the polygon's centroid is at the origin, as a simple shape's center of mass has to be.
    internal static ConvexPart Polygon(Vector2[] vertices)
    {
        int count = vertices.Length;
        float area = 0, inertia = 0;
        Vector2 centroid = Vector2.Zero;
        Vector2 origin = vertices[0];
        for (int i = 1; i < count - 1; i++)
        {
            // A fan of triangles from the first corner keeps the sums small for polygons far from the origin.
            Vector2 e1 = vertices[i] - origin, e2 = vertices[i + 1] - origin;
            float cross = Cross(e1, e2);
            float triangle = cross * 0.5f;
            area += triangle;
            centroid += triangle * (e1 + e2) / 3f;
            inertia += cross / 12f * (Vector2.Dot(e1, e1) + Vector2.Dot(e1, e2) + Vector2.Dot(e2, e2));
        }

        centroid /= area;
        inertia -= area * centroid.LengthSquared();
        centroid += origin;

        Vector2[] centered = new Vector2[count];
        float radius = 0;
        for (int i = 0; i < count; i++)
        {
            centered[i] = vertices[i] - centroid;
            radius = MathF.Max(radius, centered[i].Length());
        }

        return new ConvexPart(ShapeKind.Polygon, Vector2.Zero, radius, area, inertia, centered, NormalsOf(centered));
    }

    // The same part turned and moved; its area and inertia about its own center don't change.
    internal ConvexPart Transform(Vector2 offset, float cos, float sin)
    {
        Vector2 center = offset + Turn(Center, cos, sin);
        if (Kind == ShapeKind.Circle)
        {
            return new ConvexPart(Kind, center, Radius, Area, Inertia, None, None);
        }

        Vector2[] vertices = new Vector2[VertexArray.Length];
        Vector2[] normals = new Vector2[NormalArray.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = offset + Turn(VertexArray[i], cos, sin);
            normals[i] = Turn(NormalArray[i], cos, sin);
        }

        return new ConvexPart(Kind, center, Radius, Area, Inertia, vertices, normals);
    }

    internal static float Cross(Vector2 a, Vector2 b) => (a.X * b.Y) - (a.Y * b.X);

    private static Vector2 Turn(Vector2 v, float cos, float sin) => new((cos * v.X) - (sin * v.Y), (sin * v.X) + (cos * v.Y));

    private static Vector2[] NormalsOf(Vector2[] vertices)
    {
        Vector2[] normals = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 edge = vertices[i + 1 == vertices.Length ? 0 : i + 1] - vertices[i];
            normals[i] = Vector2.Normalize(new Vector2(edge.Y, -edge.X));
        }

        return normals;
    }
}
