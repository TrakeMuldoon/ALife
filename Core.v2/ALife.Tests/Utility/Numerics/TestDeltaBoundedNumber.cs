using ALife.Core;
using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the DeltaBoundedNumber class.
    /// </summary>
    internal class TestDeltaBoundedNumber
    {
        /// <summary>
        /// The test sim
        /// </summary>
        private Simulation _sim;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _sim = new Simulation(1);
        }

        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [Test]
        public void TestClamping()
        {
            var number = new DeltaBoundedNumber(_sim, 3, 1, 0, 5);
            Assert.That(number.Value, Is.EqualTo(3));
            number.Value = 9;
            Assert.That(number.Value, Is.EqualTo(4));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(5));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(5));

            number.Value = -1;
            number.Value = -1;
            number.Value = -1;
            number.Value = -1;
            number.Value = -1;
            Assert.That(number.Value, Is.EqualTo(0));
            number.Value = -1;
            Assert.That(number.Value, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [Test]
        public void TestClonedInitializer()
        {
            var parent = new DeltaBoundedNumber(_sim, 3, 1, 0, 5);
            var cloned = new DeltaBoundedNumber(parent);
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
            var parent = new DeltaBoundedNumber(_sim, 3, 1, 0, 5);
            var cloned = parent.Clone();
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Value = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }
    }
}
