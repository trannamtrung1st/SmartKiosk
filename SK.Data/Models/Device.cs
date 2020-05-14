using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Device
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AreaId { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public int? LocationId { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public int? ScheduleId { get; set; }
        public string CurrentFcmToken { get; set; }
        public string AccessToken { get; set; }

        public virtual Area Area { get; set; }
        public virtual Location Location { get; set; }
        public virtual AppUser DeviceAccount { get; set; }
        public virtual Schedule Schedule { get; set; }
    }
}
