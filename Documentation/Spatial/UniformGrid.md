# UniformGrid

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/UniformGrid.cs)

An [ISpatialIndex](ISpatialIndex.md) grid for elements that share one upper-bound size, after the "Dirty Trick: Uniform Sizes" in user4842163's answers on Stack Overflow. Every element counts as a box of `HalfWidth` by `HalfHeight` around its center, so only the center is stored and each element sits in exactly one cell. Queries widen the area by the half size to find the centers that matter.

Use it for the many common-sized things in a scene, with the half size set to the largest of them, and keep the rare big ones in a [LooseQuadTree](LooseQuadTree.md) or [LooseTightGrid](LooseTightGrid.md). Results are coarse: they're the elements whose *uniform* boxes intersect the area, ready for your own exact test.

## API

Everything in [ISpatialIndex](ISpatialIndex.md), plus:

| Member | Description |
|---|---|
| `UniformGrid(Aabb bounds, int columns, int rows, float halfWidth, float halfHeight)` | Cells work best a few times bigger than the elements. Centers outside `bounds` go to the edge cells. Throws if `bounds` has no area. |
| `int Columns`, `int Rows` | |
| `float HalfWidth`, `float HalfHeight` | The size every element counts as. |
| `int Insert(int id, Aabb bounds)`, `void Move(int element, Aabb bounds)` | Only the box's center is kept. Throw `ArgumentOutOfRangeException` if the box is bigger than the uniform size. |
| `Aabb GetBounds(int element)` | The uniform-size box around the element's center, not the box it was inserted with. |
| `void Cleanup()` | Does nothing: the grid never changes shape. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports every cell, at depth 0, as a leaf with its element count. |

## Example

```csharp
using ALife.Spatial;

// Particles up to 6 units across.
UniformGrid grid = new(new Aabb(0, 0, 1280, 720), columns: 64, rows: 36, halfWidth: 3, halfHeight: 3);
int handle = grid.Insert(id: 0, Aabb.FromCenter(10, 10, 2, 2));
Aabb stored = grid.GetBounds(handle);    // (7, 7, 13, 13)
```

## See also

- [ISpatialIndex](ISpatialIndex.md)
- [LooseTightGrid](LooseTightGrid.md)
