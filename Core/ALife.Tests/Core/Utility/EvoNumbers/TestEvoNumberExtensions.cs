using ALife.Core.Utility.EvoNumbers;

namespace ALife.Tests.Core.Utility.EvoNumbers
{
    /// <summary>
    /// Tests for the EvoNumberExtensions class.
    /// </summary>
    [TestClass]
    public class TestEvoNumberExtensions
    {
        /// <summary>
        /// Tests converting a EvoNumber to a ReadOnlyEvoNumber.
        /// </summary>
        [TestMethod]
        public void TestEvoNumberToReadOnlyEvoNumber()
        {
            var number = GetTestEvoNumber();

            Assert.AreEqual(0, number.Value);
            number.Value = 2;
            Assert.AreEqual(1, number.Value);

            var readOnlyNumber = number.ToReadOnlyEvoNumber();
            Assert.AreEqual(1, readOnlyNumber.Value);
            Assert.ThrowsException<InvalidOperationException>(() => readOnlyNumber.Value = 2);
            number.Value = 2;
            Assert.AreEqual(2, number.Value);
        }

        /// <summary>
        /// Tests converting a ReadOnlyEvoNumber to a EvoNumber.
        /// </summary>
        [TestMethod]
        public void TestReadOnlyEvoNumberToEvoNumber()
        {
            var number = GetTestReadOnlyEvoNumber();

            Assert.AreEqual(0, number.Value);
            Assert.ThrowsException<InvalidOperationException>(() => number.Value = 2);

            var rwNumber = number.ToEvoNumber();
            Assert.AreEqual(0, rwNumber.Value);
            rwNumber.Value = 2;
            Assert.AreEqual(1, rwNumber.Value);
            Assert.ThrowsException<InvalidOperationException>(() => number.Value = 2);
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
