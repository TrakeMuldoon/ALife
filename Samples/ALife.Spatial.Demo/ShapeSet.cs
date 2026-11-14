using System.Numerics;
using ALife.Spatial.Shapes;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace ALife.Spatial.Demo;

/// <summary>
/// The shapes the demo's shape modes use: every kind in a range of sizes, each with its bounding radius at that size, and the
/// inverse masses and inertias for a density of 1. An atlas texture has a picture of each kind in a row of cells, to draw
/// them turned with <see cref="QuadBatch.WriteRotated"/>.
/// </summary>
internal sealed class ShapeSet
{
    /// <summary>How many sizes each kind comes in, from the smallest radius to the largest.</summary>
    public const int Sizes = 12;

    /// <summary>The kinds before this are simple shapes; the ones from it on are compounds.</summary>
    public const int SimpleKinds = 4;

    private const int Cell = 64;

    // Transparent texels around each picture, so neighbors don't bleed into it as the mip levels shrink.
    private const int Margin = 3;

    // Each kind at a scale of 1; the set scales them so their bounding radius is the size wanted.
    private static readonly Func<float, Shape>[] Makers =
    [
        s => Shape.Circle(s),
        s => Shape.Square(1.4f * s),
        s => Shape.Triangle(1.7f * s),
        s => Shape.RegularPolygon(6, s),

        // A dumbbell: two balls on a bar that touches them.
        s => Shape.Compound(
            new ShapePart(Shape.Circle(0.5f * s), new(-s, 0)),
            new ShapePart(Shape.Circle(0.5f * s), new(s, 0)),
            new ShapePart(Shape.Box(s, 0.3f * s), Vector2.Zero)),

        // An L.
        s => Shape.Compound(
            new ShapePart(Shape.Box(2 * s, 0.6f * s), Vector2.Zero),
            new ShapePart(Shape.Box(0.6f * s, 1.4f * s), new(-0.7f * s, s))),

        // A plus.
        s => Shape.Compound(
            new ShapePart(Shape.Box(2 * s, 0.6f * s), Vector2.Zero),
            new ShapePart(Shape.Box(0.6f * s, 0.7f * s), new(0, -0.65f * s)),
            new ShapePart(Shape.Box(0.6f * s, 0.7f * s), new(0, 0.65f * s))),

        // A house: a roof on a square.
        s => Shape.Compound(
            new ShapePart(Shape.Square(s), new(0, 0.5f * s)),
            new ShapePart(Shape.Triangle(s), new(0, -s * MathF.Sqrt(3) / 6))),
    ];

    // The bounding radius of each kind at a scale of 1.
    private static readonly float[] UnitRadii = Makers.Select(make => make(1).BoundingRadius).ToArray();

    private readonly float _smallest;
    private readonly float _sizeStep;

    /// <summary>Makes every kind in <see cref="Sizes"/> sizes, spaced evenly by ratio.</summary>
    /// <param name="smallest">The smallest bounding radius.</param>
    /// <param name="largest">The largest bounding radius.</param>
    public ShapeSet(float smallest, float largest)
    {
        _smallest = smallest;
        _sizeStep = MathF.Log(largest / smallest) / (Sizes - 1);
        Shapes = new Shape[Kinds * Sizes];
        InverseMass = new float[Shapes.Length];
        InverseInertia = new float[Shapes.Length];
        for (int kind = 0; kind < Kinds; kind++)
        {
            for (int size = 0; size < Sizes; size++)
            {
                float radius = smallest * MathF.Exp(_sizeStep * size);
                Shape shape = Makers[kind](radius / UnitRadii[kind]);
                int index = (kind * Sizes) + size;
                Shapes[index] = shape;
                InverseMass[index] = 1 / shape.Area;
                InverseInertia[index] = 1 / shape.Inertia;
            }
        }
    }

    /// <summary>How many kinds there are, simple and compound.</summary>
    public static int Kinds => Makers.Length;

    /// <summary>How much bigger than a shape's bounding radius the half size of its picture's quad is, for the margin.</summary>
    public static float QuadScale => Cell * 0.5f / ((Cell * 0.5f) - Margin);

