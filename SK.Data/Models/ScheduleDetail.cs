using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class ScheduleDetail
    {
        public ScheduleDetail()
        {
            ScheduleWeekConfigs = new HashSet<ScheduleWeekConfig>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public bool? IsDefault { get; set; }
        public int ScheduleId { get; set; }

        public virtual Schedule Schedule { get; set; }
        public virtual ICollection<ScheduleWeekConfig> ScheduleWeekConfigs { get; set; }
    }
}
