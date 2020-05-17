using Dapper;
using elFinder.NetCore.Drivers;
using Microsoft.EntityFrameworkCore;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class FloorService : Service
    {
        [Inject]
        protected readonly FileService fileService;

        public FloorService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query
        public IQueryable<Floor> Floors
        {
            get
            {
                return context.Floor;
            }
        }

        public IDictionary<string, object> GetFloorDynamic(
           FloorQueryRow row, FloorQueryProjection projection,
           FloorQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case FloorQueryProjection.INFO:
                        {
                            var entity = row.Floor;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                            obj["location_id"] = entity.LocationId;
                            obj["description"] = entity.Description;
                            obj["buidling_id"] = entity.BuildingId;
                        }
                        break;
                    case FloorQueryProjection.FLOOR_PLAN:
                        {
                            var entity = row.Floor;
                            obj["floor_plan_svg"] = entity.FloorPlanSvg;
                        }
                        break;
                    case FloorQueryProjection.SELECT:
                        {
                            var entity = row.Floor;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case FloorQueryProjection.BUILDING:
                        {
                            var entity = row.Building;
                            obj["building"] = new
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

        public QueryResult<IDictionary<string, object>> GetFloorDynamic(
            IEnumerable<FloorQueryRow> rows, FloorQueryProjection projection,
            FloorQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetFloorDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryFloorDynamic(
            FloorQueryProjection projection,
            FloorQueryOptions options,
            FloorQueryFilter filter = null,
            FloorQuerySort sort = null,
            FloorQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = FloorQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !FloorQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetFloorDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetFloorDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<FloorQueryRow>> QueryFloor(
            FloorQueryFilter filter = null,
            FloorQuerySort sort = null,
            FloorQueryProjection projection = null,
            FloorQueryPaging paging = null,
            FloorQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = FloorQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !FloorQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<FloorQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<FloorQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private FloorQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new FloorQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case FloorQueryProjection.INFO:
                    case FloorQueryProjection.SELECT:
                        row.Floor = objs[i] as Floor; break;
                    case FloorQueryProjection.BUILDING: row.Building = objs[i] as BuildingRelationship; break;
                }
            }
            return row;
        }
        #endregion

        #region Create Floor
        protected void PrepareCreate(Floor entity)
        {
        }

        public Floor CreateFloor(CreateFloorModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Floor.Add(entity).Entity;
        }
        #endregion

        #region Update Floor
        public void UpdateFloor(Floor entity, UpdateFloorModel model)
        {
            model.CopyTo(entity);
        }

        public async Task UpdateFloorPlanSvgAsync(Floor entity, UpdateFloorPlanModel model,
            IFile file)
        {
            using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
            {
                var svg = await reader.ReadToEndAsync();
                UpdateFloorPlanSvg(entity, svg);
            }
        }

        public void UpdateFloorPlanSvg(Floor entity, string svg)
        {
            entity.FloorPlanSvg = svg;
        }

        #endregion
    }
}
