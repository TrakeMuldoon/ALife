# PhasedGrid

`ALife.Spatial.Phased` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Phased/PhasedGrid.cs)

An [IPackedSpatialIndex](../Packed/IPackedSpatialIndex.md) rebuilt every frame, as a plain grid or in layers like a [PackedLayeredGrid](../Packed/PackedLayeredGrid.md). Its extra is `ForEachPairInParallel`, which reports each intersecting pair **once** across threads and lets the visitor write to **both** elements. `ForEachNeighbor` reports every pair twice and works out the collision math twice, so this does about half the work.

That works because the top layer is cut into square tiles of a few cells each, and the tiles are searched in four phases, like a checkerboard colored two ways. Tiles searched in the same phase are at least a tile apart. An element is only ever paired with elements under its own top cell or the ones around it. So tiles searched at the same time never share an element, and no locks are needed. The phases always run in the same order, so the results don't depend on how the threads get scheduled.

It also sorts on several threads, and each thread's share of the sort doesn't grow with the number of cells, so the build keeps getting faster with more threads even in very big worlds. `Order` gives the order it sorted the elements into. If you keep your own arrays in that order, elements near each other in the world are near each other in memory. The pair search's reads are then close to sequential.

Measured on a 14-core machine with 200,000 circles on 1280×720, rebuilding every frame with the demo's push-and-bounce visitor (the `ForEachNeighbor` columns use its one-sided version). "Reordered" means the circles are put in the previous frame's `Order` before each build:

| 200,000 circles | Threads | `PackedGrid`¹ | `PackedLayeredGrid` | `PhasedGrid` `ForEachPairInParallel` | …reordered |
|---|---|---|---|---|---|
| radius 3, 1 layer | 4 | | 34 ms | 18 ms | 17 ms |
| radius 3, 1 layer | 14 | 16 ms | 13 ms | 9 ms | 9 ms |
| radius 3–12, 3 layers | 4 | | 117 ms | 60 ms | 54 ms |
| radius 3–12, 3 layers | 14 | 57 ms | 45 ms | 27 ms | 26 ms |

Each figure is the build plus the pair search. ¹ `PackedGrid` always uses the whole thread pool. `PhasedGrid`'s build alone takes 0.6–1.1 ms, against 2–4 ms for a `PackedGrid` or a `PackedLayeredGrid`.

