using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class ResourceType
    {
        public ResourceType()
        {
            Resources = new HashSet<Resource>();
            Contents = new HashSet<ResourceTypeContent>();
        }

        public int Id { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<ResourceTypeContent> Contents { get; set; }
    }
}
