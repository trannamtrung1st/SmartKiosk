using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class ResourceContent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Lang { get; set; }
        public string ResourceId { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
