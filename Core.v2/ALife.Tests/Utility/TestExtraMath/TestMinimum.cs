using ALife.Core.Utility;

namespace ALife.Tests.Utility.TestExtraMath
{
    /// <summary>
    /// Tests for ExtraMath.Minimum
    /// </summary>
    internal class TestMinimum
    {
        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = -1d)]
        public double TestMinimumDouble()
        {
            return ExtraMath.Minimum(1, 2, 3, -1, 5d);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = -1)]
        public double TestMinimumInt()
        {
            return ExtraMath.Minimum(1, 2, 3, -1, 5);
        }
    }
}
