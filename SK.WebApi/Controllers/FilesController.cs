using elFinder.NetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SK.Business;
using SK.Business.Services;
using SK.WebHelpers;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace SK.WebApi.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route(ApiEndpoint.FILE_API)]
    [ApiController]
    [InjectionFilter]
    [Authorize]
    public class FilesController : BaseController
    {
        [Inject]
        private readonly FileService _service;

        [Route("connector")]
        [UseSystemJsonOutput]
        public async Task<IActionResult> Connector()
        {
            var connector = GetConnector();
            return await connector.ProcessAsync(Request);
        }

        [Route("thumb/{hash}")]
        [AllowAnonymous]
        [UseSystemJsonOutput]
        public async Task<IActionResult> Thumbs(string hash)
        {
            var connector = GetConnector();
            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
        }

        protected Connector GetConnector()
        {
            var root = GetRootVolume();
            var driver = _service.GetFileSystemDriver(root);
            var connector = _service.GetConnector(driver);
            return connector;
        }

    }
}
