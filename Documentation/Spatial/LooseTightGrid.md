# LooseTightGrid

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/LooseTightGrid.cs)

An [ISpatialIndex](ISpatialIndex.md) loose/tight double grid after user4842163's "Loose/Tight Double-Grid" answer on Stack Overflow.

- Each element is filed in exactly one **loose** cell, chosen by the center of its box. Each loose cell keeps a box around its elements.
- A coarser **tight** grid lists, for each of its cells, the loose cells whose boxes overlap it. A query only looks at loose cells listed near the area, however far a big element has stretched one.

Inserts, moves and removals take constant time, and memory is nearly fixed: one loose cell per grid cell, and each element in exactly one. Loose boxes only grow between cleanups; `Cleanup` shrinks them and rebuilds the tight grid.

## API

Everything in [ISpatialIndex](ISpatialIndex.md), plus:

| Member | Description |
|---|---|
| `LooseTightGrid(Aabb bounds, int looseColumns, int looseRows, int tightColumns, int tightRows)` | Aim for loose cells about the size of a common element, and a tight grid a few times coarser. Centers outside `bounds` go to the edge cells. Throws if `bounds` has no area. |
| `int LooseColumns`, `int LooseRows` | |
| `int TightColumns`, `int TightRows` | |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports the tight cells first, at depth 0 and not as leaves, with how many loose cells each lists; then each non-empty loose cell's box, at depth 1, as a leaf with its element count. |

## Example

```csharp
using ALife.Spatial;

// A 1280x720 world: 10-unit loose cells, 40-unit tight cells.
LooseTightGrid grid = new(new Aabb(0, 0, 1280, 720), looseColumns: 128, looseRows: 72, tightColumns: 32, tightRows: 18);
int handle = grid.Insert(id: 7, Aabb.FromCenter(300, 200, 5, 5));
grid.Move(handle, Aabb.FromCenter(305, 200, 5, 5));
grid.Cleanup();
```

## See also

- [ISpatialIndex](ISpatialIndex.md)
- [LooseQuadTree](LooseQuadTree.md) · [UniformGrid](UniformGrid.md)
