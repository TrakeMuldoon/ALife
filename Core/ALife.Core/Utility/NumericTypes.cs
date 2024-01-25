using System;

namespace ALife.Core.Utility
{
    public static class NumericTypes
    {
        public static readonly Type IntType = typeof(int);
        public static readonly Type DoubleType = typeof(double);
        public static readonly Type ByteType = typeof(byte);
        public static readonly Type FloatType = typeof(float);
        public static readonly Type ShortType = typeof(short);
        public static readonly Type LongType = typeof(long);
        public static readonly Type DecimalType = typeof(decimal);
        public static readonly Type UintType = typeof(uint);
        public static readonly Type UshortType = typeof(ushort);
        public static readonly Type UlongType = typeof(ulong);
        public static readonly Type SbyteType = typeof(sbyte);

        public static readonly Type[] SupportedTypes = new Type[]
        {
            IntType,
            DoubleType,
            ByteType,
            FloatType,
            ShortType,
            LongType,
            DecimalType,
            UintType,
            UshortType,
            UlongType,
            SbyteType
        };
    }
}
