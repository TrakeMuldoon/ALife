using System.Numerics;
using ALife.Spatial.Collisions;
using ALife.Spatial.Physics;
using ALife.Spatial.Shapes;

namespace ALife.Spatial.Tests;

/// <summary>Checks <see cref="ContactResolver"/> and <see cref="RigidBody"/>: bounces, conservation, spin from off-center hits and friction.</summary>
[TestClass]
public sealed class ContactResolverTests
{
    private const float Tolerance = 1e-3f;

    [TestMethod]
    public void ElasticHeadOnHitSwapsVelocities()
    {
        Shape circle = Shape.Circle(1);
        RigidBody a = Body(circle, new(0, 0), new(2, 0));
        RigidBody b = Body(circle, new(1.9f, 0), new(-1, 0));

        Assert.IsTrue(Collide.Test(circle, a.Pose, circle, b.Pose, out Contact contact));
        Assert.IsTrue(ContactResolver.Resolve(ref a, ref b, contact, restitution: 1, friction: 0));

        Assert.AreEqual(-1, a.Velocity.X, Tolerance);
        Assert.AreEqual(2, b.Velocity.X, Tolerance);
        Assert.AreEqual(0, a.AngularVelocity, Tolerance);
        Assert.AreEqual(0, b.AngularVelocity, Tolerance);
    }

    [TestMethod]
    public void InelasticHitLeavesThemMovingTogether()
    {
        Shape circle = Shape.Circle(1);
        RigidBody a = Body(circle, new(0, 0), new(3, 0));
        RigidBody b = Body(circle, new(1.9f, 0), new(-1, 0));

        Collide.Test(circle, a.Pose, circle, b.Pose, out Contact contact);
        ContactResolver.Resolve(ref a, ref b, contact, restitution: 0, friction: 0);

        Assert.AreEqual(1, a.Velocity.X, Tolerance);
        Assert.AreEqual(1, b.Velocity.X, Tolerance);
    }

    [TestMethod]
    public void OffCenterHitSpinsTheBodyAndConservesMomentum()
    {
        // A ball moving down hits the right end of a resting bar, which should start turning clockwise on screen.
        Shape bar = Shape.Box(10, 1), ball = Shape.Circle(0.5f);
        RigidBody a = Body(ball, new(4, -0.9f), new(0, 5));
        RigidBody b = Body(bar, new(0, 0), Vector2.Zero);
        Vector2 momentum = Momentum(ball, a) + Momentum(bar, b);
        float angularMomentum = AngularMomentum(ball, a) + AngularMomentum(bar, b);

        Assert.IsTrue(Collide.Test(ball, a.Pose, bar, b.Pose, out Contact contact));
        Assert.IsTrue(ContactResolver.Resolve(ref a, ref b, contact, restitution: 0.5f, friction: 0.3f, correction: 0));

        Assert.IsTrue(b.AngularVelocity > 0, $"spin {b.AngularVelocity}");
        Assert.IsTrue(b.Velocity.Y > 0);
        Vector2 after = Momentum(ball, a) + Momentum(bar, b);
        Assert.AreEqual(momentum.X, after.X, Tolerance);
        Assert.AreEqual(momentum.Y, after.Y, Tolerance);
        Assert.AreEqual(angularMomentum, AngularMomentum(ball, a) + AngularMomentum(bar, b), Tolerance);
    }

    [TestMethod]
    public void StaticWallBouncesWithoutMoving()
    {
        Shape wall = Shape.Box(2, 20), ball = Shape.Circle(1);
        RigidBody a = RigidBody.Static(Vector2.Zero);
        RigidBody b = Body(ball, new(1.8f, 0), new(-4, 0));

        Collide.Test(wall, a.Pose, ball, b.Pose, out Contact contact);
        ContactResolver.Resolve(ref a, ref b, contact, restitution: 0.5f, friction: 0);

        Assert.AreEqual(2, b.Velocity.X, Tolerance);
        Assert.AreEqual(Vector2.Zero, a.Position);
        Assert.AreEqual(Vector2.Zero, a.Velocity);
        Assert.IsTrue(b.Position.X > 1.8f);
    }

    [TestMethod]
    public void SeparatingBodiesGetNoImpulse()
    {
        Shape circle = Shape.Circle(1);
        RigidBody a = Body(circle, new(0, 0), new(-1, 0));
        RigidBody b = Body(circle, new(1.9f, 0), new(1, 0));

        Collide.Test(circle, a.Pose, circle, b.Pose, out Contact contact);

        Assert.IsFalse(ContactResolver.Resolve(ref a, ref b, contact, restitution: 1, friction: 0.5f));
        Assert.AreEqual(-1, a.Velocity.X, Tolerance);
        Assert.AreEqual(1, b.Velocity.X, Tolerance);
    }

    [TestMethod]
    public void FrictionSlowsSlidingWithoutReversingIt()
    {
        Shape floor = Shape.Box(100, 2), box = Shape.Square(2);
        RigidBody ground = RigidBody.Static(Vector2.Zero);

        // A hard landing with a little sideways speed: friction can stop the slide, but not push it back.
        RigidBody slow = Body(box, new(0, -1.9f), new(0.1f, 10));
        Collide.Test(floor, ground.Pose, box, slow.Pose, out Contact contact);
        ContactResolver.Resolve(ref ground, ref slow, contact, restitution: 0, friction: 1);
        Assert.AreEqual(0, slow.VelocityAt(contact.Point).X, Tolerance);

        // A light landing with a lot of sideways speed: friction takes off at most the friction times the push.
        RigidBody fast = Body(box, new(0, -1.9f), new(10, 0.1f));
        Collide.Test(floor, ground.Pose, box, fast.Pose, out contact);
        ContactResolver.Resolve(ref ground, ref fast, contact, restitution: 0, friction: 1);
        Assert.IsTrue(fast.Velocity.X is > 9 and < 10, $"{fast.Velocity.X}");
    }

    [TestMethod]
    public void IntegrateMovesAndTurns()
    {
        RigidBody body = Body(Shape.Circle(1), new(1, 2), new(3, 4));
        body.AngularVelocity = 2;

        body.Integrate(0.5f);

        Assert.AreEqual(new Vector2(2.5f, 4), body.Position);
        Assert.AreEqual(1, body.Angle, Tolerance);
        Assert.AreEqual(new Vector2(3, 6), body.VelocityAt(body.Position + new Vector2(1, 0)));
    }

    private static RigidBody Body(Shape shape, Vector2 position, Vector2 velocity)
    {
        RigidBody body = RigidBody.FromShape(shape, density: 1, position);
        body.Velocity = velocity;
        return body;
    }

    private static Vector2 Momentum(Shape shape, RigidBody body) => body.Velocity * shape.Area;

    // About the origin: the orbit of the center of mass plus the spin.
    private static float AngularMomentum(Shape shape, RigidBody body) =>
        (shape.Area * ((body.Position.X * body.Velocity.Y) - (body.Position.Y * body.Velocity.X))) +
        (shape.Inertia * body.AngularVelocity);
}
