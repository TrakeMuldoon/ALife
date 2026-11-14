# CollisionDetector

`ALife.Core.Collision` · ALife.Core · [source](../../../Core/ALife.Core/Collision/CollisionDetector.cs)

`static class CollisionDetector`

The narrow phase for ALife's shapes. Once a collision map has found the objects whose boxes overlap, this checks whether their shapes really do. Every pairing of `Circle`, `Rectangle`, `AARectangle` and `Sector` works, child shapes included, through the exact tests in [Overlap](../../Spatial/Collisions/Overlap.md). Shapes that only touch overlap.

It reads each shape's centre, orientation and size as they are now. It doesn't use the corners and box a shape caches until `IShape.Reset`, so a shape that has moved since then is tested where it is. Shapes are converted to floats relative to the first shape's centre, so worlds far from zero keep their precision.

[SpatialCollisionMap](SpatialCollisionMap.md)'s `DetectCollisions` calls it for you; call it directly only to test shapes that aren't on a map.

## API

| Member | Description |
|---|---|
| `static bool Collides(IShape a, IShape b)` | Whether two shapes overlap or touch. |
| `static List<T> FineGrainedCollisionDetection<T>(IEnumerable<T> toCollide, IShape me) where T : IHasShape` | A new list of the objects whose shapes overlap or touch `me`, in the order given. |

Any other kind of shape throws `NotSupportedException`.

## Changes from the old detector

The old detector tested sectors against rectangles one edge at a time. It kept a circle crossing if its x was between the edge's ends, so for an upright edge it tested the whole infinite line. A sight sector could then see a wall that was above or below it. The new tests are exact, and `AARectangle`s, which the old one threw `NotImplementedException` on, work too.

That changes the seeded results of scenarios where agents look at axis-aligned walls, such as `CarTrackMaze` and `FieldCrossingScenario`.

## Example

```csharp
using ALife.Core;
using ALife.Core.Collision;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;

Sector eye = new Sector(new Point(100, 100), 50, new Angle(90), Colour.White);
eye.Orientation = new Angle(-45);   // looking right
AARectangle wall = new AARectangle(new Point(140, 40), 5, 120, Colour.Grey);

bool seesWall = CollisionDetector.Collides(eye, wall);   // true
```

## See also

- [SpatialCollisionMap](SpatialCollisionMap.md): finds the candidates
- [Overlap](../../Spatial/Collisions/Overlap.md): the tests underneath
