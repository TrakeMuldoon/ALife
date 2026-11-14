# Collide

`ALife.Spatial.Collisions` · ALife.Spatial · [source](../../../Core/ALife.Spatial/Collisions/Collide.cs)

The narrow phase: whether two posed [Shape](../Shapes/Shape.md)s overlap, and if they do, a `Contact` that says how to push them apart. Use it on the pairs a spatial index finds, such as a [LayeredGrid](../LayeredGrid.md) or a [PhasedGrid](../Phased/PhasedGrid.md), to check whether their boxes' shapes really touch.

It's a static class. `Test` allocates nothing and keeps no state, so any number of threads can call it at once.

## API

| Member | Description |
|---|---|
| `static bool Test(Shape a, in Pose poseA, Shape b, in Pose poseB, out Contact contact)` | True if the shapes overlap or touch, with `contact`'s normal pointing from `a` into `b`; otherwise false and a default `contact`. |

How it works:

- **Bounding radii first.** Shapes whose bounding circles don't meet are rejected straight away, and so is each pair of parts.
- **Then each pair of parts:**
  - Circles against circles is exact.
  - Polygons use the separating axis test, then clip the edges that meet for the contact point.
  - Circles against polygons find the nearest face or corner.
- **The deepest contact wins.** For compounds, `contact` is the deepest one between any part of `a` and any part of `b`. A body resting on two parts of another is pushed by the deeper one each frame, which settles it in a few frames.

### Contact

`readonly struct Contact(Vector2 normal, float depth, Vector2 point)`: where and how deeply two shapes overlap.

| Member | Description |
|---|---|
| `Vector2 Normal` | The unit direction from the first shape into the second. Moving the second along it separates them. |
| `float Depth` | How far they overlap along `Normal`; zero or more. |
| `Vector2 Point` | A point in the world halfway through the overlap, where an impulse between them acts. |
| `Contact Flip()` | The same contact seen from the second shape: the normal reversed. |

## Example

```csharp
using System.Numerics;
using ALife.Spatial.Collisions;
using ALife.Spatial.Shapes;

Shape crate = Shape.Box(20, 10);
Shape ball = Shape.Circle(6);
Pose cratePose = new(new Vector2(100, 100), 0);   // spans x 90 to 110
Pose ballPose = new(new Vector2(115, 100), 0);    // spans x 109 to 121

if (Collide.Test(crate, cratePose, ball, ballPose, out Contact contact))
{
    // contact.Normal is (1, 0), out of the crate towards the ball, and contact.Depth is 1.
}
```

To push them apart and bounce them, give the contact to [ContactResolver.Resolve](../Physics/RigidBody.md#contactresolver).

## See also

- [Shape](../Shapes/Shape.md) · [Pose](../Shapes/Pose.md)
- [RigidBody](../Physics/RigidBody.md): bodies that move, and `ContactResolver`
- [Overlap](Overlap.md): yes-or-no tests without a contact, sectors included
