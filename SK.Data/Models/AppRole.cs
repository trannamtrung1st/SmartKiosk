using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SK.Data.Models
{

    public partial class AppRole : IdentityRole<string>
    {
        public const string TBL_NAME = "AspNetRoles";

        public string DisplayName { get; set; }
        public RoleType RoleType { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }

}
