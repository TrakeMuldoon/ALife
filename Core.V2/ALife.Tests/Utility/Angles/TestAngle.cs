using ALife.Core.Utility.Angles;

namespace ALife.Tests.Utility.Angles
{
    /// <summary>
    /// Tests for the Angle class.
    /// </summary>
    internal class TestAngle
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [Test]
        public void TestClamping()
        {
            var number = new Angle(0);

            Assert.That(number.Degrees, Is.EqualTo(0));
            number.Degrees = -1;
            Assert.That(number.Degrees, Is.EqualTo(359));
            number.Degrees = -361;
            Assert.That(number.Degrees, Is.EqualTo(359));
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [Test]
        public void TestClonedInitializer()
        {
            var parent = new Angle(0);
            var cloned = new Angle(parent);
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Degrees = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }

        /// <summary>
        /// Tests the cloning.
        /// </summary>
        [Test]
        public void TestCloning()
        {
            var parent = new Angle(0);
            var cloned = parent.Clone();
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Degrees = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }

        /// <summary>
        /// Tests radian conversion.
        /// </summary>
        [Test]
        public void TestRadianConversion()
        {
            var parent = new Angle(180);
            var roundedRadians = Math.Round(parent.Radians, 2);
            Assert.That(roundedRadians, Is.EqualTo(Math.Round(Math.PI, 2)));
        }
    }
}
