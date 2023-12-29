using System.Drawing;
using ALife.Core;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Ranges;

namespace ALife.Tests.Utility.Colours
{
    /// <summary>
    /// Tests for the Colour class.
    /// </summary>
    internal class TestColour
    {
        /// <summary>
        /// The test sim
        /// </summary>
        private Simulation _sim;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _sim = new Simulation(1);
        }

        /// <summary>
        /// Tests the basics.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            var colorA = new Colour(0, 0, 0);
            Assert.That(colorA.Alpha, Is.EqualTo(255));
            Assert.That(colorA.Red, Is.EqualTo(0));
            Assert.That(colorA.Green, Is.EqualTo(0));
            Assert.That(colorA.Blue, Is.EqualTo(0));

            var colorB = new Colour(colorA);
            Assert.That(colorB, Is.EqualTo(colorA));
            colorA.Red = 255;
            Assert.That(colorB, Is.Not.EqualTo(colorA));

            Color systemColor = colorA.ToSystemColor();
            Assert.That(systemColor.A, Is.EqualTo(255));
            Assert.That(systemColor.R, Is.EqualTo(255));
            Assert.That(systemColor.G, Is.EqualTo(0));
            Assert.That(systemColor.B, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests the HSL conversion.
        /// </summary>
        [Test]
        public void TestHslA()
        {
            var expectedColour = new Colour(0, 191, 255);
            var hslColour = Colour.FromHSL(195, 1, 0.5);
            Assert.That(hslColour, Is.EqualTo(expectedColour));

            hslColour.GetAHSL(out var alpha, out var hue, out var saturation, out var lightness);
            Assert.That(alpha, Is.EqualTo(255));
            Assert.That(hue, Is.EqualTo(195));
            Assert.That(saturation, Is.EqualTo(1));
            Assert.That(lightness, Is.EqualTo(0.5));
        }

        /// <summary>
        /// Tests the HSL conversion.
        /// </summary>
        [Test]
        public void TestHslB()
        {
            var expectedColour = new Colour(0, 191, 255);
            var hslColour = Colour.FromHSV(195, 1, 1);
            Assert.That(hslColour, Is.EqualTo(expectedColour));

            hslColour.GetAHSL(out var alpha, out var hue, out var saturation, out var lightness);
            Assert.That(alpha, Is.EqualTo(255));
            Assert.That(hue, Is.EqualTo(195));
            Assert.That(saturation, Is.EqualTo(1));
            Assert.That(lightness, Is.EqualTo(0.5));
        }

        /// <summary>
        /// Tests the HSV conversion.
        /// </summary>
        [Test]
        public void TestHsvA()
        {
            var expectedColour = new Colour(0, 191, 255);
            var hslColour = Colour.FromHSV(195, 1, 1);
            Assert.That(hslColour, Is.EqualTo(expectedColour));

            hslColour.GetAHSV(out var alpha, out var hue, out var saturation, out var lightness);
            Assert.That(alpha, Is.EqualTo(255));
            Assert.That(hue, Is.EqualTo(195));
            Assert.That(saturation, Is.EqualTo(1));
            Assert.That(lightness, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests the HSV conversion.
        /// </summary>
        [Test]
        public void TestHsvB()
        {
            var expectedColour = new Colour(0, 96, 128);
            var hslColour = Colour.FromHSV(195, 1, 0.5);
            Assert.That(hslColour, Is.EqualTo(expectedColour));

            hslColour.GetAHSV(out var alpha, out var hue, out var saturation, out var lightness);
            Assert.That(alpha, Is.EqualTo(255));
            Assert.That(hue, Is.EqualTo(195));
            Assert.That(saturation, Is.EqualTo(1));
            Assert.That(Math.Round(lightness, 1), Is.EqualTo(0.5));
        }

        /// <summary>
        /// Tests the basic random color functionality.
        /// </summary>
        [Test]
        public void TestRandomColorBasic()
        {
            // NOTE: values will shift if the random number generator changes
            var color = Colour.GetRandomColour(_sim);
            Assert.That(color.Alpha, Is.EqualTo(255));
            Assert.That(color.Red, Is.EqualTo(119));
            Assert.That(color.Green, Is.EqualTo(136));
            Assert.That(color.Blue, Is.EqualTo(167));
        }

        /// <summary>
        /// Tests the random color functionality with ranges.
        /// </summary>
        [Test]
        public void TestRandomColorWithRanges()
        {
            // NOTE: values will shift if the random number generator changes
            var redRange = new ByteRange(10, 100);
            var greenRange = new ByteRange(100, 200);
            var blueRange = new ByteRange(200, 255);
            var alphaRange = new ByteRange(0, 10);
            var color = Colour.GetRandomColour(_sim, redRange, greenRange, blueRange, alphaRange);
            Assert.That(color.Alpha, Is.EqualTo(1));
            Assert.That(color.Red, Is.EqualTo(31));
            Assert.That(color.Green, Is.EqualTo(223));
            Assert.That(color.Blue, Is.EqualTo(189));
        }
    }
}
