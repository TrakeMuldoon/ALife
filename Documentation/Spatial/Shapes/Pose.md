# Pose

`ALife.Spatial.Shapes` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Shapes/Pose.cs)

Where a [Shape](Shape.md) is and which way it's turned: the position of its center of mass and its angle, with the angle's cosine and sine worked out once when the pose is made. A `readonly struct`, passed by `in` to the methods that take one. The space is y-down, as on screen, so a positive angle turns clockwise.

[RigidBody](../Physics/RigidBody.md)`.Pose` makes one from a body's position and angle.

## API

| Member | Description |
|---|---|
| `Pose(Vector2 position, float angle)` | `angle` is in radians. |
| `Vector2 Position` | Where the center of mass is. |
| `float Angle` | In radians, clockwise on screen. |
| `float Cos`, `float Sin` | The angle's cosine and sine. |
| `Vector2 Apply(Vector2 local)` | From the shape's space to the world's: turned, then moved to `Position`. |
| `Vector2 Rotate(Vector2 vector)` | A direction turned by the angle, without moving it, e.g. a normal. |
| `Vector2 InverseRotate(Vector2 vector)` | A direction turned back by the angle: from the world's directions to the shape's. |

## Example

```csharp
using System.Numerics;
using ALife.Spatial.Shapes;

Pose pose = new(new Vector2(100, 50), MathF.PI / 2);   // a quarter turn clockwise
Vector2 nose = pose.Apply(new Vector2(0, -10));       // a point 10 up the screen in the shape: (110, 50) in the world
Vector2 back = pose.InverseRotate(nose - pose.Position);   // (0, -10) again
```

## See also

- [Shape](Shape.md)
- [RigidBody](../Physics/RigidBody.md)
