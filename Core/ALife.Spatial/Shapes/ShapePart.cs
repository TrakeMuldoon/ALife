using System.Numerics;

namespace ALife.Spatial.Shapes;

/// <summary>A shape placed inside a compound, for <see cref="Shape.Compound"/>.</summary>
/// <param name="Shape">The shape, simple or compound; its center of mass goes at <paramref name="Offset"/>.</param>
/// <param name="Offset">Where its center of mass goes, relative to the other parts.</param>
/// <param name="Rotation">How far to turn it, in radians, clockwise on screen.</param>
public readonly record struct ShapePart(Shape Shape, Vector2 Offset, float Rotation = 0);
