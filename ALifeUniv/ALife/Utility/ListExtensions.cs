using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Utility
{
    public static class ListExtensions
    {
        public static List<T> CompileList<T>(IEnumerable<T>[] lists, params T[] individuals)
        {
            List<T> toReturn = new List<T>(individuals);
            foreach(var list in lists)
            {
                toReturn.AddRange(list);
            }
            return toReturn;
        }

    }
}
