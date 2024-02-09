using ALife.Core.Utility.Colours;

namespace ALife.Tests.Core.Utility.Colours
{
    /// <summary>
    /// Tests for the HslColour struct.
    /// </summary>
    [TestClass]
    public class TestHslColour
    {
        /// <summary>
        /// Tests basic functionality.
        /// </summary>
        [TestMethod]
        public void TestBasicColour()
        {
            var colourA = new HslColour(0, 0, 0);
            Assert.AreEqual(255, colourA.A);
            Assert.AreEqual(255, colourA.R);
            Assert.AreEqual(255, colourA.G);
            Assert.AreEqual(255, colourA.B);

            var colourB = new HslColour(0, 0, 0);
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
            var colourA = HslColour.Red;
            Assert.AreEqual(255, colourA.A);
            Assert.AreEqual(255, colourA.R);
            Assert.AreEqual(0, colourA.G);
            Assert.AreEqual(0, colourA.B);

            var colourB = new HslColour(0, 1, 0.5);
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

            var colour = HslColour.GetRandomColour(randomizerA);
            Assert.AreEqual(randomizerB.NextByte(255, 255), colour.A);
            Assert.AreEqual(randomizerB.Next(0, 360), colour.Hue);
            Assert.AreEqual(randomizerB.NextDouble(0, 1), colour.Saturation);
            Assert.AreEqual(randomizerB.NextDouble(0, 1), colour.Lightness);
        }
    }
}
