using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages.Owner
{
    public class CreateModel : BasePageModel<CreateModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Thêm mới đối tác",
                Menu = Menu.OWNER,
                BackUrl = BackUrl ?? Routing.OWNER
            };
        }
    }
}
