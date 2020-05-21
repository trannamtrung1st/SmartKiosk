using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class PostContentQuery
    {
        public static IQueryable<PostContent> Id(this IQueryable<PostContent> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<PostContent> IdOnly(this IQueryable<PostContent> query)
        {
            return query.Select(o => new PostContent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<PostContent> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<PostContent> Ids(this IQueryable<PostContent> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        public static IQueryable<PostContent> ByLang(this IQueryable<PostContent> query,
            string lang)
        {
            return query.Where(o => o.Lang == lang);
        }

        public static IQueryable<PostContent> OfPost(this IQueryable<PostContent> query,
            int refId)
        {
            return query.Where(o => o.PostId == refId);
        }
    }
}
