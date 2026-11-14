using System.Numerics;
using ALife.Spatial.Collisions;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Tests;

/// <summary>Checks the yes-or-no <see cref="Overlap"/> tests, touching cases, either winding, and sectors of every sweep.</summary>
[TestClass]
public sealed class OverlapTests
{
    // A sector at the origin facing right (+x), a quarter turn wide, reaching 10.
    private static readonly Sector Eye = new(Vector2.Zero, -MathF.PI / 4, MathF.PI / 2, 10);

    [TestMethod]
    public void CirclesOverlapUpToTouching()
    {
        Assert.IsTrue(Overlap.Circles(Vector2.Zero, 2, new Vector2(3, 0), 1));
        Assert.IsFalse(Overlap.Circles(Vector2.Zero, 2, new Vector2(3.01f, 0), 1));
    }

    [TestMethod]
    public void PolygonContainsEitherWayRound()
    {
        Vector2[] square = Square(0, 0, 1);
        Vector2[] reversed = square.Reverse().ToArray();

        foreach (Vector2[] polygon in new[] { square, reversed })
        {
            Assert.IsTrue(Overlap.PolygonContains(polygon, new Vector2(0.5f, -0.5f)));
            Assert.IsTrue(Overlap.PolygonContains(polygon, new Vector2(1, 0)));
            Assert.IsFalse(Overlap.PolygonContains(polygon, new Vector2(1.1f, 0)));
        }

        Assert.IsFalse(Overlap.PolygonContains([Vector2.Zero, Vector2.UnitX], Vector2.Zero));
    }

    [TestMethod]
    public void CirclePolygonFindsEdgesNotCorners()
    {
        Vector2[] square = Square(0, 0, 1);

        Assert.IsTrue(Overlap.CirclePolygon(new Vector2(2, 0), 1, square));
        Assert.IsTrue(Overlap.CirclePolygon(Vector2.Zero, 0.1f, square));
        Assert.IsTrue(Overlap.CirclePolygon(Vector2.Zero, 50, square));

        // Inside the box round the corner, but further from it than the radius.
        Assert.IsFalse(Overlap.CirclePolygon(new Vector2(1.8f, 1.8f), 1, square));
    }

    [TestMethod]
    public void PolygonsUseEveryEdge()
    {
        Vector2[] square = Square(0, 0, 1);
        Vector2[] diamond = [new(3.4f, 2), new(2, 3.4f), new(0.6f, 2), new(2, 0.6f)];

        Assert.IsTrue(Overlap.Polygons(square, Square(1.5f, 1.5f, 1)));
        Assert.IsTrue(Overlap.Polygons(square, Square(2, 0, 1).Reverse().ToArray()));
        Assert.IsFalse(Overlap.Polygons(square, Square(2.1f, 0, 1)));

        // The boxes overlap, but the diamond's edge separates it from the corner.
        Assert.IsFalse(Overlap.Polygons(square, diamond));
    }

    [TestMethod]
    public void SegmentsMeetAtEndsAndAlongALine()
    {
        Assert.IsTrue(Overlap.Segments(new(0, 0), new(2, 2), new(0, 2), new(2, 0)));
        Assert.IsTrue(Overlap.Segments(new(0, 0), new(2, 0), new(2, 0), new(2, 5)));
        Assert.IsTrue(Overlap.Segments(new(0, 0), new(2, 0), new(1, 0), new(3, 0)));
        Assert.IsTrue(Overlap.Segments(new(5, -1), new(5, 1), new(0, 0), new(10, 0)));
        Assert.IsFalse(Overlap.Segments(new(0, 0), new(2, 0), new(2.5f, 0), new(3, 0)));
        Assert.IsFalse(Overlap.Segments(new(0, 0), new(2, 0), new(0, 1), new(2, 1)));
    }

    [TestMethod]
    public void SectorCircleNeedsTheSweepOrAnEdge()
    {
        Assert.IsTrue(Overlap.SectorCircle(Eye, new Vector2(11, 0), 1));
        Assert.IsFalse(Overlap.SectorCircle(Eye, new Vector2(11.1f, 0), 1));
        Assert.IsFalse(Overlap.SectorCircle(Eye, new Vector2(-5, 0), 1));

        // Behind, but big enough to reach the apex; and off to the side, touching a straight edge.
        Assert.IsTrue(Overlap.SectorCircle(Eye, new Vector2(-2, 0), 2.1f));
        Assert.IsTrue(Overlap.SectorCircle(Eye, new Vector2(0, -5), 3.6f));
        Assert.IsFalse(Overlap.SectorCircle(Eye, new Vector2(0, -5), 3.5f));
    }

