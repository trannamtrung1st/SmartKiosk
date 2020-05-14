using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class Post
    {
        public Post()
        {
            PostResources = new HashSet<PostResource>();
            Contents = new HashSet<PostContent>();
        }

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public PostType Type { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? VisibleTime { get; set; }
        public bool? Archived { get; set; }
        public int LocationId { get; set; }
        public int OwnerId { get; set; }

        public virtual Location Location { get; set; }
        public virtual Owner Owner { get; set; }
        public virtual ICollection<PostContent> Contents { get; set; }
        public virtual ICollection<PostResource> PostResources { get; set; }
    }
}
