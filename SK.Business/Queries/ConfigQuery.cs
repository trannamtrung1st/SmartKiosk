using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ConfigQuery
    {
        public static IEnumerable<IGrouping<int, ScheduleWeekConfig>> GroupByScheduleDetail(
            this IEnumerable<ScheduleWeekConfig> query)
        {
            return query.GroupBy(o => o.ScheduleDetailId);
        }

        public static IQueryable<Config> Id(this IQueryable<Config> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Config> IdOnly(this IQueryable<Config> query)
        {
            return query.Select(o => new Config { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Config> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Config> Ids(this IQueryable<Config> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            ConfigQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ConfigQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(Config)}" +
                                $".{nameof(Config.Name)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, ConfigQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Config)}.{nameof(Config.Id)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(Config)}" +
                    $".{nameof(Config.Name)}) > 0");
            }
            if (filter.loc_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.loc_id);
                listFilters.Add($"{nameof(Config)}.{nameof(Config.LocationId)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, ConfigQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => ConfigQueryProjection.Projections.ContainsKey(f))
                .Select(f => ConfigQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => ConfigQueryProjection.Results.ContainsKey(f))
                .Select(f => ConfigQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, ConfigQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => ConfigQueryProjection.Joins.ContainsKey(f))
                .Select(f => ConfigQueryProjection.Joins[f]);
            if (joins.Any())
            {
                var joinClause = string.Join('\n', joins);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.JOIN, joinClause);
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {nameof(Config)} as {nameof(Config)}\n" +
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
