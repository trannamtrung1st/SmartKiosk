using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Helpers;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages
{
    public class AccessDeniedModel : BasePageModel<AccessDeniedModel>, IStatusModel
    {
        public string Message { get; set; } = "You're not allowed to access this resource(s)";
        public int Code { get; set; } = (int)HttpStatusCode.Forbidden;
        public string Layout { get; set; } = null;
        public string MessageTitle { get; set; } = "Access denied";
        public string StatusCodeStyle { get; set; } = "danger";
        public string OriginalUrl { get; set; }

        public IActionResult OnGet(string return_url = null)
        {
            if (return_url == null) return LocalRedirect(Routing.DASHBOARD);
            SetPageInfo();
            OriginalUrl = return_url;
            return this.StatusView();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Access denied"
            };
        }
    }
}
