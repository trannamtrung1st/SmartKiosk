using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages.ResType
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
                Menu = Menu.RES_TYPE,
                Title = "Thêm mới loại địa điểm",
                BackUrl = BackUrl ?? Routing.RES_TYPE
            };
        }
    }
}
