﻿using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    #region Query
    public class ScheduleWeekConfigRelationship : ScheduleWeekConfig, IDapperRelationship
    {
        public string GetTableName() => nameof(ScheduleWeekConfig);
    }
    #endregion
}
