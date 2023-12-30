using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the CircularBoundedNumber class.
    /// </summary>
    internal class TestCircularBoundedNumber
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [Test]
        public void TestClamping()
        {
            var number = new CircularBoundedNumber(0, 0, 360);

            Assert.That(number.Value, Is.EqualTo(0));
            number.Value = -1;
            Assert.That(number.Value, Is.EqualTo(359));
            number.Value = -361;
            Assert.That(number.Value, Is.EqualTo(358));
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [Test]
        public void TestClonedInitializer()
        {
            var parent = new CircularBoundedNumber(0, 5, 10);
            var cloned = new CircularBoundedNumber(parent);
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
            var parent = new CircularBoundedNumber(0, 5, 10);
            var cloned = parent.Clone();
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Value = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }
    }
}
