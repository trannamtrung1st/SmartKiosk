using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class AppUserQuery
    {
        public static IQueryable<AppUser> Id(this IQueryable<AppUser> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<AppUser> IdOnly(this IQueryable<AppUser> query)
        {
            return query.Select(o => new AppUser { Id = o.Id });
        }

        public static bool Exists(this IQueryable<AppUser> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<AppUser> Ids(this IQueryable<AppUser> query, IEnumerable<string> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            AppUserQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case AppUserQuerySort.USERNAME:
                        {
                            listSorts.Add($"{AppUser.TBL_NAME}" +
                                $".{nameof(AppUser.UserName)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, AppUserQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{AppUser.TBL_NAME}.{nameof(AppUser.Id)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, AppUserQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => AppUserQueryProjection.Projections.ContainsKey(f))
                .Select(f => AppUserQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => AppUserQueryProjection.Results.ContainsKey(f))
                .Select(f => AppUserQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, AppUserQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => AppUserQueryProjection.Joins.ContainsKey(f))
                .Select(f => AppUserQueryProjection.Joins[f]);
            if (joins.Any())
            {
                var joinClause = string.Join('\n', joins);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.JOIN, joinClause);
            }
            return query;
        }

        public static DynamicSql SqlExtras(
            this DynamicSql query, AppUserQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var extras = model.GetFieldsArr()
                .Where(f => AppUserQueryProjection.Extras.ContainsKey(f))
                .Select(f => AppUserQueryProjection.Extras[f]);
            if (extras.Any())
            {
                var extraSqls = string.Join(';', extras);
                var originalQuery = query.PreparedForm;
                query.DynamicForm += ";\n" + extraSqls;
                query.DynamicForm = query.DynamicForm
                    .Replace(AppUserQueryPlaceholder.USER_SUB_QUERY, originalQuery);
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {AppUser.TBL_NAME} as {AppUser.TBL_NAME}\n" +
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
