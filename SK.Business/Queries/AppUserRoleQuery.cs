using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class AppUserRoleQuery
    {
        public static IEnumerable<IGrouping<string, AppUserRoleQueryRow>> GroupByAppUser(
            this IEnumerable<AppUserRoleQueryRow> query)
        {
            return query.GroupBy(o => o.UserRole.UserId);
        }

    }
}
