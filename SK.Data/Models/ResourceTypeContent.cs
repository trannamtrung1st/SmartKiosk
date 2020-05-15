using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class ResourceTypeContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public int ResourceTypeId { get; set; }
        public virtual ResourceType ResourceType { get; set; }
    }
}
