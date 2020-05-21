using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ResourceContentQuery
    {
        public static IQueryable<ResourceContent> Id(this IQueryable<ResourceContent> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ResourceContent> IdOnly(this IQueryable<ResourceContent> query)
        {
            return query.Select(o => new ResourceContent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<ResourceContent> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ResourceContent> Ids(this IQueryable<ResourceContent> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        public static IQueryable<ResourceContent> ByLang(this IQueryable<ResourceContent> query,
            string lang)
        {
            return query.Where(o => o.Lang == lang);
        }

        public static IQueryable<ResourceContent> OfResource(this IQueryable<ResourceContent> query,
            int refId)
        {
            return query.Where(o => o.ResourceId == refId);
        }
    }
}
