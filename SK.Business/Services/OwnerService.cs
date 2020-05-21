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
    public class OwnerService : Service
    {
        public OwnerService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Owner
        public IQueryable<Owner> Owners
        {
            get
            {
                return context.Owner;
            }
        }

        public IDictionary<string, object> GetOwnerDynamic(
            OwnerQueryRow row, OwnerQueryProjection projection,
            OwnerQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case OwnerQueryProjection.INFO:
                        {
                            var entity = row.Owner;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                            obj["phone"] = entity.Phone;
                            obj["description"] = entity.Description;
                            obj["archived"] = entity.Archived;
                        }
                        break;
                    case OwnerQueryProjection.SELECT:
                        {
                            var entity = row.Owner;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetOwnerDynamic(
            IEnumerable<OwnerQueryRow> rows, OwnerQueryProjection projection,
            OwnerQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetOwnerDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryOwnerDynamic(
            OwnerQueryProjection projection,
            OwnerQueryOptions options,
            OwnerQueryFilter filter = null,
            OwnerQuerySort sort = null,
            OwnerQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = OwnerQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !OwnerQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetOwnerDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetOwnerDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<OwnerQueryRow>> QueryOwner(
            OwnerQueryFilter filter = null,
            OwnerQuerySort sort = null,
            OwnerQueryProjection projection = null,
            OwnerQueryPaging paging = null,
            OwnerQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = OwnerQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !OwnerQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<OwnerQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<OwnerQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private OwnerQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new OwnerQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case OwnerQueryProjection.INFO:
                    case OwnerQueryProjection.SELECT:
                        row.Owner = objs[i] as Owner; break;
                }
            }
            return row;
        }
        #endregion

        #region Create Owner
        protected void PrepareCreate(Owner entity)
        {
        }

        public Owner CreateOwner(CreateOwnerModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Owner.Add(entity).Entity;
        }
        #endregion

        #region Update Owner
        public void UpdateOwner(Owner entity, UpdateOwnerModel model)
        {
            model.CopyTo(entity);
        }

        public void ChangeArchivedState(Owner entity, bool archived)
        {
            entity.Archived = archived;
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetOwners(
            ClaimsPrincipal principal,
            OwnerQueryFilter filter,
            OwnerQuerySort sort,
            OwnerQueryProjection projection,
            OwnerQueryPaging paging,
            OwnerQueryOptions options)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreateOwner(ClaimsPrincipal principal,
            CreateOwnerModel model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateUpdateOwner(ClaimsPrincipal principal,
            Owner entity, UpdateOwnerModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion

    }
}
