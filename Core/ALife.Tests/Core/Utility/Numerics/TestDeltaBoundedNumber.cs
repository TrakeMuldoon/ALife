using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Core.Utility.Numerics
{
    /// <summary>
    /// Tests for the DeltaBoundedNumber class.
    /// </summary>
    [TestClass]
    public class TestDeltaBoundedNumber
    {
        /// <summary>
        /// Tests the clamping.
        /// </summary>
        [TestMethod]
        public void TestClamping()
        {
            var number = new DeltaBoundedNumber(3, 1, 0, 5);
            Assert.AreEqual(3, number.Value);
            number.Value = 9;
            Assert.AreEqual(4, number.Value);
            number.Value = 15;
            Assert.AreEqual(5, number.Value);
            number.Value = 15;
            Assert.AreEqual(5, number.Value);

            number.Value = -1;
            number.Value = -1;
            number.Value = -1;
            number.Value = -1;
            number.Value = -1;
            Assert.AreEqual(0, number.Value);
            number.Value = -1;
            Assert.AreEqual(0, number.Value);
        }

        /// <summary>
        /// Tests the cloned initializer.
        /// </summary>
        [TestMethod]
        public void TestClonedInitializer()
        {
            var parent = new DeltaBoundedNumber(3, 1, 0, 5);
            var cloned = new DeltaBoundedNumber(parent);
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
            var parent = new DeltaBoundedNumber(3, 1, 0, 5);
            var cloned = parent.Clone();
            Assert.AreEqual(parent, cloned);

            parent.Value = 7;
            Assert.AreNotEqual(parent, cloned);
        }
    }
}
