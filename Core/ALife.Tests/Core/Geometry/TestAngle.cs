using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Maths;

namespace ALife.Tests.Core.Geometry
{
    [TestClass]
    public class TestAngle
    {
        private const double Delta = 1e-10;

        [TestMethod]
        public void Constructor_Degrees_StoresCorrectly()
        {
            Angle a = new Angle(45);
            Assert.AreEqual(45.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void Constructor_DegreesAbove360_Wraps()
        {
            Angle a = new Angle(370);
            Assert.AreEqual(10.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void Constructor_NegativeDegrees_Wraps()
        {
            Angle a = new Angle(-10);
            Assert.AreEqual(350.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void Constructor_Exactly360_WrapsToZero()
        {
            Angle a = new Angle(360);
            Assert.AreEqual(0.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void Constructor_Copy_IsEqual()
        {
            Angle original = new Angle(123.456);
            Angle copy = new Angle(original);
            Assert.AreEqual(original.Degrees, copy.Degrees, Delta);
            Assert.AreEqual(original.Radians, copy.Radians, Delta);
        }

        [TestMethod]
        public void Constructor_WithRadiansFlagTrue_ConvertsFromRadians()
        {
            Angle a = new Angle(Math.PI, true);
            Assert.AreEqual(180.0, a.Degrees, Delta);
            Assert.AreEqual(Math.PI, a.Radians, Delta);
        }

        [TestMethod]
        public void Constructor_WithRadiansFlagFalse_TreatesAsDegrees()
        {
            Angle a = new Angle(90.0, false);
            Assert.AreEqual(90.0, a.Degrees, Delta);
            Assert.AreEqual(Math.PI / 2, a.Radians, Delta);
        }

        [TestMethod]
        public void Zero_HasZeroDegrees()
        {
            Assert.AreEqual(0.0, Angle.Zero.Degrees, Delta);
            Assert.AreEqual(0.0, Angle.Zero.Radians, Delta);
        }

        [TestMethod]
        public void Degrees_Getter_ReturnsCorrectValue()
        {
            Angle a = new Angle(135);
            Assert.AreEqual(135.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void Degrees_Setter_UpdatesDegreesAndRadians()
        {
            Angle a = new Angle(0);
            a.Degrees = 90;
            Assert.AreEqual(90.0, a.Degrees, Delta);
            Assert.AreEqual(Math.PI / 2, a.Radians, Delta);
        }

        [TestMethod]
        public void Radians_Getter_ReturnsCorrectValue()
        {
            Angle a = new Angle(180);
            Assert.AreEqual(Math.PI, a.Radians, Delta);
        }

        [TestMethod]
        public void Radians_Getter_90Degrees()
        {
            Angle a = new Angle(90);
            Assert.AreEqual(Math.PI / 2, a.Radians, Delta);
        }

        [TestMethod]
        public void Radians_Setter_UpdatesDegreesAndRadians()
        {
            Angle a = new Angle(0);
            a.Radians = Math.PI;
            Assert.AreEqual(180.0, a.Degrees, Delta);
            Assert.AreEqual(Math.PI, a.Radians, Delta);
        }

        [TestMethod]
        public void InverseDegrees_Returns_NegativeComplement()
        {
            Angle a = new Angle(270);
            Assert.AreEqual(-90.0, a.InverseDegrees, Delta);
        }

        [TestMethod]
        public void InverseDegrees_180Degrees_ReturnsNegative180()
        {
            Angle a = new Angle(180);
            Assert.AreEqual(-180.0, a.InverseDegrees, Delta);
        }

        [TestMethod]
        public void InverseRadians_Returns_NegativeComplement()
        {
            Angle a = new Angle(270);
            Assert.AreEqual(-(Math.PI / 2), a.InverseRadians, Delta);
        }

        [TestMethod]
        public void SetDegrees_UpdatesDegreesAndRadians()
        {
            Angle a = new Angle(0);
            a.SetDegrees(90);
            Assert.AreEqual(90.0, a.Degrees, Delta);
            Assert.AreEqual(Math.PI / 2, a.Radians, Delta);
        }

        [TestMethod]
        public void FromDegrees_ReturnsCorrectAngle()
        {
            Angle a = Angle.FromDegrees(90);
            Assert.AreEqual(90.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void FromRadians_Pi_Returns180Degrees()
        {
            Angle a = Angle.FromRadians(Math.PI);
            Assert.AreEqual(180.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void FromRadians_NegativePi_Returns180Degrees()
        {
            Angle a = Angle.FromRadians(-Math.PI);
            Assert.AreEqual(180.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void FromRadians_ThreePi_Returns180Degrees()
        {
            Angle a = Angle.FromRadians(3 * Math.PI);
            Assert.AreEqual(180.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void FromRadians_Zero_ReturnsZero()
        {
            Angle a = Angle.FromRadians(0);
            Assert.AreEqual(0.0, a.Degrees, Delta);
        }

        [TestMethod]
        public void AddAngleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(45);
            Angle result = a + b;
            Assert.AreEqual(135.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void AddAngleAngle_WrapsCorrectly()
        {
            Angle a = new Angle(350);
            Angle b = new Angle(20);
            Angle result = a + b;
            Assert.AreEqual(10.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void AddAngleDouble_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle result = a + 45.0;
            Assert.AreEqual(135.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void AddDoubleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle result = 45.0 + a;
            Assert.AreEqual(135.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void SubtractAngleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(45);
            Angle result = a - b;
            Assert.AreEqual(45.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void SubtractAngleAngle_WrapsCorrectly()
        {
            Angle a = new Angle(10);
            Angle b = new Angle(20);
            Angle result = a - b;
            Assert.AreEqual(350.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void SubtractAngleDouble_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle result = a - 45.0;
            Assert.AreEqual(45.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void SubtractDoubleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(45);
            Angle result = 90.0 - a;
            Assert.AreEqual(45.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void MultiplyAngleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(45);
            Angle b = new Angle(2);
            Angle result = a * b;
            Assert.AreEqual(90.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void MultiplyAngleDouble_ReturnsCorrectResult()
        {
            Angle a = new Angle(45);
            Angle result = a * 2.0;
            Assert.AreEqual(90.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void MultiplyDoubleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(45);
            Angle result = 2.0 * a;
            Assert.AreEqual(90.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void DivideAngleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(2);
            Angle result = a / b;
            Assert.AreEqual(45.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void DivideAngleDouble_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle result = a / 2.0;
            Assert.AreEqual(45.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void DivideDoubleAngle_ReturnsCorrectResult()
        {
            Angle a = new Angle(90);
            Angle result = 180.0 / a;
            Assert.AreEqual(2.0, result.Degrees, Delta);
        }

        [TestMethod]
        public void EqualOperator_EqualAngles_ReturnsTrue()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(90);
            Assert.IsTrue(a == b);
        }

        [TestMethod]
        public void EqualOperator_DifferentAngles_ReturnsFalse()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(91);
            Assert.IsFalse(a == b);
        }

        [TestMethod]
        public void NotEqualOperator_DifferentAngles_ReturnsTrue()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(91);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void NotEqualOperator_EqualAngles_ReturnsFalse()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(90);
            Assert.IsFalse(a != b);
        }

        [TestMethod]
        public void LessThan_Smaller_ReturnsTrue()
        {
            Angle a = new Angle(45);
            Angle b = new Angle(90);
            Assert.IsTrue(a < b);
        }

        [TestMethod]
        public void LessThan_Larger_ReturnsFalse()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(45);
            Assert.IsFalse(a < b);
        }

        [TestMethod]
        public void LessThanOrEqual_Equal_ReturnsTrue()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(90);
            Assert.IsTrue(a <= b);
        }

        [TestMethod]
        public void GreaterThan_Larger_ReturnsTrue()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(45);
            Assert.IsTrue(a > b);
        }

        [TestMethod]
        public void GreaterThan_Smaller_ReturnsFalse()
        {
            Angle a = new Angle(45);
            Angle b = new Angle(90);
            Assert.IsFalse(a > b);
        }

        [TestMethod]
        public void GreaterThanOrEqual_Equal_ReturnsTrue()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(90);
            Assert.IsTrue(a >= b);
        }

        [TestMethod]
        public void Clone_CreatesEqualAngle()
        {
            Angle original = new Angle(123.456);
            Angle clone = original.Clone();
            Assert.AreEqual(original.Degrees, clone.Degrees, Delta);
            Assert.AreEqual(original.Radians, clone.Radians, Delta);
        }

        [TestMethod]
        public void Equals_SameAngle_ReturnsTrue()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(90);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void Equals_DifferentAngle_ReturnsFalse()
        {
            Angle a = new Angle(90);
            Angle b = new Angle(91);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void Equals_WrappedEquivalent_ReturnsTrue()
        {
            Angle a = new Angle(360);
            Angle b = new Angle(0);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void ToString_ContainsDegrees()
        {
            Angle a = new Angle(90);
            string s = a.ToString();
            Assert.IsTrue(s.Contains("90"));
        }

        [TestMethod]
        public void GetTransformationMatrix_ZeroAngle_ReturnsIdentityRotation()
        {
            Angle a = Angle.Zero;
            Matrix m = a.GetTransformationMatrix();
            Assert.AreEqual(1.0, m.M11, Delta);
            Assert.AreEqual(0.0, m.M12, Delta);
            Assert.AreEqual(0.0, m.M21, Delta);
            Assert.AreEqual(1.0, m.M22, Delta);
        }

        [TestMethod]
        public void GetTransformationMatrix_WithTranslation_SetsMat41And42()
        {
            Angle a = Angle.Zero;
            Point translation = new Point(5, 3);
            Matrix m = a.GetTransformationMatrix(translation);
            Assert.AreEqual(5.0, m.M41, Delta);
            Assert.AreEqual(3.0, m.M42, Delta);
        }

        [TestMethod]
        public void GetTransformationMatrix_WithXYTranslation_SetsMat41And42()
        {
            Angle a = Angle.Zero;
            Matrix m = a.GetTransformationMatrix(7.0, 4.0);
            Assert.AreEqual(7.0, m.M41, Delta);
            Assert.AreEqual(4.0, m.M42, Delta);
        }
    }
}
