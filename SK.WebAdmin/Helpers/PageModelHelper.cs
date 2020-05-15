using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Helpers
{
    public static class PageModelHelper
    {

        public static IActionResult MessageView<T>(this T model)
            where T : PageModel, IMessageModel
        {
            return new ViewResult()
            {
                TempData = model.TempData,
                ViewData = model.ViewData,
                ViewName = AppView.MESSAGE
            };
        }

        public static IActionResult StatusView<T>(this T model)
            where T : PageModel, IStatusModel
        {
            return new ViewResult()
            {
                TempData = model.TempData,
                ViewData = model.ViewData,
                ViewName = AppView.STATUS
            };
        }
    }
}
