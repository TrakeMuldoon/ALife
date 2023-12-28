using ALife.Core.Utility.Maths;

namespace ALife.Tests.Utility.Maths.TestExtraMath
{
    /// <summary>
    /// Tests for ExtraMath.TestDeltaClamp
    /// </summary>
    internal class TestDeltaClamp
    {
        /// <summary>
        /// Tests that a value stays the same.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 3)]
        public double TestDeltaClampAbsoluteMinTooHigh()
        {
            // 3 is > 1, so it should be clamped to 1. 1 + 1 = 2.
            return ExtraMath.DeltaClamp(3, 3, -1, 1, -3, 3);
        }

        /// <summary>
        /// Tests that a value stays the same.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = -3)]
        public double TestDeltaClampAbsoluteMinTooLow()
        {
            // -3 is < -1, so it should be clamped to -1. -1 + 1 = 0.
            return ExtraMath.DeltaClamp(-3, -3, -1, 1, -3, 3);
        }

        /// <summary>
        /// Tests that a value stays the same.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 2)]
        public double TestDeltaClampDeltaMinTooHigh()
        {
            // 3 is > 1, so it should be clamped to 1. 1 + 1 = 2.
            return ExtraMath.DeltaClamp(3, 1, -1, 1, -3, 3);
        }

        /// <summary>
        /// Tests that a value stays the same.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 0)]
        public double TestDeltaClampDeltaMinTooLow()
        {
            // -3 is < -1, so it should be clamped to -1. -1 + 1 = 0.
            return ExtraMath.DeltaClamp(-3, 1, -1, 1, -3, 3);
        }

        /// <summary>
        /// Tests that a value stays the same.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 2)]
        public double TestDeltaClampNormal()
        {
            return ExtraMath.DeltaClamp(2, 1, -1, 1, -3, 3);
        }

        /// <summary>
        /// Tests that a value stays the same.
        /// </summary>
        /// <returns>The actual value.</returns>
        [Test(ExpectedResult = 1)]
        public double TestDeltaClampNormalNoChange()
        {
            return ExtraMath.DeltaClamp(1, 1, -1, 1, -3, 3);
        }
    }
}
