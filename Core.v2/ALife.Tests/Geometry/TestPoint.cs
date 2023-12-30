using ALife.Core.Geometry;

namespace ALife.Tests.Geometry
{
    /// <summary>
    /// Tests for the Point class.
    /// </summary>
    internal class TestPoint
    {
        /// <summary>
        /// Tests this instance.
        /// </summary>
        [Test]
        public void Test()
        {
            var point = new Point(0, 0);
            Assert.That(point.X, Is.EqualTo(0));
            Assert.That(point.Y, Is.EqualTo(0));
            point.X = 1;
            point.Y = 2;
            Assert.That(point.X, Is.EqualTo(1));
            Assert.That(point.Y, Is.EqualTo(2));
            Assert.That(point.ToString(), Is.EqualTo("(1, 2)"));

            var point2 = new Point(point);
            Assert.That(point2, Is.EqualTo(point));
            point2.X = 3;
            Assert.That(point2, Is.Not.EqualTo(point));
        }

        /// <summary>
        /// Tests to vector2.
        /// </summary>
        [Test]
        public void TestToVector2()
        {
            var point = new Point(1, 2);
            var vector = point.ToVector2();
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(2));

            var point2 = vector.ToPoint();
            Assert.That(point2, Is.EqualTo(point));
        }
    }
}
