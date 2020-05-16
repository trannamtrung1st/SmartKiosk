using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ScheduleQuery
    {
        public static IQueryable<Schedule> Id(this IQueryable<Schedule> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Schedule> IdOnly(this IQueryable<Schedule> query)
        {
            return query.Select(o => new Schedule { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Schedule> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Schedule> Ids(this IQueryable<Schedule> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            ScheduleQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ScheduleQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(Schedule)}" +
                                $".{nameof(Schedule.Name)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, ScheduleQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Schedule)}.{nameof(Schedule.Id)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(Schedule)}" +
                    $".{nameof(Schedule.Name)}) > 0");
            }
            if (filter.loc_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.loc_id);
                listFilters.Add($"{nameof(Schedule)}.{nameof(Schedule.LocationId)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, ScheduleQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => ScheduleQueryProjection.Projections.ContainsKey(f))
                .Select(f => ScheduleQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => ScheduleQueryProjection.Results.ContainsKey(f))
                .Select(f => ScheduleQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, ScheduleQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => ScheduleQueryProjection.Joins.ContainsKey(f))
                .Select(f => ScheduleQueryProjection.Joins[f]);
            if (joins.Any())
            {
                var joinClause = string.Join('\n', joins);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.JOIN, joinClause);
            }
            return query;
        }

        public static DynamicSql SqlExtras(
            this DynamicSql query, ScheduleQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var extras = model.GetFieldsArr()
                .Where(f => ScheduleQueryProjection.Extras.ContainsKey(f))
                .Select(f => ScheduleQueryProjection.Extras[f]);
            if (extras.Any())
            {
                var extraSqls = string.Join(';', extras);
                var originalQuery = query.PreparedForm;
                query.DynamicForm += ";\n" + extraSqls;
                query.DynamicForm = query.DynamicForm
                    .Replace(ScheduleQueryPlaceholder.SCHEDULE_SUB_QUERY, originalQuery);
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {nameof(Schedule)} as {nameof(Schedule)}\n" +
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
