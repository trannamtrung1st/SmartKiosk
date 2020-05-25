﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.WebAdmin.Models;

namespace SK.WebAdmin.Pages.EtCate
{
    public class DetailModel : BasePageModel<DetailModel>
    {
        public int Id { get; set; }
        public void OnGet(int id)
        {
            SetPageInfo();
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Chi tiết danh mục",
                Menu = Menu.ENTITY_CATE,
                BackUrl = BackUrl ?? Routing.ENTITY_CATE
            };
        }
    }
}
