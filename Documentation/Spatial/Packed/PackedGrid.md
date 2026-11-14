# PackedGrid

`ALife.Spatial.Packed` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Packed/PackedGrid.cs)

An [IPackedSpatialIndex](IPackedSpatialIndex.md) grid rebuilt every frame by a counting sort. Each element is filed in one cell by the center of its box, and the boxes are stored cell by cell in flat arrays. That puts a cell and its neighbor to the right, or a row of three cells, in one unbroken run of memory, which the searches scan several boxes at a time with SIMD. As long as no element is bigger than a cell, overlapping elements are always in the same or neighboring cells, so finding every pair only means comparing each cell with half of its neighbors.

It's the fastest packed index when elements are of similar size. With 200,000 circles of radius 3 on 1280×720 and 8-unit cells, it builds in about 2 ms and finds all 2.4 million pairs in about 10 ms in parallel.

### Sizing the cells

Make cells a little bigger than your biggest common element. An element has to be smaller than a cell (by at least 0.1%, to stay clear of rounding) to go in the grid, and a few elements per cell is the sweet spot. For circles of radius `r`, cells of about `2.1 * r` to `3 * r` work well.

Bigger elements still work. They go in a [PackedQuadTree](PackedQuadTree.md) on the side (counted by `OversizedCount`), and each one is compared with the grid by a query over its area. That suits a few big elements among many small ones. If most elements are oversized, the cells are too small and the grid is slower than a `PackedQuadTree` alone.

## API

Everything in [IPackedSpatialIndex](IPackedSpatialIndex.md), plus:

| Member | Description |
|---|---|
| `PackedGrid(Aabb bounds, int columns, int rows)` | An empty grid. Centers outside `bounds` go to the edge cells. Throws `ArgumentOutOfRangeException` if `columns` or `rows` is under 1, and `ArgumentException` if `bounds` has no area. |
| `int Columns`, `int Rows` | |
| `float CellWidth`, `float CellHeight` | The cell size. Elements have to be a little narrower and shorter than this to go in the grid. |
| `int OversizedCount` | How many elements in the last `Build` were too big for a cell and went in the side tree. Check it while tuning: it should be 0 or a small share. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports every cell, at depth 0, as a leaf with its element count. Oversized elements aren't counted. |

## Example

```csharp
using ALife.Spatial;
using ALife.Spatial.Packed;

// Circles up to 6 units across, so cells of 8.
float cell = 8;
Aabb world = new(0, 0, 1280, 720);
PackedGrid grid = new(world, columns: (int)(world.Width / cell), rows: (int)(world.Height / cell));

grid.Build(boxes);
if (grid.OversizedCount > boxes.Length / 100)
{
    // More than 1% didn't fit: the cells are too small for these elements.
}

List<int> hits = new();
grid.Query(Aabb.FromCenter(mouseX, mouseY, 50, 50), hits);   // positions in boxes
```

See [IPackedSpatialIndex](IPackedSpatialIndex.md#example) for a full frame with `ForEachNeighbor`.

## See also

- [IPackedSpatialIndex](IPackedSpatialIndex.md)
- [PackedQuadTree](PackedQuadTree.md)
- [UniformGrid](../UniformGrid.md): the incrementally updated grid
