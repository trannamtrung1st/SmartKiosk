﻿using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ScheduleDetailQuery
    {
        public static IEnumerable<IGrouping<int, ScheduleDetail>> GroupBySchedule(
            this IEnumerable<ScheduleDetail> query)
        {
            return query.GroupBy(o => o.ScheduleId);
        }
    }
}
