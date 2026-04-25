using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Maths;
using System.Numerics;

namespace ALife.Tests.Core.Utility.Maths
{
    [TestClass]
    public class TestMatrix
    {
        private const double Delta = 1e-10;
        private const double RotationDelta = 1e-9;

        [TestMethod]
        public void DefaultConstructor_AllFieldsAreZero()
        {
            Matrix m = new Matrix();
            Assert.AreEqual(0.0, m.M11, Delta);
            Assert.AreEqual(0.0, m.M12, Delta);
            Assert.AreEqual(0.0, m.M13, Delta);
            Assert.AreEqual(0.0, m.M14, Delta);
            Assert.AreEqual(0.0, m.M21, Delta);
            Assert.AreEqual(0.0, m.M22, Delta);
            Assert.AreEqual(0.0, m.M23, Delta);
            Assert.AreEqual(0.0, m.M24, Delta);
            Assert.AreEqual(0.0, m.M41, Delta);
            Assert.AreEqual(0.0, m.M42, Delta);
            Assert.AreEqual(0.0, m.M43, Delta);
            Assert.AreEqual(0.0, m.M44, Delta);
        }

        [TestMethod]
        public void FullConstructor_StoresAllValues()
        {
            Matrix m = new Matrix(
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16);
            Assert.AreEqual(1.0, m.M11, Delta);
            Assert.AreEqual(2.0, m.M12, Delta);
            Assert.AreEqual(6.0, m.M22, Delta);
            Assert.AreEqual(13.0, m.M41, Delta);
            Assert.AreEqual(16.0, m.M44, Delta);
        }

        [TestMethod]
        public void Zero_AllFieldsAreZero()
        {
            Matrix z = Matrix.Zero;
            Assert.AreEqual(0.0, z.M11, Delta);
            Assert.AreEqual(0.0, z.M44, Delta);
        }

        // CreateFromTranslation

