using ALife.Core.Utility.Maths;

namespace ALife.Tests.Core.Utility.Maths
{
    /// <summary>
    /// Additional edge case tests for ExtraMath not covered by the generated test suite.
    /// </summary>
    [TestClass]
    public class TestExtraMathEdgeCases
    {
        private const double Delta = 1e-10;

        // CircularClamp edge cases

        [TestMethod]
        public void CircularClamp_SwappedMinMax_StillWrapsCorrectly()
        {
            // min > max — the implementation should swap them
            double result = ExtraMath.CircularClamp(5.0, 10.0, 0.0);
            Assert.AreEqual(5.0, result, Delta);
        }

        [TestMethod]
        public void CircularClamp_ValueExactlyAtMax_WrapsToMin()
        {
            double result = ExtraMath.CircularClamp(10.0, 0.0, 10.0);
            Assert.AreEqual(0.0, result, Delta);
        }

        [TestMethod]
        public void CircularClamp_ValueExactlyAtMin_ReturnsMin()
        {
            double result = ExtraMath.CircularClamp(0.0, 0.0, 10.0);
            Assert.AreEqual(0.0, result, Delta);
        }

        [TestMethod]
        public void CircularClamp_ValueFarAboveMax_WrapsCorrectly()
        {
            double result = ExtraMath.CircularClamp(25.0, 0.0, 10.0);
            Assert.AreEqual(5.0, result, Delta);
        }

        [TestMethod]
        public void CircularClamp_ValueFarBelowMin_WrapsCorrectly()
        {
            double result = ExtraMath.CircularClamp(-5.0, 0.0, 10.0);
            Assert.AreEqual(5.0, result, Delta);
        }

        [TestMethod]
        public void CircularClamp_NegativeRange_WrapsCorrectly()
        {
            // Range [-10, 0): value 5 → wraps to -5
            double result = ExtraMath.CircularClamp(5.0, -10.0, 0.0);
            Assert.AreEqual(-5.0, result, Delta);
        }

        [TestMethod]
        public void CircularClamp_NegativeRangeExactlyAtMax_WrapsToMin()
        {
            double result = ExtraMath.CircularClamp(0.0, -10.0, 0.0);
            Assert.AreEqual(-10.0, result, Delta);
        }

        // Clamp edge cases

        [TestMethod]
        public void Clamp_ValueBelowMin_ReturnsMin()
        {
            Assert.AreEqual(1.0, ExtraMath.Clamp(-5.0, 1.0, 10.0), Delta);
        }

        [TestMethod]
        public void Clamp_ValueAboveMax_ReturnsMax()
        {
            Assert.AreEqual(10.0, ExtraMath.Clamp(50.0, 1.0, 10.0), Delta);
        }

        [TestMethod]
        public void Clamp_ValueInRange_ReturnsValue()
        {
            Assert.AreEqual(5.0, ExtraMath.Clamp(5.0, 1.0, 10.0), Delta);
        }

        [TestMethod]
        public void Clamp_ValueAtMin_ReturnsMin()
        {
            Assert.AreEqual(1.0, ExtraMath.Clamp(1.0, 1.0, 10.0), Delta);
        }

        [TestMethod]
        public void Clamp_ValueAtMax_ReturnsMax()
        {
            Assert.AreEqual(10.0, ExtraMath.Clamp(10.0, 1.0, 10.0), Delta);
        }

        // DeltaClamp edge cases

        [TestMethod]
        public void DeltaClamp_DeltaExceedsDeltaMax_ClampedToMax()
        {
            // currentValue=5, newValue=20, deltaMax=3 → clamped delta is 3 → 8
            double result = ExtraMath.DeltaClamp(20.0, 5.0, -5.0, 3.0, 0.0, 100.0);
            Assert.AreEqual(8.0, result, Delta);
        }

        [TestMethod]
        public void DeltaClamp_DeltaBelowDeltaMin_ClampedToMin()
        {
            // currentValue=5, newValue=0, deltaMin=-2 → clamped delta is -2 → 3
            double result = ExtraMath.DeltaClamp(0.0, 5.0, -2.0, 2.0, 0.0, 100.0);
            Assert.AreEqual(3.0, result, Delta);
        }

        [TestMethod]
        public void DeltaClamp_ResultExceedsAbsoluteMax_ClampedToAbsoluteMax()
        {
            // currentValue=98, newValue=105, deltaMax=5 → delta clamped to 5, 98+5=103, then absolute max 100
            double result = ExtraMath.DeltaClamp(105.0, 98.0, -5.0, 5.0, 0.0, 100.0);
            Assert.AreEqual(100.0, result, Delta);
        }

        [TestMethod]
        public void DeltaClamp_ResultBelowAbsoluteMin_ClampedToAbsoluteMin()
        {
            // currentValue=2, newValue=-10, deltaMin=-5 → delta=-5, 2-5=-3, then absolute min 0
            double result = ExtraMath.DeltaClamp(-10.0, 2.0, -5.0, 5.0, 0.0, 100.0);
            Assert.AreEqual(0.0, result, Delta);
        }

        // Maximum/Minimum edge cases

        [TestMethod]
        public void Maximum_NegativeValues_ReturnsLargest()
        {
            Assert.AreEqual(-1.0, ExtraMath.Maximum(-5.0, -3.0, -1.0), Delta);
        }

        [TestMethod]
        public void Minimum_NegativeValues_ReturnsSmallest()
        {
            Assert.AreEqual(-5.0, ExtraMath.Minimum(-5.0, -3.0, -1.0), Delta);
        }

        [TestMethod]
        public void Maximum_MixedSigns_ReturnsLargest()
        {
            Assert.AreEqual(3.0, ExtraMath.Maximum(-5.0, 0.0, 3.0), Delta);
        }

        [TestMethod]
        public void Minimum_MixedSigns_ReturnsSmallest()
        {
            Assert.AreEqual(-5.0, ExtraMath.Minimum(-5.0, 0.0, 3.0), Delta);
        }

        [TestMethod]
        public void MinMaxDelta_AllSameValues_ReturnsZero()
        {
            Assert.AreEqual(0.0, ExtraMath.MinMaxDelta(5.0, 5.0, 5.0), Delta);
        }

        [TestMethod]
        public void MinMaxDelta_NegativeValues_ReturnsCorrectDelta()
        {
            Assert.AreEqual(4.0, ExtraMath.MinMaxDelta(-5.0, -3.0, -1.0), Delta);
        }
    }
}
