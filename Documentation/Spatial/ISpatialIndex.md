# ISpatialIndex

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/ISpatialIndex.cs)

A spatial index over boxes, for broad-phase collision detection: it finds the elements whose boxes may intersect an area, and moving an element is cheap. Four implementations follow user4842163's answers to [Efficient (and well explained) implementation of a Quadtree for 2D collision detection](https://stackoverflow.com/questions/41946007/efficient-and-well-explained-implementation-of-a-quadtree-for-2d-collision-det) on Stack Overflow.

Elements are referred to by the `int` handle `Insert` returns. Store it with your object, and keep your object's index in your own array as the element's `id`: queries return handles, and `GetId` maps them back.

Not thread-safe, not even for concurrent queries, because queries reuse scratch buffers so they don't allocate. The exception is [LayeredGrid](LayeredGrid.md), whose queries need no buffers and can run on several threads at once. A handle stays the same while the element moves, and is reused after `Remove`; using a removed handle is undefined.

## Which one?

| Type | Element filed in | Good for | Watch out for |
|---|---|---|---|
| [QuadTree](QuadTree.md) | every leaf its box overlaps | many similar, smallish elements; fastest queries | huge elements land in many leaves; whole-number coordinates |
| [LooseQuadTree](LooseQuadTree.md) | one leaf, by its center | sizes that vary wildly | queries test more node boxes |
| [LooseTightGrid](LooseTightGrid.md) | one grid cell, by its center | varied sizes in a bounded world; constant-time updates | needs two cell sizes tuned to the scene |
| [UniformGrid](UniformGrid.md) | one grid cell, by its center | elements all up to one known size; simplest and cheapest | every element counts as the biggest size |
| [LayeredGrid](LayeredGrid.md) | one cell of the finest layer it fits, by its center | sizes within a known range; constant-time updates; finding every pair at once, across threads | needs the top cell size and layer count tuned to the scene; pairs come as handles |

Beyond a few tens of thousands of moving elements, use a `LayeredGrid` and its pair searches, or an [IPackedSpatialIndex](Packed/IPackedSpatialIndex.md): [PackedGrid](Packed/PackedGrid.md), [PackedQuadTree](Packed/PackedQuadTree.md), [PackedLayeredGrid](Packed/PackedLayeredGrid.md) or [PhasedGrid](Phased/PhasedGrid.md). They're rebuilt every frame instead of updated, find every pair in one pass across all cores, and handle 200,000 elements at 60 fps.

## API

| Member | Description |
|---|---|
| `Aabb Bounds` | The area covered. Elements outside it still work, but are all filed at its edges. |
| `int Count` | How many elements there are. |
| `int Insert(int id, Aabb bounds)` | Adds an element and returns its handle. `id` is yours (e.g. an array index); it isn't interpreted and needn't be unique. |
| `void Remove(int element)` | Removes an element. Empty nodes stay until `Cleanup`. |
| `void Move(int element, Aabb bounds)` | Changes an element's box, keeping its handle. |
| `int GetId(int element)` | The `id` it was inserted with. |
| `Aabb GetBounds(int element)` | The box as stored: rounded down to whole units by `QuadTree`, the uniform-size box for `UniformGrid`. |
| `void Query(Aabb area, List<int> results, int omitElement = -1)` | Adds the handles of the elements whose stored boxes intersect `area`, each once. The list isn't cleared first. `omitElement` leaves one out, usually the element you're finding collisions for. |
| `void Cleanup()` | Frees empty nodes and, for the loose structures, shrinks boxes to fit again. Call it once a frame, after moving everything. |
| `void Clear()` | Removes every element and resets the structure. |
| `void VisitNodes(SpatialNodeVisitor visitor)` | Calls `visitor` for each node or cell, e.g. to draw the structure. The visitor mustn't change the index. |

### SpatialNodeVisitor

`delegate void SpatialNodeVisitor(Aabb bounds, int depth, bool isLeaf, int count)`

| Parameter | Description |
|---|---|
| `bounds` | The node's box. For the loose structures it's the box around its elements, which can be `Aabb.Empty`. |
| `depth` | 0 for a quadtree's root or a grid's coarsest cells. |
| `isLeaf` | Whether the node holds elements directly. |
| `count` | For a leaf, its element count. For a `LooseTightGrid` tight cell, how many loose cells it lists. Otherwise 0. |

