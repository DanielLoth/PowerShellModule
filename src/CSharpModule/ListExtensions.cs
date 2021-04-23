using System.Collections.Generic;

namespace CSharpModule
{
    public static class ListExtensions
    {
        public static void AddRangeAndSort<T>(this List<T> list, IEnumerable<T> itemsToAdd)
        {
            list.AddRange(itemsToAdd);
            list.Sort();
        }
    }
}
