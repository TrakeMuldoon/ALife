using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Core.Utility.Numerics
{
    /// <summary>
    /// Tests for the CircularBoundedNumber class.
    /// </summary>
    [TestClass]
    public class TestCircularBoundedNumber
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [TestMethod]
        public void TestClamping()
        {
            var number = new CircularBoundedNumber(0, 0, 360);

            Assert.AreEqual(0, number.Value);
            number.Value = -1;
            Assert.AreEqual(359, number.Value);

            number.Value -= 1;
            Assert.AreEqual(358, number.Value);

            number.Value = -359;
            Assert.AreEqual(1, number.Value);
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [TestMethod]
        public void TestClonedInitializer()
        {
            var parent = new CircularBoundedNumber(0, 5, 10);
            var cloned = new CircularBoundedNumber(parent);
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
            var parent = new CircularBoundedNumber(0, 5, 10);
            var cloned = parent.Clone();
            Assert.AreEqual(parent, cloned);

            parent.Value = 7;
            Assert.AreNotEqual(parent, cloned);
        }
    }
}
