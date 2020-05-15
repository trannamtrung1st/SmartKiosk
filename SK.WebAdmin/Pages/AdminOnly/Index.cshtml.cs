using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.Data;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages.AdminOnly
{
    [Authorize(Roles = RoleName.ADMIN)]
    public class IndexModel : BasePageModel<IndexModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Menu = Menu.ADMIN_ONLY,
                Title = "Admin only"
            };
        }
    }
}
