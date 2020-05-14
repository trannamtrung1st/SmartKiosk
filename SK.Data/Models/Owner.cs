using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Posts = new HashSet<Post>();
            Resources = new HashSet<Resource>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
    }
}
