using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{

    public partial class Resource
    {
        public Resource()
        {
            CategoriesOfResources = new HashSet<CategoriesOfResources>();
            Contents = new HashSet<ResourceContent>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string LogoUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool Archived { get; set; }
        public int TypeId { get; set; }
        public int OwnerId { get; set; }
        public int LocationId { get; set; }
        public int BuildingId { get; set; }
        public int FloorId { get; set; }
        public int AreaId { get; set; }

        public virtual Area Area { get; set; }
        public virtual Location Location { get; set; }
        public virtual Owner Owner { get; set; }
        public virtual ResourceType ResourceType { get; set; }
        public virtual ICollection<CategoriesOfResources> CategoriesOfResources { get; set; }
        public virtual ICollection<ResourceContent> Contents { get; set; }
    }
}
