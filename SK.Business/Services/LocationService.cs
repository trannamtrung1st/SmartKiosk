using Dapper;
using Microsoft.EntityFrameworkCore;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class LocationService : Service
    {
        public LocationService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Location
        public IQueryable<Location> Locations
        {
            get
            {
                return context.Location;
            }
        }

        public IDictionary<string, object> GetLocationDynamic(
            LocationQueryRow row, LocationQueryProjection projection,
            LocationQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case LocationQueryProjection.INFO:
                        {
                            var entity = row.Location;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                            obj["address"] = entity.Address;
                            obj["description"] = entity.Description;
                            obj["archived"] = entity.Archived;
                        }
                        break;
                    case LocationQueryProjection.SELECT:
                        {
                            var entity = row.Location;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetLocationDynamic(
            IEnumerable<LocationQueryRow> rows, LocationQueryProjection projection,
            LocationQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetLocationDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryLocationDynamic(
            LocationQueryProjection projection,
            LocationQueryOptions options,
            LocationQueryFilter filter = null,
            LocationQuerySort sort = null,
            LocationQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = LocationQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !LocationQueryOptions.IsLoadAllAllowed))
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
                var single = queryResult.FirstOrDefault();
                if (single == null) return null;
                var singleResult = GetLocationDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetLocationDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<LocationQueryRow>> QueryLocation(
            LocationQueryFilter filter = null,
            LocationQuerySort sort = null,
            LocationQueryProjection projection = null,
            LocationQueryPaging paging = null,
            LocationQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = LocationQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !LocationQueryOptions.IsLoadAllAllowed))
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
                var single = queryResult.FirstOrDefault();
                return new QueryResult<LocationQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<LocationQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private LocationQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new LocationQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case LocationQueryProjection.INFO:
                    case LocationQueryProjection.SELECT:
                        row.Location = objs[i] as Location; break;
                }
            }
            return row;
        }
        #endregion

        #region Create Location
        protected void PrepareCreate(Location entity)
        {
        }

        public Location CreateLocation(CreateLocationModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Location.Add(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetLocations(
            ClaimsPrincipal principal,
            LocationQueryFilter filter,
            LocationQuerySort sort,
            LocationQueryProjection projection,
            LocationQueryPaging paging,
            LocationQueryOptions options)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreateLocation(ClaimsPrincipal principal,
            CreateLocationModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion
    }
}
