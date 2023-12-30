using ALife.Core.Geometry;

namespace ALife.Tests.Geometry.TestGeometryMath
{
    /// <summary>
    /// Tests for the ALife.Core.Geometry.GeometryMath.GetTranslationVector method.
    /// </summary>
    internal class TestGetTranslationVector
    {
        [Test]
        public void TestWithAngleAndDistance()
        {
            var angle = Angle.FromRadians(GeometryMath.QuarterPi);
            var result = GeometryMath.GetTranslationVector(angle, 1d);

            var expectedX = Math.Round(0.71f, 2);
            var expectedY = Math.Round(0.71f, 2);
            var actualX = Math.Round(result.X, 2);
            var actualY = Math.Round(result.Y, 2);

            Assert.That(actualX, Is.EqualTo(expectedX));
            Assert.That(actualY, Is.EqualTo(expectedY));
        }

        [Test]
        public void TestWithRadiansAndDistance()
        {
            var result = GeometryMath.GetTranslationVector(GeometryMath.QuarterPi, 1d);

            var expectedX = Math.Round(0.71f, 2);
            var expectedY = Math.Round(0.71f, 2);
            var actualX = Math.Round(result.X, 2);
            var actualY = Math.Round(result.Y, 2);

            Assert.That(actualX, Is.EqualTo(expectedX));
            Assert.That(actualY, Is.EqualTo(expectedY));
        }
    }
}
