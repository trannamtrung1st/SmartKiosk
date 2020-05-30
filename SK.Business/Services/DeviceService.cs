using Dapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        public IQueryable<Device> Devices
        {
            get
            {
                return context.Device;
            }
        }

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
                                    floor_plan_svg = entity.FloorPlanSvg
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

        #region Create Device
        protected void PrepareCreate(Device entity)
        {
        }

        public AppUser MakeDeviceAccount(CreateDeviceModel model)
        {
            var entity = new AppUser
            {
                UserName = model.username,
                PasswordHash = model.password,
                ActivationCode = model.ActivationCode
            };
            return entity;
        }

        public Device CreateDevice(CreateDeviceModel model, string accountId)
        {
            var entity = model.ToDest();
            entity.Id = accountId;
            PrepareCreate(entity);
            return context.Device.Add(entity).Entity;
        }
        #endregion

        #region Update Device
        public void UpdateDevice(Device entity, UpdateDeviceModel model)
        {
            model.CopyTo(entity);
        }
        public void ChangeDeviceToken(Device entity, string newFCMToken, string newAccessToken)
        {
            entity.CurrentFcmToken = newFCMToken;
            entity.AccessToken = newAccessToken;
        }

        public void SetScheduleForDevices(Schedule schedule, IEnumerable<string> deviceIds)
        {
            var entities = Devices.IdOnly().Ids(deviceIds).ToList();
            foreach (var e in entities)
            {
                e.ScheduleId = schedule?.Id;
                context.Entry(e).Property(o => o.ScheduleId).IsModified = true;
            }
        }
        #endregion

        #region Control Device
        public async Task<BatchResponse> TriggerDevicesAsync(TriggerDevicesModel model)
        {
            var messages = model.DeviceIds.Select(id => new Message
            {
                Topic = id,
                Data = new Dictionary<string, string>()
                {
                    { "action", model.Action }
                }
            });
            return await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetDevices(
            ClaimsPrincipal principal,
            DeviceQueryFilter filter,
            DeviceQuerySort sort,
            DeviceQueryProjection projection,
            DeviceQueryPaging paging,
            DeviceQueryOptions options)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreateDevice(ClaimsPrincipal principal,
            CreateDeviceModel model, string validActCode)
        {
            var builder = new AppResultBuilder();
            if (model.ActivationCode != validActCode)
                builder = builder.InvalidActivationCode();
            if (builder.Results.Any())
                return ValidationResult.Fail(builder);
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateUpdateDevice(ClaimsPrincipal principal,
            Device entity, UpdateDeviceModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion

        public string GetActivationCode(string username)
        {
            var bytes = Encoding.ASCII.GetBytes(ActivationCodeSecrect.SECRET);
            string code = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    username,
                    bytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
            return code;
        }
    }
}
