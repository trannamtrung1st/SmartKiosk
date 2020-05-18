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
    public class ResourceTypeService : Service
    {
        public ResourceTypeService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query ResourceType
        public IQueryable<ResourceType> ResourceTypes
        {
            get
            {
                return context.ResourceType;
            }
        }

        public IQueryable<ResourceTypeContent> ResourceTypeContents
        {
            get
            {
                return context.ResourceTypeContent;
            }
        }

        public IDictionary<string, object> GetResourceTypeDynamic(
            ResourceTypeQueryRow row, ResourceTypeQueryProjection projection,
            ResourceTypeQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ResourceTypeQueryProjection.INFO:
                        {
                            var entity = row.ResourceType;
                            obj["id"] = entity.Id;
                            obj["archived"] = entity.Archived;
                        }
                        break;
                    case ResourceTypeQueryProjection.CONTENT:
                        {
                            var entity = row.Content;
                            obj["content_id"] = entity.Id;
                            obj["lang"] = entity.Lang;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetResourceTypeDynamic(
            IEnumerable<ResourceTypeQueryRow> rows, ResourceTypeQueryProjection projection,
            ResourceTypeQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetResourceTypeDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryResourceTypeDynamic(
            ResourceTypeQueryProjection projection,
            ResourceTypeQueryOptions options,
            ResourceTypeQueryFilter filter = null,
            ResourceTypeQuerySort sort = null,
            ResourceTypeQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ResourceTypeQuery.CreateDynamicSql();
            #region General
            if (filter != null) query = query.SqlFilter(filter);
            query = query.SqlJoin(projection, filter);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options.count_total) countQuery = query.SqlCount("*");
            query = query.SqlProjectFields(projection);
            #endregion
            await openConn;
            if (!options.single_only)
            {
                #region List query
                if (sort != null) query = query.SqlSort(sort);
                if (paging != null && (!options.load_all || !ResourceTypeQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetResourceTypeDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetResourceTypeDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<ResourceTypeQueryRow>> QueryResourceType(
            ResourceTypeQueryFilter filter = null,
            ResourceTypeQuerySort sort = null,
            ResourceTypeQueryProjection projection = null,
            ResourceTypeQueryPaging paging = null,
            ResourceTypeQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ResourceTypeQuery.CreateDynamicSql();
            #region General
            if (filter != null) query = query.SqlFilter(filter);
            if (projection != null) query = query.SqlJoin(projection, filter);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options != null && options.count_total) countQuery = query.SqlCount("*");
            if (projection != null) query = query.SqlProjectFields(projection);
            #endregion
            await openConn;
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.SqlSort(sort);
                if (paging != null && (!options.load_all || !ResourceTypeQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<ResourceTypeQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<ResourceTypeQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private ResourceTypeQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new ResourceTypeQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case ResourceTypeQueryProjection.INFO: row.ResourceType = objs[i] as ResourceType; break;
                    case ResourceTypeQueryProjection.CONTENT: row.Content = objs[i] as ResourceTypeContentRelationship; break;
                }
            }
            return row;
        }
        #endregion

    }
}
