using ALife.Core.Utility.Maths;

namespace ALife.Tests.Utility.Maths.TestExtraMath
{
    /// <summary>
    /// Tests for ExtraMath.Maximum
    /// </summary>
    internal class TestMaximum
    {
        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 5d)]
        public double TestMaximumDouble()
        {
            return ExtraMath.Maximum(1, 2, 3, -1, 5d);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 5)]
        public double TestMaximumInt()
        {
            return ExtraMath.Maximum(1, 2, 3, -1, 5);
        }
    }
}
