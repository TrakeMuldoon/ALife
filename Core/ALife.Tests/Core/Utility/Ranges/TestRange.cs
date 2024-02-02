using ALife.Core.Utility.Ranges;

namespace ALife.Tests.Core.Utility.Ranges
{
    /// <summary>
    /// Tests for the Range struct.
    /// </summary>
    [TestClass]
    public class TestRange
    {
        /// <summary>
        /// Tests the maximum.
        /// </summary>
        [TestMethod]
        public void TestChangingMaximum()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(10, range.Maximum);
            range.Maximum = 5;
            Assert.AreEqual(5, range.Maximum);
        }

        /// <summary>
        /// Tests the minimum.
        /// </summary>
        [TestMethod]
        public void TestChangingMinimum()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.Minimum);
            range.Minimum = 5;
            Assert.AreEqual(5, range.Minimum);
        }

        /// <summary>
        /// Tests the CircularClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestCircularClampValueA()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(5, range.CircularClampValue(-5));
        }

        /// <summary>
        /// Tests the CircularClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestCircularClampValueB()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.CircularClampValue(0));
        }

        /// <summary>
        /// Tests the CircularClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestCircularClampValueC()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(5, range.CircularClampValue(5));
        }

        /// <summary>
        /// Tests the CircularClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestCircularClampValueD()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.CircularClampValue(10));
        }

        /// <summary>
        /// Tests the CircularClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestCircularClampValueE()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(5, range.CircularClampValue(15));
        }

        /// <summary>
        /// Tests the CircularClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestCircularClampValueF()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.CircularClampValue(20));
        }

        /// <summary>
        /// Tests the ClampValue() method.
        /// </summary>
        [TestMethod]
        public void TestClampValue()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.ClampValue(-5));
            Assert.AreEqual(0, range.ClampValue(0));
            Assert.AreEqual(5, range.ClampValue(5));
            Assert.AreEqual(10, range.ClampValue(10));
            Assert.AreEqual(10, range.ClampValue(15));
        }

        /// <summary>
        /// Tests the difference calculation.
        /// </summary>
        [TestMethod]
        public void TestDifferenceCalculation()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(10, range.Maximum);
            range.Minimum = 5;
            Assert.AreEqual(5, range.Minimum);
            Assert.AreEqual(10, range.Maximum);
            range.Minimum = 17;
            Assert.AreEqual(10, range.Minimum);
            Assert.AreEqual(17, range.Maximum);
        }

        /// <summary>
        /// Tests the maximum and minimum flipping.
        /// </summary>
        [TestMethod]
        public void TestMaximumFlipping()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(10, range.Maximum);
            range.Maximum = -5;
            Assert.AreEqual(-5, range.Minimum);
            Assert.AreEqual(0, range.Maximum);
        }

        /// <summary>
        /// Tests the maximum and minimum flipping.
        /// </summary>
        [TestMethod]
        public void TestMinimumFlipping()
        {
            var range = new Range<int>(0, 10);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(10, range.Maximum);
            range.Minimum = 15;
            Assert.AreEqual(10, range.Minimum);
            Assert.AreEqual(15, range.Maximum);
        }
    }
}
