using ALife.Core.Utility.Random;

namespace ALife.Tests.Utility.Random
{
    /// <summary>
    /// Tests for the FastRandom class.
    /// TODO: Add more tests.
    /// </summary>
    [TestClass]
    public class TestFastRandom
    {
        /// <summary>
        /// Tests some basic functionality.
        /// </summary>
        [TestMethod]
        public void TestBasicFunctionality()
        {
            FastRandom random = new FastRandom(1);
            FastRandom random2 = new FastRandom(1);

            for(int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(random.Next(), random2.Next());
            }
        }
    }
}
