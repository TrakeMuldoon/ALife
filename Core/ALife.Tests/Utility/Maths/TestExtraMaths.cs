
using System;
using System.Linq;
using ALife.Core.Utility.Maths;

namespace ALife.Tests.Utility.Maths
{
    /// <summary>
    /// Tests for the ExtraMaths class.
    /// </summary>
    [TestClass]
    public class TestExtraMaths
    {
        
        [TestMethod]
        public void TestInt16CircularClamp()
        {
            Int16 min = (Int16)(-1);
            Int16 two = (Int16)(2);
            Int16 max = (Int16)Math.Round((decimal)(Int16.MaxValue / two));
            Int16 negativeNum = -2;
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(max - 1, ExtraMaths.CircularClamp(negativeNum, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp((Int16)(max + 1), min, max));
        }

        
        [TestMethod]
        public void TestInt32CircularClamp()
        {
            Int32 min = (Int32)(-1);
            Int32 two = (Int32)(2);
            Int32 max = (Int32)Math.Round((decimal)(Int32.MaxValue / two));
            Int32 negativeNum = -2;
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(max - 1, ExtraMaths.CircularClamp(negativeNum, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp((Int32)(max + 1), min, max));
        }

        
        [TestMethod]
        public void TestInt64CircularClamp()
        {
            Int64 min = (Int64)(-1);
            Int64 two = (Int64)(2);
            Int64 max = (Int64)Math.Round((decimal)(Int64.MaxValue / two));
            Int64 negativeNum = -2;
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(max - 1, ExtraMaths.CircularClamp(negativeNum, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp((Int64)(max + 1), min, max));
        }

        
        [TestMethod]
        public void TestDecimalCircularClamp()
        {
            Decimal min = (Decimal)(-1);
            Decimal two = (Decimal)(2);
            Decimal max = (Decimal)Math.Round((decimal)(Decimal.MaxValue / two));
            Decimal negativeNum = -2;
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(max - 1, ExtraMaths.CircularClamp(negativeNum, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp((Decimal)(max + 1), min, max));
        }

                
        [TestMethod]
        public void TestDoubleCircularClamp()
        {
            Double min = (Double)(-1);
            Double two = (Double)(2);

            Double max = (Double)Math.Round(Double.MaxValue / two);
            Double negativeNum = -2;
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(max - 1, ExtraMaths.CircularClamp(negativeNum, min, max));
        }

        
        [TestMethod]
        public void TestSingleCircularClamp()
        {
            Single min = (Single)(-1);
            Single two = (Single)(2);

            Single max = (Single)Math.Round(Single.MaxValue / two);
            Single negativeNum = -2;
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(max - 1, ExtraMaths.CircularClamp(negativeNum, min, max));
        }

        
        
        [TestMethod]
        public void TestUInt16CircularClamp()
        {
            UInt16 min = 0;
            UInt16 max = (UInt16)Math.Round(UInt16.MaxValue / 2d);
            
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp(max + 1, min, max));
        }

        
        [TestMethod]
        public void TestUInt32CircularClamp()
        {
            UInt32 min = 0;
            UInt32 max = (UInt32)Math.Round(UInt32.MaxValue / 2d);
            
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp(max + 1, min, max));
        }

        
        [TestMethod]
        public void TestUInt64CircularClamp()
        {
            UInt64 min = 0;
            UInt64 max = (UInt64)Math.Round(UInt64.MaxValue / 2d);
            
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp(max + 1, min, max));
        }

        
        [TestMethod]
        public void TestByteCircularClamp()
        {
            Byte min = 0;
            Byte max = (Byte)Math.Round(Byte.MaxValue / 2d);
            
            
            Assert.AreEqual(min, ExtraMaths.CircularClamp(min, min, max));
            Assert.AreEqual(min, ExtraMaths.CircularClamp(max, min, max));
            Assert.AreEqual(min + 1, ExtraMaths.CircularClamp(max + 1, min, max));
        }

        
        
        [TestMethod]
        public void TestInt16Clamp()
        {
            Int16 min = 1;
            Int16 max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((Int16)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((Int16)(max - 1), ExtraMaths.Clamp((Int16)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((Int16)(max + 1), min, max));
        }

        [TestMethod]
        public void TestInt16DeltaClamp()
        {
            Int16 absoluteMin = 0;
            Int16 absoluteMax = 3;
            
            Int16 deltaMin = 1;
            Int16 deltaMax = 2;
            
            Int16 zero = 1;
            Int16 one = 1;
            Int16 two = 2;
            Int16 three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestInt16Maximum()
        {
            Int16 one = 1;
            Int16 two = 2;
            Int16 three = 3;
            Int16 four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestInt16Minimum()
        {
            Int16 one = 1;
            Int16 two = 2;
            Int16 three = 3;
            Int16 four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestInt16MinMaxDelta()
        {
            Int16 zero = 0;
            Int16 one = 1;
            Int16 two = 2;
            Int16 three = 3;
            Int16 four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestInt32Clamp()
        {
            Int32 min = 1;
            Int32 max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((Int32)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((Int32)(max - 1), ExtraMaths.Clamp((Int32)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((Int32)(max + 1), min, max));
        }

        [TestMethod]
        public void TestInt32DeltaClamp()
        {
            Int32 absoluteMin = 0;
            Int32 absoluteMax = 3;
            
            Int32 deltaMin = 1;
            Int32 deltaMax = 2;
            
            Int32 zero = 1;
            Int32 one = 1;
            Int32 two = 2;
            Int32 three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestInt32Maximum()
        {
            Int32 one = 1;
            Int32 two = 2;
            Int32 three = 3;
            Int32 four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestInt32Minimum()
        {
            Int32 one = 1;
            Int32 two = 2;
            Int32 three = 3;
            Int32 four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestInt32MinMaxDelta()
        {
            Int32 zero = 0;
            Int32 one = 1;
            Int32 two = 2;
            Int32 three = 3;
            Int32 four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestInt64Clamp()
        {
            Int64 min = 1;
            Int64 max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((Int64)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((Int64)(max - 1), ExtraMaths.Clamp((Int64)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((Int64)(max + 1), min, max));
        }

        [TestMethod]
        public void TestInt64DeltaClamp()
        {
            Int64 absoluteMin = 0;
            Int64 absoluteMax = 3;
            
            Int64 deltaMin = 1;
            Int64 deltaMax = 2;
            
            Int64 zero = 1;
            Int64 one = 1;
            Int64 two = 2;
            Int64 three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestInt64Maximum()
        {
            Int64 one = 1;
            Int64 two = 2;
            Int64 three = 3;
            Int64 four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestInt64Minimum()
        {
            Int64 one = 1;
            Int64 two = 2;
            Int64 three = 3;
            Int64 four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestInt64MinMaxDelta()
        {
            Int64 zero = 0;
            Int64 one = 1;
            Int64 two = 2;
            Int64 three = 3;
            Int64 four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestUInt16Clamp()
        {
            UInt16 min = 1;
            UInt16 max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((UInt16)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((UInt16)(max - 1), ExtraMaths.Clamp((UInt16)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((UInt16)(max + 1), min, max));
        }

        [TestMethod]
        public void TestUInt16DeltaClamp()
        {
            UInt16 absoluteMin = 0;
            UInt16 absoluteMax = 3;
            
            UInt16 deltaMin = 1;
            UInt16 deltaMax = 2;
            
            UInt16 zero = 1;
            UInt16 one = 1;
            UInt16 two = 2;
            UInt16 three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestUInt16Maximum()
        {
            UInt16 one = 1;
            UInt16 two = 2;
            UInt16 three = 3;
            UInt16 four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestUInt16Minimum()
        {
            UInt16 one = 1;
            UInt16 two = 2;
            UInt16 three = 3;
            UInt16 four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestUInt16MinMaxDelta()
        {
            UInt16 zero = 0;
            UInt16 one = 1;
            UInt16 two = 2;
            UInt16 three = 3;
            UInt16 four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestUInt32Clamp()
        {
            UInt32 min = 1;
            UInt32 max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((UInt32)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((UInt32)(max - 1), ExtraMaths.Clamp((UInt32)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((UInt32)(max + 1), min, max));
        }

        [TestMethod]
        public void TestUInt32DeltaClamp()
        {
            UInt32 absoluteMin = 0;
            UInt32 absoluteMax = 3;
            
            UInt32 deltaMin = 1;
            UInt32 deltaMax = 2;
            
            UInt32 zero = 1;
            UInt32 one = 1;
            UInt32 two = 2;
            UInt32 three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestUInt32Maximum()
        {
            UInt32 one = 1;
            UInt32 two = 2;
            UInt32 three = 3;
            UInt32 four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestUInt32Minimum()
        {
            UInt32 one = 1;
            UInt32 two = 2;
            UInt32 three = 3;
            UInt32 four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestUInt32MinMaxDelta()
        {
            UInt32 zero = 0;
            UInt32 one = 1;
            UInt32 two = 2;
            UInt32 three = 3;
            UInt32 four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestUInt64Clamp()
        {
            UInt64 min = 1;
            UInt64 max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((UInt64)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((UInt64)(max - 1), ExtraMaths.Clamp((UInt64)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((UInt64)(max + 1), min, max));
        }

        [TestMethod]
        public void TestUInt64DeltaClamp()
        {
            UInt64 absoluteMin = 0;
            UInt64 absoluteMax = 3;
            
            UInt64 deltaMin = 1;
            UInt64 deltaMax = 2;
            
            UInt64 zero = 1;
            UInt64 one = 1;
            UInt64 two = 2;
            UInt64 three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestUInt64Maximum()
        {
            UInt64 one = 1;
            UInt64 two = 2;
            UInt64 three = 3;
            UInt64 four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestUInt64Minimum()
        {
            UInt64 one = 1;
            UInt64 two = 2;
            UInt64 three = 3;
            UInt64 four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestUInt64MinMaxDelta()
        {
            UInt64 zero = 0;
            UInt64 one = 1;
            UInt64 two = 2;
            UInt64 three = 3;
            UInt64 four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestDoubleClamp()
        {
            Double min = 1;
            Double max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((Double)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((Double)(max - 1), ExtraMaths.Clamp((Double)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((Double)(max + 1), min, max));
        }

        [TestMethod]
        public void TestDoubleDeltaClamp()
        {
            Double absoluteMin = 0;
            Double absoluteMax = 3;
            
            Double deltaMin = 1;
            Double deltaMax = 2;
            
            Double zero = 1;
            Double one = 1;
            Double two = 2;
            Double three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestDoubleMaximum()
        {
            Double one = 1;
            Double two = 2;
            Double three = 3;
            Double four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestDoubleMinimum()
        {
            Double one = 1;
            Double two = 2;
            Double three = 3;
            Double four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestDoubleMinMaxDelta()
        {
            Double zero = 0;
            Double one = 1;
            Double two = 2;
            Double three = 3;
            Double four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestSingleClamp()
        {
            Single min = 1;
            Single max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((Single)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((Single)(max - 1), ExtraMaths.Clamp((Single)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((Single)(max + 1), min, max));
        }

        [TestMethod]
        public void TestSingleDeltaClamp()
        {
            Single absoluteMin = 0;
            Single absoluteMax = 3;
            
            Single deltaMin = 1;
            Single deltaMax = 2;
            
            Single zero = 1;
            Single one = 1;
            Single two = 2;
            Single three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestSingleMaximum()
        {
            Single one = 1;
            Single two = 2;
            Single three = 3;
            Single four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestSingleMinimum()
        {
            Single one = 1;
            Single two = 2;
            Single three = 3;
            Single four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestSingleMinMaxDelta()
        {
            Single zero = 0;
            Single one = 1;
            Single two = 2;
            Single three = 3;
            Single four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

        
        [TestMethod]
        public void TestByteClamp()
        {
            Byte min = 1;
            Byte max = 5;
            
            Assert.AreEqual(min, ExtraMaths.Clamp((Byte)(min - 1), min, max));
            Assert.AreEqual(min, ExtraMaths.Clamp(min, min, max));
            Assert.AreEqual((Byte)(max - 1), ExtraMaths.Clamp((Byte)(max - 1), min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp(max, min, max));
            Assert.AreEqual(max, ExtraMaths.Clamp((Byte)(max + 1), min, max));
        }

        [TestMethod]
        public void TestByteDeltaClamp()
        {
            Byte absoluteMin = 0;
            Byte absoluteMax = 3;
            
            Byte deltaMin = 1;
            Byte deltaMax = 2;
            
            Byte zero = 1;
            Byte one = 1;
            Byte two = 2;
            Byte three = 3;
            
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(one, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(two, ExtraMaths.DeltaClamp(two, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
            Assert.AreEqual(three, ExtraMaths.DeltaClamp(three, zero, deltaMin, deltaMax, absoluteMin, absoluteMax));
        }

        [TestMethod]
        public void TestByteMaximum()
        {
            Byte one = 1;
            Byte two = 2;
            Byte three = 3;
            Byte four = 4;
            
            Assert.AreEqual(four, ExtraMaths.Maximum(one, two, three, four));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four, two, three));
            Assert.AreEqual(four, ExtraMaths.Maximum(one, four));
            Assert.AreEqual(one, ExtraMaths.Maximum(one));
        }

        [TestMethod]
        public void TestByteMinimum()
        {
            Byte one = 1;
            Byte two = 2;
            Byte three = 3;
            Byte four = 4;
            
            Assert.AreEqual(one, ExtraMaths.Minimum(one, two, three, four));
            Assert.AreEqual(one, ExtraMaths.Minimum(four, two, one, three));
            Assert.AreEqual(one, ExtraMaths.Minimum(one, four));
            Assert.AreEqual(four, ExtraMaths.Minimum(four));
        }

        [TestMethod]
        public void TestByteMinMaxDelta()
        {
            Byte zero = 0;
            Byte one = 1;
            Byte two = 2;
            Byte three = 3;
            Byte four = 4;
            
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, two, three, four));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(four, two, one, three));
            Assert.AreEqual(three, ExtraMaths.MinMaxDelta(one, four));
            Assert.AreEqual(zero, ExtraMaths.MinMaxDelta(four));
        }

            }
}
