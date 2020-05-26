using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebAdmin.Helpers
{
    public static class RoutingHelper
    {
        public static string LocId(this string route, int locId)
        {
            return route.Replace("{loc_id}", locId.ToString());
        }

        public static string Id(this string route, int id)
        {
            return route.Replace("{id}", id.ToString());
        }
    }
}