Size it as a [PackedLayeredGrid](../Packed/PackedLayeredGrid.md#sizing-the-layers): top cells a little bigger than your biggest common element, plus enough layers to get the finest cells down to a little bigger than your smallest. Elements too big even for a top cell go in a side tree, as in a [PackedGrid](../Packed/PackedGrid.md), and are counted by `OversizedCount`. Their pairs are searched on the calling thread after the phases. A few are fine, but many mean the top cells are too small.

## API

Everything in [IPackedSpatialIndex](../Packed/IPackedSpatialIndex.md), plus:

| Member | Description |
|---|---|
| `const int MaxLayers` | 12. |
| `PhasedGrid(Aabb bounds, int columns, int rows, int layers = 1, int threads = 4)` | An empty grid. `columns` and `rows` are the top layer's, and each layer below has twice as many of each. Centers outside `bounds` go to the edge cells. Throws `ArgumentOutOfRangeException` if a count is under 1, `layers` is over `MaxLayers`, or the finest layer would have more than 2²⁴ cells. Throws `ArgumentException` if `bounds` has no area. |
| `int Columns`, `int Rows` | The top layer's. |
| `int Layers` | |
| `float CellWidth`, `float CellHeight` | A top cell's size. Each layer below halves it. |
| `int Threads` | How many threads `Build`, `ForEachNeighbor` and `ForEachPairInParallel` use at most. Settable at any time between calls. 1 runs everything on the calling thread. Throws `ArgumentOutOfRangeException` if set below 1. |
| `int OversizedCount` | How many elements in the last `Build` were too big even for a top cell, and went in the side tree. |
| `ReadOnlySpan<int> Order` | Every element of the last `Build`, in the order the grid stores them: layer by layer from the top, cell by cell, then the oversized elements, then the empty boxes. Elements in the same cell keep the order they were given in. Only valid until the next `Build`. |
| `void ForEachPairInParallel<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor` | Calls `visitor.Visit(a, b)` once for each intersecting pair, on up to `Threads` threads, and returns when they're done. See [the rules](#foreachpairinparallels-rules). |
| `void ForEachNeighbor<TVisitor>(TVisitor visitor)` | As in the interface, with each tile of the top layer as one piece of work, over up to `Threads` threads. Prefer `ForEachPairInParallel` when the visitor can write both elements. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports every top cell at depth 0, plus every finer cell that holds anything at its layer number as the depth. All are leaves with their element counts. Oversized elements aren't counted. |

### ForEachPairInParallel's rules

- Two calls that share an element never run at the same time, so the visitor may read and write both `a`'s and `b`'s state, such as `contacts[a]` and `contacts[b]`.
- Either element of a pair can come as `a`, so treat the two the same way.
- Each element's calls come in the same order on every run with the same boxes and `Threads`. So results stay the same from run to run, down to float rounding, even if the visitor changes the elements it's given (for example, moving both apart at once). A different `Threads` changes the tiles, so it can change the order.
- Each worker gets its own copy of a struct visitor, so changes to its fields are lost. Write through arrays it holds.
- Nothing may change the grid until it returns.

### Keeping your arrays in Order

`Order[i]` is the element that belongs at position `i`. To reorder, copy each of your arrays through it, then build from the reordered boxes:

```csharp
ReadOnlySpan<int> order = grid.Order;
for (int i = 0; i < order.Length; i++)
{
    spare[i] = balls[order[i]];
}

(balls, spare) = (spare, balls);
```

Do it at the start of a frame, from the previous `Build`, before the elements move. They've barely moved since, so the old order is nearly the new one. It only helps if you reorder everything indexed by element, and if nothing outside the grid holds on to element numbers across frames. Anything that does, such as a selected element, has to be mapped through the same order. On the scenes above it saves about 10% on 4 threads and less on 14, where the frame is already short. With 1,000,000 elements in a bigger world it saves up to a quarter (see [Performance](../README.md)).

## Example

One frame of circle physics, where each pair is handled once and both circles get their push:

```csharp
using ALife.Spatial;
using ALife.Spatial.Packed;
using ALife.Spatial.Phased;

// Reads every ball, writes contacts[a] and contacts[b]. Only safe with ForEachPairInParallel.
public readonly struct SeparateBoth(Ball[] balls, Contact[] contacts) : IPairVisitor
{
    public void Visit(int a, int b)
    {
        ref readonly Ball self = ref balls[a];
        ref readonly Ball other = ref balls[b];
        float dx = other.X - self.X, dy = other.Y - self.Y;
        float reach = self.Radius + other.Radius;
        float distanceSquared = dx * dx + dy * dy;
        if (distanceSquared >= reach * reach || distanceSquared == 0)
        {
            return;
        }

        float distance = MathF.Sqrt(distanceSquared);
        float push = (reach - distance) * 0.5f / distance;
        contacts[a].Dx -= dx * push;
        contacts[a].Dy -= dy * push;
        contacts[b].Dx += dx * push;
        contacts[b].Dy += dy * push;
    }
}

// Circles of radius 3: cells of about 6.3, a little over the diameter.
PhasedGrid grid = new(new Aabb(0, 0, 1280, 720), columns: 203, rows: 114, threads: Environment.ProcessorCount);

void Step()
{
    MoveBalls(balls);
    for (int i = 0; i < balls.Length; i++)
    {
        boxes[i] = Aabb.FromCenter(balls[i].X, balls[i].Y, balls[i].Radius, balls[i].Radius);
    }

    grid.Build(boxes);
    grid.ForEachPairInParallel(new SeparateBoth(balls, contacts));
    ApplyContacts(balls, contacts);
}
```

`Ball` and `Contact` are from the [IPackedSpatialIndex example](../Packed/IPackedSpatialIndex.md#example). For circles of radius 3 to 12, use `columns: 50, rows: 28, layers: 3` instead: top cells of about 25, which is a little over the biggest diameter, halving twice to about 6.

The MonoGame demo in `Samples/ALife.Spatial.Demo` uses it on key 9, with the circles kept in `Order`. `[` and `]` change its layers and `-` and `=` its threads. To start it with 200,000 circles, run `dotnet run -c Release --project Samples/ALife.Spatial.Demo -- 9 200000`.

## See also

- [IPackedSpatialIndex](../Packed/IPackedSpatialIndex.md)
- [PackedGrid](../Packed/PackedGrid.md) · [PackedLayeredGrid](../Packed/PackedLayeredGrid.md): the same grids, with `ForEachNeighbor` only
- [Performance](../README.md): every index timed across scenarios and crowd sizes
