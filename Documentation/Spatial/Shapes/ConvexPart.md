# ConvexPart

`ALife.Spatial.Shapes` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Shapes/ConvexPart.cs)

One convex piece of a [Shape](Shape.md): a circle or a convex polygon, placed in the shape's space, whose origin is the whole shape's center of mass. You don't make these yourself. A shape's factories make them, and `Shape.Parts` hands them out, e.g. to draw a shape or to work out something the shape doesn't.

It's a `readonly struct`, so a shape's parts sit side by side in one array, rather than each in an object of its own. Copying one is cheap: the corners and normals are arrays it shares, not copies. A `default` part is an empty circle at the origin with no area.

## API

| Member | Description |
|---|---|
| `ShapeKind Kind` | `Circle` or `Polygon`. |
| `Vector2 Center` | The part's own center of mass, in the shape's space. |
| `float Radius` | A circle's radius, or for a polygon the distance from `Center` to its farthest corner. |
| `float Area` | |
| `float Inertia` | The second moment of its area about `Center`: its moment of inertia at a density of 1. |
| `ReadOnlySpan<Vector2> Vertices` | A polygon's corners in the shape's space, clockwise on screen. Empty for a circle. |
| `ReadOnlySpan<Vector2> Normals` | A polygon's outward unit normals: `Normals[i]` is that of the edge from `Vertices[i]` to the next corner. Empty for a circle. |

### ShapeKind

`enum ShapeKind { Circle, Polygon }`: what a part is, so code can switch on it rather than make a virtual call. A circle has a `Center` and `Radius`; a polygon has `Vertices` and `Normals` too.

## Example

How far a point is outside a shape, or minus how far inside, as the demo works it out to draw each shape:

```csharp
using System.Numerics;
using ALife.Spatial.Shapes;

static float Distance(Shape shape, Vector2 point)
{
    float nearest = float.PositiveInfinity;
    foreach (ConvexPart part in shape.Parts)
    {
        float distance;
        if (part.Kind == ShapeKind.Circle)
        {
            distance = Vector2.Distance(point, part.Center) - part.Radius;
        }
        else
        {
            // Exact inside a polygon; a little short outside its corners.
            distance = float.NegativeInfinity;
            for (int i = 0; i < part.Vertices.Length; i++)
            {
                distance = MathF.Max(distance, Vector2.Dot(part.Normals[i], point - part.Vertices[i]));
            }
        }

        nearest = MathF.Min(nearest, distance);
    }

    return nearest;
}
```

The point is in the shape's space. For one in the world, turn it back first: `pose.InverseRotate(worldPoint - pose.Position)`.

## See also

- [Shape](Shape.md)
- [Pose](Pose.md)
