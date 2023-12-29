using ALife.Core.Utility;

namespace ALife.Tests.Utility.TestExtraMath
{
    /// <summary>
    /// Tests for ExtraMath.MinMaxDelta
    /// </summary>
    internal class TestMinMaxDelta
    {
        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 6d)]
        public double TestMinMaxDeltaDouble()
        {
            return ExtraMath.MinMaxDelta(1, 2, 3, -1, 5d);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 6)]
        public double TestMinMaxDeltaInt()
        {
            return ExtraMath.MinMaxDelta(1, 2, 3, -1, 5);
        }
    }
}
