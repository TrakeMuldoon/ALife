using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Maths;
using System.Numerics;

namespace ALife.Tests.Core.Geometry.Shapes
{
    [TestClass]
    public class TestPoint
    {
        private const double Delta = 1e-10;
        private const double RotationDelta = 1e-9;

        [TestMethod]
        public void Constructor_XY_StoresCorrectly()
        {
            Point p = new Point(3.5, 7.2);
            Assert.AreEqual(3.5, p.X, Delta);
            Assert.AreEqual(7.2, p.Y, Delta);
        }

        [TestMethod]
        public void Constructor_Copy_IsEqual()
        {
            Point original = new Point(1.1, 2.2);
            Point copy = new Point(original);
            Assert.AreEqual(original.X, copy.X, Delta);
            Assert.AreEqual(original.Y, copy.Y, Delta);
        }

        [TestMethod]
        public void Zero_HasZeroCoordinates()
        {
            Assert.AreEqual(0.0, Point.Zero.X, Delta);
            Assert.AreEqual(0.0, Point.Zero.Y, Delta);
        }

        [TestMethod]
        public void XInt_RoundsUp()
        {
            Point p = new Point(3.7, 0);
            Assert.AreEqual(4, p.XInt);
        }

        [TestMethod]
        public void XInt_RoundsDown()
        {
            Point p = new Point(3.3, 0);
            Assert.AreEqual(3, p.XInt);
        }

        [TestMethod]
        public void YInt_RoundsUp()
        {
            Point p = new Point(0, 5.6);
            Assert.AreEqual(6, p.YInt);
        }

        [TestMethod]
        public void YInt_RoundsDown()
        {
            Point p = new Point(0, 5.4);
            Assert.AreEqual(5, p.YInt);
        }

        [TestMethod]
        public void SetX_UpdatesX()
        {
            Point p = new Point(1, 2);
            p.SetX(10);
            Assert.AreEqual(10.0, p.X, Delta);
            Assert.AreEqual(2.0, p.Y, Delta);
        }

        [TestMethod]
        public void SetY_UpdatesY()
        {
            Point p = new Point(1, 2);
            p.SetY(20);
            Assert.AreEqual(1.0, p.X, Delta);
            Assert.AreEqual(20.0, p.Y, Delta);
        }

        [TestMethod]
        public void SetXY_UpdatesBoth()
        {
            Point p = new Point(1, 2);
            p.SetXY(10, 20);
            Assert.AreEqual(10.0, p.X, Delta);
            Assert.AreEqual(20.0, p.Y, Delta);
        }

        [TestMethod]
        public void Clone_CreatesEqualPoint()
        {
            Point original = new Point(3.3, 4.4);
            Point clone = original.Clone();
            Assert.AreEqual(original.X, clone.X, Delta);
            Assert.AreEqual(original.Y, clone.Y, Delta);
        }

        [TestMethod]
        public void Clone_MutatingCloneDoesNotAffectOriginal()
        {
            Point original = new Point(1, 2);
            Point clone = original.Clone();
            clone.SetX(99);
            Assert.AreEqual(1.0, original.X, Delta);
        }

