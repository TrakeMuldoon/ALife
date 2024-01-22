using ALife.Core.Geometry;

namespace ALife.Tests.Geometry.TestGeometryMath
{
    /// <summary>
    /// Tests for the ALife.Core.Geometry.GeometryMath.AngleBetweenPoints method.
    /// </summary>
    internal class TestAngleBetweenPoints
    {
        /// <summary>
        /// Tests the AngleBetweenPoints method.
        /// </summary>
        [Test]
        public void Test1()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.AngleBetweenPoints(source, target);
            Assert.That(result, Is.EqualTo(Angle.FromRadians(Math.PI / 4))); // 45 degrees
            Assert.That(result.InverseDegrees, Is.EqualTo(-315d)); // -315 degrees
        }

        /// <summary>
        /// Tests the AngleBetweenPoints method.
        /// </summary>
        [Test]
        public void Test2()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.AngleBetweenPoints(target, source);
            Assert.That(result, Is.EqualTo(Angle.FromRadians(Math.PI / 4 + Math.PI))); // 225 degrees
            Assert.That(result.InverseDegrees, Is.EqualTo(-135d)); // -135 degrees
        }
    }
}
