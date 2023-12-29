using ALife.Core;
using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the ReadOnlyEvoNumber class.
    /// </summary>
    internal class TestReadOnlyEvoNumber
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
        /// Tests this instance.
        /// </summary>
        [Test]
        public void Test()
        {
            var number = GetTestNumber();

            Assert.That(number.Value, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests the cloning.
        /// </summary>
        [Test]
        public void TestCloning()
        {
            var number = GetTestNumber();

            Assert.That(number.Value, Is.EqualTo(0));
            var clone = number.Clone();
            Assert.That(clone.Value, Is.EqualTo(0));
            clone.Value = 1;
            Assert.That(number.Value, Is.EqualTo(0));
            Assert.That(clone.Value, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests the cloning alternate.
        /// </summary>
        [Test]
        public void TestCloningAlternate()
        {
            var number = GetTestNumber();

            Assert.That(number.Value, Is.EqualTo(0));
            var clone = new EvoNumber(number);
            Assert.That(clone.Value, Is.EqualTo(0));
            clone.Value = 1;
            Assert.That(number.Value, Is.EqualTo(0));
            Assert.That(clone.Value, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests the evolve.
        /// </summary>
        [Test]
        public void TestEvolve()
        {
            var number = GetTestNumber();

            Assert.That(number.Value, Is.EqualTo(0));

            // NOTE: This test will be impacted heavily by any changes we make to the random number generator
            var evolved1 = number.Evolve(_sim);
            Assert.That(evolved1.Value, Is.EqualTo(0));

            var evolved2 = number.Evolve(_sim.Random);
            var roundedValue = Math.Round(evolved2.Value, 2);
            var expectedRoundedValue = 0.15d;
            Assert.That(roundedValue, Is.EqualTo(expectedRoundedValue));
        }

        /// <summary>
        /// Tests the value updating.
        /// </summary>
        [Test]
        public void TestValueUpdating()
        {
            var number = GetTestNumber();

            Assert.That(number.Value, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => number.Value = 2);
        }

        /// <summary>
        /// Gets the test number.
        /// </summary>
        /// <returns></returns>
        private ReadOnlyEvoNumber GetTestNumber()
        {
            return new ReadOnlyEvoNumber(0, 1, 0, 1, -2, 2, 1, 1, 1, 2);
        }
    }
}
