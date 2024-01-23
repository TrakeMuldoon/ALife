using ALife.Core.Utility.EvoNumbers;

namespace ALife.Tests.Utility.EvoNumbers
{
    /// <summary>
    /// Tests for the EvoNumber class.
    /// </summary>
    [TestClass]
    public class TestEvoNumber
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
            clone.Value = 1;
            Assert.AreEqual(0, number.Value);
            Assert.AreEqual(1, clone.Value);
        }

        /// <summary>
        /// Tests the cloning alternate.
        /// </summary>
        [TestMethod]
        public void TestCloningAlternate()
        {
            var number = GetTestNumber();

            Assert.AreEqual(0, number.Value);
            var clone = new EvoNumber(number);
            Assert.AreEqual(0, clone.Value);
            clone.Value = 1;
            Assert.AreEqual(0, number.Value);
            Assert.AreEqual(1, clone.Value);
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
            var randomizer = new Core.Utility.Random.FastRandom(1);
            var evolved1 = number.Evolve(randomizer);
            Assert.AreEqual(0, number.Value);

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
            number.Value = 2;
            Assert.AreEqual(1, number.Value);
        }

        /// <summary>
        /// Gets the test number.
        /// </summary>
        /// <returns></returns>
        private EvoNumber GetTestNumber()
        {
            return new EvoNumber(0, 1, 0, 1, -2, 2, 1, 1, 1, 2);
        }
    }
}
