using elFinder.NetCore;
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
using SK.Business.Models;
using TNT.Core.Helpers.DI;

namespace SK.WebApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        [Inject]
        protected readonly DbContext context;

        public string UserId
        {
            get
            {
                return User.Identity.Name;
            }
        }

        protected T Service<T>()
        {
            return HttpContext.RequestServices.GetRequiredService<T>();
        }

        protected IActionResult Error(object obj = default)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, obj);
        }

        protected string GetAuthorityLeftPart()
        {
            return new Uri(Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
        }

        #region elFinder
        protected RootVolume GetRootVolume()
        {
            var leftPart = GetAuthorityLeftPart();
            var root = new RootVolume(
                Startup.MapPath($"~/{Settings.Instance.UploadFolderPath}"),
                $"{leftPart}/{Settings.Instance.UploadFolderPath}/",
                $"{leftPart}/{ApiEndpoint.FILE_API}/thumb/")
            {
                ThumbnailSize = 256,
                //IsReadOnly = !User.IsInRole("Administrators")
                IsReadOnly = false, // Can be readonly according to user's membership permission
                IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
                Alias = "home", // Beautiful name given to the root/home folder
                //MaxUploadSizeInKb = 2048, // Limit imposed to user uploaded file <= 2048 KB
                //LockedFolders = new List<string>(new string[] { "Folder1" })
            };
            return root;
        }
        #endregion

        protected FileDestinationMetadata GetFileDestinationMetadata()
        {
            var rootVolume = GetRootVolume();
            var sourceUrl = GetAuthorityLeftPart();
            var rootPath = Settings.Instance.WebRootPath;
            return new FileDestinationMetadata(rootVolume, sourceUrl, rootPath);
        }

    }
}
