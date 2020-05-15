using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{

    public abstract class Service
    {
        [Inject]
        protected readonly DataContext context;

        public Service(ServiceInjection inj)
        {
            inj.Inject(this);
        }

    }
}
