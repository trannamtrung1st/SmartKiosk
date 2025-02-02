﻿using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    #region Query
    public class CateOfResQueryRow
    {
        public CategoriesOfResources CateOfRes { get; set; }
        public EntityCategoryRelationship Category { get; set; }
        public EntityCategoryContentRelationship CategoryContent { get; set; }
        public ResourceRelationship Resource { get; set; }
        public ResourceContentRelationship ResourceContent { get; set; }
    }
    #endregion
}
