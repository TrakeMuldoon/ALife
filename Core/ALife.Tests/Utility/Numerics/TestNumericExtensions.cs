using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the NumericsExtensions class.
    /// </summary>
    [TestClass]
    public class TestNumericExtensions
    {
        /// <summary>
        /// Tests converting a BoundedNumber to a BoundedManualNumber.
        /// </summary>
        [TestMethod]
        public void TestBoundedToManualBoundedNumbers()
        {
            var number = new BoundedNumber(0, 5, 10);
            Assert.AreEqual(5, number.Value);
            number.Value = 9;
            Assert.AreEqual(9, number.Value);
            number.Value = 15;
            Assert.AreEqual(10, number.Value);
            number.Value = 5;
            Assert.AreEqual(5, number.Value);

            var manualNumber = number.ToManuallyBoundedNumber();
            Assert.AreEqual(5, number.Value);
            manualNumber.Value = 9;
            Assert.AreEqual(5, number.Value);
            Assert.AreEqual(9, manualNumber.Value);
            manualNumber.Value = 15;
            Assert.AreEqual(15, manualNumber.Value);
            manualNumber.Clamp();
            Assert.AreEqual(10, manualNumber.Value);
        }

        /// <summary>
        /// Tests converting a BoundedManualNumber to a BoundedNumber.
        /// </summary>
        [TestMethod]
        public void TestManualBoundedToBoundedNumbers()
        {
            var number = new ManualBoundedNumber(0, 5, 10);
            Assert.AreEqual(0, number.Value);
            number.Value = 9;
            Assert.AreEqual(9, number.Value);
            number.Value = 15;
            Assert.AreEqual(15, number.Value);
            number.Clamp();
            Assert.AreEqual(10, number.Value);
            number.Value = 5;
            Assert.AreEqual(5, number.Value);

            var autoNumber = number.ToAutoBoundedNumber();
            Assert.AreEqual(5, number.Value);
            autoNumber.Value = 9;
            Assert.AreEqual(5, number.Value);
            Assert.AreEqual(9, autoNumber.Value);
            autoNumber.Value = 15;
            Assert.AreEqual(10, autoNumber.Value);
        }
    }
}
