using System.Collections.Generic;

namespace ALife.Core.Utility
{
    /// <summary>
    /// Various helpers for lists.
    /// </summary>
    public static class ListHelpers
    {
        /// <summary>
        /// Compiles the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists">The lists.</param>
        /// <param name="individuals">The individuals.</param>
        /// <returns>The compiled list</returns>
        public static List<T> CompileList<T>(IEnumerable<T>[] lists, params T[] individuals)
        {
            List<T> toReturn = new List<T>(individuals);
            if(lists != null)
            {
                foreach(var list in lists)
                {
                    toReturn.AddRange(list);
                }
            }
            return toReturn;
        }
    }
}
