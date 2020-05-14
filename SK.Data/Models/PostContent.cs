using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public class PostContent
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Lang { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public virtual Post Post { get; set; }
    }
}
