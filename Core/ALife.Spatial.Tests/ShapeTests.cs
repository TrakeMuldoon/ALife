using System.Numerics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Tests;

/// <summary>Checks the <see cref="Shape"/> factories: areas, inertias, centering, corner order and bounds.</summary>
[TestClass]
public sealed class ShapeTests
{
    private const float Tolerance = 1e-4f;

    [TestMethod]
    public void CircleHasTheAreaAndInertiaOfADisc()
    {
        Shape circle = Shape.Circle(2);

        Assert.AreEqual(MathF.PI * 4, circle.Area, Tolerance);
        Assert.AreEqual(MathF.PI * 16 / 2, circle.Inertia, Tolerance);
        Assert.AreEqual(2, circle.BoundingRadius, Tolerance);
        Assert.AreEqual(ShapeKind.Circle, circle.Parts[0].Kind);
    }

    [TestMethod]
    public void BoxHasTheAreaAndInertiaOfARectangle()
    {
        Shape box = Shape.Box(4, 2);

        Assert.AreEqual(8, box.Area, Tolerance);
        Assert.AreEqual(4 * 2 * ((4 * 4) + (2 * 2)) / 12f, box.Inertia, Tolerance);
        Assert.AreEqual(MathF.Sqrt(5), box.BoundingRadius, Tolerance);
    }

    [TestMethod]
    public void TriangleIsCenteredAndPointsUp()
    {
        Shape triangle = Shape.Triangle(3);
        ConvexPart part = triangle.Parts[0];

        Assert.AreEqual(MathF.Sqrt(3) / 4 * 9, triangle.Area, Tolerance);
        Assert.AreEqual(3, part.Vertices.Length);
        Assert.AreEqual(0, Sum(part.Vertices).Length(), Tolerance);
        Assert.AreEqual(0, part.Vertices[0].X, Tolerance);
        Assert.IsTrue(part.Vertices[0].Y < 0);
        Assert.AreEqual(MathF.Sqrt(3), triangle.BoundingRadius, Tolerance);
    }

