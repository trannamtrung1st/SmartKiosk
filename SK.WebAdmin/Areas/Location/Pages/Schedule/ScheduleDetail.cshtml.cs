using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Helpers;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Areas.Location.Pages.Schedule
{
    public class ScheduleDetailModel : LocationPageModel<DetailModel>
    {
        public int ScheduleId { get; set; }
        public int Id { get; set; }
        public void OnGet(int schedule_id, int id)
        {
            SetPageInfo();
            ScheduleId = schedule_id;
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Cài đặt khoảng thời gian",
                Menu = Menu.LOCATION_SCHEDULE,
                BackUrl = BackUrl ?? Routing.LOCATION_SCHEDULE.LocId(LocId)
            };
        }
    }
}
