using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SK.Business.Queries;
using SK.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebApi.Filters
{
    public class AppAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var user = httpContext.User;
            if (user.IsInRole(Data.RoleName.Device))
            {
                var service = httpContext.RequestServices.GetRequiredService<DeviceService>();
                var device = service.Devices.SelectTokenOnly()
                    .Id(user.Identity.Name).FirstOrDefault();
                var reqAuth = httpContext.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(' ')[1];
                if (device.AccessToken != reqAuth)
                    filterContext.Result = new UnauthorizedResult();
            }
        }
    }
}
