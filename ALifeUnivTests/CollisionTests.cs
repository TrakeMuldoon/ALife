using ALifeUni.ALife;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace AlifeTestProject
{
    class ShapeWrapper : IHasShape
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point p1 = new Point(50, 50);
            Circle cir1 = new Circle(p1, 15);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Point p2 = new Point(61, 61);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point p1 = new Point(50, 50);
            Circle cir1 = new Circle(p1, 15);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Point p2 = new Point(79, 79);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point p1 = new Point(50, 50);
            Circle cir1 = new Circle(p1, 15);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Point p2 = new Point(55, 50);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point p1 = new Point(50, 50);
            Circle cir1 = new Circle(p1, 10);
            ShapeWrapper wrap1 = new ShapeWrapper("Cir1", cir1);

            Point p2 = new Point(100, 100);
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

        #region Circle Rectangle
        [TestMethod]
        public void CircleRectangleCircleSide1()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colors.Red);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colors.Red);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colors.Red);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colors.Red);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(80, 80);
            Circle shape1 = new Circle(point1, 11);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 30, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(85, 70);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(70, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(130, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(115, 130);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(115, 130);
            Circle shape1 = new Circle(point1, 50);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(120, 110);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(100, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(120, 110);
            Rectangle shape2 = new Rectangle(point2, 70, 40, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(200, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(120, 110);
            Rectangle shape2 = new Rectangle(point2, 70, 40, Colors.Pink);
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

        #region Sector Rectangle
        [TestMethod]
        public void SectorRectangleLeftSideBB_NC()
        {
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 40);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 45);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(65, 45);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 45);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 41);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(80, 66);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(84, 69);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(80, 66);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(84, 69);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 85);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 73);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 85);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 65);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(87, 69);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(35, 65);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(40, 50);
            Sector shape1 = new Sector(point1, 50, new Angle(70), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(65, 65);
            Rectangle shape2 = new Rectangle(point2, 10, 20, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(40, 50);
            Sector shape1 = new Sector(point1, 25, new Angle(40), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(50, 65);
            Rectangle shape2 = new Rectangle(point2, 40, 60, Colors.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = new CollisionGrid<ShapeWrapper>(1000, 1000);
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colors.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(25, 65);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colors.Pink);
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
        { Assert.Fail("Not Implmented"); }
        
        [TestMethod]
        public void SectorCircleLeftArmNC()
        { Assert.Fail("Not Implmeented"); }
        
        [TestMethod]
        public void SectorCircleSweep()
        { Assert.Fail("Not Implmeented"); }

        [TestMethod]
        public void SectorCircleSweepNC()
        { Assert.Fail("Not Implmeented"); }

        [TestMethod]
        public void SectorCircleRightArm()
        { Assert.Fail("Not Implmeented"); }
        
        [TestMethod]
        public void SectorCircleRightArmNC()
        { Assert.Fail("Not Implmeented"); }
        
        [TestMethod]
        public void SectorCircleSecContainsCircle()
        { Assert.Fail("Not Implmeented"); }
        
        [TestMethod]
        public void SectorCircleCircleContainsSec() 
        { Assert.Fail("Not Implmeented"); }
        
        [TestMethod]
        public void SectorCircleNoCollision()
        { Assert.Fail("Not Implmeented"); }
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

