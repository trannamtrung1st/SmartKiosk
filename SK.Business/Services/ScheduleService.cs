using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class ScheduleService : Service
    {
        public ScheduleService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Schedule
        public IQueryable<Schedule> Schedules
        {
            get
            {
                return context.Schedule;
            }
        }
        #endregion

        #region Create Schedule
        protected void PrepareCreate(Schedule entity)
        {
        }

        public Schedule CreateSchedule(CreateScheduleModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Schedule.Add(entity).Entity;

        }
        #endregion

        #region Update Schedule
        public void UpdateSchedule(Schedule entity, UpdateScheduleModel model)
        {
            model.CopyTo(entity);
        }

        #endregion

    }
}
