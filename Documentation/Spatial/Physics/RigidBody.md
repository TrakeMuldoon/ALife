# RigidBody

`ALife.Spatial.Physics` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Physics/RigidBody.cs)

The moving state of a rigid body: where it is, how it's turned, how fast it moves and spins, and how hard it is to push and turn. A plain mutable struct with public fields, for keeping in arrays and changing in place through `ref`. It doesn't refer to its [Shape](../Shapes/Shape.md); keep the shape alongside it, e.g. in a parallel array, since many bodies can share one. The space is y-down, as on screen, so a positive angle or angular velocity turns clockwise.

There's no gravity, damping or sleeping: apply forces by changing `Velocity` yourself, then `Integrate`.

## API

| Member | Description |
|---|---|
| `Vector2 Position` | Where its center of mass is. |
| `float Angle` | Radians, clockwise on screen. |
| `Vector2 Velocity` | Units a second. |
| `float AngularVelocity` | Radians a second, clockwise on screen. |
| `float InverseMass` | One over its mass; zero for a body nothing can move. |
| `float InverseInertia` | One over its moment of inertia; zero for a body nothing can turn. |
| `static RigidBody FromShape(Shape shape, float density, Vector2 position, float angle = 0)` | A body at rest with the shape's mass and inertia at a density. Throws if `shape` is null or `density` isn't above zero. |
| `static RigidBody Static(Vector2 position, float angle = 0)` | A body nothing can move or turn, such as a wall. |
| `Pose Pose` | Its position and angle as a [Pose](../Shapes/Pose.md), for placing its shape. |
| `void Integrate(float dt)` | Moves and turns it by its velocities over `dt` seconds. |
| `Vector2 VelocityAt(Vector2 point)` | How fast a point fixed to it moves, spin included. |
| `void ApplyImpulse(Vector2 impulse, Vector2 point)` | Changes its velocity and, if `point` is off its center, its spin. |

### ContactResolver

[source](../../../Core/ALife.Spatial/Physics/ContactResolver.cs)

`static class ContactResolver` pushes two touching bodies apart, given a [Contact](../Collisions/Collide.md#contact) between them.

```csharp
static bool Resolve(ref RigidBody a, ref RigidBody b, in Contact contact, float restitution, float friction,
    float slop = 0.05f, float correction = 0.4f)
```

A call does four things:

1. It moves both bodies apart along the normal, by `correction` of whatever overlap is past `slop`, heavier bodies moving less.
2. If they're still moving together, it applies a bounce impulse at the contact point. `restitution` 0 stops them dead along the normal; 1 bounces them apart at the speed they met.
3. It applies a friction impulse across the normal, at most `friction` times the bounce.
4. It returns true if it applied impulses. Two bodies that both have zero inverse mass are left alone.

Both impulses act at the contact point, so off-center hits set bodies spinning. Pass `a` and `b` in the same order as the shapes given to `Collide.Test`.

Each contact gets one pass a frame, with no iterating as a full physics engine does. Stacks jitter and sink a little, but crowds of loose bodies behave.

## Example

A frame for a crowd of bodies, with a [LayeredGrid](../LayeredGrid.md) for the broad phase. Each body's handle came from `grid.Insert(i, shapes[i].GetBounds(bodies[i].Pose))`.

```csharp
using System.Numerics;
using ALife.Spatial;
using ALife.Spatial.Collisions;
using ALife.Spatial.Physics;
using ALife.Spatial.Shapes;

void Step(Shape[] shapes, RigidBody[] bodies, int[] handles, LayeredGrid grid, List<int> found, float dt)
{
    for (int i = 0; i < bodies.Length; i++)
    {
        bodies[i].Integrate(dt);
        grid.Move(handles[i], shapes[i].GetBounds(bodies[i].Pose));
    }

    for (int i = 0; i < bodies.Length; i++)
    {
        found.Clear();
        grid.Query(shapes[i].GetBounds(bodies[i].Pose), found, handles[i]);
        foreach (int element in found)
        {
            // Each pair is found from both sides; handle it once.
            int j = grid.GetId(element);
            if (j > i && Collide.Test(shapes[i], bodies[i].Pose, shapes[j], bodies[j].Pose, out Contact contact))
            {
                ContactResolver.Resolve(ref bodies[i], ref bodies[j], contact, restitution: 0.5f, friction: 0.3f);
            }
        }
    }

    grid.Cleanup();
}
```

The MonoGame demo's shape modes do this, on whichever index is chosen: see [ISpatialIndex](../ISpatialIndex.md#example).

## See also

- [Shape](../Shapes/Shape.md) · [Pose](../Shapes/Pose.md)
- [Collide](../Collisions/Collide.md)
