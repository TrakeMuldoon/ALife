using ALife.Core.Utility.Collections;

namespace ALife.Tests.Utility.Collections
{
    /// <summary>
    /// Tests for the ListExtensions class.
    /// </summary>
    [TestClass]
    public class TestListExtensions
    {
        /// <summary>
        /// Tests the List.AddItems() method.
        /// </summary>
        [TestMethod]
        public void TestCompileList()
        {
            var expectedList = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var actualList = new List<int>() { 1, 2, 3 };
            actualList.AddItems(4, 5, 6);

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
