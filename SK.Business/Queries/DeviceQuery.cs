using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class DeviceQuery
    {
        public static IQueryable<Device> Id(this IQueryable<Device> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Device> IdOnly(this IQueryable<Device> query)
        {
            return query.Select(o => new Device { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Device> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Device> Ids(this IQueryable<Device> query, IEnumerable<string> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            DeviceQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case DeviceQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(Device)}" +
                                $".{nameof(Device.Name)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, DeviceQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Device)}.{nameof(Device.Id)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(Device)}" +
                    $".{nameof(Device.Name)}) > 0");
            }
            if (filter.loc_id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.loc_id);
                listFilters.Add($"{nameof(Device)}.{nameof(Device.LocationId)}=@{paramName}");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, DeviceQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => DeviceQueryProjection.Projections.ContainsKey(f))
                .Select(f => DeviceQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, DeviceQueryProjection model,
            DeviceQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => DeviceQueryProjection.Joins.ContainsKey(f))
                .Select(f => DeviceQueryProjection.Joins[f]);
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
                $"FROM {nameof(Device)} as {nameof(Device)}\n" +
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
