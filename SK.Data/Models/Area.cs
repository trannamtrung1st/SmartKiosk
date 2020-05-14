using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Area
    {
        public Area()
        {
            Devices = new HashSet<Device>();
            Resources = new HashSet<Resource>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int FloorId { get; set; }
        public int BuildingId { get; set; }
        public int LocationId { get; set; }
        public bool Archived { get; set; }

        public virtual Floor Floor { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
    }
}
