using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Queries
{

    public static class BaseQuery
    {
        public static IQueryable<T> SelectPage<T>(
            this IQueryable<T> query, int page, int limit)
        {
            page = page - 1;
            return query.Skip(page * limit).Take(limit);
        }

        public static DynamicSql SqlSelectPage(
            this DynamicSql query, int page, int limit)
        {
            query = DynamicSql.DeepClone(query);
            page = page - 1;
            var pName = query.AddAutoIncrParam(page * limit);
            var lName = query.AddAutoIncrParam(limit);
            query.DynamicForm = query.DynamicForm.Replace(DynamicSql.PAGING,
                $"OFFSET @{pName} ROWS FETCH NEXT @{lName} ROWS ONLY");
            return query;
        }

        public static DynamicSql SqlCount(
            this DynamicSql query, string column)
        {
            query = DynamicSql.DeepClone(query);
            query.DynamicForm = query.DynamicForm
                    .Replace(DynamicSql.PROJECTION, $"COUNT({column})");
            return query;
        }
    }
}
