using ALife.Core.Utility.Maths;

namespace ALife.Tests.Utility.Maths.TestExtraMath
{
    /// <summary>
    /// Tests for ExtraMath.CircularClamp
    /// </summary>
    internal class TestCircularClamp
    {
        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public double TestClampNormal()
        {
            return ExtraMath.CircularClamp(3, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped around.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 4)]
        public double TestClampTooHigh()
        {
            return ExtraMath.CircularClamp(9, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too low is clamped around.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public double TestClampTooLow()
        {
            return ExtraMath.CircularClamp(-1, 1, 5);
        }
    }
}