## Example

A frame of circle collisions. Each pair is found from both sides, so it's handled once by comparing IDs:

```csharp
using ALife.Spatial;

public struct Ball
{
    public float X, Y, Radius;
    public int Handle;
}

ISpatialIndex index = new QuadTree(new Aabb(0, 0, 1280, 720));
Ball[] balls = CreateBalls();
for (int i = 0; i < balls.Length; i++)
{
    balls[i].Handle = index.Insert(i, Aabb.FromCenter(balls[i].X, balls[i].Y, balls[i].Radius, balls[i].Radius));
}

List<int> hits = new();

void Step()
{
    // Move everything first...
    for (int i = 0; i < balls.Length; i++)
    {
        ref Ball ball = ref balls[i];
        index.Move(ball.Handle, Aabb.FromCenter(ball.X, ball.Y, ball.Radius, ball.Radius));
    }

    // ...then look for collisions.
    for (int i = 0; i < balls.Length; i++)
    {
        ref Ball ball = ref balls[i];
        hits.Clear();
        index.Query(Aabb.FromCenter(ball.X, ball.Y, ball.Radius, ball.Radius), hits, ball.Handle);
        foreach (int hit in hits)
        {
            int j = index.GetId(hit);
            if (j > i)
            {
                ResolveIfTouching(ref ball, ref balls[j]);
            }
        }
    }

    index.Cleanup();
}
```

Drawing the nodes:

```csharp
index.VisitNodes((bounds, depth, isLeaf, count) =>
{
    if (!bounds.IsEmpty)
    {
        DrawRectangle(bounds, ColorForDepth(depth));
    }
});
```

The MonoGame demo in `Samples/ALife.Spatial.Demo` does all of this for the first four types (keys 1–4), for the packed types on keys 5, 6 and 8, and for the `PhasedGrid` on key 9. Key 7 is the `LayeredGrid`, which moves its elements like the others but finds pairs with `ForEachNeighbor`. On it and keys 8 and 9, `[` and `]` change the layers and `-` and `=` the threads. Key 0 is ALife's original `CollisionGrid`, the one [SpatialCollisionMap](../Core/Collision/SpatialCollisionMap.md) replaced, rebuilt behind this interface with its 25-unit buckets and its costs. Tab and Shift+Tab, or `.` and `,`, step forward and back through all ten. Run it with `dotnet run -c Release --project Samples/ALife.Spatial.Demo`; add the starting index (1 to 10, with 10 for the original grid) and circle count to start elsewhere, e.g. `-- 4 20000`. A third argument profiles it for that many seconds, then prints the average step and drawing times and quits, e.g. `-- 9 200000 3`; add `nonodes` to hide the index's nodes, `serial` to run each step before drawing, `bounce` to start with bouncing on, or `shapes` or `compounds` to start with those rather than circles. By default each step runs on other threads while the frame before it is sent to the GPU and shown, which O switches off and on, so a frame takes about as long as the slower of the two rather than both together.

S cycles between circles, simple [shapes](Shapes/Shape.md) (squares, triangles and hexagons as well as circles) and compounds made of several parts. C switches bouncing on and off: overlapping bodies push apart and bounce, and shapes spin from off-center hits, through [Collide](Collisions/Collide.md) and [ContactResolver](Physics/RigidBody.md#contactresolver). V switches between one size and a range of sizes.

## See also

- [Aabb](Aabb.md)
- [QuadTree](QuadTree.md) · [LooseQuadTree](LooseQuadTree.md) · [LooseTightGrid](LooseTightGrid.md) · [UniformGrid](UniformGrid.md) · [LayeredGrid](LayeredGrid.md)
- [IPackedSpatialIndex](Packed/IPackedSpatialIndex.md): rebuilt-every-frame indexes for hundreds of thousands of elements
- [Performance](README.md): every index timed across scenarios and crowd sizes
- [Shape](Shapes/Shape.md) · [Collide](Collisions/Collide.md) · [RigidBody](Physics/RigidBody.md): the narrow phase and physics, for what the index finds
