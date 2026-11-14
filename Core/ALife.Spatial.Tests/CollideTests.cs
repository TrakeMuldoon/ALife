using System.Numerics;
using ALife.Spatial.Collisions;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Tests;

/// <summary>Checks <see cref="Collide.Test"/> for each pair of part kinds, compounds, and its normals and depths on random poses.</summary>
[TestClass]
public sealed class CollideTests
{
    private const float Tolerance = 1e-4f;

    public static IEnumerable<object[]> ShapeKinds => [["circle"], ["square"], ["triangle"], ["hexagon"], ["compound"]];

    [TestMethod]
    public void OverlappingCirclesMeetHalfwayThroughTheOverlap()
    {
        Shape circle = Shape.Circle(1);

        Assert.IsTrue(Collide.Test(circle, At(0, 0), circle, At(1.5f, 0), out Contact contact));
        AssertContact(contact, new(1, 0), 0.5f, new(0.75f, 0));
        Assert.IsFalse(Collide.Test(circle, At(0, 0), circle, At(2.1f, 0), out _));
    }

    [TestMethod]
    public void CircleAgainstAFace()
    {
        Shape box = Shape.Square(2), circle = Shape.Circle(1);

        Assert.IsTrue(Collide.Test(box, At(0, 0), circle, At(1.5f, 0), out Contact contact));
        AssertContact(contact, new(1, 0), 0.5f, new(0.75f, 0));

        Assert.IsTrue(Collide.Test(circle, At(1.5f, 0), box, At(0, 0), out contact));
        AssertContact(contact, new(-1, 0), 0.5f, new(0.75f, 0));

        Assert.IsFalse(Collide.Test(box, At(0, 0), circle, At(2.1f, 0), out _));
    }

    [TestMethod]
    public void CircleAgainstACorner()
    {
        Shape box = Shape.Square(2), circle = Shape.Circle(1);
        float distance = MathF.Sqrt(0.5f);

        Assert.IsTrue(Collide.Test(box, At(0, 0), circle, At(1.5f, 1.5f), out Contact contact));
        AssertContact(contact, Vector2.Normalize(new(1, 1)), 1 - distance, new Vector2(1.5f, 1.5f) - (Vector2.Normalize(new(1, 1)) * (1 - ((1 - distance) / 2))));

        // Inside the corner's square but beyond the radius.
        Assert.IsFalse(Collide.Test(box, At(0, 0), circle, At(1.8f, 1.8f), out _));
    }

    [TestMethod]
    public void CircleCenteredInsideAPolygonIsPushedOutTheNearestFace()
    {
        Assert.IsTrue(Collide.Test(Shape.Square(2), At(0, 0), Shape.Circle(0.5f), At(0, -0.8f), out Contact contact));

        Assert.AreEqual(0, contact.Normal.X, Tolerance);
        Assert.AreEqual(-1, contact.Normal.Y, Tolerance);
        Assert.AreEqual(0.7f, contact.Depth, Tolerance);
    }

    [TestMethod]
    public void BoxesPushApartAlongTheShallowestAxis()
    {
        Shape box = Shape.Square(2);

        Assert.IsTrue(Collide.Test(box, At(0, 0), box, At(1.5f, 0.5f), out Contact contact));
        AssertContact(contact, new(1, 0), 0.5f, new(0.75f, 0.25f));

        Assert.IsTrue(Collide.Test(box, At(1.5f, 0.5f), box, At(0, 0), out contact));
        AssertContact(contact, new(-1, 0), 0.5f, new(0.75f, 0.25f));

        Assert.IsFalse(Collide.Test(box, At(0, 0), box, At(2.1f, 0.5f), out _));
    }

    [TestMethod]
    public void CornerIntoAFace()
    {
        // A square turned 45 degrees, its left corner 0.1 into the other square's right face.
        Shape box = Shape.Square(2), diamond = Shape.Square(MathF.Sqrt(2));

        Assert.IsTrue(Collide.Test(box, At(0, 0), diamond, new Pose(new(1.9f, 0), MathF.PI / 4), out Contact contact));
        AssertContact(contact, new(1, 0), 0.1f, new(0.95f, 0));

        Assert.IsFalse(Collide.Test(box, At(0, 0), diamond, new Pose(new(2.1f, 0), MathF.PI / 4), out _));
    }

