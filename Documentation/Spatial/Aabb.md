# Aabb

`ALife.Spatial` · ALife.Spatial · [source](../../Core/ALife.Spatial/Aabb.cs)

An axis-aligned bounding box, `readonly record struct Aabb(float Left, float Top, float Right, float Bottom)`. The space is y-down, as on screen, so `Top` is the smaller y. Edges are inclusive: boxes that only touch intersect. Every [ISpatialIndex](ISpatialIndex.md) takes and returns these.

## API

| Member | Description |
|---|---|
| `Aabb(float Left, float Top, float Right, float Bottom)` | |
| `static Aabb Empty` | Holds nothing: intersects nothing, and `Empty.Union(box)` is `box`. Its left is +∞ and its right −∞. |
| `static Aabb FromCenter(float centerX, float centerY, float halfWidth, float halfHeight)` | |
| `float Width`, `float Height` | `Right - Left`, `Bottom - Top`. |
| `float CenterX`, `float CenterY` | |
| `bool IsEmpty` | Left is past right or top past bottom. |
| `bool Intersects(Aabb other)` | Overlap or touch; always false if either is empty. |
| `bool Contains(float x, float y)` | Inside or on an edge. |
| `Aabb Union(Aabb other)` | The smallest box holding both. |
| `Aabb Inflate(float x, float y)` | Grown by `x` on the left and right and `y` on the top and bottom. |

## Example

```csharp
using ALife.Spatial;

Aabb player = Aabb.FromCenter(100, 80, 8, 8);
Aabb pickup = new(105, 85, 115, 95);

bool touching = player.Intersects(pickup);           // true
Aabb both = player.Union(pickup);                    // (92, 72, 115, 95)
Aabb reach = player.Inflate(32, 32);                 // what the player can reach
```

## See also

- [ISpatialIndex](ISpatialIndex.md)
