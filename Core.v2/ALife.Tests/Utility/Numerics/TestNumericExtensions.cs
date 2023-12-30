using ALife.Core.Utility.Numerics;

namespace ALife.Tests.Utility.Numerics
{
    /// <summary>
    /// Tests for the NumericsExtensions class.
    /// </summary>
    internal class TestNumericExtensions
    {
        /// <summary>
        /// Tests converting a BoundedNumber to a BoundedManualNumber.
        /// </summary>
        [Test]
        public void TestBoundedToManualBoundedNumbers()
        {
            var number = new BoundedNumber(0, 5, 10);
            Assert.That(number.Value, Is.EqualTo(5));
            number.Value = 9;
            Assert.That(number.Value, Is.EqualTo(9));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(10));
            number.Value = 5;
            Assert.That(number.Value, Is.EqualTo(5));

            var manualNumber = number.ToManuallyBoundedNumber();
            Assert.That(manualNumber.Value, Is.EqualTo(5));
            manualNumber.Value = 9;
            Assert.That(number.Value, Is.EqualTo(5));
            Assert.That(manualNumber.Value, Is.EqualTo(9));
            manualNumber.Value = 15;
            Assert.That(manualNumber.Value, Is.EqualTo(15));
            manualNumber.Clamp();
            Assert.That(manualNumber.Value, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests converting a BoundedManualNumber to a BoundedNumber.
        /// </summary>
        [Test]
        public void TestManualBoundedToBoundedNumbers()
        {
            var number = new BoundedManualNumber(0, 5, 10);
            Assert.That(number.Value, Is.EqualTo(0));
            number.Value = 9;
            Assert.That(number.Value, Is.EqualTo(9));
            number.Value = 15;
            Assert.That(number.Value, Is.EqualTo(15));
            number.Clamp();
            Assert.That(number.Value, Is.EqualTo(10));
            number.Value = 5;
            Assert.That(number.Value, Is.EqualTo(5));

            var autoNumber = number.ToAutoBoundedNumber();
            Assert.That(autoNumber.Value, Is.EqualTo(5));
            autoNumber.Value = 9;
            Assert.That(number.Value, Is.EqualTo(5));
            Assert.That(autoNumber.Value, Is.EqualTo(9));
            autoNumber.Value = 15;
            Assert.That(autoNumber.Value, Is.EqualTo(10));
        }
    }
}
