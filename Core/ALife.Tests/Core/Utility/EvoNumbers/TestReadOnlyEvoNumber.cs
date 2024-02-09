using ALife.Core.Utility.EvoNumbers;
using ALife.Core.Utility.Random;

namespace ALife.Tests.Core.Utility.EvoNumbers
{
    /// <summary>
    /// Tests for the ReadOnlyEvoNumber class.
    /// </summary>
    [TestClass]
    public class TestReadOnlyEvoNumber
    {
        /// <summary>
        /// Tests this instance.
        /// </summary>
        [TestMethod]
        public void Test()
        {
            var number = GetTestNumber();
            Assert.AreEqual(0, number.Value);
        }

        /// <summary>
        /// Tests the cloning.
        /// </summary>
        [TestMethod]
        public void TestCloning()
        {
            var number = GetTestNumber();

            Assert.AreEqual(0, number.Value);
            var clone = number.Clone();
            Assert.AreEqual(0, clone.Value);
        }

        /// <summary>
        /// Tests the cloning alternate.
        /// </summary>
        [TestMethod]
        public void TestCloningAlternate()
        {
            var number = GetTestNumber();

            Assert.AreEqual(0, number.Value);
            var clone = new ReadOnlyEvoNumber(number);
            Assert.AreEqual(0, clone.Value);
            Assert.AreEqual(number, clone);
        }

        /// <summary>
        /// Tests the evolve.
        /// </summary>
        [TestMethod]
        public void TestEvolve()
        {
            var number = GetTestNumber();

            Assert.AreEqual(0, number.Value);

            // NOTE: This test will be impacted heavily by any changes we make to the random number generator
            var randomizer = new FastRandom(1);
            var evolved1 = number.Evolve(randomizer);
            Assert.AreEqual(0, evolved1.Value);

            var evolved2 = number.Evolve(randomizer);
            var roundedValue = Math.Round(evolved2.Value, 2);
            var expectedRoundedValue = 0.15d;
            Assert.AreEqual(expectedRoundedValue, roundedValue);
        }

        /// <summary>
        /// Tests the value updating.
        /// </summary>
        [TestMethod]
        public void TestValueUpdating()
        {
            var number = GetTestNumber();

            Assert.AreEqual(0, number.Value);
            Assert.ThrowsException<InvalidOperationException>(() => number.Value = 2);
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
