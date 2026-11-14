using ALife.Core.Collision;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;

namespace ALife.Tests.Core.Collision
{
    /// <summary>What <see cref="SpatialCollisionMap{T}"/> does beyond the collisions <see cref="CollisionTests"/> checks.</summary>
    [TestClass]
    public class SpatialCollisionMapTests
    {
        [TestMethod]
        public void MovedObjectIsFoundWhereItWent()
        {
            SpatialCollisionMap<ShapeWrapper> map = new SpatialCollisionMap<ShapeWrapper>(1000, 1000, "TestGrid");
            ShapeWrapper mover = new ShapeWrapper("Mover", new Circle(new Point(50, 50), 10));
            ShapeWrapper still = new ShapeWrapper("Still", new Circle(new Point(800, 800), 10));
            map.Insert(mover);
            map.Insert(still);
            Assert.AreEqual(0, map.DetectCollisions(still).Count);

            mover.Shape.CentrePoint = new Point(805, 805);
            map.MoveObject(mover);

            CollectionAssert.AreEqual(new[] { mover }, map.DetectCollisions(still));
            Assert.AreEqual(0, map.QueryForBoundingBoxCollisions(new BoundingBox(30, 30, 70, 70)).Count);
        }

        [TestMethod]
        public void MovedRectangleIsFoundWhereItIsNotWhereItsCachedBoxSays()
        {
            SpatialCollisionMap<ShapeWrapper> map = new SpatialCollisionMap<ShapeWrapper>(1000, 1000, "TestGrid");
            Rectangle rectangle = new Rectangle(new Point(500, 500), 20, 10, Colour.Pink);
            ShapeWrapper mover = new ShapeWrapper("Mover", rectangle);
            ShapeWrapper still = new ShapeWrapper("Still", new Circle(new Point(598, 500), 5));
            map.Insert(mover);
            map.Insert(still);

            // Moved and turned without a Reset, so its BoundingBox is still the old one.
            BoundingBox cached = rectangle.BoundingBox;
            rectangle.CentrePoint = new Point(590, 500);
            rectangle.Orientation.Degrees = 90;
            map.MoveObject(mover);
            Assert.AreEqual(cached.MinX, rectangle.BoundingBox.MinX);

            CollectionAssert.AreEqual(new[] { mover }, map.QueryForBoundingBoxCollisions(new BoundingBox(589, 508, 591, 509)));
            Assert.AreEqual(0, map.QueryForBoundingBoxCollisions(new BoundingBox(495, 495, 505, 505)).Count);
            CollectionAssert.AreEqual(new[] { mover }, map.DetectCollisions(still));
        }

        [TestMethod]
        public void RemovedObjectIsGoneAndTheRestRemain()
        {
            SpatialCollisionMap<ShapeWrapper> map = new SpatialCollisionMap<ShapeWrapper>(1000, 1000, "TestGrid");
            List<ShapeWrapper> wrappers = new List<ShapeWrapper>();
            for(int i = 0; i < 10; i++)
            {
                ShapeWrapper wrapper = new ShapeWrapper("W" + i, new Circle(new Point(50 + (i * 30), 50), 10));
                wrappers.Add(wrapper);
                map.Insert(wrapper);
            }

            map.RemoveObject(wrappers[3]);
            map.RemoveObject(wrappers[0]);

            Assert.AreEqual(8, map.Count);
            CollectionAssert.AreEquivalent(wrappers.Skip(1).Where(w => w != wrappers[3]).ToList(), map.EnumerateItems().ToList());
            Assert.AreEqual(0, map.QueryForBoundingBoxCollisions(wrappers[3].Shape.BoundingBox).Count);
            CollectionAssert.AreEqual(new[] { wrappers[4] }, map.QueryForBoundingBoxCollisions(wrappers[4].Shape.BoundingBox));

            // A removed object's handle is reused by the next one in.
            ShapeWrapper late = new ShapeWrapper("Late", new Circle(new Point(500, 500), 10));
            map.Insert(late);
            CollectionAssert.AreEqual(new[] { late }, map.QueryForBoundingBoxCollisions(late.Shape.BoundingBox));
        }

        [TestMethod]
        public void TouchingBoxesDoNotCollide()
        {
            SpatialCollisionMap<ShapeWrapper> map = new SpatialCollisionMap<ShapeWrapper>(1000, 1000, "TestGrid");
            map.Insert(new ShapeWrapper("Box", new Circle(new Point(50, 50), 10)));

            Assert.AreEqual(0, map.QueryForBoundingBoxCollisions(new BoundingBox(60, 40, 80, 60)).Count);
            Assert.AreEqual(1, map.QueryForBoundingBoxCollisions(new BoundingBox(59, 40, 80, 60)).Count);
        }

        [TestMethod]
        public void BigAndOutOfBoundsObjectsAreFound()
        {
            SpatialCollisionMap<ShapeWrapper> map = new SpatialCollisionMap<ShapeWrapper>(1000, 1000, "TestGrid");
            ShapeWrapper wall = new ShapeWrapper("Wall", new Circle(new Point(500, 500), 700));
            ShapeWrapper outside = new ShapeWrapper("Outside", new Circle(new Point(-40, 1100), 10));
            map.Insert(wall);
            map.Insert(outside);

            CollectionAssert.AreEqual(new[] { wall }, map.QueryForBoundingBoxCollisions(new BoundingBox(10, 10, 20, 20)));
            CollectionAssert.AreEquivalent(new[] { wall, outside }, map.QueryForBoundingBoxCollisions(new BoundingBox(-45, 1095, -35, 1105)));

            map.RemoveObject(wall);
            Assert.AreEqual(0, map.QueryForBoundingBoxCollisions(new BoundingBox(10, 10, 20, 20)).Count);
        }
    }
}