    /// <summary>Every kind in every size, by <see cref="Index"/>.</summary>
    public Shape[] Shapes { get; }

    /// <summary>One over each shape's mass.</summary>
    public float[] InverseMass { get; }

    /// <summary>One over each shape's moment of inertia.</summary>
    public float[] InverseInertia { get; }

    /// <summary>The kind a shape's index is of.</summary>
    public static int KindOf(int index) => index / Sizes;

    /// <summary>Where a kind's picture starts across the atlas, from 0 to 1.</summary>
    public static float TextureLeft(int kind) => kind / (float)Kinds;

    /// <summary>Where a kind's picture ends across the atlas, from 0 to 1.</summary>
    public static float TextureRight(int kind) => (kind + 1) / (float)Kinds;

    /// <summary>The index of a kind in the size nearest a bounding radius.</summary>
    public int Index(int kind, float radius)
    {
        int size = Math.Clamp((int)MathF.Round(MathF.Log(radius / _smallest) / _sizeStep), 0, Sizes - 1);
        return (kind * Sizes) + size;
    }

    /// <summary>
    /// A row of white pictures of each kind, premultiplied, with a lighter rim and, on the circle, a spoke, so that turning
    /// shows. Edges between a compound's parts get the rim too, which shows what it's made of.
    /// </summary>
    public static Texture2D CreateAtlas(GraphicsDevice device)
    {
        int width = Cell * Kinds, height = Cell;
        float scale = (Cell * 0.5f) - Margin;
        Vector4[] level = new Vector4[width * height];
        for (int kind = 0; kind < Kinds; kind++)
        {
            Shape shape = Makers[kind](1 / UnitRadii[kind]);
            for (int y = 0; y < Cell; y++)
            {
                for (int x = 0; x < Cell; x++)
                {
                    Vector2 point = new Vector2(x + 0.5f - (Cell * 0.5f), y + 0.5f - (Cell * 0.5f)) / scale;
                    float distance = Distance(shape, point);
                    float coverage = Math.Clamp(0.5f - (distance * scale), 0, 1);
                    bool rim = distance > -0.15f || (kind == 0 && point.X > 0 && MathF.Abs(point.Y) < 0.08f);
                    float brightness = rim ? 1 : 0.7f;
                    level[(y * width) + (kind * Cell) + x] = new Vector4(brightness * coverage, brightness * coverage, brightness * coverage, coverage);
                }
            }
        }

        Texture2D texture = new(device, width, height, mipmap: true, SurfaceFormat.Color);
        for (int mip = 0; mip < texture.LevelCount; mip++)
        {
            if (mip > 0)
            {
                (level, width, height) = Halve(level, width, height);
            }

            texture.SetData(mip, null, level.Select(texel => new Color(texel.X, texel.Y, texel.Z, texel.W)).ToArray(), 0, level.Length);
        }

        return texture;
    }

    // How far a point is outside the shape, or minus how far inside: exact for circles and inside polygons, a little short
    // outside a polygon's corners, which only rounds them slightly.
    private static float Distance(Shape shape, Vector2 point)
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

    // The next mip level: each texel the average of the two by two above it, or of what's left of them at an odd edge.
    private static (Vector4[] Level, int Width, int Height) Halve(Vector4[] level, int width, int height)
    {
        int smallWidth = Math.Max(1, width / 2), smallHeight = Math.Max(1, height / 2);
        Vector4[] smaller = new Vector4[smallWidth * smallHeight];
        for (int y = 0; y < smallHeight; y++)
        {
            for (int x = 0; x < smallWidth; x++)
            {
                int x0 = 2 * x, y0 = 2 * y;
                int x1 = Math.Min(x0 + 1, width - 1), y1 = Math.Min(y0 + 1, height - 1);
                smaller[(y * smallWidth) + x] =
                    (level[(y0 * width) + x0] + level[(y0 * width) + x1] + level[(y1 * width) + x0] + level[(y1 * width) + x1]) / 4;
            }
        }

        return (smaller, smallWidth, smallHeight);
    }
}
