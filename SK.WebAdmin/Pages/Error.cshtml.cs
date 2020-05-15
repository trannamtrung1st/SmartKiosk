using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : SingleHandlePageModel<ErrorModel>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; } = "Empty";

        protected override IActionResult OnAllMethod()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            if (exception == null) return LocalRedirect(Routing.DASHBOARD);
            SetPageInfo();
#if !RELEASE
            Message = exception.Message;
#else
            Message = "We will work on fixing that right away.";
#endif
            _logger.Error(exception);
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return Page();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Error"
            };
        }
    }
}
