﻿using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class ScheduleDetailQuery
    {
        public static IEnumerable<IGrouping<int, ScheduleDetail>> GroupBySchedule(
            this IEnumerable<ScheduleDetail> query)
        {
            return query.GroupBy(o => o.ScheduleId);
        }

        public static IQueryable<ScheduleDetail> Id(this IQueryable<ScheduleDetail> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ScheduleDetail> IdOnly(this IQueryable<ScheduleDetail> query)
        {
            return query.Select(o => new ScheduleDetail { Id = o.Id });
        }

        public static IQueryable<ScheduleDetail> IsDefault(this IQueryable<ScheduleDetail> query)
        {
            return query.Where(o => o.IsDefault == true);
        }

        public static IQueryable<ScheduleDetail> BySchedule(this IQueryable<ScheduleDetail> query, int refId)
        {
            return query.Where(o => o.ScheduleId == refId);
        }

        public static bool Exists(this IQueryable<ScheduleDetail> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ScheduleDetail> Ids(this IQueryable<ScheduleDetail> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            ScheduleDetailQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ScheduleDetailQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(ScheduleDetail)}" +
                                $".{nameof(ScheduleDetail.Name)}{(asc ? "" : " DESC")}");
                        }
                        break;
                    case ScheduleDetailQuerySort.START_TIME:
                        {
                            listSorts.Add($"{nameof(ScheduleDetail)}" +
                                $".{nameof(ScheduleDetail.FromTime)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, ScheduleDetailQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(ScheduleDetail)}.{nameof(ScheduleDetail.Id)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(ScheduleDetail)}" +
                    $".{nameof(ScheduleDetail.Name)}) > 0");
            }
            if (filter.schedule_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.schedule_id);
                listFilters.Add($"{nameof(ScheduleDetail)}.{nameof(ScheduleDetail.ScheduleId)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, ScheduleDetailQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => ScheduleDetailQueryProjection.Projections.ContainsKey(f))
                .Select(f => ScheduleDetailQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => ScheduleDetailQueryProjection.Results.ContainsKey(f))
                .Select(f => ScheduleDetailQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, ScheduleDetailQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => ScheduleDetailQueryProjection.Joins.ContainsKey(f))
                .Select(f => ScheduleDetailQueryProjection.Joins[f]);
            if (joins.Any())
            {
                var joinClause = string.Join('\n', joins);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.JOIN, joinClause);
            }
            return query;
        }

        public static DynamicSql SqlExtras(
            this DynamicSql query, ScheduleDetailQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var extras = model.GetFieldsArr()
                .Where(f => ScheduleDetailQueryProjection.Extras.ContainsKey(f))
                .Select(f => ScheduleDetailQueryProjection.Extras[f]);
            if (extras.Any())
            {
                var extraSqls = string.Join(';', extras);
                var originalQuery = query.PreparedViewForm;
                query.DynamicForm += ";\n" + extraSqls;
                query.DynamicForm = query.DynamicForm
                    .Replace(ScheduleDetailQueryPlaceholder.SCHEDULE_DETAIL_SUB_QUERY, originalQuery);
            }
            return query;
        }

        public static DynamicSql CreateDynamicSql()
        {
            var sql = $"SELECT {DynamicSql.PROJECTION} " +
                $"FROM {nameof(ScheduleDetail)} as {nameof(ScheduleDetail)}\n" +
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
