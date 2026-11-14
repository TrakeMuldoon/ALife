# PackedLayeredGrid

`ALife.Spatial.Packed` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Packed/PackedLayeredGrid.cs)

An [IPackedSpatialIndex](IPackedSpatialIndex.md) made of a stack of uniform grids, rebuilt every frame. Each layer's cells are a quarter the size of the one above: half as wide and half as tall. Each element goes in the finest layer whose cells it fits in, by the center of its box, so small elements share small cells and big ones share big cells. Each layer is counting-sorted into flat arrays, as in a [PackedGrid](PackedGrid.md), and searched with SIMD.

Use it when sizes vary but stay within a known range. It handles mixed sizes without the [PackedGrid](PackedGrid.md)'s side tree and without the [PackedQuadTree](PackedQuadTree.md)'s tree walk. `ForEachNeighbor` splits its work by top-layer cell over a set number of threads (`Threads`, 4 by default), so you choose how many cores it takes.

Measured on a 14-core machine with 1280×720 and 4 layers:

| 200,000 circles | Build | `ForEachNeighbor`, 4 threads | `ForEachNeighbor`, 14 threads |
|---|---|---|---|
| radius 3 | 2.2 ms | 18 ms | 6–7 ms |
| radius 3 to 12, mostly small | 3.6 ms | 66 ms | 24 ms |

With every core in use, it's a little faster than a `PackedGrid` on same-sized circles. On mixed sizes it beats both other packed types: a `PackedGrid` takes about 46 ms in all and a `PackedQuadTree` about 35 ms.

### Sizing the layers

Size the top layer's cells a little bigger than your biggest common element, then add layers until the finest cells are a little bigger than your smallest common element. For circles of radius `r` to `R`, top cells of about `2.1 * R` and enough layers to get down to about `2.1 * r` work well. An element has to be smaller than a cell (by at least 0.1%, to stay clear of rounding) to go in that layer.

Elements too big even for a top cell still work. They go in the top layer and are counted by `OversizedCount`, but every search of the top layer then looks as far as the biggest one reaches. A few are fine. Many mean the top cells are too small.

Layers that hold nothing are skipped, but each still costs a pass over its cell counts in every `Build`. Don't add layers far below your smallest element.

## API

Everything in [IPackedSpatialIndex](IPackedSpatialIndex.md), plus:

| Member | Description |
|---|---|
| `const int MaxLayers` | 12. |
| `PackedLayeredGrid(Aabb bounds, int columns, int rows, int layers = 4, int threads = 4)` | An empty grid. `columns` and `rows` are the top layer's, and each layer below has twice as many of each. Centers outside `bounds` go to the edge cells. Throws `ArgumentOutOfRangeException` if a count is under 1, `layers` is over `MaxLayers`, or the finest layer would have more than 2²⁴ cells. Throws `ArgumentException` if `bounds` has no area. |
| `int Columns`, `int Rows` | The top layer's. |
| `int Layers` | |
| `float CellWidth`, `float CellHeight` | A top cell's size. Each layer below halves it. |
| `int Threads` | How many threads `ForEachNeighbor` uses at most. Settable at any time between searches. 1 runs it on the calling thread. More threads than top cells gain nothing. Throws `ArgumentOutOfRangeException` if set below 1. |
| `int OversizedCount` | How many elements in the last `Build` were too big even for a top cell. |
| `void ForEachNeighbor<TVisitor>(TVisitor visitor)` | As in the interface, with each top cell as one piece of work, covering every layer's elements under it, over up to `Threads` threads. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports every top cell at depth 0, plus every finer cell that holds anything at its layer number as the depth. All are leaves with their element counts. |

## Example

```csharp
using ALife.Spatial;
using ALife.Spatial.Packed;

// Circles of radius 3 to 12: top cells of about 51 (a bit over the biggest, 24 across) and 4 layers, down to cells of about 6.
Aabb world = new(0, 0, 1280, 720);
PackedLayeredGrid grid = new(world, columns: 25, rows: 14, layers: 4, threads: 4);

grid.Build(boxes);
grid.ForEachNeighbor(new Separate(balls, contacts));

// Use every core when nothing else needs them.
grid.Threads = Environment.ProcessorCount;
```

`Separate` is the visitor from the [IPackedSpatialIndex example](IPackedSpatialIndex.md#example), which shows a full frame.

## See also

- [IPackedSpatialIndex](IPackedSpatialIndex.md)
- [PackedGrid](PackedGrid.md) · [PackedQuadTree](PackedQuadTree.md)
- [LayeredGrid](../LayeredGrid.md): the incrementally updated layered grid
