using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace oapp.Extensions
{
    public static partial class OExtension
    {
        public static IQueryable<T> Including<T>(this IQueryable<T> self, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            return includeProperties.Aggregate(self, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}