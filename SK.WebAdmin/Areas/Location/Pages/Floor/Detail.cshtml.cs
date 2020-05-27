using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Helpers;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Areas.Location.Pages.Floor
{
    public class DetailModel : LocationPageModel<DetailModel>
    {
        public int Id { get; set; }
        public void OnGet(int id)
        {
            SetPageInfo();
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Chi tiết tầng",
                Menu = Menu.LOCATION_FLOOR,
                BackUrl = BackUrl ?? Routing.LOCATION_FLOOR.LocId(LocId)
            };
        }
    }
}
