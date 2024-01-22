using ALife.Core.Geometry;

namespace ALife.Tests.Geometry.TestGeometryMath
{
    /// <summary>
    /// Tests for the ALife.Core.Geometry.GeometryMath.Distance method.
    /// </summary>
    internal class TestDistance
    {
        /// <summary>
        /// Tests the Distance method.
        /// </summary>
        [Test]
        public void Test1()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.Distance(source, target);
            Assert.That(result, Is.EqualTo(Math.Sqrt(2)));
        }

        /// <summary>
        /// Tests the Distance method.
        /// </summary>
        [Test]
        public void Test2()
        {
            var source = new Point(0, 0);
            var target = new Point(1, 1);
            var result = GeometryMath.Distance(target, source);
            Assert.That(result, Is.EqualTo(Math.Sqrt(2)));
        }
    }
}
