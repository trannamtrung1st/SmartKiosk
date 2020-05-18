﻿using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{
    public static class OwnerQuery
    {
        public static IQueryable<Owner> Id(this IQueryable<Owner> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Owner> IdOnly(this IQueryable<Owner> query)
        {
            return query.Select(o => new Owner { Id = o.Id });
        }

        public static bool Exists(this IQueryable<Owner> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<Owner> Ids(this IQueryable<Owner> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        #region DynamicSql
        public static DynamicSql SqlSort(this DynamicSql query,
            OwnerQuerySort model)
        {
            query = DynamicSql.DeepClone(query);
            var listSorts = new List<string>();
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case OwnerQuerySort.NAME:
                        {
                            listSorts.Add($"{nameof(Owner)}" +
                                $".{nameof(Owner.Name)}{(asc ? "" : " DESC")}");
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
            this DynamicSql query, OwnerQueryFilter filter)
        {
            query = DynamicSql.DeepClone(query);
            var listFilters = new List<string>();
            if (filter.id != null)
            {
                var paramName = query.AddAutoIncrParam(filter.id);
                listFilters.Add($"{nameof(Owner)}.{nameof(Owner.Id)}=@{paramName}");
            }
            if (filter.name_contains != null)
            {
                var paramName = query.AddAutoIncrParam(filter.name_contains);
                listFilters.Add($"CHARINDEX(@{paramName}, {nameof(Owner)}" +
                    $".{nameof(Owner.Name)}) > 0");
            }
            if (listFilters.Any())
            {
                var whereClause = "WHERE " + string.Join(" AND ", listFilters);
                query.DynamicForm = query.DynamicForm.Replace(DynamicSql.FILTER, whereClause);
            }
            return query;
        }

        public static DynamicSql SqlProjectFields(
            this DynamicSql query, OwnerQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var finalFields = model.GetFieldsArr()
                .Where(f => OwnerQueryProjection.Projections.ContainsKey(f))
                .Select(f => OwnerQueryProjection.Projections[f]);
            if (finalFields.Any())
            {
                var projectionClause = string.Join(',', finalFields);
                query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, projectionClause);
            }
            var finalResults = model.GetFieldsArr()
                .Where(f => OwnerQueryProjection.Results.ContainsKey(f))
                .Select(f => OwnerQueryProjection.Results[f]);
            query.MultiResults.AddRange(finalResults);
            return query;
        }

        public static DynamicSql SqlJoin(
            this DynamicSql query, OwnerQueryProjection model)
        {
            query = DynamicSql.DeepClone(query);
            var joins = model.GetFieldsArr()
                .Where(f => OwnerQueryProjection.Joins.ContainsKey(f))
                .Select(f => OwnerQueryProjection.Joins[f]);
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
                $"FROM {nameof(Owner)} as {nameof(Owner)}\n" +
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
