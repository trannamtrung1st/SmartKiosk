using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebAdmin.Models
{
    public abstract class LocationPageModel<T> : BasePageModel<T>, ILocationPageModel
    {
        [BindProperty(Name = "loc_id", SupportsGet = true)]
        public int LocId { get; set; }
    }
}
