# Shape

`ALife.Spatial.Shapes` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Shapes/Shape.cs)

A rigid 2D shape: one or more convex [ConvexPart](ConvexPart.md)s laid out around its center of mass, which is the origin of its space. A circle, box or polygon has one part; a compound has several. Shapes never change once made, so one can be shared by any number of bodies, each placed by its own [Pose](Pose.md). The space is y-down, as on screen, so angles turn clockwise.

A shape carries what physics needs at a density of 1: its `Area` (the mass) and `Inertia` (the moment of inertia about its center of mass). Its `BoundingRadius` gives a circle that holds it at any angle, which [Collide.Test](../Collisions/Collide.md) checks before anything else.

## API

| Member | Description |
|---|---|
| `const int MaxVertices` | The most corners a polygon part can have: 16. |
| `ReadOnlySpan<ConvexPart> Parts` | The convex pieces, in the shape's space. |
| `float Area` | The total area. Parts of a compound that overlap count twice. |
| `float Inertia` | The second moment of the area about the center of mass. Multiply by a density for the real moment of inertia, as `RigidBody.FromShape` does. |
| `float BoundingRadius` | The distance from the center of mass to the farthest point. |
| `static Shape Circle(float radius)` | |
| `static Shape Box(float width, float height)` | A rectangle, unturned, with its edges along the axes. |
| `static Shape Square(float size)` | |
| `static Shape Triangle(float side)` | An equilateral triangle with a corner pointing up the screen. |
| `static Shape RegularPolygon(int sides, float radius)` | From 3 to `MaxVertices` sides, with a corner pointing up the screen. `radius` is the distance to each corner. |
| `static Shape Polygon(params ReadOnlySpan<Vector2> points)` | The convex hull of the points, moved so its centroid is the origin. The points needn't be convex or in order; ones inside the hull or on its edges are dropped. |
| `static Shape Compound(params ReadOnlySpan<ShapePart> parts)` | Shapes fixed together. Each one's parts are turned and moved into place, then the whole is moved so its combined center of mass is the origin. A compound inside a compound is flattened into its parts. |
| `Aabb GetBounds(in Pose pose)` | The smallest [Aabb](../Aabb.md) around the shape where a pose puts it, e.g. to file it in an [ISpatialIndex](../ISpatialIndex.md). |

Sizes must be above zero, or the factories throw `ArgumentOutOfRangeException`. `Polygon` throws `ArgumentException` if the hull has no area or more than `MaxVertices` corners, and `Compound` if it's given no parts.

Keep a compound's parts apart, or just touching, for an even density. Because overlapping parts count twice, a compound of overlapping parts is heavier and harder to turn than its outline suggests.

### ShapePart

`readonly record struct ShapePart(Shape Shape, Vector2 Offset, float Rotation = 0)`: one piece of a compound for `Compound`. The piece's center of mass goes at `Offset`, relative to the other pieces, and it's turned by `Rotation` radians, clockwise on screen. Where the pieces end up doesn't matter, only where they are relative to each other, since the compound is recentered.

## Example

```csharp
using System.Numerics;
using ALife.Spatial;
using ALife.Spatial.Shapes;

// One shape for every body of this kind: a dumbbell of two balls on a bar.
Shape dumbbell = Shape.Compound(
    new ShapePart(Shape.Circle(5), new Vector2(-10, 0)),
    new ShapePart(Shape.Circle(5), new Vector2(10, 0)),
    new ShapePart(Shape.Box(10, 3), Vector2.Zero));

// Where one of them is, turned a quarter turn.
Pose pose = new(new Vector2(200, 150), MathF.PI / 2);
Aabb box = dumbbell.GetBounds(pose);   // about (195, 135, 205, 165)
```

## See also

- [ConvexPart](ConvexPart.md): the pieces, and `ShapeKind`
- [Pose](Pose.md)
- [Collide](../Collisions/Collide.md): whether two shapes overlap
- [Sector](Sector.md) and [Overlap](../Collisions/Overlap.md): sectors, and yes-or-no tests against them
- [RigidBody](../Physics/RigidBody.md): making them move
