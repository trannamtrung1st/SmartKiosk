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
}
