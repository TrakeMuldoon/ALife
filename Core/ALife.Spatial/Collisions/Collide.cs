using System.Numerics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Collisions;

/// <summary>
/// The narrow phase: whether two posed <see cref="Shape"/>s overlap, and if so a <see cref="Contact"/> to push them apart.
/// Safe to call from any number of threads at once; it allocates nothing.
/// </summary>
public static class Collide
{
    // How much deeper the second polygon's best face has to be before it's used as the reference instead of the first's, so
    // the choice doesn't flicker between two nearly equal faces from frame to frame.
    private const float FaceBias = 0.98f;
    private const float FaceTolerance = 0.001f;

    /// <summary>
    /// Tests two shapes for overlap. A compound gives the deepest contact between any of its parts and any of the other
    /// shape's, so one resting on two parts at once is pushed by the deeper of the two each frame.
    /// </summary>
    /// <param name="a">The first shape.</param>
    /// <param name="poseA">Where the first shape is.</param>
    /// <param name="b">The second shape.</param>
    /// <param name="poseB">Where the second shape is.</param>
    /// <param name="contact">The contact, with its normal from <paramref name="a"/> into <paramref name="b"/>; default if none.</param>
    /// <returns>True if the shapes overlap or touch.</returns>
    public static bool Test(Shape a, in Pose poseA, Shape b, in Pose poseB, out Contact contact)
    {
        contact = default;
        float reach = a.BoundingRadius + b.BoundingRadius;
        if (Vector2.DistanceSquared(poseA.Position, poseB.Position) > reach * reach)
        {
            return false;
        }

        Span<Vector2> verticesA = stackalloc Vector2[Shape.MaxVertices];
        Span<Vector2> normalsA = stackalloc Vector2[Shape.MaxVertices];
        Span<Vector2> verticesB = stackalloc Vector2[Shape.MaxVertices];
        Span<Vector2> normalsB = stackalloc Vector2[Shape.MaxVertices];
        bool found = false;
        foreach (ConvexPart partA in a.PartArray)
        {
            Vector2 centerA = poseA.Apply(partA.Center);
            bool placedA = false;
            foreach (ConvexPart partB in b.PartArray)
            {
                Vector2 centerB = poseB.Apply(partB.Center);
                float radii = partA.Radius + partB.Radius;
                if (Vector2.DistanceSquared(centerA, centerB) > radii * radii)
                {
                    continue;
                }

                bool hit;
                Contact candidate;
                if (partA.Kind == ShapeKind.Circle && partB.Kind == ShapeKind.Circle)
                {
                    hit = Circles(centerA, partA.Radius, centerB, partB.Radius, out candidate);
                }
                else if (partA.Kind == ShapeKind.Circle)
                {
                    int countB = Place(in partB, poseB, verticesB, normalsB);
                    hit = CirclePolygon(verticesB[..countB], normalsB[..countB], centerA, partA.Radius, out candidate);
                    candidate = candidate.Flip();
                }
                else
                {
                    if (!placedA)
                    {
                        Place(in partA, poseA, verticesA, normalsA);
                        placedA = true;
                    }

                    int countA = partA.VertexArray.Length;
                    if (partB.Kind == ShapeKind.Circle)
                    {
                        hit = CirclePolygon(verticesA[..countA], normalsA[..countA], centerB, partB.Radius, out candidate);
                    }
                    else
                    {
                        int countB = Place(in partB, poseB, verticesB, normalsB);
                        hit = Polygons(verticesA[..countA], normalsA[..countA], verticesB[..countB], normalsB[..countB], out candidate);
                    }
                }

                if (hit && (!found || candidate.Depth > contact.Depth))
                {
                    contact = candidate;
                    found = true;
                }
            }
        }

        return found;
    }

    // A polygon part's corners and normals in the world; returns how many.
    private static int Place(in ConvexPart part, in Pose pose, Span<Vector2> vertices, Span<Vector2> normals)
    {
        Vector2[] localVertices = part.VertexArray, localNormals = part.NormalArray;
        for (int i = 0; i < localVertices.Length; i++)
        {
            vertices[i] = pose.Apply(localVertices[i]);
            normals[i] = pose.Rotate(localNormals[i]);
        }

        return localVertices.Length;
    }

    private static bool Circles(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB, out Contact contact)
    {
        Vector2 offset = centerB - centerA;
        float radii = radiusA + radiusB;
        float distanceSquared = offset.LengthSquared();
        if (distanceSquared > radii * radii)
        {
            contact = default;
            return false;
        }

        // Circles on the same spot can be pushed apart any way; along x is as good as another.
        float distance = MathF.Sqrt(distanceSquared);
        Vector2 normal = distance > 1e-6f ? offset / distance : Vector2.UnitX;
        float depth = radii - distance;
        contact = new Contact(normal, depth, centerA + (normal * (radiusA - (depth * 0.5f))));
        return true;
    }

    // The contact's normal points from the polygon to the circle.
    private static bool CirclePolygon(
        ReadOnlySpan<Vector2> vertices, ReadOnlySpan<Vector2> normals, Vector2 center, float radius, out Contact contact)
    {
        contact = default;

        // The face the center is farthest outside of, or least inside.
        float separation = float.NegativeInfinity;
        int face = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector2.Dot(normals[i], center - vertices[i]);
            if (distance > radius)
            {
                return false;
            }

            if (distance > separation)
            {
                separation = distance;
                face = i;
            }
        }

