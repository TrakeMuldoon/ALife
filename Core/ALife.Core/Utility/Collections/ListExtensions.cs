using System.Collections.Generic;

namespace ALife.Core.Utility.Collections
{
    /// <summary>
    /// Extensions for lists.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="items">The items.</param>
        public static void AddItems<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }
    }
}
