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
    public class Deviceervice : Service
    {
        public Deviceervice(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Device
        public IDictionary<string, object> GetDeviceDynamic(
            DeviceQueryRow row, DeviceQueryProjection projection,
            DeviceQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case DeviceQueryProjection.INFO:
                        {
                            var entity = row.Device;
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
                        }
                        break;
                    case DeviceQueryProjection.AREA:
                        {
                            var entity = row.Area;
                            if (entity != null)
                                obj["area"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name,
                                    code = entity.Code,
                                };
                        }
                        break;
                    case DeviceQueryProjection.FLOOR:
                        {
                            var entity = row.Floor;
                            if (entity != null)
                                obj["floor"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name,
                                    code = entity.Code,
                                };
                        }
                        break;
                    case DeviceQueryProjection.BUILDING:
                        {
                            var entity = row.Building;
                            if (entity != null)
                                obj["building"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name,
                                    code = entity.Code
                                };
                        }
                        break;
                    case DeviceQueryProjection.LOCATION:
                        {
                            var entity = row.Location;
                            if (entity != null)
                                obj["location"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name,
                                    code = entity.Code,
                                };
                        }
                        break;
                    case DeviceQueryProjection.ACCOUNT:
                        {
                            var entity = row.DeviceAccount;
                            obj["account"] = new
                            {
                                id = entity.Id,
                                username = entity.UserName
                            };
                        }
                        break;
                    case DeviceQueryProjection.SCHEDULE:
                        {
                            var entity = row.Schedule;
                            if (entity != null)
                                obj["schedule"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name,
                                    code = entity.Code,
                                };
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetDeviceDynamic(
            IEnumerable<DeviceQueryRow> rows, DeviceQueryProjection projection,
            DeviceQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetDeviceDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryDeviceDynamic(
            DeviceQueryProjection projection,
            DeviceQueryOptions options,
            DeviceQueryFilter filter = null,
            DeviceQuerySort sort = null,
            DeviceQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = DeviceQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !DeviceQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            var queryResult = await conn.QueryAsync(
                sql: query.PreparedForm,
                types: query.GetTypesArr(),
                map: (objs) => ProcessMultiResults(query, objs),
                splitOn: string.Join(',', query.GetSplitOns()),
                param: query.DynamicParameters);
            if (options.single_only)
            {
                var single = queryResult.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetDeviceDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetDeviceDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<DeviceQueryRow>> QueryDevice(
            DeviceQueryFilter filter = null,
            DeviceQuerySort sort = null,
            DeviceQueryProjection projection = null,
            DeviceQueryPaging paging = null,
            DeviceQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = DeviceQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !DeviceQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            var queryResult = await conn.QueryAsync(
                sql: query.PreparedForm,
                types: query.GetTypesArr(),
                map: (objs) => ProcessMultiResults(query, objs),
                splitOn: string.Join(',', query.GetSplitOns()),
                param: query.DynamicParameters);
            if (options != null && options.single_only)
            {
                var single = queryResult.SingleOrDefault();
                return new QueryResult<DeviceQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<DeviceQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private DeviceQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new DeviceQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case DeviceQueryProjection.INFO: row.Device = objs[i] as Device; break;
                    case DeviceQueryProjection.ACCOUNT: row.DeviceAccount = objs[i] as AppUserRelationship; break;
                    case DeviceQueryProjection.AREA: row.Area = objs[i] as AreaRelationship; break;
                    case DeviceQueryProjection.BUILDING: row.Building = objs[i] as BuildingRelationship; break;
                    case DeviceQueryProjection.FLOOR: row.Floor = objs[i] as FloorRelationship; break;
                    case DeviceQueryProjection.LOCATION: row.Location = objs[i] as LocationRelationship; break;
                    case DeviceQueryProjection.SCHEDULE: row.Schedule = objs[i] as ScheduleRelationship; break;
                }
            }
            return row;
        }
        #endregion

        #region Update Device
        public void UpdateDevice(Device entity, UpdateDeviceModel model)
        {
            model.CopyTo(entity);
        }
        #endregion

    }
}
