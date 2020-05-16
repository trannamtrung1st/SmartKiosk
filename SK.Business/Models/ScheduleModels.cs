using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    #region Query
    public class ScheduleRelationship : Schedule, IDapperRelationship
    {
        public string GetTableName() => nameof(Schedule);
    }
    #endregion
}
