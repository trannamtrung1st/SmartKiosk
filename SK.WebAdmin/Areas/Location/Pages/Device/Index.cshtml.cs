﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Helpers;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Areas.Location.Pages.Device
{
    public class IndexModel : LocationPageModel<IndexModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Thiết bị",
                Menu = Menu.LOCATION_DEVICE,
                BackUrl = BackUrl ?? Routing.LOCATION_DASHBOARD.LocId(LocId)
            };
        }
    }
}
