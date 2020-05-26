using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Helpers;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Areas.Location.Pages.Resource
{
    public class CreateModel : LocationPageModel<CreateModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Menu = Menu.LOCATION_RESOURCE,
                Title = "Thêm mới địa điểm",
                BackUrl = BackUrl ?? Routing.LOCATION_RESOURCE.LocId(LocId)
            };
        }
    }
}
