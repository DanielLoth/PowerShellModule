using System.Collections.Generic;
using System.Linq;

namespace CSharpModule
{
    public static class ListExtensions
    {
        public static void AddRangeAndSort<T>(this List<T> list, IEnumerable<T> itemsToAdd)
        {
            list.AddRange(itemsToAdd);
            list.Sort();
        }

        public static List<T> AsList<T>(this IEnumerable<T> source) =>
            (source == null || source is List<T>) ? (List<T>)source : source.ToList();
    }
}
