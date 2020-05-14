using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class EntityCategory
    {
        public EntityCategory()
        {
            CategoriesOfResources = new HashSet<CategoriesOfResources>();
        }

        public int Id { get; set; }

        public virtual ICollection<CategoriesOfResources> CategoriesOfResources { get; set; }
        public virtual ICollection<EntityCategoryContent> EntityCategoryContents { get; set; }
    }
}
