using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages.Location
{
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
                Title = "Trung tâm",
                Menu = Menu.LOCATION,
                BackUrl = BackUrl ?? Routing.DASHBOARD
            };
        }
    }
}
