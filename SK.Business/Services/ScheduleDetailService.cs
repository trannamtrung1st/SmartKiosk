using Dapper;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SK.Business.Helpers;
using SK.Business.Models;
using SK.Business.Queries;
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

        public IDictionary<string, object> GetScheduleDetailDynamic(
            ScheduleDetailQueryRow row, ScheduleDetailQueryProjection projection,
            ScheduleDetailQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ScheduleDetailQueryProjection.INFO:
                        {
                            var entity = row.ScheduleDetail;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            var fromTime = entity.FromTime?
                                .ToTimeZone(options.time_zone, options.culture);
                            var fromTimeStr = fromTime?
                                .ToString(options.date_format, options.culture);
                            var toTime = entity.ToTime?
                                .ToTimeZone(options.time_zone, options.culture);
                            var toTimeStr = toTime?
                                .ToString(options.date_format, options.culture);
                            obj["from_time"] = fromTimeStr;
                            obj["to_time"] = toTimeStr;
                            obj["is_default"] = entity.IsDefault;
                            obj["schedule_id"] = entity.ScheduleId;
                        }
                        break;
                    case ScheduleDetailQueryProjection.CONFIGS:
                        {
                            var entities = row.ScheduleDetail.ScheduleWeekConfigs;
                            obj["schedule_week_configs"] = entities
                                .Select(c => new
                                {
                                    to_time = c.ToTime,
                                    to_day_of_week = c.ToDayOfWeek,
                                    id = c.Id,
                                    from_time = c.FromTime,
                                    from_day_of_week = c.FromDayOfWeek,
                                    all_day = c.AllDay,
                                    config_id = c.ConfigId
                                }).ToList();
                        }
                        break;
                    case ScheduleDetailQueryProjection.SCHEDULE:
                        {
                            var entity = row.Schedule;
                            obj["schedule"] = new
                            {
                                id = entity.Id,
                                name = entity.Name,
                                code = entity.Code
                            };
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetScheduleDetailDynamic(
            IEnumerable<ScheduleDetailQueryRow> rows, ScheduleDetailQueryProjection projection,
            ScheduleDetailQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetScheduleDetailDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryScheduleDetailDynamic(
            ScheduleDetailQueryProjection projection,
            ScheduleDetailQueryOptions options,
            ScheduleDetailQueryFilter filter = null,
            ScheduleDetailQuerySort sort = null,
            ScheduleDetailQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ScheduleDetailQuery.CreateDynamicSql();
            #region General
            if (filter != null) query = query.SqlFilter(filter);
            query = query.SqlJoin(projection);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options.count_total) countQuery = query.SqlCount("*");
            query = query.SqlProjectFields(projection);
            #endregion
            await openConn;
            if (!options.single_only)
            {
                #region List query
                if (sort != null) query = query.SqlSort(sort);
                if (paging != null && (!options.load_all || !ScheduleDetailQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            query = query.SqlExtras(projection);
            var multipleResult = await conn.QueryMultipleAsync(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            using (multipleResult)
            {
                var queryResult = multipleResult.Read(
                    types: query.GetTypesArr(),
                    map: (objs) => ProcessMultiResults(query, objs),
                    splitOn: string.Join(',', query.GetSplitOns()));
                var extraKeys = projection.GetFieldsArr()
                    .Where(f => ScheduleDetailQueryProjection.Extras.ContainsKey(f));
                IEnumerable<ScheduleWeekConfig> configs = null;
                foreach (var key in extraKeys)
                {
                    switch (key)
                    {
                        case ScheduleDetailQueryProjection.CONFIGS:
                            configs = multipleResult.Read<ScheduleWeekConfig>().ToList();
                            break;
                    }
                }
                ProcessExtras(queryResult, configs);
                if (options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    var singleResult = GetScheduleDetailDynamic(single, projection, options);
                    return new QueryResult<IDictionary<string, object>>()
                    {
                        SingleResult = singleResult
                    };
                }
                if (options.count_total) totalCount = await countTask;
                var result = GetScheduleDetailDynamic(queryResult, projection, options, totalCount);
                return result;
            }
        }

        public async Task<QueryResult<ScheduleDetailQueryRow>> QueryScheduleDetail(
            ScheduleDetailQueryFilter filter = null,
            ScheduleDetailQuerySort sort = null,
            ScheduleDetailQueryProjection projection = null,
            ScheduleDetailQueryPaging paging = null,
            ScheduleDetailQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ScheduleDetailQuery.CreateDynamicSql();
            #region General
            if (filter != null) query = query.SqlFilter(filter);
            if (projection != null) query = query.SqlJoin(projection);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options != null && options.count_total) countQuery = query.SqlCount("*");
            if (projection != null) query = query.SqlProjectFields(projection);
            #endregion
            await openConn;
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.SqlSort(sort);
                if (paging != null && (!options.load_all || !ScheduleDetailQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            if (projection != null) query = query.SqlExtras(projection);
            var multipleResult = await conn.QueryMultipleAsync(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            using (multipleResult)
            {
                var queryResult = multipleResult.Read(
                    types: query.GetTypesArr(),
                    map: (objs) => ProcessMultiResults(query, objs),
                    splitOn: string.Join(',', query.GetSplitOns()));
                if (projection != null)
                {
                    var extraKeys = projection.GetFieldsArr()
                    .Where(f => ScheduleDetailQueryProjection.Extras.ContainsKey(f));
                    IEnumerable<ScheduleWeekConfig> configs = null;
                    foreach (var key in extraKeys)
                    {
                        switch (key)
                        {
                            case ScheduleDetailQueryProjection.CONFIGS:
                                configs = multipleResult.Read<ScheduleWeekConfig>().ToList();
                                break;
                        }
                    }
                    ProcessExtras(queryResult, configs);
                }
                if (options != null && options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    return new QueryResult<ScheduleDetailQueryRow>
                    {
                        SingleResult = single
                    };
                }
                if (options != null && options.count_total) totalCount = await countTask;
                return new QueryResult<ScheduleDetailQueryRow>
                {
                    Results = queryResult,
                    TotalCount = totalCount
                };
            }
        }

        private ScheduleDetailQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new ScheduleDetailQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case ScheduleDetailQueryProjection.INFO: row.ScheduleDetail = objs[i] as ScheduleDetail; break;
                    case ScheduleDetailQueryProjection.SCHEDULE: row.Schedule = objs[i] as ScheduleRelationship; break;
                }
            }
            return row;
        }

        private void ProcessExtras(IEnumerable<ScheduleDetailQueryRow> entities,
            IEnumerable<ScheduleWeekConfig> configs)
        {
            var contentsMap = configs?.GroupByScheduleDetail().ToDictionary(o => o.Key);
            foreach (var e in entities)
            {
                var entity = e.ScheduleDetail;
                if (contentsMap != null && contentsMap.ContainsKey(entity.Id))
                    entity.ScheduleWeekConfigs = contentsMap[entity.Id].ToList();
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
