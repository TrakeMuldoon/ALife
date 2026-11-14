# SpatialCollisionMap

`ALife.Core.Collision` · ALife.Core · [source](../../../Core/ALife.Core/Collision/SpatialCollisionMap.cs)

`sealed class SpatialCollisionMap<T> : ICollisionMap<T> where T : class, IHasShape`

The collision map `Planet` uses for every collision level and for its zone map. Underneath is an `ALife.Spatial` [LayeredGrid](../../Spatial/LayeredGrid.md) of 4 layers, with cells from 25 units across at the finest to 200 at the top. Each object goes in the finest cell that fits it, so agents share small cells and walls and zones sit in big ones, and neither slows the other down.

Fine-grained collisions go through [CollisionDetector](CollisionDetector.md), so every ALife shape works, sectors and axis-aligned rectangles included.

## Performance

It replaced `CollisionGrid<T>`, which kept a list of objects in every 25-unit bucket they covered. Ticks a second in `PerformanceBenchmarkScenario`:

| Agents | `CollisionGrid<T>` and the old detector | This map and the old detector | This map and [CollisionDetector](CollisionDetector.md) |
|---|---:|---:|---:|
| 1,000 | 355 | 524 | 606 |
| 2,500 | 109 | 171 | 212 |
| 5,000 | 47 | 86 | 110 |

The columns were measured on different runs of the same machine, so compare them roughly. Moving and removing objects take constant time, an object as big as the world is stored once, and objects past the world's edge are filed in the edge cells and found. To watch the old grid beside the new indexes, press 0 in the MonoGame demo (see [ISpatialIndex](../../Spatial/ISpatialIndex.md#example)).

Seeded runs of most scenarios come out as they did with `CollisionGrid<T>`. A few don't:

- **Order.** Results come in the grid's order, not by bucket, so code that acts on the first result can pick a different object, e.g. `RabbitScenario`'s collision behaviour and `EyeIdentifierInput`.
- **Edges.** Agents past the world's edge now collide.
- **Sight.** The old detector let sectors see axis-aligned walls they didn't reach (see [CollisionDetector](CollisionDetector.md#changes-from-the-old-detector)), which changes `CarTrackMaze` and `FieldCrossingScenario`.

## Boxes

ALife's shapes cache their bounding boxes until `IShape.Reset`, which objects call at the end of their turn. The map doesn't use that cache: `Insert` and `MoveObject` work out the box from the shape's centre, orientation and size as they are now, and store it. Queries compare against the stored boxes, and `DetectCollisions` then tests each candidate's current shape.

So an object is found where it was last given to `Insert` or `MoveObject`. Call `MoveObject` after every move or turn, as `MoveCluster` and `RotateCluster` do, or others' queries can miss it.

## API

| Member | Description |
|---|---|
| `SpatialCollisionMap(int gridHeight, int gridWidth, string gridName)` | An empty map covering a world. |
| `const int FineCellSize` | 25: the finest cells' size. |
| `int Height`, `int Width`, `string GridName` | As given. |
| `int Count` | How many objects are on the map. |
| `bool Insert(T newObject)` | Adds an object at its shape's box as it is now; always true. Throws `ArgumentException` if it's already on the map. |
| `void RemoveObject(T killMe)` | Throws `KeyNotFoundException` if it isn't on the map. |
| `void MoveObject(T moveMe)` | Stores its shape's box as it is now. |
| `List<T> QueryForBoundingBoxCollisions(T queryObject)` | The objects whose stored boxes overlap its box as it is now, leaving it out. Boxes that only touch don't count. |
| `List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox)` | |
| `List<T> QueryForBoundingBoxCollisions(BoundingBox queryBox, T self)` | Leaving out `self`. |
| `List<T> DetectCollisions(T self)` | The objects whose shapes really touch its shape, through [CollisionDetector](CollisionDetector.md). |
| `List<T> DetectCollisions(IHasShape detector, T self)` | The objects touching `detector`'s shape, leaving out `self`, e.g. for an agent's senses. |
| `IEnumerable<T> EnumerateItems()` | Every object. Forgiving: it stops quietly rather than throwing when objects are removed under it, so a renderer can enumerate while the simulation runs. |

Each query allocates only the list it returns.

## Threads

Queries and `DetectCollisions` can run on several threads at once, as long as nothing adds, moves or removes objects meanwhile. Each thread has its own scratch list. `Insert`, `MoveObject` and `RemoveObject` aren't thread-safe.

## Example

A move inside an agent's action, as `MoveCluster` does it, with `self` the agent and `newCentre` where it's going:

```csharp
using ALife.Core;
using ALife.Core.Collision;
using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects;
using System.Collections.Generic;

Point oldCentre = self.Shape.CentrePoint;

ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[self.CollisionLevel];

self.Shape.CentrePoint = newCentre;
List<WorldObject> collisions = collider.DetectCollisions(self);
if(collisions.Count == 0)
{
    collider.MoveObject(self);
}
else
{
    self.Shape.CentrePoint = oldCentre;
}
```

## See also

- [LayeredGrid](../../Spatial/LayeredGrid.md): the index underneath
- [CollisionDetector](CollisionDetector.md): the narrow phase
- [ALife.Spatial](../../Spatial/README.md): the other indexes, and how they compare
