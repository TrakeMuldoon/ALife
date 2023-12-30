using ALife.Core.Geometry;
using System.Numerics;

namespace ALife.Tests.Geometry.TestGeometryMath
{
    /// <summary>
    /// Tests for the ALife.Core.Geometry.GeometryMath.TranslateByVector method.
    /// </summary>
    internal class TestTranslateByVector
    {
        [Test]
        public void TestWithAngleAndDistance()
        {
            var source = new Point(0, 0);
            var angle = Angle.FromRadians(GeometryMath.QuarterPi);
            var result = GeometryMath.TranslateByVector(source, angle, 1d);

            var expected = new Point(0.71, 0.71);
            var actualX = Math.Round(result.X, 2);
            var actualY = Math.Round(result.Y, 2);

            Assert.That(actualX, Is.EqualTo(expected.X));
            Assert.That(actualY, Is.EqualTo(expected.Y));
        }

        [Test]
        public void TestWithRadiansAndDistance()
        {
            var source = new Point(0, 0);
            var result = GeometryMath.TranslateByVector(source, GeometryMath.QuarterPi, 1d);

            var expected = new Point(0.71, 0.71);
            var actualX = Math.Round(result.X, 2);
            var actualY = Math.Round(result.Y, 2);

            Assert.That(actualX, Is.EqualTo(expected.X));
            Assert.That(actualY, Is.EqualTo(expected.Y));
        }

        [Test]
        public void TestWithVector2()
        {
            var source = new Point(0, 0);
            var vector = new Vector2(0.71f, 0.71f);
            var result = GeometryMath.TranslateByVector(source, vector);

            var expected = new Point(0.71, 0.71);
            var actualX = Math.Round(result.X, 2);
            var actualY = Math.Round(result.Y, 2);

            Assert.That(actualX, Is.EqualTo(expected.X));
            Assert.That(actualY, Is.EqualTo(expected.Y));
        }
    }
}
