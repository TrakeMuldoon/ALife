# LayeredGrid

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/LayeredGrid.cs)

An [ISpatialIndex](ISpatialIndex.md) made of a stack of uniform grids. Each layer's cells are a quarter the size of the one above: half as wide and half as tall. Each element goes in the finest layer whose cells it fits in, by the center of its box, so small elements share small cells and big ones big cells. Unlike a [UniformGrid](UniformGrid.md), it keeps each element's exact box, and elements needn't share a size. Moving an element is constant time, and the grid never changes shape.

It also finds every intersecting pair in one pass, which the other `ISpatialIndex` types can't. `ForEachPair` runs on one thread and reports each pair once. `ForEachNeighbor` splits the work by top-layer cell over a set number of threads (`Threads`, 4 by default) and reports each pair from both sides. Both report handles, and work with the same struct [IPairVisitor](Packed/IPackedSpatialIndex.md#ipairvisitor) as the packed indexes. That takes it far past the other incremental indexes. With 200,000 circles of radius 3 on 1280×720 and 4 layers, moving them all takes about 4 ms and `ForEachNeighbor` about 35 ms on 4 threads, or 14 ms on 14. Querying each circle in turn on a `UniformGrid` takes about 200 ms.

`Query` uses no scratch buffers, unlike the other `ISpatialIndex` types, so it's safe to call from several threads at once while nothing changes the grid. `Insert`, `Move`, `Remove`, `Cleanup` and `Clear` aren't thread-safe.

Size the layers as for a [PackedLayeredGrid](Packed/PackedLayeredGrid.md#sizing-the-layers). Make the top cells a little bigger than your biggest common element, and add layers until the finest cells are a little bigger than your smallest. Elements too big for a top cell go in the top layer anyway, and widen every search of it until they shrink, move to a lower layer or are removed and `Cleanup` runs.

## API

Everything in [ISpatialIndex](ISpatialIndex.md), plus:

| Member | Description |
|---|---|
| `const int MaxLayers` | 12. |
| `LayeredGrid(Aabb bounds, int columns, int rows, int layers = 4, int threads = 4)` | An empty grid. `columns` and `rows` are the top layer's, and each layer below has twice as many of each. Centers outside `bounds` go to the edge cells. Throws `ArgumentOutOfRangeException` if a count is under 1, `layers` is over `MaxLayers`, or the finest layer would have more than 2²⁴ cells. Throws `ArgumentException` if `bounds` has no area. |
| `int Columns`, `int Rows` | The top layer's. |
| `int Layers` | |
| `float CellWidth`, `float CellHeight` | A top cell's size. Each layer below halves it. |
| `int Threads` | How many threads `ForEachNeighbor` uses at most. Settable at any time between searches. 1 runs it on the calling thread. Throws `ArgumentOutOfRangeException` if set below 1. |
| `int OversizedCount` | How many elements are too big even for a top cell. |
| `Aabb GetBounds(int element)` | The exact box the element was inserted or last moved with. |
| `void Query(Aabb area, List<int> results, int omitElement = -1)` | As in the interface. Safe to run on several threads at once while nothing changes the grid. |
| `void ForEachPair<TVisitor>(ref TVisitor visitor) where TVisitor : IPairVisitor` | Calls `visitor.Visit(a, b)` once for each pair of handles whose boxes intersect, on this thread. The visitor is passed by reference, so a struct can count as it goes. |
| `void ForEachNeighbor<TVisitor>(TVisitor visitor) where TVisitor : IPairVisitor` | Calls `visitor.Visit(a, b)` for every handle `a` and every other handle `b` whose box intersects it, on up to `Threads` threads, and returns when they're done. Each pair comes up twice. It follows the same [threading rules](Packed/IPackedSpatialIndex.md#foreachneighbors-threading-rules) as the packed indexes: write only `a`'s own state, and nothing may change the grid until it returns. |
| `void Cleanup()` | Shrinks how far top-layer searches reach to fit the oversized elements still there. Nothing else needs tidying. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Reports every top cell at depth 0, plus every finer cell that holds anything at its layer number as the depth. All are leaves with their element counts. |

## Example

Pairs come as handles, so map them back to your own array with `GetId` before using them:

```csharp
using ALife.Spatial;
using ALife.Spatial.Packed;

// Circles of radius 3 to 12: top cells of about 51 and 4 layers, down to cells of about 6.
LayeredGrid grid = new(new Aabb(0, 0, 1280, 720), columns: 25, rows: 14, layers: 4, threads: 4);
for (int i = 0; i < balls.Length; i++)
{
    balls[i].Handle = grid.Insert(i, Aabb.FromCenter(balls[i].X, balls[i].Y, balls[i].Radius, balls[i].Radius));
}

void Step()
{
    for (int i = 0; i < balls.Length; i++)
    {
        ref Ball ball = ref balls[i];
        MoveBall(ref ball);
        grid.Move(ball.Handle, Aabb.FromCenter(ball.X, ball.Y, ball.Radius, ball.Radius));
    }

    grid.Cleanup();
    grid.ForEachNeighbor(new ByHandle(grid, new Separate(balls, contacts)));
    ApplyContacts(balls, contacts);
}

// Turns handles into array positions for a visitor that works on those.
public readonly struct ByHandle(LayeredGrid grid, Separate inner) : IPairVisitor
{
    public void Visit(int a, int b) => inner.Visit(grid.GetId(a), grid.GetId(b));
}
```

`Separate` is the visitor from the [IPackedSpatialIndex example](Packed/IPackedSpatialIndex.md#example), which writes only `contacts[a]`. That's still safe here because each handle has its own ID.

## See also

- [ISpatialIndex](ISpatialIndex.md)
- [UniformGrid](UniformGrid.md) · [LooseTightGrid](LooseTightGrid.md)
- [PackedLayeredGrid](Packed/PackedLayeredGrid.md): the same layers, rebuilt every frame
