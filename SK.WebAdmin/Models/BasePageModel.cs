using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.WebAdmin.Models
{
    public abstract class BasePageModel<T> : PageModel, IInfoPageModel
    {
        public PageInfo Info { get; set; }

        protected abstract void SetPageInfo();
    }
}
