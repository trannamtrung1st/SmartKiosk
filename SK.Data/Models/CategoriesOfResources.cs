using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class CategoriesOfResources
    {
        public int CategoryId { get; set; }
        public int ResourceId { get; set; }
        
        public virtual EntityCategory Category { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
