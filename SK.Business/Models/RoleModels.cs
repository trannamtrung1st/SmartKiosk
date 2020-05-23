using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateRoleModel : MappingModel<AppRole>
    {
        public CreateRoleModel()
        {
        }

        public CreateRoleModel(AppRole entity) : base(entity)
        {
        }

        public string Name { get; set; }
    }

    public class UpdateRoleModel : MappingModel<AppRole>
    {
        public UpdateRoleModel()
        {
        }

        public UpdateRoleModel(AppRole entity) : base(entity)
        {
        }

        public string Name { get; set; }
    }

    #region Query
    public class AppRoleModel : MappingModel<AppRole>
    {
        public AppRoleModel()
        {
        }

        public AppRoleModel(AppRole entity) : base(entity)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("role_type")]
        public string RoleType { get; set; }
    }


    public class AppRoleRelationship : AppRole, IDapperRelationship
    {
        public string GetTableName() => AppRole.TBL_NAME;
    }
    #endregion
}
