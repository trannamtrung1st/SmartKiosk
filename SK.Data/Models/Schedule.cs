using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Schedule
    {
        public Schedule()
        {
            Devices = new HashSet<Device>();
            ScheduleDetails = new HashSet<ScheduleDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }

        public virtual Location Location { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
    }
}
