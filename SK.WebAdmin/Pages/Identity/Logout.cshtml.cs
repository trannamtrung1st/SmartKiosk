using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.Business.Services;
using SK.WebAdmin.Models;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace SK.WebAdmin.Pages.Identity
{
    [InjectionFilter]
    public class LogoutModel : BasePageModel<LogoutModel>
    {
        [Inject]
        protected readonly IdentityService identityService;

        public async Task<IActionResult> OnGet()
        {
            if (User.Identity.IsAuthenticated)
                await identityService.SignOutAsync();
            return LocalRedirect(Routing.DASHBOARD);
        }

        protected override void SetPageInfo()
        {
            Info = null;
        }
    }
}
