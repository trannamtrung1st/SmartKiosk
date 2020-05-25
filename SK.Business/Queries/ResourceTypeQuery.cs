using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ResourceTypeQuery
    {
        public static IQueryable<ResourceType> Id(this IQueryable<ResourceType> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ResourceType> IdOnly(this IQueryable<ResourceType> query)
        {
            return query.Select(o => new ResourceType { Id = o.Id });
        }

        public static bool Exists(this IQueryable<ResourceType> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ResourceType> Ids(this IQueryable<ResourceType> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            ResourceTypeQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ResourceTypeQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(ResourceTypeContent)}" +
                                $".{nameof(ResourceTypeContent.Name)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, ResourceTypeQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(ResourceType)}.{nameof(ResourceType.Id)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(ResourceTypeContent)}" +
                    $".{nameof(ResourceTypeContent.Name)}) > 0");
            }
            if (filter.archived != 2)
            {
                var paramName = query.AddAutoIncrParam(filter.archived);
                listFilters.Add($"{nameof(ResourceType)}.{nameof(ResourceType.Archived)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, ResourceTypeQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => ResourceTypeQueryProjection.Projections.ContainsKey(f))
                .Select(f => ResourceTypeQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => ResourceTypeQueryProjection.Results.ContainsKey(f))
                .Select(f => ResourceTypeQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, ResourceTypeQueryProjection model,
            ResourceTypeQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => ResourceTypeQueryProjection.Joins.ContainsKey(f))
                .Select(f => ResourceTypeQueryProjection.Joins[f]);
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
                        var postContentLang = $"{nameof(ResourceTypeContent)}.{nameof(ResourceTypeContent.Lang)}";
                        contentFilters.Add($"{postContentLang}=@{paramName}");
                    }
                    if (contentFilters.Any())
                    {
                        var whereClause = "WHERE " + string.Join(" AND ", contentFilters);
                        query.DynamicForm = query.DynamicForm
                            .Replace(ResourceTypeQueryPlaceholder.RES_TYPE_CONTENT_FILTER, whereClause);
                    }
                }
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {nameof(ResourceType)} as {nameof(ResourceType)}\n" +
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
