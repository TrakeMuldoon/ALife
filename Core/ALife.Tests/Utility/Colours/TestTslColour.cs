using ALife.Core.Utility.Colours;

namespace ALife.Tests.Utility.Colours
{
    /// <summary>
    /// Tests for the TslColour struct.
    /// </summary>
    [TestClass]
    public class TestTslColour
    {
        /// <summary>
        /// Tests basic functionality.
        /// </summary>
        [TestMethod]
        public void TestBasicColour()
        {
            var colourA = new TslColour(0, 0, 0);
            Assert.AreEqual(255, colourA.A);
            Assert.AreEqual(0, colourA.R);
            Assert.AreEqual(0, colourA.G);
            Assert.AreEqual(0, colourA.B);

            var colourB = new TslColour(0, 0, 0);
            Assert.AreEqual(colourA, colourB);

            var colourC = colourA.Clone();
            Assert.AreEqual(colourA, colourC);
            Assert.AreEqual(colourB, colourC);
        }

        /// <summary>
        /// Tests basic functionality for a predefined colour.
        /// </summary>
        [TestMethod]
        public void TestPredefinedColour()
        {
            var colourA = TslColour.Red;
            Assert.AreEqual(255, colourA.A);
            Assert.AreEqual(255, colourA.R);
            Assert.AreEqual(0, colourA.G);
            Assert.AreEqual(0, colourA.B);

            var colourB = TslColour.FromRGB(255, 0, 0);
            Assert.IsTrue(colourA.WasPredefined);
            Assert.IsFalse(colourB.WasPredefined);
            Assert.AreEqual(colourA, colourB);

            var colourC = colourA.Clone();
            Assert.IsTrue(colourA.WasPredefined);
            Assert.IsFalse(colourB.WasPredefined);
            Assert.IsTrue(colourC.WasPredefined);
            Assert.AreEqual(colourA, colourC);
            Assert.AreEqual(colourB, colourC);
        }

        /// <summary>
        /// Tests basic functionality.
        /// </summary>
        [TestMethod]
        public void TestRandomizedColour()
        {
            var randomizerA = new ALife.Core.Utility.Random.FastRandom(1);
            var randomizerB = new ALife.Core.Utility.Random.FastRandom(1);

            var colour = TslColour.GetRandomColour(randomizerA, alphaMin: 255, alphaMax: 255);
            Assert.AreEqual(255, colour.A);
            Assert.AreEqual(147, colour.R);
            Assert.AreEqual(255, colour.G);
            Assert.AreEqual(15, colour.B);
        }
    }
}
