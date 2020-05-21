using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class EntityCategoryContentQuery
    {
        public static IQueryable<EntityCategoryContent> Id(this IQueryable<EntityCategoryContent> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<EntityCategoryContent> IdOnly(this IQueryable<EntityCategoryContent> query)
        {
            return query.Select(o => new EntityCategoryContent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<EntityCategoryContent> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<EntityCategoryContent> Ids(this IQueryable<EntityCategoryContent> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        public static IQueryable<EntityCategoryContent> ByLang(this IQueryable<EntityCategoryContent> query,
            string lang)
        {
            return query.Where(o => o.Lang == lang);
        }

        public static IQueryable<EntityCategoryContent> OfCategory(this IQueryable<EntityCategoryContent> query,
            int refId)
        {
            return query.Where(o => o.CategoryId == refId);
        }
    }
}
