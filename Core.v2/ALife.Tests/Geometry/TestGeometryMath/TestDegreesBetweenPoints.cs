using ALife.Core.Geometry;

namespace ALife.Tests.Geometry.TestGeometryMath
{
    /// <summary>
    /// Tests for the ALife.Core.Geometry.GeometryMath.DegreesBetweenPoints method.
    /// </summary>
    internal class TestDegreesBetweenPoints
    {
        /// <summary>
        /// Tests the DegreesBetweenPoints method.
        /// </summary>
        [Test]
        public void Test1()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.DegreesBetweenPoints(source, target);
            Assert.That(result, Is.EqualTo(45)); // 45 degrees
        }

        /// <summary>
        /// Tests the DegreesBetweenPoints method.
        /// </summary>
        [Test]
        public void Test2()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.DegreesBetweenPoints(target, source);
            Assert.That(result, Is.EqualTo(225)); // 225 degrees
        }
    }
}
