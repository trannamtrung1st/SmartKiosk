using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class ScheduleDetailService : Service
    {
        public ScheduleDetailService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query ScheduleDetail
        public IQueryable<ScheduleDetail> ScheduleDetails
        {
            get
            {
                return context.ScheduleDetail;
            }
        }
        #endregion

        #region Create ScheduleDetail
        protected void PrepareCreate(ScheduleDetail entity)
        {
            //prepare something before create
            entity.FromTime = entity.FromTime?.ToUniversalTime();
            entity.ToTime = entity.ToTime?.ToUniversalTime();
        }

        public ScheduleDetail CreateScheduleDetail(CreateScheduleDetailModel model)
        {
            var entity = model.ToDest();
            if (entity.IsDefault == false)
            {
                var dates = ParseDateStr(model.StartEndDateStr);
                entity.FromTime = dates.Item1;
                entity.ToTime = dates.Item2;
            }
            PrepareCreate(entity);
            return context.ScheduleDetail.Add(entity).Entity;
        }
        #endregion

        #region Update ScheduleDetail
        public async Task<ScheduleDetail> UpdateScheduleDetailTransactionAsync(ScheduleDetail entity, UpdateScheduleDetailModel model)
        {
            model.CopyTo(entity);
            if (entity.IsDefault == false)
            {
                var dates = ParseDateStr(model.StartEndDateStr);
                entity.FromTime = dates.Item1;
                entity.ToTime = dates.Item2;
            }
            var weekConfigs = model.WeekConfigs.Select(wc =>
            {
                var config = wc.ToDest();
                config.ScheduleDetailId = entity.Id;
                return config;
            }).ToList();
            var deleteTask = DeleteAllConfigsOfScheduleDetailAsync(entity);
            var insertTask = context.BulkInsertAsync(weekConfigs);
            await deleteTask; await insertTask;
            return entity;
        }

        protected async Task<int> DeleteAllConfigsOfScheduleDetailAsync(ScheduleDetail entity)
        {
            var id = new SqlParameter("id", entity.Id);
            var sql = $"DELETE FROM {nameof(ScheduleWeekConfig)} WHERE " +
                $"{nameof(ScheduleWeekConfig.ScheduleDetailId)}={id.ParameterName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql, id);
            return result;
        }
        #endregion

        protected ValueTuple<DateTime, DateTime> ParseDateStr(string str)
        {
            var dateParts = str.Split('-');
            var startTime = DateTime.ParseExact(dateParts[0].Trim(), "dd/MM/yyyy", CultureInfo.CurrentCulture)
                .ToUniversalTime();
            var endTime = DateTime.ParseExact(dateParts[1].Trim(), "dd/MM/yyyy", CultureInfo.CurrentCulture)
                .ToUniversalTime();
            return ValueTuple.Create(startTime, endTime);
        }
    }
}