        [TestMethod]
        public void Equals_EqualPoints_ReturnsTrue()
        {
            Point a = new Point(3, 4);
            Point b = new Point(3, 4);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void Equals_DifferentPoints_ReturnsFalse()
        {
            Point a = new Point(3, 4);
            Point b = new Point(3, 5);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void EqualOperator_EqualPoints_ReturnsTrue()
        {
            Point a = new Point(3, 4);
            Point b = new Point(3, 4);
            Assert.IsTrue(a == b);
        }

        [TestMethod]
        public void EqualOperator_DifferentPoints_ReturnsFalse()
        {
            Point a = new Point(3, 4);
            Point b = new Point(5, 4);
            Assert.IsFalse(a == b);
        }

        [TestMethod]
        public void NotEqualOperator_DifferentPoints_ReturnsTrue()
        {
            Point a = new Point(3, 4);
            Point b = new Point(3, 5);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void NotEqualOperator_EqualPoints_ReturnsFalse()
        {
            Point a = new Point(3, 4);
            Point b = new Point(3, 4);
            Assert.IsFalse(a != b);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedFormat()
        {
            Point p = new Point(3, 4);
            Assert.AreEqual("(3, 4)", p.ToString());
        }

        [TestMethod]
        public void ToVector2_ReturnsCorrectVector()
        {
            Point p = new Point(3.5, 4.5);
            Vector2 v = p.ToVector2();
            Assert.AreEqual(3.5f, v.X, 1e-5f);
            Assert.AreEqual(4.5f, v.Y, 1e-5f);
        }

        [TestMethod]
        public void FromTransformation_Identity_ReturnsSamePoint()
        {
            Point p = new Point(5, 3);
            Matrix identity = Matrix.CreateFromTranslation(0, 0);
            Point result = Point.FromTransformation(p, identity);
            Assert.AreEqual(5.0, result.X, Delta);
            Assert.AreEqual(3.0, result.Y, Delta);
        }

        [TestMethod]
        public void FromTransformation_Translation_TranslatesPoint()
        {
            Point p = new Point(1, 2);
            Matrix translation = Matrix.CreateFromTranslation(10, 20);
            Point result = Point.FromTransformation(p, translation);
            Assert.AreEqual(11.0, result.X, Delta);
            Assert.AreEqual(22.0, result.Y, Delta);
        }

        [TestMethod]
        public void FromTransformation_Rotation90Degrees_RotatesCorrectly()
        {
            Point p = new Point(1, 0);
            Matrix rotation = Matrix.CreateFromAngle(Angle.FromDegrees(90));
            Point result = Point.FromTransformation(p, rotation);
            Assert.AreEqual(0.0, result.X, RotationDelta);
            Assert.AreEqual(1.0, result.Y, RotationDelta);
        }

        [TestMethod]
        public void FromTransformation_Rotation180Degrees_RotatesCorrectly()
        {
            Point p = new Point(1, 0);
            Matrix rotation = Matrix.CreateFromAngle(Angle.FromDegrees(180));
            Point result = Point.FromTransformation(p, rotation);
            Assert.AreEqual(-1.0, result.X, RotationDelta);
            Assert.AreEqual(0.0, result.Y, RotationDelta);
        }

        [TestMethod]
        public void GetTransformedPoint_Translation_ReturnsTranslatedPoint()
        {
            Point p = new Point(2, 3);
            Matrix translation = Matrix.CreateFromTranslation(4, 5);
            Point result = p.GetTransformedPoint(translation);
            Assert.AreEqual(6.0, result.X, Delta);
            Assert.AreEqual(8.0, result.Y, Delta);
        }

        [TestMethod]
        public void GetTransformationMatrix_NoArgs_ReturnsTranslationMatrix()
        {
            Point p = new Point(5, 7);
            Matrix m = p.GetTransformationMatrix();
            Assert.AreEqual(5.0, m.M41, Delta);
            Assert.AreEqual(7.0, m.M42, Delta);
            Assert.AreEqual(1.0, m.M11, Delta);
            Assert.AreEqual(1.0, m.M22, Delta);
        }

        [TestMethod]
        public void GetTransformationMatrix_WithAngle_SetsTranslationAndRotation()
        {
            Point p = new Point(5, 3);
            Angle angle = Angle.Zero;
            Matrix m = p.GetTransformationMatrix(angle);
            Assert.AreEqual(5.0, m.M41, Delta);
            Assert.AreEqual(3.0, m.M42, Delta);
        }

        [TestMethod]
        public void Transform_ByPoint_TranslatesCorrectly()
        {
            Point p = new Point(1, 2);
            p.Transform(new Point(3, 4));
            Assert.AreEqual(4.0, p.X, Delta);
            Assert.AreEqual(6.0, p.Y, Delta);
        }

        [TestMethod]
        public void Transform_ByXY_TranslatesCorrectly()
        {
            Point p = new Point(1, 2);
            p.Transform(3.0, 4.0);
            Assert.AreEqual(4.0, p.X, Delta);
            Assert.AreEqual(6.0, p.Y, Delta);
        }

        [TestMethod]
        public void Transform_ByMatrix_AppliesMatrix()
        {
            Point p = new Point(1, 0);
            Matrix translation = Matrix.CreateFromTranslation(5, 5);
            p.Transform(translation);
            Assert.AreEqual(6.0, p.X, Delta);
            Assert.AreEqual(5.0, p.Y, Delta);
        }

        [TestMethod]
        public void Transform_ByAngle90Degrees_RotatesCorrectly()
        {
            Point p = new Point(1, 0);
            p.Transform(Angle.FromDegrees(90));
            Assert.AreEqual(0.0, p.X, RotationDelta);
            Assert.AreEqual(1.0, p.Y, RotationDelta);
        }

        [TestMethod]
        public void Transform_ByAngle180Degrees_RotatesCorrectly()
        {
            Point p = new Point(1, 0);
            p.Transform(Angle.FromDegrees(180));
            Assert.AreEqual(-1.0, p.X, RotationDelta);
            Assert.AreEqual(0.0, p.Y, RotationDelta);
        }

        [TestMethod]
        public void Transform_ByRadiansPiOver2_RotatesCorrectly()
        {
            Point p = new Point(0, 1);
            p.Transform(Math.PI / 2);
            Assert.AreEqual(-1.0, p.X, RotationDelta);
            Assert.AreEqual(0.0, p.Y, RotationDelta);
        }

        [TestMethod]
        public void Transform_ByXYAndAngle_TranslatesAndRotates()
        {
            Point p = new Point(1, 0);
            p.Transform(5.0, 0.0, Angle.FromDegrees(90));
            Assert.AreEqual(5.0, p.X, RotationDelta);
            Assert.AreEqual(1.0, p.Y, RotationDelta);
        }

        [TestMethod]
        public void Transform_ByXYAndRadians_TranslatesAndRotates()
        {
            Point p = new Point(1, 0);
            p.Transform(5.0, 0.0, Math.PI / 2);
            Assert.AreEqual(5.0, p.X, RotationDelta);
            Assert.AreEqual(1.0, p.Y, RotationDelta);
        }

        [TestMethod]
        public void Transform_ByPointAndAngle_TranslatesAndRotates()
        {
            Point p = new Point(1, 0);
            p.Transform(new Point(5, 0), Angle.FromDegrees(90));
            Assert.AreEqual(5.0, p.X, RotationDelta);
            Assert.AreEqual(1.0, p.Y, RotationDelta);
        }
    }
}
