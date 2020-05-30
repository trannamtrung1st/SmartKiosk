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
    public class BuildingService : Service
    {
        public BuildingService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query
        public IQueryable<Building> Buildings
        {
            get
            {
                return context.Building;
            }
        }

        public IDictionary<string, object> GetBuildingDynamic(
           BuildingQueryRow row, BuildingQueryProjection projection,
           BuildingQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case BuildingQueryProjection.INFO:
                        {
                            var entity = row.Building;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                            obj["description"] = entity.Description;
                            obj["location_id"] = entity.LocationId;
                        }
                        break;
                    case BuildingQueryProjection.LOCATION:
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
                    case BuildingQueryProjection.SELECT:
                        {
                            var entity = row.Building;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["code"] = entity.Code;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetBuildingDynamic(
            IEnumerable<BuildingQueryRow> rows, BuildingQueryProjection projection,
            BuildingQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetBuildingDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryBuildingDynamic(
            BuildingQueryProjection projection,
            BuildingQueryOptions options,
            BuildingQueryFilter filter = null,
            BuildingQuerySort sort = null,
            BuildingQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = BuildingQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !BuildingQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetBuildingDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetBuildingDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<BuildingQueryRow>> QueryBuilding(
            BuildingQueryFilter filter = null,
            BuildingQuerySort sort = null,
            BuildingQueryProjection projection = null,
            BuildingQueryPaging paging = null,
            BuildingQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = BuildingQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !BuildingQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<BuildingQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<BuildingQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private BuildingQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new BuildingQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case BuildingQueryProjection.INFO:
                    case BuildingQueryProjection.SELECT: 
                        row.Building = objs[i] as Building; break;
                    case BuildingQueryProjection.LOCATION: row.Location = objs[i] as LocationRelationship; break;
                }
            }
            return row;
        }
        #endregion

        #region Create Building
        protected void PrepareCreate(Building entity)
        {
        }

        public Building CreateBuilding(CreateBuildingModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Building.Add(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetBuildings(
            ClaimsPrincipal principal,
            BuildingQueryFilter filter,
            BuildingQuerySort sort,
            BuildingQueryProjection projection,
            BuildingQueryPaging paging,
            BuildingQueryOptions options)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreateBuilding(ClaimsPrincipal principal,
            CreateBuildingModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion
    }
}
