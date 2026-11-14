using System.Numerics;

namespace ALife.Spatial.Collisions;

/// <summary>Where and how deeply two shapes overlap, from <see cref="Collide.Test"/>.</summary>
/// <param name="normal">The unit direction from the first shape into the second: moving the second along it separates them.</param>
/// <param name="depth">How far they overlap along <paramref name="normal"/>; zero or more.</param>
/// <param name="point">A point in the world halfway through the overlap, where an impulse between them acts.</param>
public readonly struct Contact(Vector2 normal, float depth, Vector2 point)
{
    /// <summary>The unit direction from the first shape into the second: moving the second along it separates them.</summary>
    public Vector2 Normal { get; } = normal;

    /// <summary>How far they overlap along <see cref="Normal"/>; zero or more.</summary>
    public float Depth { get; } = depth;

    /// <summary>A point in the world halfway through the overlap, where an impulse between them acts.</summary>
    public Vector2 Point { get; } = point;

    /// <summary>The same contact seen from the other shape: the normal reversed.</summary>
    /// <returns>The flipped contact.</returns>
    public Contact Flip() => new(-Normal, Depth, Point);
}
