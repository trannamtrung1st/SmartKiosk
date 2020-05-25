using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages.EtCate
{
    public class CreateModel : BasePageModel<CreateModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Menu = Menu.ENTITY_CATE,
                Title = "Thêm mới danh mục",
                BackUrl = BackUrl ?? Routing.ENTITY_CATE
            };
        }
    }
}
