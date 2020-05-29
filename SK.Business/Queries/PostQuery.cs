using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class PostQuery
    {
        public static IQueryable<Post> Id(this IQueryable<Post> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Post> IdOnly(this IQueryable<Post> query)
        {
            return query.Select(o => new Post { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Post> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Post> Ids(this IQueryable<Post> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            PostQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case PostQuerySort.TITLE:
                        {
                            listSorts.Add($"{nameof(PostContent)}" +
                                $".{nameof(PostContent.Title)}{(asc ? "" : " DESC")}");
                        }
                        break;
                    case PostQuerySort.CREATED_TIME:
                        {
                            listSorts.Add($"{nameof(Post)}" +
                                $".{nameof(Post.CreatedTime)}{(asc ? "" : " DESC")}");
                        }
                        break;
                }
            }
            if (listSorts.Any())
            {
                var orderByClause = "ORDER BY " + string.Join(',', listSorts);
                query.SortClause = orderByClause;
            }
            return query;
        }

        public static DynamicSql SqlFilter(
            this DynamicSql query, PostQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Post)}.{nameof(Post.Id)}=@{paramName}");
            }
            if (filter.type != null)
            {
                var paramName = query.AddAutoIncrParam(filter.type);
                listFilters.Add($"{nameof(Post)}.{nameof(Post.Type)}=@{paramName}");
            }
            if (filter.loc_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.loc_id);
                listFilters.Add($"{nameof(Post)}.{nameof(Post.LocationId)}=@{paramName}");
            }
            if (filter.not_eq_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.not_eq_id);
                listFilters.Add($"{nameof(Post)}.{nameof(Post.Id)}!=@{paramName}");
            }
            if (filter.title_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.title_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(PostContent)}" +
                    $".{nameof(PostContent.Title)}) > 0");
            }
            if (filter.ids != null)
            {
                var listDataParams = query.AddAutoIncrSqlInParam(filter.ids);
                listFilters.Add($"{nameof(Post)}.{nameof(Post.Id)} IN ({listDataParams.Placeholder})");
            }
            if (filter.archived != 2)
            {
                var paramName = query.AddAutoIncrParam(filter.archived);
                listFilters.Add($"{nameof(Post)}.{nameof(Post.Archived)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, PostQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => PostQueryProjection.Projections.ContainsKey(f))
                .Select(f => PostQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => PostQueryProjection.Results.ContainsKey(f))
                .Select(f => PostQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, PostQueryProjection model,
            PostQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => PostQueryProjection.Joins.ContainsKey(f))
                .Select(f => PostQueryProjection.Joins[f]);
            if (joins.Any())
            {
                var joinClause = string.Join('\n', joins);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.JOIN, joinClause);
                if (filter != null)
                {
                    var contentFilters = new List<string>();
                    if (filter.lang != null)
                    {
                        var paramName = query.AddAutoIncrParam(filter.lang);
                        var postContentLang = $"{nameof(PostContent)}.{nameof(PostContent.Lang)}";
                        contentFilters.Add($"{postContentLang}=@{paramName}");
                    }
                    if (contentFilters.Any())
                    {
                        var whereClause = "WHERE " + string.Join(" AND ", contentFilters);
                        query.DynamicForm = query.DynamicForm
                            .Replace(PostQueryPlaceholder.POST_CONTENT_FILTER, whereClause);
                    }
                }
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {nameof(Post)} as {nameof(Post)}\n" +
                $"{DynamicSql.JOIN}\n" +
                $"{DynamicSql.FILTER}\n" +
                $"{DynamicSql.GROUP}\n" +
                $"{DynamicSql.SORT}\n" +
                $"{DynamicSql.PAGING}";
            return new DynamicSql
            {
                DynamicForm = sql
            };
        }
        #endregion
    }
}
