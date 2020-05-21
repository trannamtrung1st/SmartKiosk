using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NLog.Fluent;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.Data;
using TNT.Core.Helpers.DI;
using static Dapper.SqlMapper;

namespace SK.Business.Services
{
    public class ResourceService : Service
    {
        [Inject]
        private readonly FileService _fileService;

        public ResourceService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Resource
        public IQueryable<Resource> Resources
        {
            get
            {
                return context.Resource;
            }
        }

        public IQueryable<ResourceContent> ResourceContents
        {
            get
            {
                return context.ResourceContent;
            }
        }

        public IDictionary<string, object> GetResourceDynamic(
            ResourceQueryRow row, ResourceQueryProjection projection,
            ResourceQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ResourceQueryProjection.INFO:
                        {
                            var entity = row.Resource;
                            obj["archived"] = entity.Archived;
                            obj["area_id"] = entity.AreaId;
                            obj["building_id"] = entity.BuildingId;
                            obj["code"] = entity.Code;
                            obj["floor_id"] = entity.FloorId;
                            obj["id"] = entity.Id;
                            obj["image_url"] = entity.ImageUrl;
                            obj["location_id"] = entity.LocationId;
                            obj["logo_url"] = entity.LogoUrl;
                            obj["owner_id"] = entity.OwnerId;
                            obj["phone"] = entity.Phone;
                            obj["type_id"] = entity.TypeId;
                        }
                        break;
                    case ResourceQueryProjection.CONTENT:
                        {
                            var entity = row.Content;
                            obj["content_id"] = entity.Id;
                            obj["lang"] = entity.Lang;
                            obj["name"] = entity.Name;
                            obj["description"] = entity.Description;
                        }
                        break;
                    case ResourceQueryProjection.CONTENT_CONTENT:
                        {
                            var entity = row.Content;
                            obj["content"] = entity.Content;
                        }
                        break;
                    case ResourceQueryProjection.LOCATION:
                        {
                            var entity = row.Location;
                            obj["location"] = new
                            {
                                id = entity.Id,
                                code = entity.Code,
                                name = entity.Name,
                            };
                        }
                        break;
                    case ResourceQueryProjection.AREA:
                        {
                            var entity = row.Area;
                            obj["area"] = new
                            {
                                id = entity.Id,
                                code = entity.Code,
                                name = entity.Name,
                            };
                        }
                        break;
                    case ResourceQueryProjection.OWNER:
                        {
                            var entity = row.Owner;
                            obj["owner"] = new
                            {
                                id = entity.Id,
                                code = entity.Code,
                                name = entity.Name,
                            };
                        }
                        break;
                    case ResourceQueryProjection.CATEGORIES:
                        {
                            var entities = row.Resource.CategoriesOfResources;
                            obj["categories"] = entities.Select(o =>
                            {
                                var cate = o.Category;
                                var content = o.Content;
                                return new
                                {
                                    id = cate.Id,
                                    archived = cate.Archived,
                                    content_id = content.Id,
                                    name = content.Name,
                                    lang = content.Lang
                                };
                            }).ToList();
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetResourceDynamic(
            IEnumerable<ResourceQueryRow> rows, ResourceQueryProjection projection,
            ResourceQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetResourceDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryResourceDynamic(
            ResourceQueryProjection projection,
            ResourceQueryOptions options,
            ResourceQueryFilter filter = null,
            ResourceQuerySort sort = null,
            ResourceQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ResourceQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !ResourceQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            query = query.SqlExtras(projection, filter);
            var multipleResult = await conn.QueryMultipleAsync(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            using (multipleResult)
            {
                var queryResult = multipleResult.Read(
                    types: query.GetTypesArr(),
                    map: (objs) => ProcessMultiResults(query, objs),
                    splitOn: string.Join(',', query.GetSplitOns()));
                var extraKeys = projection.GetFieldsArr()
                    .Where(f => ResourceQueryProjection.Extras.ContainsKey(f));
                IEnumerable<CateOfResQueryRow> categories = null;
                foreach (var key in extraKeys)
                {
                    switch (key)
                    {
                        case ResourceQueryProjection.CATEGORIES:
                            categories = GetCategoriesQueryResult(multipleResult);
                            break;
                    }
                }
                ProcessExtras(queryResult, categories);
                if (options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    var singleResult = GetResourceDynamic(single, projection, options);
                    return new QueryResult<IDictionary<string, object>>()
                    {
                        SingleResult = singleResult
                    };
                }
                if (options.count_total) totalCount = await countTask;
                var result = GetResourceDynamic(queryResult, projection, options, totalCount);
                return result;
            }
        }

        public async Task<QueryResult<ResourceQueryRow>> QueryResource(
            ResourceQueryFilter filter = null,
            ResourceQuerySort sort = null,
            ResourceQueryProjection projection = null,
            ResourceQueryPaging paging = null,
            ResourceQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ResourceQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !ResourceQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            if (projection != null) query = query.SqlExtras(projection, filter);
            var multipleResult = await conn.QueryMultipleAsync(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            using (multipleResult)
            {
                var queryResult = multipleResult.Read(
                    types: query.GetTypesArr(),
                    map: (objs) => ProcessMultiResults(query, objs),
                    splitOn: string.Join(',', query.GetSplitOns()));
                if (projection != null)
                {
                    var extraKeys = projection.GetFieldsArr()
                        .Where(f => ResourceQueryProjection.Extras.ContainsKey(f));
                    IEnumerable<CateOfResQueryRow> categories = null;
                    foreach (var key in extraKeys)
                    {
                        switch (key)
                        {
                            case ResourceQueryProjection.CATEGORIES:
                                categories = GetCategoriesQueryResult(multipleResult);
                                break;
                        }
                    }
                    ProcessExtras(queryResult, categories);
                }
                if (options != null && options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    return new QueryResult<ResourceQueryRow>
                    {
                        SingleResult = single
                    };
                }
                if (options != null && options.count_total) totalCount = await countTask;
                return new QueryResult<ResourceQueryRow>
                {
                    Results = queryResult,
                    TotalCount = totalCount
                };
            }
        }

        private ResourceQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new ResourceQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case ResourceQueryProjection.INFO: row.Resource = objs[i] as ResourceQueryResult; break;
                    case ResourceQueryProjection.AREA: row.Area = objs[i] as AreaRelationship; break;
                    case ResourceQueryProjection.CONTENT: row.Content = objs[i] as ResourceContentRelationship; break;
                    case ResourceQueryProjection.OWNER: row.Owner = objs[i] as OwnerRelationship; break;
                    case ResourceQueryProjection.LOCATION: row.Location = objs[i] as LocationRelationship; break;
                }
            }
            return row;
        }

        private IEnumerable<CateOfResQueryRow> GetCategoriesQueryResult(GridReader multipleResult)
        {
            var rows = multipleResult.Read(
                types: new[] { typeof(CategoriesOfResources), typeof(EntityCategoryRelationship), typeof(EntityCategoryContentRelationship) },
                map: (objs) =>
                {
                    var row = new CateOfResQueryRow();
                    row.CateOfRes = objs[0] as CategoriesOfResources;
                    row.Category = objs[1] as EntityCategoryRelationship;
                    row.Content = objs[2] as EntityCategoryContentRelationship;
                    return row;
                }, splitOn: $"{nameof(EntityCategory)}.{nameof(EntityCategory.Id)}," +
                $"{nameof(EntityCategoryContent)}.{nameof(EntityCategoryContent.Id)}")
                .ToList();
            return rows;
        }

        private void ProcessExtras(IEnumerable<ResourceQueryRow> entities,
            IEnumerable<CateOfResQueryRow> categories)
        {
            var contentMaps = categories?.GroupByResource().ToDictionary(o => o.Key);
            foreach (var e in entities)
            {
                var entity = e.Resource;
                if (contentMaps != null && contentMaps.ContainsKey(entity.Id))
                    entity.CategoriesOfResources = contentMaps[entity.Id].ToList();
            }
        }
        #endregion

        #region Create Resource
        protected void PrepareCreate(Resource entity)
        {
        }

        public async Task<Resource> CreateResourceAsync(CreateResourceModel model,
            FileDestinationMetadata metadata)
        {
            var entity = model.ToDest();
            if (model.Image != null)
                await SetResourceImageUrlAsync(entity, model.Image, metadata);
            if (model.Logo != null)
                await SetResourceImageUrlAsync(entity, model.Logo, metadata);
            AddResourceCategoriesToResource(model.CategoryIds, entity);
            PrepareCreate(entity);
            return context.Resource.Add(entity).Entity;
        }
        #endregion

        #region Update Resource
        private void CreateResourceContents(IList<CreateResourceContentModel> model, Resource entity)
        {
            var entities = model.Select(o =>
            {
                var content = o.ToDest();
                content.ResourceId = entity.Id;
                return content;
            }).ToList();
            context.ResourceContent.AddRange(entities);
        }

        private void UpdateResourceContents(IList<UpdateResourceContentModel> model, int refId)
        {
            foreach (var o in model)
            {
                var entity = ResourceContents.ByLang(o.Lang)
                    .OfResource(refId).IdOnly().FirstOrDefault();
                //not found => null => exception
                o.CopyTo(entity);
                var entry = context.Entry(entity);
                entry.Property(nameof(UpdateResourceContentModel.Content)).IsModified = true;
                entry.Property(nameof(UpdateResourceContentModel.Description)).IsModified = true;
                entry.Property(nameof(UpdateResourceContentModel.Name)).IsModified = true;
            }
        }

        private void AddResourceCategoriesToResource(IList<int> ids, Resource entity)
        {
            var categories = ids.Select(id => new CategoriesOfResources { CategoryId = id })
                .ToList();
            entity.CategoriesOfResources = categories;
        }

        public async Task UpdateResourceTransactionAsync(Resource entity,
            UpdateResourceModel model,
            FileDestinationMetadata metadata = null)
        {
            if (model.NewResourceContents != null)
                CreateResourceContents(model.NewResourceContents, entity);
            if (model.UpdateResourceContents != null)
                UpdateResourceContents(model.UpdateResourceContents, entity.Id);
            if (model.DeleteResourceContentLangs != null)
                await DeleteContentsOfResourceAsync(model.DeleteResourceContentLangs, entity.Id);
            if (model.CategoryIds != null)
            {
                await DeleteAllCategoriesOfResourceAsync(entity);
                AddResourceCategoriesToResource(model.CategoryIds, entity);
            }
            if (model.Image != null)
                await SetResourceImageUrlAsync(entity, model.Image, metadata);
            if (model.Logo != null)
                await SetResourceLogoUrlAsync(entity, model.Logo, metadata);
        }
        #endregion

        #region Delete Resource
        protected async Task<int> DeleteResourceContentByIdsAsync(IEnumerable<int> ids)
        {
            var parameters = ids.GetDataParameters("id");
            var sql = $"DELETE FROM {nameof(ResourceContent)} WHERE " +
                $"{nameof(ResourceContent.Id)} IN " +
                $"({parameters.Placeholder})";
            var sqlParams = parameters.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value));
            var result = await context.Database
                .ExecuteSqlRawAsync(sql, sqlParams);
            return result;
        }

