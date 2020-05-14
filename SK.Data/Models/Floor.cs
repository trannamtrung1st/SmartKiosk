using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Floor
    {
        public Floor()
        {
            Areas = new HashSet<Area>();
            Devices = new HashSet<Device>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }
        public int BuildingId { get; set; }
        public string FloorPlanSvg { get; set; }

        public virtual Building Building { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Area> Areas { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
    }
}
