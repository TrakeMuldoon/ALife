using ALife.Core.Utility;

namespace ALife.Tests.Utility.TestExtraMath
{
    /// <summary>
    /// Tests for ExtraMath.Clamp
    /// </summary>
    internal class TestClamp
    {
        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public double TestClampNormal()
        {
            return ExtraMath.Clamp(3, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 5)]
        public double TestClampTooHigh()
        {
            return ExtraMath.Clamp(10, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too low is clamped to the minimum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 1)]
        public double TestClampTooLow()
        {
            return ExtraMath.Clamp(-1, 1, 5);
        }
    }
}
