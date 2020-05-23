using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    #region Query
    public class AppUserRoleQueryRow
    {
        public AppUserRole UserRole { get; set; }
        public AppRole Role { get; set; }
    }
    #endregion
}
