using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class ScheduleWeekConfig
    {
        public int Id { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        public bool? AllDay { get; set; }
        public int? FromDayOfWeek { get; set; }
        public int? ToDayOfWeek { get; set; }
        public int ScheduleDetailId { get; set; }
        public int ConfigId { get; set; }

        public virtual Config Config { get; set; }
        public virtual ScheduleDetail ScheduleDetail { get; set; }
    }
}
