using Dapper;
using Microsoft.EntityFrameworkCore;
using SK.Business.Helpers;
using SK.Business.Models;
using SK.Business.Queries;
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

        public IDictionary<string, object> GetScheduleDynamic(
            ScheduleQueryRow row, ScheduleQueryProjection projection,
            ScheduleQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ScheduleQueryProjection.INFO:
                        {
                            var entity = row.Schedule;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                            obj["description"] = entity.Description;
                            obj["location_id"] = entity.LocationId;
                        }
                        break;
                    case ScheduleQueryProjection.DETAILS:
                        {
                            var entities = row.Schedule.ScheduleDetails;
                            obj["schedule_details"] = entities.Select(o =>
                                {
                                    var fromTime = o.FromTime?
                                        .ToTimeZone(options.time_zone, options.culture);
                                    var fromTimeStr = fromTime?
                                        .ToString(options.date_format, options.culture);
                                    var toTime = o.ToTime?
                                        .ToTimeZone(options.time_zone, options.culture);
                                    var toTimeStr = toTime?
                                        .ToString(options.date_format, options.culture);
                                    return new
                                    {
                                        name = o.Name,
                                        from_time = fromTimeStr,
                                        to_time = toTimeStr,
                                        id = o.Id,
                                        is_default = o.IsDefault,
                                    };
                                });
                        }
                        break;
                    case ScheduleQueryProjection.LOCATION:
                        {
                            var entity = row.Location;
                            obj["location"] = new
                            {
                                id = entity.Id,
                                name = entity.Name,
                                code = entity.Code
                            };
                        }
                        break;
                    case ScheduleQueryProjection.SELECT:
                        {
                            var entity = row.Schedule;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["code"] = entity.Code;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetScheduleDynamic(
            IEnumerable<ScheduleQueryRow> rows, ScheduleQueryProjection projection,
            ScheduleQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetScheduleDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryScheduleDynamic(
            ScheduleQueryProjection projection,
            ScheduleQueryOptions options,
            ScheduleQueryFilter filter = null,
            ScheduleQuerySort sort = null,
            ScheduleQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ScheduleQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !ScheduleQueryOptions.IsLoadAllAllowed))
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
                    .Where(f => ScheduleQueryProjection.Extras.ContainsKey(f));
                IEnumerable<ScheduleDetail> details = null;
                foreach (var key in extraKeys)
                {
                    switch (key)
                    {
                        case ScheduleQueryProjection.DETAILS:
                            details = multipleResult.Read<ScheduleDetail>().ToList();
                            break;
                    }
                }
                ProcessExtras(queryResult, details);
                if (options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    var singleResult = GetScheduleDynamic(single, projection, options);
                    return new QueryResult<IDictionary<string, object>>()
                    {
                        SingleResult = singleResult
                    };
                }
                if (options.count_total) totalCount = await countTask;
                var result = GetScheduleDynamic(queryResult, projection, options, totalCount);
                return result;
            }
        }

        public async Task<QueryResult<ScheduleQueryRow>> QuerySchedule(
            ScheduleQueryFilter filter = null,
            ScheduleQuerySort sort = null,
            ScheduleQueryProjection projection = null,
            ScheduleQueryPaging paging = null,
            ScheduleQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ScheduleQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !ScheduleQueryOptions.IsLoadAllAllowed))
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
                    .Where(f => ScheduleQueryProjection.Extras.ContainsKey(f));
                    IEnumerable<ScheduleDetail> details = null;
                    foreach (var key in extraKeys)
                    {
                        switch (key)
                        {
                            case ScheduleQueryProjection.DETAILS:
                                details = multipleResult.Read<ScheduleDetail>().ToList();
                                break;
                        }
                    }
                    ProcessExtras(queryResult, details);
                }
                if (options != null && options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    return new QueryResult<ScheduleQueryRow>
                    {
                        SingleResult = single
                    };
                }
                if (options != null && options.count_total) totalCount = await countTask;
                return new QueryResult<ScheduleQueryRow>
                {
                    Results = queryResult,
                    TotalCount = totalCount
                };
            }
        }

        private ScheduleQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new ScheduleQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case ScheduleQueryProjection.INFO:
                    case ScheduleQueryProjection.SELECT:
                        row.Schedule = objs[i] as Schedule; break;
                    case ScheduleQueryProjection.LOCATION: row.Location = objs[i] as LocationRelationship; break;
                }
            }
            return row;
        }

        private void ProcessExtras(IEnumerable<ScheduleQueryRow> entities,
            IEnumerable<ScheduleDetail> details)
        {
            var contentsMap = details?.GroupBySchedule().ToDictionary(o => o.Key);
            foreach (var e in entities)
            {
                var entity = e.Schedule;
                if (contentsMap != null && contentsMap.ContainsKey(entity.Id))
                    entity.ScheduleDetails = contentsMap[entity.Id].ToList();
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
