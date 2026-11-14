# Sector

`ALife.Spatial.Shapes` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Shapes/Sector.cs)

`readonly struct Sector(Vector2 apex, float start, float sweep, float radius)`

A slice of a disc in the world: the points no further than `Radius` from `Apex` whose direction from it is within `Sweep` of `Start`. The space is y-down, as on screen, so the sweep turns clockwise. Any sweep from none to a full turn works, including ones over half a turn, which aren't convex.

It's for areas that only need to know what they overlap, such as an agent's eye, through [Overlap](../Collisions/Overlap.md). Unlike a [Shape](Shape.md) it has no mass and makes no contacts, and it's placed in the world directly rather than through a [Pose](Pose.md).

## API

| Member | Description |
|---|---|
| `Sector(Vector2 apex, float start, float sweep, float radius)` | Angles are in radians. A `sweep` of a full turn or more is the whole disc, and is stored as one turn. Throws `ArgumentOutOfRangeException` if `sweep` or `radius` is negative. |
| `Vector2 Apex` | The point of the slice: the disc's center. |
| `float Start` | The direction the first straight edge leaves the apex in, clockwise on screen from +x. |
| `float Sweep` | How far round from `Start` the slice reaches, from zero to a full turn. |
| `float Radius` | How far from the apex it reaches. |
| `Vector2 StartDirection`, `Vector2 EndDirection` | Unit directions of the two straight edges, at `Start` and at `Start + Sweep`. |
| `Vector2 StartPoint`, `Vector2 EndPoint` | Where the straight edges meet the arc. |
| `bool IsFull` | Whether the sweep is a full turn. |
| `bool InSweep(Vector2 direction)` | Whether a direction from the apex is within the sweep, edges included. The zero vector is. A zero sweep holds only `StartDirection`, not its opposite. |
| `bool Contains(Vector2 point)` | Whether a point is in the sector, edges included. |
| `Aabb GetBounds()` | The smallest [Aabb](../Aabb.md) around it: the apex, the ends of the arc, and wherever the arc passes due right, down, left or up. |

## Example

```csharp
using System.Numerics;
using ALife.Spatial;
using ALife.Spatial.Shapes;

// An eye at (100, 100) looking right, a quarter turn wide, seeing 50 units.
Sector eye = new(new Vector2(100, 100), -MathF.PI / 4, MathF.PI / 2, 50);

bool ahead = eye.Contains(new Vector2(140, 110));   // true
bool behind = eye.Contains(new Vector2(60, 100));   // false
Aabb box = eye.GetBounds();                         // left 100, right 150, top and bottom about 64.6 and 135.4
```

## See also

- [Overlap](../Collisions/Overlap.md): what a sector overlaps
- [SpatialCollisionMap](../../Core/Collision/SpatialCollisionMap.md): ALife's sight sectors go through it
