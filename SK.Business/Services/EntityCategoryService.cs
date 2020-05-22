using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.Data;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class EntityCategoryService : Service
    {
        public EntityCategoryService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query EntityCategory
        public IQueryable<EntityCategory> EntityCategories
        {
            get
            {
                return context.EntityCategory;
            }
        }

        public IQueryable<EntityCategoryContent> EntityCategoryContents
        {
            get
            {
                return context.EntityCategoryContent;
            }
        }

        public IDictionary<string, object> GetEntityCategoryDynamic(
            EntityCategoryQueryRow row, EntityCategoryQueryProjection projection,
            EntityCategoryQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case EntityCategoryQueryProjection.INFO:
                        {
                            var entity = row.EntityCategory;
                            obj["id"] = entity.Id;
                            obj["archived"] = entity.Archived;
                        }
                        break;
                    case EntityCategoryQueryProjection.CONTENT:
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

        public QueryResult<IDictionary<string, object>> GetEntityCategoryDynamic(
            IEnumerable<EntityCategoryQueryRow> rows, EntityCategoryQueryProjection projection,
            EntityCategoryQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetEntityCategoryDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryEntityCategoryDynamic(
            EntityCategoryQueryProjection projection,
            EntityCategoryQueryOptions options,
            EntityCategoryQueryFilter filter = null,
            EntityCategoryQuerySort sort = null,
            EntityCategoryQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = EntityCategoryQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !EntityCategoryQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetEntityCategoryDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetEntityCategoryDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<EntityCategoryQueryRow>> QueryEntityCategory(
            EntityCategoryQueryFilter filter = null,
            EntityCategoryQuerySort sort = null,
            EntityCategoryQueryProjection projection = null,
            EntityCategoryQueryPaging paging = null,
            EntityCategoryQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = EntityCategoryQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !EntityCategoryQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<EntityCategoryQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<EntityCategoryQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private EntityCategoryQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new EntityCategoryQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case EntityCategoryQueryProjection.INFO: row.EntityCategory = objs[i] as EntityCategory; break;
                    case EntityCategoryQueryProjection.CONTENT: row.Content = objs[i] as EntityCategoryContentRelationship; break;
                }
            }
            return row;
        }
        #endregion


        #region Create EntityCategory
        protected void PrepareCreate(EntityCategory entity)
        {
        }

        public EntityCategory CreateEntityCategory(CreateEntityCategoryModel model,
            FileDestinationMetadata metadata = null)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.EntityCategory.Add(entity).Entity;
        }
        #endregion

        #region Update EntityCategory
        private void CreateEntityCategoryContents(IList<CreateEntityCategoryContentModel> model, EntityCategory entity)
        {
            var entities = model.Select(o =>
            {
                var content = o.ToDest();
                content.CategoryId = entity.Id;
                return content;
            }).ToList();
            context.EntityCategoryContent.AddRange(entities);
        }

        private void UpdateEntityCategoryContents(
            IList<UpdateEntityCategoryContentModel> model, int refId)
        {
            foreach (var o in model)
            {
                var entity = EntityCategoryContents.ByLang(o.Lang)
                    .OfCategory(refId).IdOnly().FirstOrDefault();
                //not found => null => exception
                o.CopyTo(entity);
                var entry = context.Entry(entity);
                entry.Property(nameof(UpdateEntityCategoryContentModel.Name)).IsModified = true;
            }
        }

        public async Task UpdateEntityCategoryTransactionAsync(EntityCategory entity,
            UpdateEntityCategoryModel model)
        {
            if (model.NewEntityCategoryContents != null)
                CreateEntityCategoryContents(model.NewEntityCategoryContents, entity);
            if (model.UpdateEntityCategoryContents != null)
                UpdateEntityCategoryContents(model.UpdateEntityCategoryContents, entity.Id);
            if (model.DeleteEntityCategoryContentLangs != null)
                await DeleteContentsOfCategoryAsync(model.DeleteEntityCategoryContentLangs, entity.Id);
        }

        public void ChangeArchivedState(EntityCategory entity, bool archived)
        {
            entity.Archived = archived;
        }
        #endregion

        #region Delete EntityCategory
        protected async Task<int> DeleteEntityCategoryContentByIdsAsync(IEnumerable<int> ids)
        {
            var parameters = ids.GetDataParameters("id");
            var sql = $"DELETE FROM {nameof(EntityCategoryContent)} WHERE " +
                $"{nameof(EntityCategoryContent.Id)} IN " +
                $"({parameters.Placeholder})";
            var sqlParams = parameters.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value));
            var result = await context.Database
                .ExecuteSqlRawAsync(sql, sqlParams);
            return result;
        }

        protected async Task<int> DeleteContentsOfCategoryAsync(IEnumerable<string> langs, int refId)
        {
            var parameters = langs.GetDataParameters("lang");
            var refIdParam = new SqlParameter("refId", refId);
            var sql = $"DELETE FROM {nameof(EntityCategoryContent)} WHERE " +
                $"{nameof(EntityCategoryContent.Lang)} IN " +
                $"({parameters.Placeholder}) AND {nameof(EntityCategoryContent.CategoryId)}=@{refIdParam.ParameterName}";
            var sqlParams = parameters.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value))
                .ToList();
            sqlParams.Add(refIdParam);
            var result = await context.Database
                .ExecuteSqlRawAsync(sql, sqlParams);
            return result;
        }

        protected async Task<int> DeleteAllContentsOfEntityCategoryAsync(EntityCategory entity)
        {
            var id = new SqlParameter("id", entity.Id);
            var sql = $"DELETE FROM {nameof(EntityCategoryContent)} WHERE " +
                $"{nameof(EntityCategoryContent.CategoryId)}=@{id.ParameterName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql, id);
            return result;
        }

        public async Task<EntityCategory> DeleteEntityCategoryTransactionAsync(EntityCategory entity)
        {
            await DeleteAllContentsOfEntityCategoryAsync(entity);
            return context.EntityCategory.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetEntityCategories(
            ClaimsPrincipal principal,
            EntityCategoryQueryFilter filter,
            EntityCategoryQuerySort sort,
            EntityCategoryQueryProjection projection,
            EntityCategoryQueryPaging paging,
            EntityCategoryQueryOptions options)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreateEntityCategory(ClaimsPrincipal principal,
            CreateEntityCategoryModel model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateUpdateEntityCategory(ClaimsPrincipal principal,
            EntityCategory entity, UpdateEntityCategoryModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion


    }
}
