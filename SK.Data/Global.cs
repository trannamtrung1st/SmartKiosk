using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SK.Data.Models;
using TNT.Core.Helpers.DI;

namespace SK.Data
{

    public class Global
    {
        private static void InitDI(IServiceCollection services)
        {
            services.AddScoped<DataContext>()
                .AddScoped<DbContext, DataContext>();
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });
        }

        public static void Init(IServiceCollection services)
        {
            InitDI(services);
        }

    }

}
