using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Config
    {
        public Config()
        {
            ScheduleWeekConfigs = new HashSet<ScheduleWeekConfig>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScreenSaverPlaylist { get; set; }
        public string HomeConfig { get; set; }
        public string MapConfig { get; set; }
        public string ProgramEventConfig { get; set; }
        public string NotiConfig { get; set; }
        public string ContactConfig { get; set; }
        public int LocationId { get; set; }

        public virtual Location Location { get; set; }
        public virtual ICollection<ScheduleWeekConfig> ScheduleWeekConfigs { get; set; }
    }
}
