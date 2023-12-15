using System.Collections.Generic;

namespace ALife.Core.Utility
{
    public static class ListExtensions
    {
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
