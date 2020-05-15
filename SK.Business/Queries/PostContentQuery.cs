using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class PostContentQuery
    {
        public static IQueryable<PostContent> Ids(this IQueryable<PostContent> query,
            IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        public static IQueryable<PostContent> IdOnly(this IQueryable<PostContent> query)
        {
            return query.Select(o => new PostContent { Id = o.Id });
        }

        public static IQueryable<PostContent> ByLang(this IQueryable<PostContent> query,
            string lang)
        {
            return query.Where(o => o.Lang == lang);
        }

    }
}
