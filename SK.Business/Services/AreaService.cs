using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class AreaService : Service
    {
        public AreaService(ServiceInjection inj) : base(inj)
        {
        }

        #region Create Area
        protected void PrepareCreate(Area entity)
        {
        }

        public Area CreateArea(CreateAreaModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Area.Add(entity).Entity;
        }
        #endregion

        #region Update Area
        public void UpdateArea(Area entity, UpdateAreaModel model)
        {
            model.CopyTo(entity);
        }
        #endregion
    }
}