    [TestMethod]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(16)]
    public void PolygonNormalsAreUnitAndOutward(int sides)
    {
        ConvexPart part = Shape.RegularPolygon(sides, 10).Parts[0];

        for (int i = 0; i < sides; i++)
        {
            Vector2 next = part.Vertices[(i + 1) % sides];
            Assert.AreEqual(1, part.Normals[i].Length(), Tolerance);
            Assert.AreEqual(0, Vector2.Dot(part.Normals[i], next - part.Vertices[i]), Tolerance);
            Assert.IsTrue(Vector2.Dot(part.Normals[i], part.Vertices[i]) > 0);
        }
    }

    [TestMethod]
    public void DefaultPartIsAnEmptyCircle()
    {
        ConvexPart part = default;

        Assert.AreEqual(ShapeKind.Circle, part.Kind);
        Assert.AreEqual(0, part.Radius);
        Assert.AreEqual(0, part.Vertices.Length);
        Assert.AreEqual(0, part.Normals.Length);
    }

    [TestMethod]
    public void PolygonTakesTheHullAndCentersIt()
    {
        // A 4 by 2 rectangle given out of order, with a point inside, one on an edge and a repeat.
        Shape shape = Shape.Polygon(new(4, 2), new(2, 1), new(0, 0), new(2, 0), new(0, 2), new(4, 0), new(0, 0));
        ConvexPart part = shape.Parts[0];

        Assert.AreEqual(4, part.Vertices.Length);
        Assert.AreEqual(8, shape.Area, Tolerance);
        Assert.AreEqual(Shape.Box(4, 2).Inertia, shape.Inertia, Tolerance);
        Assert.AreEqual(0, Sum(part.Vertices).Length(), Tolerance);
        Assert.IsTrue(part.Normals.ToArray().Contains(new Vector2(0, -1)));
    }

    [TestMethod]
    public void PolygonRejectsDegenerateAndTooManyCorners()
    {
        Assert.ThrowsExactly<ArgumentException>(() => Shape.Polygon(new(0, 0), new(1, 1), new(2, 2)));
        Assert.ThrowsExactly<ArgumentException>(() => Shape.Polygon(new(0, 0), new(1, 1)));

        Vector2[] many = new Vector2[Shape.MaxVertices + 1];
        for (int i = 0; i < many.Length; i++)
        {
            (float sin, float cos) = MathF.SinCos(2 * MathF.PI * i / many.Length);
            many[i] = new Vector2(cos, sin) * 10;
        }

        Assert.ThrowsExactly<ArgumentException>(() => Shape.Polygon(many));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Shape.RegularPolygon(2, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Shape.Circle(0));
    }

    [TestMethod]
    public void CompoundOfTwoSquaresMatchesTheRectangle()
    {
        Shape square = Shape.Square(1);
        Shape compound = Shape.Compound(new ShapePart(square, new(10, 5)), new ShapePart(square, new(11, 5)));
        Shape box = Shape.Box(2, 1);

        Assert.AreEqual(2, compound.Parts.Length);
        Assert.AreEqual(box.Area, compound.Area, Tolerance);
        Assert.AreEqual(box.Inertia, compound.Inertia, Tolerance);
        Assert.AreEqual(box.BoundingRadius, compound.BoundingRadius, Tolerance);
        Assert.AreEqual(-0.5f, compound.Parts[0].Center.X, Tolerance);
        Assert.AreEqual(0.5f, compound.Parts[1].Center.X, Tolerance);
    }

    [TestMethod]
    public void CompoundCentersOnTheCombinedCenterOfMass()
    {
        // A big circle and a small one: the center of mass sits nearer the big one, by their areas.
        Shape compound = Shape.Compound(new ShapePart(Shape.Circle(2), Vector2.Zero), new ShapePart(Shape.Circle(1), new(6, 0)));

        Vector2 moment = Vector2.Zero;
        foreach (ConvexPart part in compound.Parts)
        {
            moment += part.Area * part.Center;
        }

        Assert.AreEqual(0, moment.Length(), Tolerance);
        Assert.AreEqual(-6 / 5f, compound.Parts[0].Center.X, Tolerance);
        Assert.AreEqual(6 - (6 / 5f) + 1, compound.BoundingRadius, Tolerance);
    }

    [TestMethod]
    public void CompoundTurnsItsParts()
    {
        Shape bar = Shape.Box(4, 1);
        Shape cross = Shape.Compound(new ShapePart(bar, Vector2.Zero), new ShapePart(bar, Vector2.Zero, MathF.PI / 2));

        Aabb bounds = Bounds(cross.Parts[1].Vertices);

        Assert.AreEqual(1, bounds.Width, Tolerance);
        Assert.AreEqual(4, bounds.Height, Tolerance);
    }

    [TestMethod]
    public void BoundsFollowThePose()
    {
        Aabb square = Shape.Square(2).GetBounds(new Pose(new Vector2(10, 20), MathF.PI / 4));
        Aabb circle = Shape.Circle(3).GetBounds(new Pose(new Vector2(-5, 5), 1));

        Assert.AreEqual(10 - MathF.Sqrt(2), square.Left, Tolerance);
        Assert.AreEqual(20 + MathF.Sqrt(2), square.Bottom, Tolerance);
        Assert.AreEqual(new Aabb(-8, 2, -2, 8), circle);
    }

    [TestMethod]
    public void PoseTurnsClockwiseOnScreen()
    {
        Pose pose = new(new Vector2(1, 1), MathF.PI / 2);

        // Right turns to down, which is +y.
        Vector2 turned = pose.Rotate(Vector2.UnitX);
        Assert.AreEqual(0, turned.X, Tolerance);
        Assert.AreEqual(1, turned.Y, Tolerance);
        Assert.AreEqual(1, pose.Apply(Vector2.UnitX).X, Tolerance);
        Assert.AreEqual(2, pose.Apply(Vector2.UnitX).Y, Tolerance);
        Assert.AreEqual(1, pose.InverseRotate(turned).X, Tolerance);
    }

    private static Vector2 Sum(ReadOnlySpan<Vector2> points)
    {
        Vector2 sum = Vector2.Zero;
        foreach (Vector2 point in points)
        {
            sum += point;
        }

        return sum;
    }

    private static Aabb Bounds(ReadOnlySpan<Vector2> points)
    {
        Aabb bounds = Aabb.Empty;
        foreach (Vector2 point in points)
        {
            bounds = bounds.Union(new Aabb(point.X, point.Y, point.X, point.Y));
        }

        return bounds;
    }
}
