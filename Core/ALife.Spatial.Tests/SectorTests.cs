using System.Numerics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Tests;

/// <summary>Checks <see cref="Sector"/>: which directions its sweep holds, narrow, reflex and full, and its bounds.</summary>
[TestClass]
public sealed class SectorTests
{
    private const float Tolerance = 1e-4f;

    [TestMethod]
    public void NarrowSweepHoldsOnlyItsWedge()
    {
        Sector sector = new(Vector2.Zero, 0, MathF.PI / 4, 10);

        Assert.IsTrue(sector.InSweep(new Vector2(1, 0.5f)));
        Assert.IsTrue(sector.InSweep(Vector2.UnitX));
        Assert.IsFalse(sector.InSweep(new Vector2(1, -0.1f)));
        Assert.IsFalse(sector.InSweep(new Vector2(0.1f, 1)));
        Assert.IsFalse(sector.InSweep(-Vector2.UnitX));
    }

    [TestMethod]
    public void ZeroSweepIsARayNotALine()
    {
        Sector sector = new(Vector2.Zero, 0, 0, 10);

        Assert.IsTrue(sector.InSweep(Vector2.UnitX));
        Assert.IsFalse(sector.InSweep(-Vector2.UnitX));
    }

    [TestMethod]
    public void SweepTurnsClockwiseOnScreen()
    {
        // From right, a quarter turn reaches down, which is +y.
        Sector sector = new(Vector2.Zero, 0, MathF.PI / 2, 10);

        Assert.IsTrue(sector.InSweep(new Vector2(1, 1)));
        Assert.IsFalse(sector.InSweep(new Vector2(1, -1)));
        Assert.AreEqual(0, sector.EndPoint.X, Tolerance);
        Assert.AreEqual(10, sector.EndPoint.Y, Tolerance);
    }

    [TestMethod]
    public void ReflexSweepHoldsAllButTheGap()
    {
        Sector sector = new(Vector2.Zero, 0, 3 * MathF.PI / 2, 10);

        Assert.IsTrue(sector.InSweep(new Vector2(1, 0.01f)));
        Assert.IsTrue(sector.InSweep(Vector2.UnitY));
        Assert.IsTrue(sector.InSweep(-Vector2.UnitX));
        Assert.IsTrue(sector.InSweep(new Vector2(-1, -1)));
        Assert.IsFalse(sector.InSweep(new Vector2(1, -1)));
    }

    [TestMethod]
    public void FullSweepHoldsEverythingAndIsClamped()
    {
        Sector sector = new(new Vector2(3, 4), 1, 10, 5);

        Assert.IsTrue(sector.IsFull);
        Assert.AreEqual(MathF.Tau, sector.Sweep);
        Assert.IsTrue(sector.InSweep(new Vector2(-1, -1)));
        Assert.IsTrue(sector.Contains(new Vector2(3, 8.9f)));
        Assert.IsFalse(sector.Contains(new Vector2(3, 9.1f)));
        Assert.AreEqual(new Aabb(-2, -1, 8, 9), sector.GetBounds());
    }

    [TestMethod]
    public void ContainsNeedsTheSweepAndTheRadius()
    {
        Sector sector = new(new Vector2(10, 10), 0, MathF.PI / 2, 5);

        Assert.IsTrue(sector.Contains(new Vector2(10, 10)));
        Assert.IsTrue(sector.Contains(new Vector2(13, 13)));
        Assert.IsFalse(sector.Contains(new Vector2(14, 14)));
        Assert.IsFalse(sector.Contains(new Vector2(7, 7)));
    }

    [TestMethod]
    public void BoundsHoldTheArcWhereItPassesAnAxis()
    {
        Aabb quarter = new Sector(new Vector2(10, 10), 0, MathF.PI / 2, 5).GetBounds();
        Aabb leftHalf = new Sector(new Vector2(10, 10), MathF.PI / 2, MathF.PI, 5).GetBounds();

        Assert.AreEqual(10, quarter.Left, Tolerance);
        Assert.AreEqual(10, quarter.Top, Tolerance);
        Assert.AreEqual(15, quarter.Right, Tolerance);
        Assert.AreEqual(15, quarter.Bottom, Tolerance);

        Assert.AreEqual(5, leftHalf.Left, Tolerance);
        Assert.AreEqual(5, leftHalf.Top, Tolerance);
        Assert.AreEqual(10, leftHalf.Right, Tolerance);
        Assert.AreEqual(15, leftHalf.Bottom, Tolerance);
    }

    [TestMethod]
    public void NegativeSweepOrRadiusIsRejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Sector(Vector2.Zero, 0, -1, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Sector(Vector2.Zero, 0, 1, -1));
    }
}
