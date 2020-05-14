using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Location
    {
        public Location()
        {
            Areas = new HashSet<Area>();
            Buildings = new HashSet<Building>();
            Configs = new HashSet<Config>();
            Devices = new HashSet<Device>();
            Floors = new HashSet<Floor>();
            Posts = new HashSet<Post>();
            Resources = new HashSet<Resource>();
            Schedules = new HashSet<Schedule>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<Area> Areas { get; set; }
        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<Config> Configs { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Floor> Floors { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
