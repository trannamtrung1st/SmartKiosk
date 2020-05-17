using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class FloorService : Service
    {
        public FloorService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query
        public IQueryable<Floor> Floors
        {
            get
            {
                return context.Floor;
            }
        }
        #endregion
    }
}
