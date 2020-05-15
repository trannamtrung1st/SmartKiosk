using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SK.Data.Models
{

    public class AppUser : IdentityUser<string>
    {
        public const string TBL_NAME = "AspNetUsers";
        public string FullName { get; set; }
        public string ActivationCode { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
        public virtual Device LinkedDevice { get; set; }
    }

}
