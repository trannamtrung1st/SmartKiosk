using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ResourceTypeContentQuery
    {
        public static IQueryable<ResourceTypeContent> Id(this IQueryable<ResourceTypeContent> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ResourceTypeContent> IdOnly(this IQueryable<ResourceTypeContent> query)
        {
            return query.Select(o => new ResourceTypeContent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<ResourceTypeContent> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ResourceTypeContent> Ids(this IQueryable<ResourceTypeContent> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        public static IQueryable<ResourceTypeContent> ByLang(this IQueryable<ResourceTypeContent> query,
            string lang)
        {
            return query.Where(o => o.Lang == lang);
        }

        public static IQueryable<ResourceTypeContent> OfResourceType(this IQueryable<ResourceTypeContent> query,
            int refId)
        {
            return query.Where(o => o.ResourceTypeId == refId);
        }
    }
}
