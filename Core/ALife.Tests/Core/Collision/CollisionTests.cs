using ALife.Core.Collision;
using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility.Colours;

namespace ALife.Tests.Core.Collision
{
    //TODO: Rename this folder and namespace.
    public class ShapeWrapper : IHasShape
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
        /// <summary>The map each test runs against.</summary>
        protected virtual ICollisionMap<ShapeWrapper> CreateMap()
        {
            return new SpatialCollisionMap<ShapeWrapper>(1000, 1000, "TestGrid");
        }

        #region Circle Circle
        [TestMethod]
        public void CircleCircleCollision()
        {
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colour.Red);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colour.Red);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colour.Red);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();

            Point ccp = new Point(50, 50);
            Circle circle = new Circle(ccp, 10);
            ShapeWrapper wrapC = new ShapeWrapper("Circle", circle);

            Point rcp = new Point(50, 64);
            Rectangle rectangle = new Rectangle(rcp, 10, 10, Colour.Red);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(80, 80);
            Circle shape1 = new Circle(point1, 11);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 30, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(85, 70);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(70, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(130, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(115, 130);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(100, 100);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(115, 130);
            Circle shape1 = new Circle(point1, 50);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(120, 110);
            Rectangle shape2 = new Rectangle(point2, 40, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(100, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(120, 110);
            Rectangle shape2 = new Rectangle(point2, 70, 40, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(200, 100);
            Circle shape1 = new Circle(point1, 10);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(120, 110);
            Rectangle shape2 = new Rectangle(point2, 70, 40, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 40);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 45);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(65, 45);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 45);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 20, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(60, 41);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(80, 66);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(84, 69);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(80, 66);
            Rectangle shape2 = new Rectangle(point2, 20, 10, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(84, 69);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 85);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 73);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 85);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(45, 65);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(87, 69);
            Rectangle shape2 = new Rectangle(point2, 20, 50, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(35, 65);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(40, 50);
            Sector shape1 = new Sector(point1, 50, new Angle(70), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(65, 65);
            Rectangle shape2 = new Rectangle(point2, 10, 20, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(40, 50);
            Sector shape1 = new Sector(point1, 25, new Angle(40), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(50, 65);
            Rectangle shape2 = new Rectangle(point2, 40, 60, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 30, new Angle(50), Colour.Pink);
            shape1.Orientation.Degrees = 5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(25, 65);
            Rectangle shape2 = new Rectangle(point2, 20, 40, Colour.Pink);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(80, 116);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(105, 100);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(120, 116);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(105, 100);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(170, 92);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 150;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(105, 100);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(95, 65);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(105, 100);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(95, 65);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(125, 80);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(95, 65);
            Sector shape1 = new Sector(point1, 20, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(125, 80);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(50, 50);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(105, 100);
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
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Point point1 = new Point(20, 20);
            Sector shape1 = new Sector(point1, 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = 0;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Point point2 = new Point(125, 80);
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

        #region Sector Sector
        [TestMethod]
        public void SectorSectorFacingEachOther()
        {
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Sector shape1 = new Sector(new Point(100, 100), 50, new Angle(45), Colour.Pink);
            shape1.Orientation.Degrees = -22.5;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Sector shape2 = new Sector(new Point(190, 100), 50, new Angle(45), Colour.Pink);
            shape2.Orientation.Degrees = 157.5;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            VerifyCollisionTarget(collMap.DetectCollisions(wrap1), wrap2);
            VerifyCollisionTarget(collMap.DetectCollisions(wrap2), wrap1);
        }

        [TestMethod]
        public void SectorSectorBackToBackNC()
        {
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            Sector shape1 = new Sector(new Point(100, 100), 50, new Angle(90), Colour.Pink);
            shape1.Orientation.Degrees = -45;
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Sector shape2 = new Sector(new Point(95, 100), 50, new Angle(90), Colour.Pink);
            shape2.Orientation.Degrees = 135;
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            VerifyCollisionTarget(collMap.DetectCollisions(wrap1));
            VerifyCollisionTarget(collMap.DetectCollisions(wrap2));
        }
        #endregion

        #region AARectangle
        [TestMethod]
        public void AARectangleCircleSide()
        {
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            AARectangle shape1 = new AARectangle(new Point(100, 100), 50, 20, Colour.Pink);
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            Circle shape2 = new Circle(new Point(160, 110), 15);
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            VerifyCollisionTarget(collMap.DetectCollisions(wrap1), wrap2);
            VerifyCollisionTarget(collMap.DetectCollisions(wrap2), wrap1);
        }

        [TestMethod]
        public void AARectangleCircleCornerBBNC()
        {
            ICollisionMap<ShapeWrapper> collMap = CreateMap();
            AARectangle shape1 = new AARectangle(new Point(100, 100), 50, 20, Colour.Pink);
            ShapeWrapper wrap1 = new ShapeWrapper("shape1", shape1);

            // The boxes overlap, but the circle is further than its radius from the corner.
            Circle shape2 = new Circle(new Point(160, 128), 12);
            ShapeWrapper wrap2 = new ShapeWrapper("shape2", shape2);

            if(!collMap.Insert(wrap1)) { Assert.Fail("Failed To Insert wrap 1"); }
            if(!collMap.Insert(wrap2)) { Assert.Fail("Failed To Insert wrap 2"); }

            VerifyCollisionTarget(collMap.DetectCollisions(wrap1));
            VerifyCollisionTarget(collMap.DetectCollisions(wrap2));
        }

        [TestMethod]
        public void AARectangleRectangleAndSector()
        {
            AARectangle box = new AARectangle(new Point(100, 100), 50, 20, Colour.Pink);
            Rectangle turned = new Rectangle(new Point(157, 122), 20, 10, Colour.Pink);
            turned.Orientation.Degrees = 45;
            Rectangle far = new Rectangle(new Point(170, 135), 20, 10, Colour.Pink);
            far.Orientation.Degrees = 45;
            Sector eye = new Sector(new Point(125, 150), 40, new Angle(30), Colour.Pink);
            eye.Orientation.Degrees = 255;

            Assert.IsTrue(CollisionDetector.Collides(box, turned));
            Assert.IsFalse(CollisionDetector.Collides(box, far));
            Assert.IsTrue(CollisionDetector.Collides(eye, box));
            Assert.IsTrue(CollisionDetector.Collides(box, box));
        }
        #endregion

        #region Child shapes
        [TestMethod]
        public void ChildSectorFollowsItsParent()
        {
            // A parent facing down, with an eye 10 below its centre looking down and down-left.
            Circle parent = new Circle(new Point(100, 100), 10);
            parent.Orientation.Degrees = 90;
            ChildSector eye = new ChildSector(parent, new Angle(0), 10, new Angle(0), 30, new Angle(45));

            Assert.IsTrue(CollisionDetector.Collides(eye, new Circle(new Point(95, 130), 3)));
            Assert.IsFalse(CollisionDetector.Collides(eye, new Circle(new Point(120, 130), 3)));

            // Turned a quarter turn further, it now looks left and up-left.
            parent.Orientation.Degrees = 180;
            Assert.IsFalse(CollisionDetector.Collides(eye, new Circle(new Point(95, 130), 3)));
            Assert.IsTrue(CollisionDetector.Collides(eye, new Circle(new Point(70, 95), 3)));
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
