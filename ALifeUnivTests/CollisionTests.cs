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

        [TestMethod] public void CircleRectangleCircleSide2()
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

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 70);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colors.Red);
            ShapeWrapper wrapR = new ShapeWrapper("Rectangle", rectangle);

            if(!collMap.Insert(wrapC)) { Assert.Fail("Failed To Insert"); }
            if(!collMap.Insert(wrapR)) { Assert.Fail("Failed To Insert"); }

            List<ShapeWrapper> collision1 = collMap.DetectCollisions(wrapC);
            VerifyCollisionTarget(collision1, wrapR);

            List<ShapeWrapper> collision2 = collMap.DetectCollisions(wrapR);
            VerifyCollisionTarget(collision2, wrapC);

            Assert.Fail("BAD TEST. Change to BB No NC");
        }

        [TestMethod] public void CircleRectangleCircleCornersNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleContainsRectangle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleRectangleContainsCircle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleNoCollision() { Assert.Fail("Not Implmeented"); }
        #endregion

        #region Sector Rectangle
        [TestMethod]
        public void SectorRectangleLeftArmSide1() 
        {
            Assert.Fail("Not Implmeented"); 
        }
        [TestMethod] public void SectorRectangleLeftArmSide2() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleLeftArmSide3() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleLeftArmSide4() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide1() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide2() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide3() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide4() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide1() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide2() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide3() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide4() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleLeftArmSide1NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleLeftArmSide2NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleLeftArmSide3NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleLeftArmSide4NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide1NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide2NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide3NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSweepSide4NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide1NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide2NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide3NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRightArmSide4NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleSectorContainsRectangle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleRectangleContainsSector() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorRectangleNoCollision() { Assert.Fail("Not Implmeented"); }
        #endregion

        #region Sector Circle
        [TestMethod] public void SectorCircleLeftArm() { Assert.Fail("Not Implmented"); }
        [TestMethod] public void SectorCircleLeftArmNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleSweep() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleSweepNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleRightArm() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleRightArmNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleSecContainsCircle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleCircleContainsSec() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleNoCollision() { Assert.Fail("Not Implmeented"); }
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

