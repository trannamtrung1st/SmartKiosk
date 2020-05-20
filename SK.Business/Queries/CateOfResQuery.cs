using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class CategoriesOfResourcesQuery
    {
        public static IEnumerable<IGrouping<int, CateOfResQueryRow>> GroupByResource(
            this IEnumerable<CateOfResQueryRow> query)
        {
            return query.GroupBy(o => o.CateOfRes.ResourceId);
        }
    }
}
