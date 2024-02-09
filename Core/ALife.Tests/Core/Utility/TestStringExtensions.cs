using ALife.Core.Utility;

namespace ALife.Tests.Core.Utility
{
    /// <summary>
    /// Tests for the StringExtensions class.
    /// </summary>
    [TestClass]
    public class TestStringExtensions
    {
        /// <summary>
        /// Test for the string.ReplaceAny method.
        /// </summary>
        [TestMethod]
        public void TestReplaceAny1()
        {
            var to_replace = new string[] {"a", "b", "c" };

            var result = "abc".ReplaceAny("x", to_replace);
            Assert.AreEqual("xxx", result);
        }

        /// <summary>
        /// Test for the string.ReplaceAny method.
        /// </summary>
        [TestMethod]
        public void TestReplaceAny2()
        {
            var to_replace = new string[] {"a", "b", "c" };

            var result = "abcd".ReplaceAny("x", to_replace);
            Assert.AreEqual("xxxd", result);
        }

        /// <summary>
        /// Test for the string.ReplaceAny method.
        /// </summary>
        [TestMethod]
        public void TestReplaceAny3()
        {
            var to_replace = new string[] {"a", "b", "c" };

            var result = "".ReplaceAny("x", to_replace);
            Assert.AreEqual("", result);
        }

        /// <summary>
        /// Test for the string.ReplaceAny method.
        /// </summary>
        [TestMethod]
        public void TestReplaceAny4()
        {
            var result = "abc".ReplaceAny("x");
            Assert.AreEqual("abc", result);
        }

        /// <summary>
        /// Test for the string.ReplaceAny method.
        /// </summary>
        [TestMethod]
        public void TestReplaceAny5()
        {
            var result = "xaxXXXABCbCBAXXXxcx".ReplaceAny("x", "a", "b", "c");
            Assert.AreEqual("xxxXXXABCxCBAXXXxxx", result);
        }
    }
}
