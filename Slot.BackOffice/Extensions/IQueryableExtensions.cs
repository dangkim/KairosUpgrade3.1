using Slot.BackOffice.Data.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Slot.BackOffice.Extensions
{
    public static class IQueryableExtensions
    {
        public static IEnumerable<T> TakeIf<T>(this IEnumerable<T> enumerable, int? count)
        {
            if (count.HasValue)
                return enumerable.Take(count.Value);
            else
                return enumerable;
        }

        public static List<T> WithFilters<T>(this IEnumerable<T> enumerable, BaseQuery query)
        {
            if (!string.IsNullOrWhiteSpace(query.Ordering))
            {
                enumerable = enumerable
                            .AsQueryable()
                            .OrderBy(query.Ordering);
            }

            if (query.Limit.HasValue)
            {
                enumerable = enumerable.Take(query.Limit.Value);
            }

            return enumerable.ToList();
        }
    }
}
