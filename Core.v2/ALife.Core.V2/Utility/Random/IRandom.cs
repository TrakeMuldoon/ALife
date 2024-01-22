namespace ALife.Core.Utility.Random
{
    /// <summary>
    /// An interface for a random number generator.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue-1. MaxValue is not generated in order to remain
        /// functionally equivalent to System.Random.Next(). This does slightly eat into some of the performance gain
        /// over System.Random, but not much. For better performance see:
        ///
        /// Call NextInt() for an int over the range 0 to int.MaxValue.
        ///
        /// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range including
        /// negative values.
        /// </summary>
        /// <returns>A random number.</returns>
        int Next();

        /// <summary>
        /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
        /// </summary>
        /// <param name="upperBound"></param>
        /// <returns>A random number.</returns>
        int Next(int upperBound);

        /// <summary>
        /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound. upperBound
        /// must be &gt;= lowerBound. lowerBound may be negative.
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns>A random number.</returns>
        int Next(int lowerBound, int upperBound);

        /// <summary>
        /// Generates a single random bit. This method's performance is improved by generating 32 bits in one operation
        /// and storing them ready for future calls.
        /// </summary>
        /// <returns>A random bool.</returns>
        bool NextBool();

        /// <summary>
        /// Generates a random byte over the range 0 to byte.MaxValue-1. MaxValue is not generated in order to remain
        /// functionally equivalent to System.Random.Next(). This does slightly eat into some of the performance gain
        /// over System.Random, but not much.
        /// </summary>
        /// <returns>A random number.</returns>
        byte NextByte();

        /// <summary>
        /// Generates a random byte over the range 0 to upperBound-1, and not including upperBound.
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>A random number.</returns>
        byte NextByte(byte upperBound);

        /// <summary>
        /// Generates a random byte over the range lowerBound to upperBound-1, and not including upperBound. upperBound
        /// must be &gt;= lowerBound. lowerBound may be negative.
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>A random number.</returns>
        byte NextByte(byte lowerBound, byte upperBound);

        /// <summary>
        /// Fills the provided byte array with random bytes. This method is functionally equivalent to System.Random.NextBytes().
        /// </summary>
        /// <param name="buffer">The next bytes.</param>
        void NextBytes(byte[] buffer);

        /// <summary>
        /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
        /// </summary>
        /// <returns>A random number.</returns>
        double NextDouble();

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue, inclusive. This method differs from Next() only in
        /// that the range is 0 to int.MaxValue and not 0 to int.MaxValue-1.
        ///
        /// The slight difference in range means this method is slightly faster than Next() but is not functionally
        /// equivalent to System.Random.Next().
        /// </summary>
        /// <returns>A random number.</returns>
        int NextInt();

        /// <summary>
        /// Generates a uint. Values returned are over the full range of a uint, uint.MinValue to uint.MaxValue, inclusive.
        ///
        /// This is the fastest method for generating a single random number because the underlying random number
        /// generator algorithm generates 32 random bits that can be cast directly to a uint.
        /// </summary>
        /// <returns>A random number.</returns>
        uint NextUInt();

        /// <summary>
        /// Reinitialises using an int value as a seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        void Reinitalize(int seed);
    }
}
