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
        public IDictionary<string, object> CreatePostDynamicData(
            Post entity, PostQueryProjection projection,
            PostQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case PostQueryProjection.INFO:
                        obj["id"] = entity.Id;
                        obj["image_url"] = entity.ImageUrl;
                        var time = entity.CreatedTime
                            .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                        var timeStr = time.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                        obj["created_time"] = new
                        {
                            display = timeStr,
                            iso = $"{time.ToUniversalTime():s}Z"
                        };
                        break;
                }
            }
            return obj;
        }

        public IDictionary<string, object> CreatePostDynamicData(
            PostWithContent entity, PostQueryProjection projection,
            PostQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case PostQueryProjection.INFO:
                        obj["id"] = entity.Id;
                        obj["image_url"] = entity.ImageUrl;
                        var time = entity.CreatedTime
                            .ToTimeZone(options.time_zone, options.culture, entity.Lang);
                        var timeStr = time.ToString(options.date_format, options.culture, entity.Lang);
                        obj["created_time"] = new
                        {
                            display = timeStr,
                            iso = $"{time.ToUniversalTime():s}Z"
                        };
                        break;
                    case PostQueryProjection.CONTENT_OVERVIEW:
                        obj["content_id"] = entity.ContentId;
                        obj["lang"] = entity.Lang;
                        obj["title"] = entity.Title;
                        break;
                    case PostQueryProjection.CONTENT_CONTENT:
                        obj["content"] = entity.Content;
                        break;
                }
            }
            return obj;
        }

        public QueryResult<object> CreatePostDynamicData(
            IEnumerable<PostWithContent> entities, PostQueryProjection projection,
            PostQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in entities)
            {
                var obj = CreatePostDynamicData(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<object>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public QueryResult<object> CreatePostDynamicData(
            IEnumerable<Post> entities, PostQueryProjection projection,
            PostQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in entities)
            {
                var obj = CreatePostDynamicData(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<object>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<Return> GetPostDynamicDataAsync<In, Out, Return>(
            PostQueryFilter filter = null,
            PostQuerySort sort = null,
            PostQueryProjection projection = null,
            PostQueryPaging paging = null,
            PostQueryOptions options = null,
            Func<In, Out> projectionExpr = null)
            where Out : class where In : class where Return : class
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = PostQuery.CreateDynamicSql();
            #region General
            query = filter == null ? query : query.SqlFilter(filter);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options != null && options.count_total)
                countQuery = query.SqlCount($"{nameof(Post)}.{nameof(Post.Id)}");
            query = projection == null ? query :
                query.SqlProjectFields(projection).SqlJoin(projection, filter);
            #endregion
            if (options != null && options.single_only)
            {
                #region Single query
                await openConn;
                if (projection != null && projection.fields.Contains(PostQueryProjection.CONTENT))
                {
                    #region Join with content
                    var singleJoinTask = conn.QueryAsync<PostWithContent>(
                        sql: query.PreparedForm,
                        param: query.DynamicParameters);
                    var singleJoin = (await singleJoinTask).SingleOrDefault();
                    if (singleJoin == null) return default;
                    var singleJoinResult = projectionExpr == null ?
                        singleJoin as Out : projectionExpr(singleJoin as In);
                    return singleJoinResult as Return;
                    #endregion
                }
                #region No join
                var singleTask = conn.QueryAsync<Post>(
                    sql: query.PreparedForm,
                    param: query.DynamicParameters);
                var single = (await singleTask).SingleOrDefault();
                if (single == null) return null;
                var singleResult = projectionExpr == null ?
                    single as Out : projectionExpr(single as In);
                return singleResult as Return;
                #endregion
                #endregion
            }
            #region List query
            query = sort == null ? query : query.SqlSort(sort);
            if (options == null || !options.load_all || !PostQueryOptions.IsLoadAllAllowed)
                query = paging == null ? throw new ArgumentNullException(nameof(paging))
                    : query.SqlSelectPage(paging.page, paging.limit);
            #endregion
            await openConn;
            #region Count query
            if (options != null && options.count_total)
                countTask = conn.ExecuteScalarAsync<int>(
                    sql: countQuery.PreparedForm,
                    param: countQuery.DynamicParameters);
            #endregion
            if (projection != null && projection.fields.Contains(PostQueryProjection.CONTENT))
            {
                #region Join with content
                var listJoinTask = conn.QueryAsync<PostWithContent>(
                    sql: query.PreparedForm,
                    param: query.DynamicParameters);
                var listJoin = (await listJoinTask).ToList();
                if (options != null && options.count_total) totalCount = await countTask;
                var resultJoin = new QueryResult<Out>();
                resultJoin.Results = projectionExpr == null ?
                    listJoin.Select(o => o as Out).ToList() :
                    listJoin.Select(o => projectionExpr(o as In)).ToList();
                resultJoin.TotalCount = totalCount;
                return resultJoin as Return;
                #endregion
            }
            #region No join
            var listTask = conn.QueryAsync<Post>(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            if (options.count_total) totalCount = await countTask;
            var list = (await listTask).ToList();
            var result = new QueryResult<Out>();
            result.Results = projectionExpr == null ?
                    list.Select(o => o as Out).ToList() :
                    list.Select(o => projectionExpr(o as In)).ToList();
            result.TotalCount = totalCount;
            return result as Return;
            #endregion
        }

        public async Task<object> GetPostDynamicDataAsync(
            PostQueryFilter filter,
            PostQuerySort sort,
            PostQueryProjection projection,
            PostQueryPaging paging,
            PostQueryOptions options)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = PostQuery.CreateDynamicSql();
            #region General
            query = query.SqlFilter(filter);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options.count_total)
                countQuery = query.SqlCount($"{nameof(Post)}.{nameof(Post.Id)}");
            query = query.SqlProjectFields(projection);
            query = query.SqlJoin(projection, filter);
            #endregion
            if (options.single_only)
            {
                #region Single query
                await openConn;
                if (projection.fields.Contains(PostQueryProjection.CONTENT))
                {
                    #region Join with content
                    var singleJoinTask = conn.QueryAsync<PostWithContent>(
                        sql: query.PreparedForm,
                        param: query.DynamicParameters);
                    var singleJoin = (await singleJoinTask).SingleOrDefault();
                    if (singleJoin == null) return null;
                    var singleJoinResult = CreatePostDynamicData(singleJoin, projection, options);
                    return singleJoinResult;
                    #endregion
                }
                #region No join
                var singleTask = conn.QueryAsync<Post>(
                    sql: query.PreparedForm,
                    param: query.DynamicParameters);
                var single = (await singleTask).SingleOrDefault();
                if (single == null) return null;
                var singleResult = CreatePostDynamicData(single, projection, options);
                return singleResult;
                #endregion
                #endregion
            }
            #region List query
            query = query.SqlSort(sort);
            if (!options.load_all || !PostQueryOptions.IsLoadAllAllowed)
                query = query.SqlSelectPage(paging.page, paging.limit);
            #endregion
            await openConn;
            #region Count query
            if (options.count_total)
                countTask = conn.ExecuteScalarAsync<int>(
                    sql: countQuery.PreparedForm,
                    param: countQuery.DynamicParameters);
            #endregion
            if (projection.fields.Contains(PostQueryProjection.CONTENT))
            {
                #region Join with content
                var listJoinTask = conn.QueryAsync<PostWithContent>(
                    sql: query.PreparedForm,
                    param: query.DynamicParameters);
                var listJoin = (await listJoinTask).ToList();
                if (options.count_total) totalCount = await countTask;
                var resultJoin = CreatePostDynamicData(listJoin, projection, options, totalCount);
                return resultJoin;
                #endregion
            }
            #region No join
            var listTask = conn.QueryAsync<Post>(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            if (options.count_total) totalCount = await countTask;
            var list = (await listTask).ToList();
            var result = CreatePostDynamicData(list, projection, options, totalCount);
            return result;
            #endregion
        }
        #endregion

        #region Create Post
        protected void PrepareCreate(Post entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
        }

        public async Task<Post> CreatePostAsync(CreatePostModel model,
            FileDestinationMetadata metadata = null)
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
