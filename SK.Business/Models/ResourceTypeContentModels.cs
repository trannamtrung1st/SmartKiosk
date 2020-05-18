using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    #region Query
    public class ResourceTypeContentRelationship : ResourceTypeContent, IDapperRelationship
    {
        public string GetTableName() => nameof(ResourceTypeContent);
    }
    #endregion
}
