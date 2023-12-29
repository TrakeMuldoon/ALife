using ALife.Core.Utility;

namespace ALife.Tests.Utility
{
    /// <summary>
    /// Tests for the ListExtensions class.
    /// </summary>
    internal class TestListExtensions
    {
        [Test]
        public void TestCompileList()
        {
            var expectedList = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var actualList = ListHelpers.CompileList<int>(new IEnumerable<int>[] { new List<int>() { 1, 2, 3 } }, 4, 5, 6);

            expectedList.Sort();
            actualList.Sort();
            Assert.That(actualList, Is.EqualTo(expectedList));
        }
    }
}
