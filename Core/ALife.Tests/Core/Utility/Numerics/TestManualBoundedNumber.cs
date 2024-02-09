using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Core.Utility.Numerics
{
    /// <summary>
    /// Tests for the BoundedManualNumber class.
    /// </summary>
    [TestClass]
    public class TestManualBoundedNumber
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [TestMethod]
        public void TestClamping()
        {
            var number = new ManualBoundedNumber(0, 5, 10);
            Assert.AreEqual(0, number.Value);
            var response = number.Clamp();
            Assert.AreEqual(5, response);
            Assert.AreEqual(5, number.Value);
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [TestMethod]
        public void TestClonedInitializer()
        {
            var parent = new ManualBoundedNumber(0, 5, 10);
            var cloned = new ManualBoundedNumber(parent);
            Assert.AreEqual(parent, cloned);

            parent.Value = 7;
            Assert.AreNotEqual(parent, cloned);
        }

        /// <summary>
        /// Tests the cloning.
        /// </summary>
        [TestMethod]
        public void TestCloning()
        {
            var parent = new ManualBoundedNumber(0, 5, 10);
            var cloned = parent.Clone();
            Assert.AreEqual(parent, cloned);

            parent.Value = 7;
            Assert.AreNotEqual(parent, cloned);
        }
    }
}