    [TestMethod]
    public void ReflexSectorReachesBehindButNotIntoItsGap()
    {
        // Everything but the quarter facing right.
        Sector wide = new(Vector2.Zero, MathF.PI / 4, 3 * MathF.PI / 2, 10);

        Assert.IsTrue(Overlap.SectorCircle(wide, new Vector2(-10, 0), 1));
        Assert.IsTrue(Overlap.SectorCircle(wide, new Vector2(0, 10), 1));
        Assert.IsFalse(Overlap.SectorCircle(wide, new Vector2(8, 0), 1));
    }

    [TestMethod]
    public void VerticalSegmentAcrossOnlyTheArcIsFound()
    {
        // Neither end is in the sector, and the segment misses both straight edges.
        Assert.IsTrue(Overlap.SectorSegment(Eye, new Vector2(9.9f, -3), new Vector2(9.9f, 3)));
        Assert.IsFalse(Overlap.SectorSegment(Eye, new Vector2(10.1f, -3), new Vector2(10.1f, 3)));
        Assert.IsTrue(Overlap.SectorSegment(Eye, new Vector2(5, -20), new Vector2(5, 20)));
        Assert.IsFalse(Overlap.SectorSegment(Eye, new Vector2(-1, -20), new Vector2(-1, 20)));
    }

    [TestMethod]
    public void SectorPolygonInsideOrAround()
    {
        Assert.IsTrue(Overlap.SectorPolygon(Eye, Square(0, 0, 50)));
        Assert.IsTrue(Overlap.SectorPolygon(Eye, Square(5, 0, 0.5f)));
        Assert.IsTrue(Overlap.SectorPolygon(Eye, Square(5, -5, 0.5f)));
        Assert.IsFalse(Overlap.SectorPolygon(Eye, Square(-5, 0, 2)));
        Assert.IsFalse(Overlap.SectorPolygon(Eye, Square(5, -8, 1)));
    }

    [TestMethod]
    public void SectorsMeetByApexEdgeOrArc()
    {
        Sector facingLeft(float x) => new(new Vector2(x, 0), 3 * MathF.PI / 4, MathF.PI / 2, 10);

        // Only the arcs cross: each one's straight edges stop short of the other.
        Assert.IsTrue(Overlap.Sectors(Eye, facingLeft(19)));
        Assert.IsFalse(Overlap.Sectors(Eye, facingLeft(20.1f)));

        // A small one inside, an edge through the other's side, and two back to back.
        Assert.IsTrue(Overlap.Sectors(Eye, new Sector(new Vector2(5, 0), 0, 0.1f, 1)));
        Assert.IsTrue(Overlap.Sectors(Eye, new Sector(new Vector2(5, -10), MathF.PI / 2, 0.1f, 20)));
        Assert.IsFalse(Overlap.Sectors(Eye, facingLeft(-1)));
        Assert.IsTrue(Overlap.Sectors(Eye, Eye));
    }

    [TestMethod]
    public void SectorTestsEachPartOfAPosedShape()
    {
        // A dumbbell turned upright: its lower ball reaches into the eye, the rest is above it.
        Shape dumbbell = Shape.Compound(
            new ShapePart(Shape.Circle(1), new Vector2(-6, 0)),
            new ShapePart(Shape.Circle(1), new Vector2(6, 0)),
            new ShapePart(Shape.Box(12, 0.5f), Vector2.Zero));

        Assert.IsTrue(Overlap.Test(Eye, dumbbell, new Pose(new Vector2(5, -10), MathF.PI / 2)));
        Assert.IsFalse(Overlap.Test(Eye, dumbbell, new Pose(new Vector2(5, -14), MathF.PI / 2)));
        Assert.IsTrue(Overlap.Test(Eye, Shape.Square(2), new Pose(new Vector2(10.5f, 0), MathF.PI / 4)));
        Assert.IsFalse(Overlap.Test(Eye, Shape.Square(2), new Pose(new Vector2(30, 0), 0)));
    }

    // A square's corners, clockwise on screen.
    private static Vector2[] Square(float x, float y, float half) =>
        [new(x - half, y - half), new(x + half, y - half), new(x + half, y + half), new(x - half, y + half)];
}
