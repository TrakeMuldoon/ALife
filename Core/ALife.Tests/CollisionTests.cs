using ALife.Core.Collision;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using System.Drawing;

namespace ALife.Tests
{
    internal class ShapeWrapper : IHasShape
    {
        public ShapeWrapper(string name, IShape myShape)
        {
            Name = name;
            Shape = myShape;
        }

        public string Name { get; set; }
        public IShape Shape { get; set; }
    }

    [TestClass]
    public class CollisionTests
    {
        #region Circle Circle
        [TestMethod]
        public void CircleCircleCollision()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point p1 = new Core.Geometry.Shapes.Point(50, 50);
            Circle cir1 = new Circle(p1, 15);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Core.Geometry.Shapes.Point p2 = new Core.Geometry.Shapes.Point(61, 61);
            Circle cir2 = new Circle(p2, 15);
            ShapeWrapper wrap2 = new ShapeWrapper("Cir2", cir2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            VerifyCollisionTarget(collision1, wrap2);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void CircleCircleBB_NC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point p1 = new Core.Geometry.Shapes.Point(50, 50);
            Circle cir1 = new Circle(p1, 15);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Core.Geometry.Shapes.Point p2 = new Core.Geometry.Shapes.Point(79, 79);
            Circle cir2 = new Circle(p2, 15);
            ShapeWrapper wrap2 = new ShapeWrapper("Cir2", cir2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            VerifyCollisionTarget(collision1);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void CircleCircleA_Contains_B()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point p1 = new Core.Geometry.Shapes.Point(50, 50);
            Circle cir1 = new Circle(p1, 15);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Core.Geometry.Shapes.Point p2 = new Core.Geometry.Shapes.Point(55, 50);
            Circle cir2 = new Circle(p2, 5);
            ShapeWrapper wrap2 = new ShapeWrapper("Cir2", cir2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            VerifyCollisionTarget(collision1, wrap2);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void CircleCircleNoCollision()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point p1 = new Core.Geometry.Shapes.Point(50, 50);
            Circle cir1 = new Circle(p1, 10);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Core.Geometry.Shapes.Point p2 = new Core.Geometry.Shapes.Point(100, 100);
            Circle cir2 = new Circle(p2, 10);
            ShapeWrapper wrap2 = new ShapeWrapper("Cir2", cir2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            VerifyCollisionTarget(collision1);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);
            VerifyCollisionTarget(collision2);
        }
        #endregion

        #region Circle Core.Geometry.Shapes.Rectangle
        [TestMethod]
        public void CircleRectangleCircleSide1()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point ccp = new Core.Geometry.Shapes.Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Core.Geometry.Shapes.Point rcp = new Core.Geometry.Shapes.Point(50, 64);
            Core.Geometry.Shapes.Rectangle rectangle = new Core.Geometry.Shapes.Rectangle(rcp, 10, 10, Color.Red);
            ShapeWrapper wrapR = new ShapeWrapper("Rectangle", rectangle);

            if(!collMap.Insert(wrapC)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrapR)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrapC);
            VerifyCollisionTarget(collision1, wrapR);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrapR);
            VerifyCollisionTarget(collision2, wrapC);
        }

        [TestMethod]
        public void CircleRectangleCircleSide2()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point ccp = new Core.Geometry.Shapes.Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Core.Geometry.Shapes.Point rcp = new Core.Geometry.Shapes.Point(50, 64);
            Core.Geometry.Shapes.Rectangle rectangle = new Core.Geometry.Shapes.Rectangle(rcp, 10, 10, Color.Red);
            rectangle.Orientation = new Angle(90);
            ShapeWrapper wrapR = new ShapeWrapper("Rectangle", rectangle);

