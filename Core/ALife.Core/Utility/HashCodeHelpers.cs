using System;
using System.Runtime.CompilerServices;

namespace ALife.Core.Utility
{
    /// <summary>
    /// Various helpers for hashcodes.
    /// </summary>
    public static class HashCodeHelper
    {
        /// <summary>
        /// Combines the hash codes into a single hash code.
        /// </summary>
        /// <param name="hashCodes">The hashcodes to combine.</param>
        /// <returns>The combined hashcode.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine(params int[] hashCodes)
        {
            int hash = HashCode.Combine(hashCodes);
            return hash;
        }

        /// <summary>
        /// Combines the hash codes of the objects into a single hash code.
        /// </summary>
        /// <param name="objects"></param>
        /// <returns>The combined hashcode.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine(params object[] objects)
        {
            int[] hashes = new int[objects.Length];
            for(int i = 0; i < objects.Length; i++)
            {
                hashes[i] = objects[i].GetHashCode();
            }

            return Combine(hashes);
        }
    }
}