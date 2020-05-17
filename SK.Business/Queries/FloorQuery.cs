using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class FloorQuery
    {
        public static IQueryable<Floor> Id(this IQueryable<Floor> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Floor> IdOnly(this IQueryable<Floor> query)
        {
            return query.Select(o => new Floor { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Floor> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Floor> Ids(this IQueryable<Floor> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            FloorQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case FloorQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(Floor)}" +
                                $".{nameof(Floor.Name)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, FloorQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Floor)}.{nameof(Floor.Id)}=@{paramName}");
            }
            if (filter.building_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.building_id);
                listFilters.Add($"{nameof(Floor)}.{nameof(Floor.BuildingId)}=@{paramName}");
            }
            if (filter.loc_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.loc_id);
                listFilters.Add($"{nameof(Floor)}.{nameof(Floor.LocationId)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(Floor)}" +
                    $".{nameof(Floor.Name)}) > 0");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, FloorQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => FloorQueryProjection.Projections.ContainsKey(f))
                .Select(f => FloorQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => FloorQueryProjection.Results.ContainsKey(f))
                .Select(f => FloorQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, FloorQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => FloorQueryProjection.Joins.ContainsKey(f))
                .Select(f => FloorQueryProjection.Joins[f]);
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
                $"FROM {nameof(Floor)} as {nameof(Floor)}\n" +
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