        Vector2 v1 = vertices[face], v2 = vertices[face + 1 == vertices.Length ? 0 : face + 1];
        Vector2 normal = normals[face];
        float depth = radius - separation;

        // Outside the face but past one of its ends, the closest point is that corner rather than the face.
        if (separation > 1e-6f)
        {
            bool pastV1 = Vector2.Dot(center - v1, v2 - v1) <= 0;
            if (pastV1 || Vector2.Dot(center - v2, v1 - v2) <= 0)
            {
                Vector2 offset = center - (pastV1 ? v1 : v2);
                float distanceSquared = offset.LengthSquared();
                if (distanceSquared > radius * radius)
                {
                    return false;
                }

                float distance = MathF.Sqrt(distanceSquared);
                if (distance > 1e-6f)
                {
                    normal = offset / distance;
                }

                depth = radius - distance;
            }
        }

        contact = new Contact(normal, depth, center - (normal * (radius - (depth * 0.5f))));
        return true;
    }

    // Separating axes over both polygons' faces, then the incident edge clipped to the reference face, as in Box2D.
    private static bool Polygons(
        ReadOnlySpan<Vector2> verticesA, ReadOnlySpan<Vector2> normalsA,
        ReadOnlySpan<Vector2> verticesB, ReadOnlySpan<Vector2> normalsB,
        out Contact contact)
    {
        contact = default;
        float separationA = MaxSeparation(verticesA, normalsA, verticesB, out int edgeA);
        if (separationA > 0)
        {
            return false;
        }

        float separationB = MaxSeparation(verticesB, normalsB, verticesA, out int edgeB);
        if (separationB > 0)
        {
            return false;
        }

        // The reference face is the one the other polygon is least deep through; the incident polygon is clipped against it.
        bool flip = separationB > (FaceBias * separationA) + FaceTolerance;
        ReadOnlySpan<Vector2> referenceVertices = flip ? verticesB : verticesA;
        ReadOnlySpan<Vector2> incidentVertices = flip ? verticesA : verticesB;
        ReadOnlySpan<Vector2> incidentNormals = flip ? normalsA : normalsB;
        int edge = flip ? edgeB : edgeA;
        Vector2 normal = flip ? normalsB[edgeB] : normalsA[edgeA];

        // The incident edge faces the reference face most squarely.
        int incident = 0;
        float facing = float.PositiveInfinity;
        for (int i = 0; i < incidentNormals.Length; i++)
        {
            float dot = Vector2.Dot(normal, incidentNormals[i]);
            if (dot < facing)
            {
                facing = dot;
                incident = i;
            }
        }

        Vector2 p0 = incidentVertices[incident];
        Vector2 p1 = incidentVertices[incident + 1 == incidentVertices.Length ? 0 : incident + 1];
        Vector2 v1 = referenceVertices[edge];
        Vector2 v2 = referenceVertices[edge + 1 == referenceVertices.Length ? 0 : edge + 1];
        Vector2 tangent = Vector2.Normalize(v2 - v1);
        if (!Clip(ref p0, ref p1, -tangent, -Vector2.Dot(tangent, v1)) || !Clip(ref p0, ref p1, tangent, Vector2.Dot(tangent, v2)))
        {
            return false;
        }

        // Each clipped point behind the reference face is a contact; the point is halfway between it and the face.
        float depth = 0;
        int points = 0;
        Vector2 sum = Vector2.Zero;
        foreach (Vector2 point in (ReadOnlySpan<Vector2>)[p0, p1])
        {
            float separation = Vector2.Dot(normal, point - v1);
            if (separation <= 0)
            {
                sum += point - (normal * (separation * 0.5f));
                depth = MathF.Max(depth, -separation);
                points++;
            }
        }

        if (points == 0)
        {
            return false;
        }

        contact = new Contact(flip ? -normal : normal, depth, sum / points);
        return true;
    }

    // The face of the first polygon the second is farthest outside of, or least inside: positive means they're apart.
    private static float MaxSeparation(
        ReadOnlySpan<Vector2> vertices, ReadOnlySpan<Vector2> normals, ReadOnlySpan<Vector2> others, out int edge)
    {
        edge = 0;
        float best = float.NegativeInfinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 normal = normals[i], vertex = vertices[i];
            float deepest = float.PositiveInfinity;
            foreach (Vector2 other in others)
            {
                deepest = MathF.Min(deepest, Vector2.Dot(normal, other - vertex));
            }

            if (deepest > best)
            {
                best = deepest;
                edge = i;
                if (best > 0)
                {
                    break;
                }
            }
        }

        return best;
    }

    // Keeps the part of a segment on the inner side of a line, where Dot(normal, point) <= offset; false if none is.
    private static bool Clip(ref Vector2 p0, ref Vector2 p1, Vector2 normal, float offset)
    {
        float d0 = Vector2.Dot(normal, p0) - offset, d1 = Vector2.Dot(normal, p1) - offset;
        if (d0 > 0 && d1 > 0)
        {
            return false;
        }

        if (d0 > 0)
        {
            p0 += (p1 - p0) * (d0 / (d0 - d1));
        }
        else if (d1 > 0)
        {
            p1 += (p0 - p1) * (d1 / (d1 - d0));
        }

        return true;
    }
}
