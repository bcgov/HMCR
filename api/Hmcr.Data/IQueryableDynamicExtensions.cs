using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace Hmcr.Data
{
    public static class IQueryableDynamicExtensions
    {
        public static IOrderedQueryable<TSource> DynamicOrderBy<TSource>(this IQueryable<TSource> source, string ordering, params object[] args)
        {
            return source.OrderBy(ordering, args);
        }
    }
}
