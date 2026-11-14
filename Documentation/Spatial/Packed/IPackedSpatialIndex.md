# IPackedSpatialIndex

`ALife.Spatial.Packed` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Packed/IPackedSpatialIndex.cs)

A spatial index that's rebuilt from scratch every frame instead of updated one element at a time. Each `Build` sorts the boxes into flat arrays, so searches stream through memory, and nothing carries over between frames that would need tidying. It finds every intersecting pair in one pass, on one thread or spread across all of them. That's what lets it keep up with hundreds of thousands of moving elements, where an [ISpatialIndex](../ISpatialIndex.md) slows to a crawl past a few tens of thousands.

On a 1280×720 world with 200,000 circles of radius 3 (about 2.4 million touching pairs a frame), a [PackedGrid](PackedGrid.md) builds in about 2 ms and finds every pair in about 10 ms in parallel. The fastest `ISpatialIndex` takes about 200 ms for the same frame.

Elements have no handles. An element is the position of its box in the span given to `Build`, so keep your elements in an array and build from their boxes in the same order. `Build` isn't thread-safe. Once it has finished, `Query`, `ForEachPair` and `ForEachNeighbor` only read, so they can run on several threads at once. `VisitNodes` on a `PackedQuadTree` is the exception: it can't run alongside another `VisitNodes` call.

## Which one?

| Type | Good for | Watch out for |
|---|---|---|
| [PackedGrid](PackedGrid.md) | elements of similar size; the fastest build and pair search | cells must be a little bigger than the elements; bigger ones go to a slower side tree |
| [PackedQuadTree](PackedQuadTree.md) | sizes that vary widely, big elements included; no tuning | searches walk a tree, so they're slower than a well-sized grid's |
| [PackedLayeredGrid](PackedLayeredGrid.md) | sizes within a known range; a set number of threads | needs the top cell size and layer count tuned to the scene; uses 4 threads unless told otherwise |
| [PhasedGrid](../Phased/PhasedGrid.md) | the fastest frame when the visitor can handle both elements of a pair: each pair found once, in parallel; plain or layered | sized like the others; uses 4 threads unless told otherwise; oversized elements are searched on one thread |

## API

| Member | Description |
|---|---|
| `Aabb Bounds` | The area covered. Elements outside it still work, but are all filed at its edges. |
| `int Count` | How many boxes the last `Build` was given, empty ones included. |
| `void Build(ReadOnlySpan<Aabb> boxes)` | Replaces the contents. Element `i` is `boxes[i]`. The boxes are copied, so the span can change afterwards. An empty box is kept but never found. |
| `Aabb GetBounds(int element)` | The box as given to `Build`. |
| `void Query(Aabb area, List<int> results, int omitElement = -1)` | Adds the elements whose boxes intersect `area`, each once. The list isn't cleared first. |
| `void ForEachPair<TVisitor>(ref TVisitor visitor)` | Calls `visitor.Visit(a, b)` once for each intersecting pair, on this thread. The visitor is passed by reference, so a struct visitor can count or collect as it goes. |
| `void ForEachNeighbor<TVisitor>(TVisitor visitor)` | Calls `visitor.Visit(a, b)` for every element `a` and every other element `b` that intersects it, across the thread pool (or a `PackedLayeredGrid`'s `Threads`). Each pair comes up twice, once from each side. Returns when every call has finished. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Calls `visitor` for each node or cell, e.g. to draw the structure. See [SpatialNodeVisitor](../ISpatialIndex.md#spatialnodevisitor). |

Both pair methods are constrained to `where TVisitor : IPairVisitor`.

### ForEachNeighbor's threading rules

- Calls with the same `a` never run at the same time, so the visitor may write element `a`'s own state, such as `contacts[a]`.
- It must only *read* other elements. Keep what you write apart from what you read, as in the example below, or the results depend on timing.
- Each worker gets its own copy of a struct visitor, so changes to its fields are lost. Write through arrays it holds instead.

### IPairVisitor

`interface IPairVisitor { void Visit(int a, int b); }`

Receives pairs of intersecting elements as their positions in the built span, or as handles from a [LayeredGrid](../LayeredGrid.md). A [PhasedGrid](../Phased/PhasedGrid.md)'s `ForEachPairInParallel` is the one parallel search where the visitor may write both elements. Implement it on a struct. The pair methods are generic over the visitor, so the JIT compiles a copy of the search for each struct visitor and can inline `Visit`. A class visitor works too, at the cost of one interface call per pair.

## Example

One frame of circle physics, in parallel. Each circle's push is worked out from its neighbors' positions as they were at the start of the step and written to its own `Contact`. The pushes are applied afterwards, so no thread reads a circle another thread is changing.

```csharp
using ALife.Spatial;
using ALife.Spatial.Packed;

public struct Ball
{
    public float X, Y, Radius;
}

public struct Contact
{
    public float Dx, Dy;
}

// Reads every ball, writes only contacts[a].
public readonly struct Separate(Ball[] balls, Contact[] contacts) : IPairVisitor
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
    }
}

Ball[] balls = CreateBalls();
Aabb[] boxes = new Aabb[balls.Length];
Contact[] contacts = new Contact[balls.Length];

// Cells a little over the biggest diameter (6 here): 160 x 90 cells of 8 units.
IPackedSpatialIndex index = new PackedGrid(new Aabb(0, 0, 1280, 720), columns: 160, rows: 90);

void Step()
{
    MoveBalls(balls);
    for (int i = 0; i < balls.Length; i++)
    {
        boxes[i] = Aabb.FromCenter(balls[i].X, balls[i].Y, balls[i].Radius, balls[i].Radius);
    }

    index.Build(boxes);
    index.ForEachNeighbor(new Separate(balls, contacts));

    for (int i = 0; i < balls.Length; i++)
    {
        balls[i].X += contacts[i].Dx;
        balls[i].Y += contacts[i].Dy;
        contacts[i] = default;
    }
}
```

The loops before and after the search can also be split across threads (with `Parallel.For` over chunks of a few thousand), since each one only touches element `i`.

To count pairs on one thread, use `ForEachPair` with a struct that keeps a total. Each pair comes up once:

```csharp
public struct PairCounter : IPairVisitor
{
    public int Count;

    public void Visit(int a, int b) => Count++;
}

PairCounter counter = new();
index.ForEachPair(ref counter);
Console.WriteLine($"{counter.Count} boxes overlap");
```

The MonoGame demo in `Samples/ALife.Spatial.Demo` uses the packed types on keys 5, 6 and 8, the `LayeredGrid`'s pair search on key 7 and the `PhasedGrid` on key 9. On the layered and phased grids, `[` and `]` change the layers and `-` and `=` the threads. To start it with 200,000 circles on the `PackedGrid`, run `dotnet run -c Release --project Samples/ALife.Spatial.Demo -- 5 200000`.

## See also

- [PackedGrid](PackedGrid.md) · [PackedQuadTree](PackedQuadTree.md) · [PackedLayeredGrid](PackedLayeredGrid.md)
- [PhasedGrid](../Phased/PhasedGrid.md): finds each pair once in parallel
- [ISpatialIndex](../ISpatialIndex.md): the incrementally updated indexes
- [Performance](../README.md): every index timed across scenarios and crowd sizes
- [Aabb](../Aabb.md)
