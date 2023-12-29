using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the BoundedNumber class.
    /// </summary>
    internal class TestBoundedNumber
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [Test]
        public void TestClamping()
        {
            var number = new BoundedNumber(0, 5, 10);
            Assert.That(number.Value, Is.EqualTo(5));
            number.Value = 9;
            Assert.That(number.Value, Is.EqualTo(9));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [Test]
        public void TestClonedInitializer()
        {
            var parent = new BoundedNumber(0, 5, 10);
            var cloned = new BoundedNumber(parent);
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Value = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }

        /// <summary>
        /// Tests the cloning.
        /// </summary>
        [Test]
        public void TestCloning()
        {
            var parent = new BoundedNumber(0, 5, 10);
            var cloned = parent.Clone();
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Value = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }
    }
}
