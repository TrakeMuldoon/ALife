# QuadTree

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/QuadTree.cs)

An [ISpatialIndex](ISpatialIndex.md) quadtree after user4842163's "Efficient Quadtrees" answers on Stack Overflow. An element is filed in every leaf its box overlaps, so it suits many elements of similar, smallish sizes, and has the fastest queries of the four. For sizes that vary wildly use [LooseQuadTree](LooseQuadTree.md) or [LooseTightGrid](LooseTightGrid.md).

How it's built, as in the answer:

- Nodes are 8 bytes: the first child (or first element-list entry) and an element count, which is -1 for a branch. A branch's four children are next to each other, so one index finds them all.
- Node boxes aren't stored. They're worked out while descending, from the root's center and half size.
- Elements, the per-leaf element lists and groups of four nodes all come from free lists, so a running simulation stops allocating once it reaches its peak size.
- Coordinates are whole numbers. Boxes are rounded down, so queries can return elements up to a unit away, and `GetBounds` returns the rounded box. Scale your world up if you need finer detail.
- Queries mark elements as they add them, so an element in several leaves is returned once.

The leaf search fixes a slip in the answer's `find_leaves`, which computes the top edge as `my - hx` where it means `my - hy`. That only matters for non-square bounds.

## API

Everything in [ISpatialIndex](ISpatialIndex.md), plus:

| Member | Description |
|---|---|
| `QuadTree(Aabb bounds, int maxElements = 8, int maxDepth = 8)` | `bounds` is rounded out to whole units and an even size. A leaf splits when it has more than `maxElements`, unless it's at `maxDepth` (the root is depth 0) or its children would be less than two units across. Aim for the smallest leaves to be a bit bigger than an average element. |
| `int MaxElements` | |
| `int MaxDepth` | Since coordinates are whole numbers, nodes also stop splitting at two units across, however high this is. |
| `void Cleanup()` | A branch whose four children are all empty leaves becomes an empty leaf. That leaf is only freed by the next call, so emptying a whole tree takes up to `MaxDepth` calls; called once a frame, that's spread over frames, and areas emptied for a moment don't throw nodes away just to make them again. |

## Example

```csharp
using ALife.Spatial;

QuadTree tree = new(new Aabb(0, 0, 4096, 4096), maxElements: 8, maxDepth: 8);
int handle = tree.Insert(id: 0, Aabb.FromCenter(100, 100, 4, 4));

List<int> nearby = new();
tree.Query(new Aabb(64, 64, 128, 128), nearby);    // [handle]
```

See [ISpatialIndex](ISpatialIndex.md#example) for a full collision loop.

## See also

- [ISpatialIndex](ISpatialIndex.md)
- [LooseQuadTree](LooseQuadTree.md)
