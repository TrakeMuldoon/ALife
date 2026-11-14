using System.Numerics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Collisions;

/// <summary>
/// Yes-or-no overlap tests between circles, convex polygons, line segments and <see cref="Sector"/>s in the world, for
/// things that only need to know what they touch, such as senses, rather than how to push it away, which
/// <see cref="Collide"/> works out. Shapes that only touch overlap. Polygons are their corners in order round the edge,
/// either way round. Safe to call from any number of threads at once; it allocates nothing.
/// </summary>
public static class Overlap
{
    /// <summary>Whether two circles overlap.</summary>
    /// <param name="centerA">The first circle's center.</param>
    /// <param name="radiusA">The first circle's radius.</param>
    /// <param name="centerB">The second circle's center.</param>
    /// <param name="radiusB">The second circle's radius.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool Circles(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
    {
        float radii = radiusA + radiusB;
        return Vector2.DistanceSquared(centerA, centerB) <= radii * radii;
    }

    /// <summary>Whether a convex polygon holds a point, edges included.</summary>
    /// <param name="polygon">The polygon's corners in order, either way round.</param>
    /// <param name="point">The point.</param>
    /// <returns>True if the point is inside or on the edge; always false for fewer than three corners.</returns>
    public static bool PolygonContains(ReadOnlySpan<Vector2> polygon, Vector2 point)
    {
        if (polygon.Length < 3)
        {
            return false;
        }

        // Inside a convex polygon, the point is on the same side of every edge.
        bool left = false, right = false;
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 start = polygon[i], end = polygon[i + 1 == polygon.Length ? 0 : i + 1];
            float side = ConvexPart.Cross(end - start, point - start);
            left |= side > 0;
            right |= side < 0;
            if (left && right)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Whether a circle and a convex polygon overlap.</summary>
    /// <param name="center">The circle's center.</param>
    /// <param name="radius">The circle's radius.</param>
    /// <param name="polygon">The polygon's corners in order, either way round.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool CirclePolygon(Vector2 center, float radius, ReadOnlySpan<Vector2> polygon)
    {
        if (PolygonContains(polygon, center))
        {
            return true;
        }

        for (int i = 0; i < polygon.Length; i++)
        {
            if (SegmentCircle(polygon[i], polygon[i + 1 == polygon.Length ? 0 : i + 1], center, radius))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether two convex polygons overlap, by looking for a gap across each edge of either.</summary>
    /// <param name="a">The first polygon's corners in order, either way round.</param>
    /// <param name="b">The second polygon's corners in order, either way round.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool Polygons(ReadOnlySpan<Vector2> a, ReadOnlySpan<Vector2> b)
    {
        return !HasSeparatingEdge(a, b) && !HasSeparatingEdge(b, a);
    }

    /// <summary>Whether a line segment and a circle overlap.</summary>
    /// <param name="start">One end of the segment.</param>
    /// <param name="end">The other end.</param>
    /// <param name="center">The circle's center.</param>
    /// <param name="radius">The circle's radius.</param>
    /// <returns>True if some point of the segment is no further than the radius from the center.</returns>
    public static bool SegmentCircle(Vector2 start, Vector2 end, Vector2 center, float radius)
    {
        Vector2 along = end - start;
        float lengthSquared = along.LengthSquared();
        float t = lengthSquared > 0 ? Math.Clamp(Vector2.Dot(center - start, along) / lengthSquared, 0, 1) : 0;
        return Vector2.DistanceSquared(start + (along * t), center) <= radius * radius;
    }

    /// <summary>Whether two line segments meet, ends and overlapping lengths included.</summary>
    /// <param name="startA">One end of the first segment.</param>
    /// <param name="endA">The other end of the first segment.</param>
    /// <param name="startB">One end of the second segment.</param>
    /// <param name="endB">The other end of the second segment.</param>
    /// <returns>True if they share a point.</returns>
    public static bool Segments(Vector2 startA, Vector2 endA, Vector2 startB, Vector2 endB)
    {
        float d1 = ConvexPart.Cross(endA - startA, startB - startA);
        float d2 = ConvexPart.Cross(endA - startA, endB - startA);
        float d3 = ConvexPart.Cross(endB - startB, startA - startB);
        float d4 = ConvexPart.Cross(endB - startB, endA - startB);
        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
        {
            return true;
        }

        // Otherwise they only meet where an end of one lies on the other.
        return (d1 == 0 && OnSegment(startA, endA, startB))
            || (d2 == 0 && OnSegment(startA, endA, endB))
            || (d3 == 0 && OnSegment(startB, endB, startA))
            || (d4 == 0 && OnSegment(startB, endB, endA));
    }

    /// <summary>Whether a sector and a circle overlap.</summary>
    /// <param name="sector">The sector.</param>
    /// <param name="center">The circle's center.</param>
    /// <param name="radius">The circle's radius.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool SectorCircle(in Sector sector, Vector2 center, float radius)
    {
        Vector2 offset = center - sector.Apex;
        if (sector.InSweep(offset))
        {
            // Straight out from the apex towards the center is all sector, as far as the radius.
            float reach = sector.Radius + radius;
            return offset.LengthSquared() <= reach * reach;
        }

        // Outside the sweep, the sector's nearest point is on one of its straight edges, as the arc's nearest is at its ends.
        return SegmentCircle(sector.Apex, sector.StartPoint, center, radius) || SegmentCircle(sector.Apex, sector.EndPoint, center, radius);
    }

    /// <summary>Whether a sector and a line segment overlap.</summary>
    /// <param name="sector">The sector.</param>
    /// <param name="start">One end of the segment.</param>
    /// <param name="end">The other end.</param>
    /// <returns>True if they share a point.</returns>
    public static bool SectorSegment(in Sector sector, Vector2 start, Vector2 end)
    {
        // A segment with neither end in the sector has to cross its edge to reach it: a straight edge or the arc.
        if (sector.Contains(start) || sector.Contains(end)
            || Segments(sector.Apex, sector.StartPoint, start, end) || Segments(sector.Apex, sector.EndPoint, start, end))
        {
            return true;
        }

        // Where the segment meets the whole circle, from |start + t * along - apex|² = radius².
        Vector2 along = end - start, from = start - sector.Apex;
        float a = along.LengthSquared();
        if (a == 0)
        {
            return false;
        }

        float b = Vector2.Dot(from, along);
        float c = from.LengthSquared() - (sector.Radius * sector.Radius);
        float discriminant = (b * b) - (a * c);
        if (discriminant < 0)
        {
            return false;
        }

        float root = MathF.Sqrt(discriminant);
        return OnArc(sector, start, along, (-b - root) / a) || OnArc(sector, start, along, (-b + root) / a);
    }

    /// <summary>Whether a sector and a convex polygon overlap.</summary>
    /// <param name="sector">The sector.</param>
    /// <param name="polygon">The polygon's corners in order, either way round.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool SectorPolygon(in Sector sector, ReadOnlySpan<Vector2> polygon)
    {
        // Unless an edge of the polygon meets the sector, one is wholly inside the other, and only the polygon can hold the apex.
        if (PolygonContains(polygon, sector.Apex))
        {
            return true;
        }

        for (int i = 0; i < polygon.Length; i++)
        {
            if (SectorSegment(sector, polygon[i], polygon[i + 1 == polygon.Length ? 0 : i + 1]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether two sectors overlap.</summary>
    /// <param name="a">The first sector.</param>
    /// <param name="b">The second sector.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool Sectors(in Sector a, in Sector b)
    {
        // One wholly inside the other holds its apex; otherwise an edge of one crosses an edge of the other.
        if (b.Contains(a.Apex) || a.Contains(b.Apex)
            || SectorSegment(b, a.Apex, a.StartPoint) || SectorSegment(b, a.Apex, a.EndPoint)
            || SectorSegment(a, b.Apex, b.StartPoint) || SectorSegment(a, b.Apex, b.EndPoint))
        {
            return true;
        }

        // What's left is the two arcs crossing, where the circles meet.
        Vector2 between = b.Apex - a.Apex;
        float distance = between.Length();
        if (distance == 0 || distance > a.Radius + b.Radius || distance < MathF.Abs(a.Radius - b.Radius))
        {
            return false;
        }

        Vector2 axis = between / distance;
        float along = ((a.Radius * a.Radius) - (b.Radius * b.Radius) + (distance * distance)) / (2 * distance);
        float across = MathF.Sqrt(MathF.Max(0, (a.Radius * a.Radius) - (along * along)));
        Vector2 middle = a.Apex + (axis * along), side = new Vector2(-axis.Y, axis.X) * across;
        return OnBothArcs(a, b, middle + side) || OnBothArcs(a, b, middle - side);
    }

    /// <summary>
    /// Whether a sector and a posed <see cref="Shape"/> overlap: any of the shape's parts. The shape's bounding circle is
    /// tried first, so a far one costs one distance.
    /// </summary>
    /// <param name="sector">The sector.</param>
    /// <param name="shape">The shape.</param>
    /// <param name="pose">Where the shape is.</param>
    /// <returns>True if they overlap or touch.</returns>
    public static bool Test(in Sector sector, Shape shape, in Pose pose)
    {
        if (!Circles(sector.Apex, sector.Radius, pose.Position, shape.BoundingRadius))
        {
            return false;
        }

        Span<Vector2> corners = stackalloc Vector2[Shape.MaxVertices];
        foreach (ConvexPart part in shape.PartArray)
        {
            if (part.Kind == ShapeKind.Circle)
            {
                if (SectorCircle(sector, pose.Apply(part.Center), part.Radius))
                {
                    return true;
                }

                continue;
            }

            Vector2[] vertices = part.VertexArray;
            for (int i = 0; i < vertices.Length; i++)
            {
                corners[i] = pose.Apply(vertices[i]);
            }

            if (SectorPolygon(sector, corners[..vertices.Length]))
            {
                return true;
            }
        }

        return false;
    }

    // Whether some edge of the first polygon has the whole of the second strictly beyond it, across the edge's line.
    private static bool HasSeparatingEdge(ReadOnlySpan<Vector2> first, ReadOnlySpan<Vector2> second)
    {
        for (int i = 0; i < first.Length; i++)
        {
            Vector2 start = first[i], edge = first[i + 1 == first.Length ? 0 : i + 1] - start;
            Vector2 axis = new(-edge.Y, edge.X);
            float minFirst = float.PositiveInfinity, maxFirst = float.NegativeInfinity;
            foreach (Vector2 point in first)
            {
                float projection = Vector2.Dot(axis, point - start);
                minFirst = MathF.Min(minFirst, projection);
                maxFirst = MathF.Max(maxFirst, projection);
            }

            float minSecond = float.PositiveInfinity, maxSecond = float.NegativeInfinity;
            foreach (Vector2 point in second)
            {
                float projection = Vector2.Dot(axis, point - start);
                minSecond = MathF.Min(minSecond, projection);
                maxSecond = MathF.Max(maxSecond, projection);
            }

            if (minSecond > maxFirst || maxSecond < minFirst)
            {
                return true;
            }
        }

        return false;
    }

    // Whether a point already known to be on a segment's line is within its ends.
    private static bool OnSegment(Vector2 start, Vector2 end, Vector2 point)
    {
        return point.X >= MathF.Min(start.X, end.X) && point.X <= MathF.Max(start.X, end.X)
            && point.Y >= MathF.Min(start.Y, end.Y) && point.Y <= MathF.Max(start.Y, end.Y);
    }

    // Whether the point t of the way along a segment is on it and on the sector's arc, given it's on the circle.
    private static bool OnArc(in Sector sector, Vector2 start, Vector2 along, float t)
    {
        return t >= 0 && t <= 1 && sector.InSweep(start + (along * t) - sector.Apex);
    }

    // Whether a point on both circles is within both sweeps.
    private static bool OnBothArcs(in Sector a, in Sector b, Vector2 point)
    {
        return a.InSweep(point - a.Apex) && b.InSweep(point - b.Apex);
    }
}
