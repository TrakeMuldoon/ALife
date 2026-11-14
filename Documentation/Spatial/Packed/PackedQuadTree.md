# PackedQuadTree

`ALife.Spatial.Packed` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Packed/PackedQuadTree.cs)

An [IPackedSpatialIndex](IPackedSpatialIndex.md) loose quadtree rebuilt every frame. The elements are radix-sorted by the Morton (Z-order) code of their centers, which lays out every node's elements as one unbroken run, and the nodes are then cut from that order top-down. Each node keeps a box around its elements, as in [LooseQuadTree](../LooseQuadTree.md), so it handles sizes that vary widely, big elements included, with nothing to tune.

Building takes a few milliseconds for hundreds of thousands of elements, and the tree never drifts out of shape, since it's new each frame. Searches walk the tree, so they're slower than a [PackedGrid](PackedGrid.md)'s when elements are all of a similar size. With 200,000 circles of radius 3 on 1280×720, it builds in about 5 ms and finds every pair in about 12 ms in parallel.

## API

Everything in [IPackedSpatialIndex](IPackedSpatialIndex.md), plus:

| Member | Description |
|---|---|
| `const int MaxSupportedDepth` | 16: centers are placed on a 2¹⁶ × 2¹⁶ grid to sort them. |
| `PackedQuadTree(Aabb bounds, int maxElements = 8, int maxDepth = 8)` | An empty tree. Centers outside `bounds` are filed at its edges. A leaf splits once it holds more than `maxElements`, unless it's at `maxDepth` (0 is just the root). Throws `ArgumentOutOfRangeException` if `maxElements` is under 1 or `maxDepth` is outside 0 to `MaxSupportedDepth`, and `ArgumentException` if `bounds` has no area. |
| `int MaxElements` | How many elements a leaf holds before it splits. |
| `int MaxDepth` | How deep the tree can go. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports every node with its loose box, which is `Aabb.Empty` for an empty leaf. Unlike the searches, it isn't safe to run alongside another `VisitNodes` call on the same tree. |

## Example

```csharp
using ALife.Spatial;
using ALife.Spatial.Packed;

PackedQuadTree tree = new(new Aabb(0, 0, 1280, 720), maxElements: 8, maxDepth: 10);
tree.Build(boxes);

tree.VisitNodes((bounds, depth, isLeaf, count) =>
{
    if (!bounds.IsEmpty)
    {
        DrawRectangle(bounds, ColorForDepth(depth));
    }
});
```

See [IPackedSpatialIndex](IPackedSpatialIndex.md#example) for a full frame with `ForEachNeighbor`.

## See also

- [IPackedSpatialIndex](IPackedSpatialIndex.md)
- [PackedGrid](PackedGrid.md)
- [LooseQuadTree](../LooseQuadTree.md): the incrementally updated loose quadtree
