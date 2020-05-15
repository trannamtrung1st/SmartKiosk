using Dapper;
using Microsoft.EntityFrameworkCore;
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
    public class DeviceService : Service
    {
        public DeviceService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Device
        public IDictionary<string, object> CreateDeviceDynamicData(
            DeviceQueryResult entity, DeviceQueryProjection projection,
            DeviceQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case DeviceQueryProjection.INFO:
                        obj["id"] = entity.Id;
                        obj["code"] = entity.Code;
                        obj["name"] = entity.Name;
                        obj["description"] = entity.Description;
                        obj["area_id"] = entity.AreaId;
                        obj["buliding_id"] = entity.BuildingId;
                        obj["floor_id"] = entity.FloorId;
                        obj["location_id"] = entity.LocationId;
                        obj["lat"] = entity.Lat;
                        obj["lon"] = entity.Lon;
                        obj["schedule_id"] = entity.ScheduleId;
                        break;
                    case DeviceQueryProjection.AREA:
                        if (entity.Area != null)
                            obj["area"] = new
                            {
                                id = entity.Area.Id,
                                name = entity.Area.Name,
                                code = entity.Area.Code,
                            };
                        break;
                    case DeviceQueryProjection.FLOOR:
                        if (entity.Floor != null)
                            obj["floor"] = new
                            {
                                id = entity.Floor.Id,
                                name = entity.Floor.Name,
                                code = entity.Floor.Code,
                            };
                        break;
                    case DeviceQueryProjection.BUILDING:
                        if (entity.Building != null)
                        {
                            obj["building"] = new
                            {
                                id = entity.Building.Id,
                                name = entity.Building.Name,
                                code = entity.Building.Code
                            };
                        }
                        break;
                    case DeviceQueryProjection.LOCATION:
                        if (entity.Location != null)
                            obj["location"] = new
                            {
                                id = entity.Location.Id,
                                name = entity.Location.Name,
                                code = entity.Location.Code,
                            };
                        break;
                    case DeviceQueryProjection.ACCOUNT:
                        obj["account"] = new
                        {
                            id = entity.DeviceAccount.Id,
                            username = entity.DeviceAccount.UserName
                        };
                        break;
                    case DeviceQueryProjection.SCHEDULE:
                        if (entity.Schedule != null)
                            obj["schedule"] = new
                            {
                                id = entity.Schedule.Id,
                                name = entity.Schedule.Name,
                                code = entity.Schedule.Code,
                            };
                        break;
                }
            }
            return obj;
        }

        public QueryResult<object> CreateDeviceDynamicData(
            IEnumerable<DeviceQueryResult> entities, DeviceQueryProjection projection,
            DeviceQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in entities)
            {
                var obj = CreateDeviceDynamicData(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<object>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<object> GetDeviceDynamicDataAsync(
            DeviceQueryFilter filter,
            DeviceQuerySort sort,
            DeviceQueryProjection projection,
            DeviceQueryPaging paging,
            DeviceQueryOptions options)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = DeviceQuery.CreateDynamicSql();
            #region General
            query = query.SqlFilter(filter);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options.count_total)
                countQuery = query.SqlCount($"{nameof(Device)}.{nameof(Device.Id)}");
            query = query.SqlProjectFields(projection);
            query = query.SqlJoin(projection, filter);
            #endregion
            if (options.single_only)
            {
                #region Single query
                await openConn;
                var singleTask = conn.QueryAsync<DeviceQueryResult>(
                    sql: query.PreparedForm,
                    param: query.DynamicParameters);
                var single = (await singleTask).SingleOrDefault();
                if (single == null) return null;
                var singleResult = CreateDeviceDynamicData(single, projection, options);
                return singleResult;
                #endregion
            }
            #region List query
            query = query.SqlSort(sort);
            if (!options.load_all || !DeviceQueryOptions.IsLoadAllAllowed)
                query = query.SqlSelectPage(paging.page, paging.limit);
            #endregion
            await openConn;
            #region Count query
            if (options.count_total)
                countTask = conn.ExecuteScalarAsync<int>(
                    sql: countQuery.PreparedForm,
                    param: countQuery.DynamicParameters);
            #endregion
            var listTask = conn.QueryAsync<DeviceQueryResult>(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            if (options.count_total) totalCount = await countTask;
            var list = (await listTask).ToList();
            var result = CreateDeviceDynamicData(list, projection, options, totalCount);
            return result;
        }
        #endregion

    }
}
