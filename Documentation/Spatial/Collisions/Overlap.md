# Overlap

`ALife.Spatial.Collisions` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Collisions/Overlap.cs)

Yes-or-no overlap tests between circles, convex polygons, line segments and [Sector](../Shapes/Sector.md)s in the world. It's for things that only need to know what they touch, such as senses. To find out how to push two shapes apart, use [Collide](Collide.md) instead.

It's a static class. Every test allocates nothing and keeps no state, so any number of threads can call it at once.

- **Touching counts.** Shapes that only meet at an edge or a point overlap.
- **Polygons** are their corners in order round the edge, either way round, as a `ReadOnlySpan<Vector2>`. They must be convex. A polygon with fewer than 3 corners holds no points.

## API

| Member | Description |
|---|---|
| `static bool Circles(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)` | |
| `static bool PolygonContains(ReadOnlySpan<Vector2> polygon, Vector2 point)` | Edges included. |
| `static bool CirclePolygon(Vector2 center, float radius, ReadOnlySpan<Vector2> polygon)` | |
| `static bool Polygons(ReadOnlySpan<Vector2> a, ReadOnlySpan<Vector2> b)` | Looks for a gap across each edge of either polygon. |
| `static bool SegmentCircle(Vector2 start, Vector2 end, Vector2 center, float radius)` | |
| `static bool Segments(Vector2 startA, Vector2 endA, Vector2 startB, Vector2 endB)` | Ends and overlapping lengths included. |
| `static bool SectorCircle(in Sector sector, Vector2 center, float radius)` | |
| `static bool SectorSegment(in Sector sector, Vector2 start, Vector2 end)` | Finds a segment that crosses only the arc, at any angle, vertical included. |
| `static bool SectorPolygon(in Sector sector, ReadOnlySpan<Vector2> polygon)` | |
| `static bool Sectors(in Sector a, in Sector b)` | Includes two sectors whose arcs cross while their straight edges don't meet. |
| `static bool Test(in Sector sector, Shape shape, in Pose pose)` | Whether a sector overlaps any part of a posed [Shape](../Shapes/Shape.md). The shape's bounding circle is tried first, so a far one costs one distance. |

How the sector tests work:

- **Circle.** If the circle's center is within the sweep, it's the distance to the apex against the two radii. Otherwise the circle has to reach one of the straight edges.
- **Segment.** An end is inside, or the segment crosses a straight edge, or it meets the arc where the arc is within the sweep.
- **Polygon.** The apex is inside the polygon, or any edge overlaps the sector as a segment.
- **Sector.** Either apex is inside the other, or a straight edge of either overlaps the other, or the two circles cross at a point within both sweeps.

## Example

```csharp
using System.Numerics;
using ALife.Spatial.Collisions;
using ALife.Spatial.Shapes;

Sector eye = new(new Vector2(100, 100), -MathF.PI / 4, MathF.PI / 2, 50);   // looking right
Vector2[] wall = [new(140, 40), new(145, 40), new(145, 160), new(140, 160)];

bool seesWall = Overlap.SectorPolygon(eye, wall);                              // true
bool seesFood = Overlap.SectorCircle(eye, new Vector2(100, 160), 5);           // false: below, outside the sweep
bool seesCrate = Overlap.Test(eye, Shape.Box(10, 10), new Pose(new Vector2(120, 100), 0));   // true
```

## See also

- [Sector](../Shapes/Sector.md)
- [Collide](Collide.md): overlap with a contact, for physics
- [CollisionDetector](../../Core/Collision/CollisionDetector.md): ALife.Core's shapes, tested through these
