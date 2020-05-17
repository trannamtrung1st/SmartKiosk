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
    public class AreaService : Service
    {
        public AreaService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Area
        public IQueryable<Area> Areas
        {
            get
            {
                return context.Area;
            }
        }

        public IDictionary<string, object> GetAreaDynamic(
           AreaQueryRow row, AreaQueryProjection projection,
           AreaQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AreaQueryProjection.INFO:
                        {
                            var entity = row.Area;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["description"] = entity.Description;
                            obj["floor_id"] = entity.FloorId;
                            obj["building_id"] = entity.BuildingId;
                            obj["location_id"] = entity.LocationId;
                            obj["archived"] = entity.Archived;
                            obj["code"] = entity.Code;
                        }
                        break;
                    case AreaQueryProjection.SELECT:
                        {
                            var entity = row.Area;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["code"] = entity.Code;
                        }
                        break;
                    case AreaQueryProjection.FLOOR:
                        {
                            var entity = row.Floor;
                            obj["floor"] = new
                            {
                                id = entity.Id,
                                name = entity.Name,
                                code = entity.Code,
                            };
                        }
                        break;
                    case AreaQueryProjection.BUILDING:
                        {
                            var entity = row.Building;
                            obj["building"] = new
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

        public QueryResult<IDictionary<string, object>> GetAreaDynamic(
            IEnumerable<AreaQueryRow> rows, AreaQueryProjection projection,
            AreaQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetAreaDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAreaDynamic(
            AreaQueryProjection projection,
            AreaQueryOptions options,
            AreaQueryFilter filter = null,
            AreaQuerySort sort = null,
            AreaQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = AreaQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !AreaQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetAreaDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetAreaDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<AreaQueryRow>> QueryArea(
            AreaQueryFilter filter = null,
            AreaQuerySort sort = null,
            AreaQueryProjection projection = null,
            AreaQueryPaging paging = null,
            AreaQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = AreaQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !AreaQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<AreaQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<AreaQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private AreaQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new AreaQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case AreaQueryProjection.INFO:
                    case AreaQueryProjection.SELECT:
                        row.Area = objs[i] as Area; break;
                    case AreaQueryProjection.FLOOR: row.Floor = objs[i] as FloorRelationship; break;
                    case AreaQueryProjection.BUILDING: row.Building = objs[i] as BuildingRelationship; break;
                }
            }
            return row;
        }
        #endregion

        #region Create Area
        protected void PrepareCreate(Area entity)
        {
        }

        public Area CreateArea(CreateAreaModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Area.Add(entity).Entity;
        }
        #endregion

        #region Update Area
        public void UpdateArea(Area entity, UpdateAreaModel model)
        {
            model.CopyTo(entity);
        }
        #endregion
    }
}
