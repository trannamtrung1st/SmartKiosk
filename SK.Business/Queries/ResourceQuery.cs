using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ResourceQuery
    {
        public static IQueryable<Resource> Id(this IQueryable<Resource> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Resource> IdOnly(this IQueryable<Resource> query)
        {
            return query.Select(o => new Resource { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Resource> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Resource> Ids(this IQueryable<Resource> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            ResourceQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ResourceQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(ResourceContent)}" +
                                $".{nameof(ResourceContent.Name)}{(asc ? "" : " DESC")}");
                        }
                        break;
                }
            }
            if (listSorts.Any())
            {
                var orderByClause = "ORDER BY " + string.Join(',', listSorts);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.SORT, orderByClause);
            }
            return query;
        }

        public static DynamicSql SqlFilter(
            this DynamicSql query, ResourceQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Resource)}.{nameof(Resource.Id)}=@{paramName}");
            }
            if (filter.archived != 2)
            {
                var paramName = query.AddAutoIncrParam(filter.archived);
                listFilters.Add($"{nameof(Resource)}.{nameof(Resource.Archived)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(ResourceContent)}" +
                    $".{nameof(ResourceContent.Name)}) > 0");
            }
            if (filter.owner_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.owner_id);
                listFilters.Add($"{nameof(Resource)}.{nameof(Resource.OwnerId)}=@{paramName}");
            }
            if (filter.loc_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.loc_id);
                listFilters.Add($"{nameof(Resource)}.{nameof(Resource.LocationId)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, ResourceQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => ResourceQueryProjection.Projections.ContainsKey(f))
                .Select(f => ResourceQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => ResourceQueryProjection.Results.ContainsKey(f))
                .Select(f => ResourceQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, ResourceQueryProjection model,
            ResourceQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => ResourceQueryProjection.Joins.ContainsKey(f))
                .Select(f => ResourceQueryProjection.Joins[f]);
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
                        var resourceContentLang = $"{nameof(ResourceContent)}.{nameof(ResourceContent.Lang)}";
                        contentFilters.Add($"{resourceContentLang}=@{paramName}");
                    }
                    if (contentFilters.Any())
                    {
                        var whereClause = "WHERE " + string.Join(" AND ", contentFilters);
                        query.DynamicForm = query.DynamicForm
                            .Replace(ResourceQueryPlaceholder.RES_CONTENT_FILTER, whereClause);
                    }
                }
            }
            return query;
        }

        public static DynamicSql SqlExtras(
            this DynamicSql query, ResourceQueryProjection model,
            ResourceQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var extras = model.GetFieldsArr()
                .Where(f => ResourceQueryProjection.Extras.ContainsKey(f))
                .Select(f => ResourceQueryProjection.Extras[f]);
            if (extras.Any())
            {
                var extraSqls = string.Join(';', extras);
                var originalQuery = query.PreparedForm;
                query.DynamicForm += ";\n" + extraSqls;
                query.DynamicForm = query.DynamicForm
                    .Replace(ResourceQueryPlaceholder.RES_SUB_QUERY, originalQuery);
                if (filter != null)
                {
                    if (model.fields.Contains(ResourceQueryProjection.CATEGORIES))
                    {
                        var contentFilters = new List<string>();
                        if (filter.lang != null)
                        {
                            var paramName = query.AddAutoIncrParam(filter.lang);
                            var ecLang = $"{nameof(EntityCategoryContent)}.{nameof(EntityCategoryContent.Lang)}";
                            contentFilters.Add($"{ecLang}=@{paramName}");
                        }
                        if (contentFilters.Any())
                        {
                            var whereClause = "WHERE " + string.Join(" AND ", contentFilters);
                            query.DynamicForm = query.DynamicForm
                                .Replace(ResourceQueryPlaceholder.CATE_CONTENT_FILTER, whereClause);
                        }
                    }
                }
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {nameof(Resource)} as {nameof(Resource)}\n" +
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