            if(!collMap.Insert(wrapC)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrapR)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrapC);
            VerifyCollisionTarget(collision1, wrapR);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrapR);
            VerifyCollisionTarget(collision2, wrapC);
        }

        [TestMethod]
        public void CircleRectangleCircleSide3()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point ccp = new Core.Geometry.Shapes.Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Core.Geometry.Shapes.Point rcp = new Core.Geometry.Shapes.Point(50, 64);
            Core.Geometry.Shapes.Rectangle rectangle = new Core.Geometry.Shapes.Rectangle(rcp, 10, 10, Color.Red);
            rectangle.Orientation = new Angle(180);
            ShapeWrapper wrapR = new ShapeWrapper("Rectangle", rectangle);

            if(!collMap.Insert(wrapC)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrapR)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrapC);
            VerifyCollisionTarget(collision1, wrapR);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrapR);
            VerifyCollisionTarget(collision2, wrapC);
        }

        [TestMethod]
        public void CircleRectangleCircleSide4()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");

            Core.Geometry.Shapes.Point ccp = new Core.Geometry.Shapes.Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Core.Geometry.Shapes.Point rcp = new Core.Geometry.Shapes.Point(50, 64);
            Core.Geometry.Shapes.Rectangle rectangle = new Core.Geometry.Shapes.Rectangle(rcp, 10, 10, Color.Red);
            rectangle.Orientation = new Angle(270);
            ShapeWrapper wrapR = new ShapeWrapper("Rectangle", rectangle);

            if(!collMap.Insert(wrapC)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrapR)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrapC);
            VerifyCollisionTarget(collision1, wrapR);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrapR);
            VerifyCollisionTarget(collision2, wrapC);
        }

        [TestMethod]
        public void CircleRectangleCircleSide1NC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(80, 80);
            Circle shape1 = new Circle(point1, 11);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(100, 100);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 30, 20, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void CircleRectangleCircleCorners1BBNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(85, 70);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(100, 100);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 40, 20, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void CircleRectangleCircleCorners2BBNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(70, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(100, 100);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 40, 20, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void CircleRectangleCircleCorners3BBNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(130, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(100, 100);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 40, 20, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void CircleRectangleCircleCorners4BBNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(115, 130);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(100, 100);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 40, 20, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void CircleRectangleCircleContainsRectangle()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(115, 130);
            Circle shape1 = new Circle(point1, 50);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(120, 110);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 40, 20, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void CircleRectangleRectangleContainsCircle()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(100, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(120, 110);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 70, 40, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void CircleRectangleNoCollision()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(200, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(120, 110);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 70, 40, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }
        #endregion

        #region Sector Core.Geometry.Shapes.Rectangle
        [TestMethod]
        public void SectorRectangleLeftSideBB_NC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(60, 40);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void SectorRectangleLeftArmSide1()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(60, 45);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 45;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleLeftArmSide2()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(65, 45);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 135;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleLeftArmSide3()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(60, 45);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 215;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleLeftArmSide4()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(60, 41);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 305;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleSweepSide1()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(80, 66);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 299;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleSweepSide2()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(84, 69);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 50, Color.Pink);
            shape2.Orientation.Degrees = 27;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleSweepSide3()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(80, 66);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 10, Color.Pink);
            shape2.Orientation.Degrees = 117;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleSweepSide4()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(84, 69);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 50, Color.Pink);
            shape2.Orientation.Degrees = 207;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleRightArmSide1()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(45, 85);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 50, Color.Pink);
            shape2.Orientation.Degrees = 200;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleRightArmSide2()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(45, 73);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 40, Color.Pink);
            shape2.Orientation.Degrees = 110;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleRightArmSide3()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(45, 85);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 50, Color.Pink);
            shape2.Orientation.Degrees = 20;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleRightArmSide4()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(45, 65);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 40, Color.Pink);
            shape2.Orientation.Degrees = 120;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleSweepBB_NC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(87, 69);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 50, Color.Pink);
            shape2.Orientation.Degrees = 207;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void SectorRectangleRightArmNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(35, 65);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 40, Color.Pink);
            shape2.Orientation.Degrees = 120;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void SectorRectangleSectorContainsRectangle()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(40, 50);
            Sector shape1 = new Sector(point1, 50, new Angle(70), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(65, 65);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 10, 20, Color.Pink);
            shape2.Orientation.Degrees = 120;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleRectangleContainsSector()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(40, 50);
            Sector shape1 = new Sector(point1, 25, new Angle(40), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(50, 65);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 40, 60, Color.Pink);
            shape2.Orientation.Degrees = 120;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorRectangleNoCollision()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Color.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(25, 65);
            Core.Geometry.Shapes.Rectangle shape2 = new Core.Geometry.Shapes.Rectangle(point2, 20, 40, Color.Pink);
            shape2.Orientation.Degrees = 120;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }
        #endregion

        #region Sector Circle
        [TestMethod]
        public void SectorCircleLeftArm()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(80, 116);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(105, 100);
            Circle shape2 = new Circle(point2, 20);
            shape2.Orientation.Degrees = 0;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorCircleLeftArmNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(120, 116);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(105, 100);
            Circle shape2 = new Circle(point2, 20);
            shape2.Orientation.Degrees = 0;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void SectorCircleSweep()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(170, 92);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 150;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(105, 100);
            Circle shape2 = new Circle(point2, 20);
            shape2.Orientation.Degrees = 150;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorCircleRightArm()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(95, 65);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(105, 100);
            Circle shape2 = new Circle(point2, 20);
            shape2.Orientation.Degrees = 150;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorCircleSecContainsCircle()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(95, 65);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(125, 80);
            Circle shape2 = new Circle(point2, 10);
            shape2.Orientation.Degrees = 150;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorCircleCircleContainsSec()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(95, 65);
            Sector shape1 = new Sector(point1, 20, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(125, 80);
            Circle shape2 = new Circle(point2, 40);
            shape2.Orientation.Degrees = 150;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1, wrap2);
            VerifyCollisionTarget(collision2, wrap1);
        }

        [TestMethod]
        public void SectorCircleBBNC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(50, 50);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(105, 100);
            Circle shape2 = new Circle(point2, 20);
            shape2.Orientation.Degrees = 0;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }

        [TestMethod]
        public void SectorCircleNoCollision()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000, "TestGrid");
            Core.Geometry.Shapes.Point point1 = new Core.Geometry.Shapes.Point(20, 20);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Color.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Core.Geometry.Shapes.Point point2 = new Core.Geometry.Shapes.Point(125, 80);
            Circle shape2 = new Circle(point2, 10);
            shape2.Orientation.Degrees = 150;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrap1);
            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrap2);

            VerifyCollisionTarget(collision1);
            VerifyCollisionTarget(collision2);
        }
        #endregion

        void VerifyCollisionTarget(List<ShapeWrapper> detectedCollisions, params ShapeWrapper[] targets)
        {
            if(detectedCollisions.Count != targets.Length)
            {
                Assert.Fail("Wrong Number of collisions. Found: {0}, Expected: {1}", detectedCollisions.Count, targets.Length);
            }

            foreach(ShapeWrapper shape in targets)
            {
                if(!detectedCollisions.Contains(shape))
                {
                    Assert.Fail("Could not find expected collision: {0}", shape.Name);
                }
            }
        }
    }
}
