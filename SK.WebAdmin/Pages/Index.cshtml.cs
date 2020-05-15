﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SK.Business.Models;

namespace SK.WebAdmin.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public TokenResponseModel TokenModel { get; set; }
        public string ReturnUrl { get; set; }

        public IActionResult OnGet([FromQuery]TokenResponseModel model,
            string return_url = Routing.DASHBOARD)
        {
            if (string.IsNullOrEmpty(model?.access_token)) 
                return LocalRedirect(return_url);
            ReturnUrl = return_url;
            TokenModel = model;
            return Page();
        }
    }
}
