namespace ALife.Spatial.Shapes;

/// <summary>What a <see cref="ConvexPart"/> is, so collision code can switch on it rather than make a virtual call.</summary>
public enum ShapeKind
{
    /// <summary>A circle: <see cref="ConvexPart.Center"/> and <see cref="ConvexPart.Radius"/>.</summary>
    Circle,

    /// <summary>A convex polygon: <see cref="ConvexPart.Vertices"/> and <see cref="ConvexPart.Normals"/>.</summary>
    Polygon,
}
