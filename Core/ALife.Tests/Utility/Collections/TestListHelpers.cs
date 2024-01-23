using ALife.Core.Utility.Collections;

namespace ALife.Tests.Utility.Collections
{
    /// <summary>
    /// Tests for the ListHelpers class.
    /// </summary>
    [TestClass]
    public class TestListHelpers
    {
        /// <summary>
        /// Tests the ListHelpers.CompileList() method with a list provided
        /// </summary>
        [TestMethod]
        public void TestCompileListA()
        {
            var expectedList = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var actualList = ListHelpers.CompileList<int>(new IEnumerable<int>[] { new List<int>() { 1, 2, 3 } }, 4, 5, 6);

            expectedList.Sort();
            actualList.Sort();
            Assert.AreEqual(expectedList.Count, actualList.Count);
            for(var i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }

        /// <summary>
        /// Tests the ListHelpers.CompileList() method with a list not provided
        /// </summary>
        [TestMethod]
        public void TestCompileListB()
        {
            var expectedList = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var actualList = ListHelpers.CompileList<int>(1, 2, 3, 4, 5, 6);

            expectedList.Sort();
            actualList.Sort();
            Assert.AreEqual(expectedList.Count, actualList.Count);
            for(var i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }
    }
}
