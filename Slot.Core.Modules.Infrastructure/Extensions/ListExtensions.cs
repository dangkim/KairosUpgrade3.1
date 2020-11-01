using System.Collections.Generic;
using System.Linq;

namespace Slot.Core.Modules.Infrastructure
{
    public static class ListExtensions
    {
        public static IEnumerable<T> TakeCircular<T>(this IReadOnlyList<T> list, int position, int count)
        {
            var length = list.Count;
            for (var i = 0; i < count; i++)
            {
                yield return list[(position + i) % length];
            }
        }

        public static List<T> TakeCircularList<T>(this IReadOnlyList<T> list, int index, int count)
        {
            return list.TakeCircular(index, count).ToList();
        }

        public static int[] FindAllIndexOf<T>(this IEnumerable<T> values, T val, int seed = 0)
        {
            return values.Select((b, i) => object.Equals(b, val) ? i + seed : -1).Where(i => i != -1).ToArray();
        }
    }
}