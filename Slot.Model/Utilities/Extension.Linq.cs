using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Slot.Model.Utilities
{
    public static class LinqExtension
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> query, int pageIndex, int itemsPerPage = 10)
        {
            return query.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage);
        }

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> query, int pageIndex, int itemsPerPage = 10)
        {
            return query.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }
    }
}
