using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlifeTestProject
{
    [TestClass]
    public class CollisionTests
    {

        [TestMethod] public void SectorCircleLeftArm() { Assert.Fail("Not Implmented"); }
        [TestMethod] public void SectorCircleLeftArmNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleSweep() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleSweepNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleRightArm() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleRightArmNC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleSecContainsCircle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleCircleContainsSec() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void SectorCircleNoCollision() { Assert.Fail("Not Implmeented"); }

        #region Sector Rectangle
        [TestMethod] public void SectorRectangleLeftArmSide1() { Assert.Fail("Not Implmeented"); }
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

        #region Circle Circle
        [TestMethod] public void CircleCircleCollision() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleCircleBB_NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleCircleCirAContainsCirB() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleCircleCirBContainsCirA() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleCircleNoCollision() { Assert.Fail("Not Implmeented"); }
        #endregion

        #region Circle Rectangle
        [TestMethod] public void CircleRectangleCircleSide1() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide1NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide2() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide2NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide3() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide3NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide4() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleSide4NC() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleCircleContainsRectangle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleRectangleContainsCircle() { Assert.Fail("Not Implmeented"); }
        [TestMethod] public void CircleRectangleNoCollision() { Assert.Fail("Not Implmeented"); }
        #endregion
    }
}

