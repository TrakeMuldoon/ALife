using ALife.Core.Utility.EvoNumbersV2;

namespace ALife.Tests.Utility.EvoNumbersV2
{
    /// <summary>
    /// Tests for the EvoNumberExtensions class.
    /// </summary>
    internal class TestEvoNumberExtensions
    {
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
            return new EvoNumber(0, 1, 0, 2, -2, 2, 1, 1, 1, 2);
        }

        /// <summary>
        /// Gets the test number.
        /// </summary>
        /// <returns></returns>
        private ReadOnlyEvoNumber GetTestReadOnlyEvoNumber()
        {
            return new ReadOnlyEvoNumber(0, 1, 0, 2, -2, 2, 1, 1, 1, 2);
        }
    }
}
