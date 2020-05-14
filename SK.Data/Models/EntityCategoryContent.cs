using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class EntityCategoryContent
    {
        public string Name { get; set; }
        public string Lang { get; set; }
        public int CategoryId { get; set; }
        public EntityCategory EntityCategory { get; set; }
    }
}
