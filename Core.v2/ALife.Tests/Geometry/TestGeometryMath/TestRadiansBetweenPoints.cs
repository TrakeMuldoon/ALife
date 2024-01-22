using ALife.Core.Geometry;

namespace ALife.Tests.Geometry.TestGeometryMath
{
    /// <summary>
    /// Tests for the ALife.Core.Geometry.GeometryMath.RadiansBetweenPoints method.
    /// </summary>
    internal class TestRadiansBetweenPoints
    {
        /// <summary>
        /// Tests the RadiansBetweenPoints method.
        /// </summary>
        [Test]
        public void Test1()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.RadiansBetweenPoints(source, target);
            Assert.That(result, Is.EqualTo(Math.PI / 4)); // 45 degrees
        }

        /// <summary>
        /// Tests the RadiansBetweenPoints method.
        /// </summary>
        [Test]
        public void Test2()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.RadiansBetweenPoints(target, source);
            Assert.That(result, Is.EqualTo(Math.PI / 4 + Math.PI)); // 225 degrees
        }
    }
}
