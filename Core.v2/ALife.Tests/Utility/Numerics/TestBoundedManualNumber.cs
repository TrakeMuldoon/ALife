using ALife.Core;
using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the BoundedManualNumber class.
    /// </summary>
    internal class TestBoundedManualNumber
    {
        /// <summary>
        /// The sim
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
            var number = new BoundedManualNumber(_sim, 0, 5, 10);
            Assert.That(number.Value, Is.EqualTo(0));
            var response = number.Clamp();
            Assert.That(response, Is.EqualTo(5));
            Assert.That(number.Value, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [Test]
        public void TestClonedInitializer()
        {
            var parent = new BoundedManualNumber(_sim, 0, 5, 10);
            var cloned = new BoundedManualNumber(parent);
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
            var parent = new BoundedManualNumber(_sim, 0, 5, 10);
            var cloned = parent.Clone();
            Assert.That(cloned, Is.EqualTo(parent));

            parent.Value = 7;
            Assert.That(cloned, Is.Not.EqualTo(parent));
        }
    }
}
