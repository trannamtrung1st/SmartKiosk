﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Helpers;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Areas.Location.Pages.Config
{
    public class ContactModel : LocationPageModel<DetailModel>, ITabPageModel
    {
        public int Id { get; set; }
        public string Tab => "contact";
        public void OnGet(int id)
        {
            SetPageInfo();
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Cài đặt trang liên hệ",
                Menu = Menu.LOCATION_CONFIG,
                BackUrl = BackUrl ?? Routing.LOCATION_CONFIG.LocId(LocId)
            };
        }
    }
}
