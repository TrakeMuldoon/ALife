namespace ALife.Spatial;

/// <summary>
/// An axis-aligned bounding box. The space is y-down, as on screen: <see cref="Top"/> is the smaller y. Edges are inclusive,
/// so boxes that only touch intersect.
/// </summary>
/// <param name="Left">The smallest x.</param>
/// <param name="Top">The smallest y.</param>
/// <param name="Right">The largest x.</param>
/// <param name="Bottom">The largest y.</param>
public readonly record struct Aabb(float Left, float Top, float Right, float Bottom)
{
    /// <summary>A box holding nothing: it intersects nothing, and a union with it gives the other box.</summary>
    public static Aabb Empty { get; } = new(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);

    /// <summary>The width, <c>Right - Left</c>.</summary>
    public float Width => Right - Left;

    /// <summary>The height, <c>Bottom - Top</c>.</summary>
    public float Height => Bottom - Top;

    /// <summary>The x of the center.</summary>
    public float CenterX => (Left + Right) * 0.5f;

    /// <summary>The y of the center.</summary>
    public float CenterY => (Top + Bottom) * 0.5f;

    /// <summary>Whether the box holds nothing, as <see cref="Empty"/> does: its left is past its right or its top past its bottom.</summary>
    public bool IsEmpty => Left > Right || Top > Bottom;

    /// <summary>A box from its center and half its size.</summary>
    /// <param name="centerX">The x of the center.</param>
    /// <param name="centerY">The y of the center.</param>
    /// <param name="halfWidth">Half the width.</param>
    /// <param name="halfHeight">Half the height.</param>
    /// <returns>The box.</returns>
    public static Aabb FromCenter(float centerX, float centerY, float halfWidth, float halfHeight) =>
        new(centerX - halfWidth, centerY - halfHeight, centerX + halfWidth, centerY + halfHeight);

    /// <summary>Whether this box and <paramref name="other"/> overlap or touch.</summary>
    /// <param name="other">The other box.</param>
    /// <returns>True if they intersect; always false if either is empty.</returns>
    public bool Intersects(Aabb other) => Left <= other.Right && other.Left <= Right && Top <= other.Bottom && other.Top <= Bottom;

    /// <summary>Whether the point is inside the box or on its edge.</summary>
    /// <param name="x">The point's x.</param>
    /// <param name="y">The point's y.</param>
    /// <returns>True if the box contains the point.</returns>
    public bool Contains(float x, float y) => x >= Left && x <= Right && y >= Top && y <= Bottom;

    /// <summary>The smallest box containing both this box and <paramref name="other"/>.</summary>
    /// <param name="other">The other box.</param>
    /// <returns>The union.</returns>
    public Aabb Union(Aabb other) =>
        new(MathF.Min(Left, other.Left), MathF.Min(Top, other.Top), MathF.Max(Right, other.Right), MathF.Max(Bottom, other.Bottom));

    /// <summary>This box grown by the given amounts on each side.</summary>
    /// <param name="x">How far to move the left and right edges out.</param>
    /// <param name="y">How far to move the top and bottom edges out.</param>
    /// <returns>The bigger box.</returns>
    public Aabb Inflate(float x, float y) => new(Left - x, Top - y, Right + x, Bottom + y);
}