        [TestMethod]
        public void CreateFromTranslation_XY_SetsTranslationFields()
        {
            Matrix m = Matrix.CreateFromTranslation(5.0, 3.0);
            Assert.AreEqual(5.0, m.M41, Delta);
            Assert.AreEqual(3.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslation_XY_SetsIdentityDiagonal()
        {
            Matrix m = Matrix.CreateFromTranslation(5.0, 3.0);
            Assert.AreEqual(1.0, m.M11, Delta);
            Assert.AreEqual(1.0, m.M22, Delta);
            Assert.AreEqual(1.0, m.M33, Delta);
            Assert.AreEqual(1.0, m.M44, Delta);
        }

        [TestMethod]
        public void CreateFromTranslation_XY_OffDiagonalAreZero()
        {
            Matrix m = Matrix.CreateFromTranslation(5.0, 3.0);
            Assert.AreEqual(0.0, m.M12, Delta);
            Assert.AreEqual(0.0, m.M21, Delta);
        }

        [TestMethod]
        public void CreateFromTranslation_Point_SetsTranslationFields()
        {
            Point p = new Point(7.0, 9.0);
            Matrix m = Matrix.CreateFromTranslation(p);
            Assert.AreEqual(7.0, m.M41, Delta);
            Assert.AreEqual(9.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslation_Vector2_SetsTranslationFields()
        {
            Vector2 v = new Vector2(4.0f, 6.0f);
            Matrix m = Matrix.CreateFromTranslation(v);
            Assert.AreEqual(4.0, m.M41, Delta);
            Assert.AreEqual(6.0, m.M42, Delta);
        }

        // CreateFromAngle - rotation matrix tests

        [TestMethod]
        public void CreateFromAngle_ZeroAngle_ReturnsIdentityRotation()
        {
            Matrix m = Matrix.CreateFromAngle(Angle.Zero);
            Assert.AreEqual(1.0, m.M11, Delta);
            Assert.AreEqual(0.0, m.M12, Delta);
            Assert.AreEqual(0.0, m.M21, Delta);
            Assert.AreEqual(1.0, m.M22, Delta);
            Assert.AreEqual(1.0, m.M44, Delta);
        }

        [TestMethod]
        public void CreateFromAngle_90Degrees_HasCorrectSinCos()
        {
            Matrix m = Matrix.CreateFromAngle(Angle.FromDegrees(90));
            Assert.AreEqual(0.0, m.M11, RotationDelta);   // cos(90°) = 0
            Assert.AreEqual(1.0, m.M12, RotationDelta);   // sin(90°) = 1
            Assert.AreEqual(-1.0, m.M21, RotationDelta);  // -sin(90°) = -1
            Assert.AreEqual(0.0, m.M22, RotationDelta);   // cos(90°) = 0
            Assert.AreEqual(1.0, m.M44, Delta);
        }

        [TestMethod]
        public void CreateFromAngle_180Degrees_HasCorrectSinCos()
        {
            Matrix m = Matrix.CreateFromAngle(Angle.FromDegrees(180));
            Assert.AreEqual(-1.0, m.M11, RotationDelta);  // cos(180°) = -1
            Assert.AreEqual(0.0, m.M12, RotationDelta);   // sin(180°) ≈ 0
            Assert.AreEqual(0.0, m.M21, RotationDelta);   // -sin(180°) ≈ 0
            Assert.AreEqual(-1.0, m.M22, RotationDelta);  // cos(180°) = -1
        }

        [TestMethod]
        public void CreateFromAngle_270Degrees_HasCorrectSinCos()
        {
            Matrix m = Matrix.CreateFromAngle(Angle.FromDegrees(270));
            Assert.AreEqual(0.0, m.M11, RotationDelta);   // cos(270°) ≈ 0
            Assert.AreEqual(-1.0, m.M12, RotationDelta);  // sin(270°) = -1
            Assert.AreEqual(1.0, m.M21, RotationDelta);   // -sin(270°) = 1
            Assert.AreEqual(0.0, m.M22, RotationDelta);   // cos(270°) ≈ 0
        }

        [TestMethod]
        public void CreateFromAngle_Radians_SameAsAngleOverload()
        {
            double radians = Math.PI / 4;
            Matrix m1 = Matrix.CreateFromAngle(radians);
            Matrix m2 = Matrix.CreateFromAngle(Angle.FromRadians(radians));
            Assert.AreEqual(m1.M11, m2.M11, Delta);
            Assert.AreEqual(m1.M12, m2.M12, Delta);
            Assert.AreEqual(m1.M21, m2.M21, Delta);
            Assert.AreEqual(m1.M22, m2.M22, Delta);
        }

        [TestMethod]
        public void CreateFromAngle_NoTranslation_M41AndM42AreZero()
        {
            Matrix m = Matrix.CreateFromAngle(Angle.FromDegrees(45));
            Assert.AreEqual(0.0, m.M41, Delta);
            Assert.AreEqual(0.0, m.M42, Delta);
        }

        // CreateFromTranslationAndAngle

        [TestMethod]
        public void CreateFromTranslationAndAngle_ZeroAngle_HasIdentityRotationAndTranslation()
        {
            Matrix m = Matrix.CreateFromTranslationAndAngle(Angle.Zero, 5.0, 3.0);
            // At 0 degrees: M11=cos=1, M12=sin=0, M21=-sin=0, M22=cos=1
            Assert.AreEqual(1.0, m.M11, Delta);
            Assert.AreEqual(0.0, m.M12, Delta);
            Assert.AreEqual(0.0, m.M21, Delta);
            Assert.AreEqual(1.0, m.M22, Delta);
            Assert.AreEqual(5.0, m.M41, Delta);
            Assert.AreEqual(3.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslationAndAngle_SetsTranslationFields()
        {
            Matrix m = Matrix.CreateFromTranslationAndAngle(Angle.FromDegrees(90), 10.0, 20.0);
            Assert.AreEqual(10.0, m.M41, Delta);
            Assert.AreEqual(20.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslationAndAngle_SetsRotationFields()
        {
            Matrix m = Matrix.CreateFromTranslationAndAngle(Angle.FromDegrees(90), 0.0, 0.0);
            Assert.AreEqual(0.0, m.M11, RotationDelta);
            Assert.AreEqual(1.0, m.M12, RotationDelta);
            Assert.AreEqual(-1.0, m.M21, RotationDelta);
            Assert.AreEqual(0.0, m.M22, RotationDelta);
        }

        [TestMethod]
        public void CreateFromTranslationAndAngle_PointOverload_SetsTranslation()
        {
            Point translation = new Point(8, 6);
            Matrix m = Matrix.CreateFromTranslationAndAngle(Angle.Zero, translation);
            Assert.AreEqual(8.0, m.M41, Delta);
            Assert.AreEqual(6.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslationAndAngle_Vector2Overload_SetsTranslation()
        {
            Vector2 translation = new Vector2(3, 9);
            Matrix m = Matrix.CreateFromTranslationAndAngle(Angle.Zero, translation);
            Assert.AreEqual(3.0, m.M41, Delta);
            Assert.AreEqual(9.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslationAndAngle_RadiansPointOverload_SetsTranslation()
        {
            Point translation = new Point(2, 4);
            Matrix m = Matrix.CreateFromTranslationAndAngle(0.0, translation);
            Assert.AreEqual(2.0, m.M41, Delta);
            Assert.AreEqual(4.0, m.M42, Delta);
        }

        [TestMethod]
        public void CreateFromTranslationAndAngle_RadiansVector2Overload_SetsTranslation()
        {
            Vector2 translation = new Vector2(1, 5);
            Matrix m = Matrix.CreateFromTranslationAndAngle(0.0, translation);
            Assert.AreEqual(1.0, m.M41, Delta);
            Assert.AreEqual(5.0, m.M42, Delta);
        }

        // Equals

        [TestMethod]
        public void Equals_SameMatrix_ReturnsTrue()
        {
            Matrix m1 = Matrix.CreateFromTranslation(5, 3);
            Matrix m2 = Matrix.CreateFromTranslation(5, 3);
            Assert.IsTrue(m1.Equals(m2));
        }

        [TestMethod]
        public void Equals_DifferentMatrix_ReturnsFalse()
        {
            Matrix m1 = Matrix.CreateFromTranslation(5, 3);
            Matrix m2 = Matrix.CreateFromTranslation(5, 4);
            Assert.IsFalse(m1.Equals(m2));
        }

        [TestMethod]
        public void Equals_WithinEpsilon_ReturnsTrue()
        {
            Matrix m1 = Matrix.CreateFromTranslation(5.0, 3.0);
            Matrix m2 = Matrix.CreateFromTranslation(5.0, 3.0 + 1e-7);
            Assert.IsTrue(m1.Equals(m2));
        }

        [TestMethod]
        public void Equals_OutsideEpsilon_ReturnsFalse()
        {
            Matrix m1 = Matrix.CreateFromTranslation(5.0, 3.0);
            Matrix m2 = Matrix.CreateFromTranslation(5.0, 3.0 + 0.001);
            Assert.IsFalse(m1.Equals(m2));
        }

        [TestMethod]
        public void Equals_NullObject_ReturnsFalse()
        {
            Matrix m = Matrix.CreateFromTranslation(1, 2);
            Assert.IsFalse(m.Equals(null));
        }

        [TestMethod]
        public void ToString_ContainsBrackets()
        {
            Matrix m = Matrix.Zero;
            string s = m.ToString();
            Assert.IsTrue(s.Contains("["));
            Assert.IsTrue(s.Contains("]"));
        }
    }
}
