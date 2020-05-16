using Dapper;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SK.Business.Helpers;
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
    public class PostService : Service
    {
        [Inject]
        private readonly FileService _fileService;

        public PostService(ServiceInjection inj) : base(inj)
        {
        }

        public IQueryable<Post> Posts
        {
            get
            {
                return context.Post;
            }
        }

        public IQueryable<PostContent> PostContents
        {
            get
            {
                return context.PostContent;
            }
        }

        #region Query Post
        public IDictionary<string, object> GetPostDynamic(
            PostQueryRow row, PostQueryProjection projection,
            PostQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case PostQueryProjection.INFO:
                        {
                            var entity = row.Post;
                            var content = row.Content;
                            obj["id"] = entity.Id;
                            obj["image_url"] = entity.ImageUrl;
                            var time = entity.CreatedTime
                                .ToTimeZone(options.time_zone, options.culture, content?.Lang);
                            var timeStr = time.ToString(options.date_format, options.culture, content?.Lang);
                            obj["created_time"] = new
                            {
                                display = timeStr,
                                iso = $"{time.ToUniversalTime():s}Z"
                            };
                        }
                        break;
                    case PostQueryProjection.CONTENT:
                        {
                            var entity = row.Content;
                            obj["content_id"] = entity.Id;
                            obj["lang"] = entity.Lang;
                            obj["title"] = entity.Title;
                        }
                        break;
                    case PostQueryProjection.CONTENT_CONTENT:
                        {
                            var entity = row.Content;
                            obj["content"] = entity.Content;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetPostDynamic(
            IEnumerable<PostQueryRow> rows, PostQueryProjection projection,
            PostQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetPostDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryPostDynamic(
            PostQueryProjection projection,
            PostQueryOptions options,
            PostQueryFilter filter = null,
            PostQuerySort sort = null,
            PostQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = PostQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !PostQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetPostDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetPostDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<PostQueryRow>> QueryPost(
            PostQueryFilter filter = null,
            PostQuerySort sort = null,
            PostQueryProjection projection = null,
            PostQueryPaging paging = null,
            PostQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = PostQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !PostQueryOptions.IsLoadAllAllowed))
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
                return new QueryResult<PostQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<PostQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private PostQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new PostQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case PostQueryProjection.INFO: row.Post = objs[i] as Post; break;
                    case PostQueryProjection.CONTENT: row.Content = objs[i] as PostContentRelationship; break;
                }
            }
            return row;
        }
        #endregion

        #region Create Post
        protected void PrepareCreate(Post entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
        }

        public async Task<Post> CreatePostAsync(CreatePostModel model,
            FileDestinationMetadata metadata = null, string userId = null)
        {
            var entity = model.ToDest();
            if (model.Image != null)
                await SetPostImageUrlAsync(entity, model.Image, metadata);
            PrepareCreate(entity);
            return context.Post.Add(entity).Entity;
        }

        public async Task CreatePostContentsAsync(IList<PostContent> entities,
            TransDbContextOptions transOpt = null)
        {
            if (transOpt != null)
            {
                using (var context = new DataContext(transOpt.Options))
                {
                    context.Database.UseTransaction(transOpt.Transaction);
                    await context.BulkInsertAsync(entities);
                }
            }
            else
            {
                await context.BulkInsertAsync(entities);
            }
        }
        #endregion

        #region Update Post
        private void CreatePostContents(IList<CreatePostContentModel> model, Post entity)
        {
            var entities = model.Select(o =>
            {
                var content = o.ToDest();
                content.PostId = entity.Id;
                return content;
            }).ToList();
            context.PostContent.AddRange(entities);
        }

        private void UpdatePostContents(IList<UpdatePostContentModel> model)
        {
            foreach (var o in model)
            {
                var entity = new PostContent();
                entity.Id = o.Id;
                context.Attach(entity);
                o.CopyTo(entity);
            }
        }

        public async Task UpdatePostTransactionAsync(Post entity,
            UpdatePostModel model,
            FileDestinationMetadata metadata = null)
        {
            if (model.NewPostContents != null)
                CreatePostContents(model.NewPostContents, entity);
            if (model.UpdatePostContents != null)
                UpdatePostContents(model.UpdatePostContents);
            if (model.DeletePostContentIds != null)
                await DeletePostContentByIdsAsync(model.DeletePostContentIds);
            if (model.Image != null)
                await SetPostImageUrlAsync(entity, model.Image, metadata);
        }
        #endregion

        #region Delete Post
        protected async Task<int> DeletePostContentByIdsAsync(IEnumerable<int> ids)
        {
            var parameters = ids.GetDataParameters("id");
            var sql = $"DELETE FROM {nameof(PostContent)} WHERE " +
                $"{nameof(PostContent.Id)} IN " +
                $"({parameters.Placeholder})";
            var sqlParams = parameters.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value));
            var result = await context.Database
                .ExecuteSqlRawAsync(sql, sqlParams);
            return result;
        }

        protected async Task<int> DeleteAllContentsOfPost(Post entity)
        {
            var sql = $"DELETE FROM {nameof(PostContent)} WHERE " +
                $"{nameof(PostContent.PostId)}={entity.Id}";
            var result = await context.Database.ExecuteSqlRawAsync(sql);
            return result;
        }

        public async Task<Post> DeletePostTransactionAsync(Post entity)
        {
            await DeleteAllContentsOfPost(entity);
            return context.Post.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetPosts(
            ClaimsPrincipal principal,
            PostQueryFilter filter,
            PostQuerySort sort,
            PostQueryProjection projection,
            PostQueryPaging paging,
            PostQueryOptions options)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateDeletePost(ClaimsPrincipal principal,
            Post model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreatePost(ClaimsPrincipal principal,
            CreatePostModel model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateUpdatePost(ClaimsPrincipal principal,
            Post entity, UpdatePostModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion

        protected async Task SetPostImageUrlAsync(Post entity, FileDestination image,
            FileDestinationMetadata metadata)
        {
            var imageUrl = await _fileService.GetFileUrlAsync(image, metadata);
            entity.ImageUrl = imageUrl;
        }

    }
}
