using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class PostResource
    {
        public int PostId { get; set; }
        public int ResourceId { get; set; }

        public virtual Post Post { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
