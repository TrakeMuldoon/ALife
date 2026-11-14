# LooseQuadTree

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/LooseQuadTree.cs)

An [ISpatialIndex](ISpatialIndex.md) loose quadtree after user4842163's "Loose Quadtree" answer on Stack Overflow. Each element is filed in exactly one leaf, chosen by the center of its box, and every node keeps a box around everything below it. A huge element costs no more than a tiny one, so it suits scenes whose element sizes vary wildly. Queries test each child's box rather than a center line, so they're slower than [QuadTree](QuadTree.md)'s.

Node boxes only grow as elements are added or moved; `Cleanup` shrinks them to fit again and frees branches whose children are all empty. Coordinates are floats and aren't rounded.

## API

Everything in [ISpatialIndex](ISpatialIndex.md), plus:

| Member | Description |
|---|---|
| `LooseQuadTree(Aabb bounds, int maxElements = 8, int maxDepth = 8)` | `bounds` sets the fixed cells elements are filed by; centers outside it go to the edge cells. A leaf splits when it has more than `maxElements`, unless it's at `maxDepth`. |
| `int MaxElements` | |
| `int MaxDepth` | |
| `void Cleanup()` | Works bottom-up, so a branch whose children all empty out is freed in the same call. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports each node's loose box, which is `Aabb.Empty` for an empty leaf. |

## Example

```csharp
using ALife.Spatial;

LooseQuadTree tree = new(new Aabb(0, 0, 2000, 2000));
tree.Insert(id: 0, Aabb.FromCenter(50, 50, 2, 2));           // a bullet
int wall = tree.Insert(id: 1, new Aabb(0, 900, 2000, 940));  // a long wall, in one leaf

List<int> hits = new();
tree.Query(Aabb.FromCenter(1800, 920, 10, 10), hits);       // [wall]
```

## See also

- [ISpatialIndex](ISpatialIndex.md)
- [QuadTree](QuadTree.md) · [LooseTightGrid](LooseTightGrid.md)
