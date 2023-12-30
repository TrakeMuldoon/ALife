using ALife.Core.Utility;

namespace ALife.Tests.Utility.TestExtraMath
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
        public double TestDoubleClampNormal()
        {
            return ExtraMath.CircularClamp(3d, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped around.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 1)]
        public double TestDoubleClampTooHigh()
        {
            return ExtraMath.CircularClamp(9d, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too low is clamped around.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public double TestDoubleClampTooLow()
        {
            return ExtraMath.CircularClamp(-1d, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped to the maximum.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public int TestIntClampNormal()
        {
            return ExtraMath.CircularClamp(3, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too high is clamped around.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 1)]
        public int TestIntClampTooHigh()
        {
            return ExtraMath.CircularClamp(9, 1, 5);
        }

        /// <summary>
        /// Tests that a value that is too low is clamped around.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public int TestIntClampTooLow()
        {
            return ExtraMath.CircularClamp(-1, 1, 5);
        }
    }
}
