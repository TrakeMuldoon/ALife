using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the BoundedNumber class.
    /// </summary>
    [TestClass]
    public class TestBoundedNumber
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [TestMethod]
        public void TestClamping()
        {
            var number = new BoundedNumber(0, 5, 10);
            Assert.AreEqual(5, number.Value);
            number.Value = 9;
            Assert.AreEqual(9, number.Value);
            number.Value = 15;
            Assert.AreEqual(10, number.Value);
            number.Value = -2;
            Assert.AreEqual(5, number.Value);
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [TestMethod]
        public void TestClonedInitializer()
        {
            var parent = new BoundedNumber(0, 5, 10);
            var cloned = new BoundedNumber(parent);
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
            var parent = new BoundedNumber(0, 5, 10);
            var cloned = parent.Clone();
            Assert.AreEqual(parent, cloned);

            parent.Value = 7;
            Assert.AreNotEqual(parent, cloned);
        }
    }
}
