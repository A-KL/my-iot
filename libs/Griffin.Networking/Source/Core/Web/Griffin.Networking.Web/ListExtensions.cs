using System.Collections.Generic;

namespace Griffin.Networking.Web
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static void MergeRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (list.Contains(item))
                {
                    continue;
                }
                list.Add(item);
            }
        }
    }
}