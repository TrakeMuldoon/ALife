using ALife.Core;
using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the NumericsExtensions class.
    /// </summary>
    internal class TestNumericExtensions
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
        /// Tests converting a BoundedNumber to a BoundedManualNumber.
        /// </summary>
        [Test]
        public void TestBoundedToManualBoundedNumbers()
        {
            var number = new BoundedNumber(_sim, 0, 5, 10);
            Assert.That(number.Value, Is.EqualTo(5));
            number.Value = 9;
            Assert.That(number.Value, Is.EqualTo(9));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(10));
            number.Value = 5;
            Assert.That(number.Value, Is.EqualTo(5));

            var manualNumber = number.ToManuallyBoundedNumber();
            Assert.That(manualNumber.Value, Is.EqualTo(5));
            manualNumber.Value = 9;
            Assert.That(number.Value, Is.EqualTo(5));
            Assert.That(manualNumber.Value, Is.EqualTo(9));
            manualNumber.Value = 15;
            Assert.That(manualNumber.Value, Is.EqualTo(15));
            manualNumber.Clamp();
            Assert.That(manualNumber.Value, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests converting a EvoNumber to a ReadOnlyEvoNumber.
        /// </summary>
        [Test]
        public void TestEvoNumberToReadOnlyEvoNumber()
        {
            var number = GetTestEvoNumber();

            Assert.That(number.Value, Is.EqualTo(0));
            number.Value = 2;
            Assert.That(number.Value, Is.EqualTo(1));

            var readOnlyNumber = number.ToReadOnlyEvoNumber();
            Assert.That(readOnlyNumber.Value, Is.EqualTo(1));
            Assert.Throws<InvalidOperationException>(() => readOnlyNumber.Value = 2);
            number.Value = 2;
            Assert.That(number.Value, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests converting a BoundedManualNumber to a BoundedNumber.
        /// </summary>
        [Test]
        public void TestManualBoundedToBoundedNumbers()
        {
            var number = new BoundedManualNumber(_sim, 0, 5, 10);
            Assert.That(number.Value, Is.EqualTo(0));
            number.Value = 9;
            Assert.That(number.Value, Is.EqualTo(9));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(15));
            number.Clamp();
            Assert.That(number.Value, Is.EqualTo(10));
            number.Value = 5;
            Assert.That(number.Value, Is.EqualTo(5));

            var autoNumber = number.ToAutoBoundedNumber();
            Assert.That(autoNumber.Value, Is.EqualTo(5));
            autoNumber.Value = 9;
            Assert.That(number.Value, Is.EqualTo(5));
            Assert.That(autoNumber.Value, Is.EqualTo(9));
            autoNumber.Value = 15;
            Assert.That(autoNumber.Value, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests converting a ReadOnlyEvoNumber to a EvoNumber.
        /// </summary>
        [Test]
        public void TestReadOnlyEvoNumberToEvoNumber()
        {
            var number = GetTestReadOnlyEvoNumber();

            Assert.That(number.Value, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => number.Value = 2);

            var rwNumber = number.ToEvoNumber();
            Assert.That(rwNumber.Value, Is.EqualTo(0));
            rwNumber.Value = 2;
            Assert.That(rwNumber.Value, Is.EqualTo(1));
            Assert.Throws<InvalidOperationException>(() => number.Value = 2);
        }

        /// <summary>
        /// Gets the test number.
        /// </summary>
        /// <returns></returns>
        private EvoNumber GetTestEvoNumber()
        {
            return new EvoNumber(_sim, 0, 1, 0, 2, -2, 2, 1, 1, 1, 2);
        }

        /// <summary>
        /// Gets the test number.
        /// </summary>
        /// <returns></returns>
        private ReadOnlyEvoNumber GetTestReadOnlyEvoNumber()
        {
            return new ReadOnlyEvoNumber(_sim, 0, 1, 0, 2, -2, 2, 1, 1, 1, 2);
        }
    }
}
