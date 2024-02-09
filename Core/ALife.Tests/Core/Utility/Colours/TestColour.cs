using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Random;

namespace ALife.Tests.Core.Utility.Colours
{
    /// <summary>
    /// Tests for the Colour struct.
    /// </summary>
    [TestClass]
    public class TestColour
    {
        /// <summary>
        /// Tests basic functionality.
        /// </summary>
        [TestMethod]
        public void TestBasicColour()
        {
            Colour colourA = new Colour(0, 0, 0);
            Assert.AreEqual(255, colourA.A);
            Assert.AreEqual(0, colourA.R);
            Assert.AreEqual(0, colourA.G);
            Assert.AreEqual(0, colourA.B);

            Colour colourB = new Colour(0, 0, 0);
            Assert.AreEqual(colourA, colourB);

            IColour colourC = colourA.Clone();
            Assert.AreEqual(colourA, colourC);
            Assert.AreEqual(colourB, colourC);
        }

        /// <summary>
        /// Tests basic functionality for a predefined colour.
        /// </summary>
        [TestMethod]
        public void TestPredefinedColour()
        {
            Colour colourA = Colour.Red;
            Assert.AreEqual(255, colourA.A);
            Assert.AreEqual(255, colourA.R);
            Assert.AreEqual(0, colourA.G);
            Assert.AreEqual(0, colourA.B);

            Colour colourB = new Colour(255, 0, 0);
            Assert.IsTrue(colourA.WasPredefined);
            Assert.IsFalse(colourB.WasPredefined);
            Assert.AreEqual(colourA, colourB);

            IColour colourC = colourA.Clone();
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
            IRandom randomizerA = new FastRandom(1);
            IRandom randomizerB = new FastRandom(1);

            Colour colour = Colour.GetRandomColour(randomizerA);
            Assert.AreEqual(randomizerB.NextByte(255, 255), colour.A);
            Assert.AreEqual(randomizerB.NextByte(100, 255), colour.R);
            Assert.AreEqual(randomizerB.NextByte(100, 255), colour.G);
            Assert.AreEqual(randomizerB.NextByte(100, 255), colour.B);
        }
    }
}