    [TestMethod]
    public void CompoundGivesTheDeepestPartContact()
    {
        // A dumbbell with circles at x = -2 and 2, against two small circles at -3.3 and 3.3 from their middle: both ends
        // touch, and whichever end the probe is nudged towards is the shallower one.
        Shape circle = Shape.Circle(1), small = Shape.Circle(0.5f);
        Shape dumbbell = Shape.Compound(new ShapePart(circle, new(-2, 0)), new ShapePart(circle, new(2, 0)));
        Shape probe = Shape.Compound(new ShapePart(small, new(-3.3f, 0)), new ShapePart(small, new(3.3f, 0)));

        // Nudged left: 0.1 into the left end, 0.3 into the right.
        Assert.IsTrue(Collide.Test(dumbbell, At(0, 0), probe, At(-0.1f, 0), out Contact contact));
        Assert.AreEqual(1, contact.Normal.X, Tolerance);
        Assert.AreEqual(0.3f, contact.Depth, Tolerance);
        Assert.IsTrue(contact.Point.X > 0);

        Assert.IsTrue(Collide.Test(dumbbell, At(0, 0), probe, At(0.1f, 0), out contact));
        Assert.AreEqual(-1, contact.Normal.X, Tolerance);
        Assert.AreEqual(0.3f, contact.Depth, Tolerance);
        Assert.IsTrue(contact.Point.X < 0);
    }

    [TestMethod]
    public void CompoundMissesThroughItsGap()
    {
        Shape circle = Shape.Circle(1);
        Shape dumbbell = Shape.Compound(new ShapePart(circle, new(-2, 0)), new ShapePart(circle, new(2, 0)));

        Assert.IsFalse(Collide.Test(dumbbell, At(0, 0), Shape.Circle(0.5f), At(0, 0), out _));
    }

    [TestMethod]
    [DynamicData(nameof(ShapeKinds))]
    public void RandomPosesGiveConsistentContacts(string kind)
    {
        Random random = new(1234);
        Shape a = Make(kind), b = Make("triangle"), c = Make("compound");
        int hits = 0;
        for (int i = 0; i < 2000; i++)
        {
            Shape other = i % 2 == 0 ? b : c;
            Pose poseA = new(Vector2.Zero, random.NextSingle() * 7);
            Pose poseB = new(new Vector2((random.NextSingle() * 8) - 4, (random.NextSingle() * 8) - 4), random.NextSingle() * 7);

            bool hit = Collide.Test(a, poseA, other, poseB, out Contact contact);
            bool reverse = Collide.Test(other, poseB, a, poseA, out _);
            Assert.AreEqual(hit, reverse, $"pose {i}");
            if (!hit)
            {
                continue;
            }

            hits++;
            Assert.AreEqual(1, contact.Normal.Length(), Tolerance);
            Assert.IsTrue(contact.Depth >= 0);

            // Moving the second shape out along the normal by the depth leaves at most a sliver of overlap, unless a
            // compound's other part pair was deeper in another direction.
            Pose moved = new(poseB.Position + (contact.Normal * (contact.Depth + 0.01f)), poseB.Angle);
            if (Collide.Test(a, poseA, other, moved, out Contact after) && kind != "compound" && other != c)
            {
                Assert.IsTrue(after.Depth < 0.02f, $"pose {i}: {after.Depth} left after moving {contact.Depth}");
            }
        }

        Assert.IsTrue(hits > 200, $"only {hits} hits");
    }

    private static Pose At(float x, float y) => new(new Vector2(x, y), 0);

    private static Shape Make(string kind) => kind switch
    {
        "circle" => Shape.Circle(1),
        "square" => Shape.Square(2),
        "triangle" => Shape.Triangle(2.5f),
        "hexagon" => Shape.RegularPolygon(6, 1.2f),
        _ => Shape.Compound(new ShapePart(Shape.Square(1.5f), Vector2.Zero), new ShapePart(Shape.Circle(0.6f), new(1.4f, 0))),
    };

    private static void AssertContact(Contact contact, Vector2 normal, float depth, Vector2 point)
    {
        Assert.AreEqual(normal.X, contact.Normal.X, Tolerance, "normal x");
        Assert.AreEqual(normal.Y, contact.Normal.Y, Tolerance, "normal y");
        Assert.AreEqual(depth, contact.Depth, Tolerance, "depth");
        Assert.AreEqual(point.X, contact.Point.X, Tolerance, "point x");
        Assert.AreEqual(point.Y, contact.Point.Y, Tolerance, "point y");
    }
}