        protected async Task<int> DeleteContentsOfResourceAsync(IEnumerable<string> langs, int refId)
        {
            var parameters = langs.GetDataParameters("lang");
            var refIdParam = new SqlParameter("refId", refId);
            var sql = $"DELETE FROM {nameof(ResourceContent)} WHERE " +
                $"{nameof(ResourceContent.Lang)} IN " +
                $"({parameters.Placeholder}) AND {nameof(ResourceContent.ResourceId)}=@{refIdParam.ParameterName}";
            var sqlParams = parameters.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value))
                .ToList();
            sqlParams.Add(refIdParam);
            var result = await context.Database
                .ExecuteSqlRawAsync(sql, sqlParams);
            return result;
        }

        protected async Task<int> DeleteAllContentsOfResourceAsync(Resource entity)
        {
            var id = new SqlParameter("id", entity.Id);
            var sql = $"DELETE FROM {nameof(ResourceContent)} WHERE " +
                $"{nameof(ResourceContent.ResourceId)}=@{id.ParameterName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql, id);
            return result;
        }

        protected async Task<int> DeleteAllCategoriesOfResourceAsync(Resource entity)
        {
            var id = new SqlParameter("id", entity.Id);
            var sql = $"DELETE FROM {nameof(CategoriesOfResources)} WHERE " +
                $"{nameof(CategoriesOfResources.ResourceId)}=@{id.ParameterName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql, id);
            return result;
        }

        public async Task<Resource> DeleteResourceTransactionAsync(Resource entity)
        {
            await DeleteAllContentsOfResourceAsync(entity);
            return context.Resource.Remove(entity).Entity;
        }
        #endregion

        protected async Task SetResourceImageUrlAsync(Resource entity,
            FileDestination image,
            FileDestinationMetadata metadata)
        {
            var imageUrl = await _fileService.GetFileUrlAsync(image, metadata);
            entity.ImageUrl = imageUrl;
        }

        protected async Task SetResourceLogoUrlAsync(Resource entity,
            FileDestination logo,
            FileDestinationMetadata metadata)
        {
            var logoUrl = await _fileService.GetFileUrlAsync(logo, metadata);
            entity.LogoUrl = logoUrl;
        }

    }
}
