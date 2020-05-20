using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class ResourceContentRelationship : ResourceContent, IDapperRelationship
    {
        public string GetTableName() => nameof(ResourceContent);
    }
}
