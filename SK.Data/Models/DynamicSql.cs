using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TNT.Core.Helpers.Data;

namespace SK.Data.Models
{

    public class DynamicSql
    {
        public string DynamicForm { get; set; }
        public string PreparedForm
        {
            get
            {
                if (DynamicForm == null) return null;
                var prepared = DynamicForm;
                if (!prepared.Contains(PAGING) && prepared.Contains(SORT))
                    prepared = prepared.Replace(SORT, "ORDER BY (SELECT 1)");
                var r = new Regex("\\$\\(.+?\\)");
                prepared = r.Replace(prepared, "");
                return prepared;
            }
        }
        public List<SqlParameter> Parameters { get; set; }

        public DynamicParameters DynamicParameters
        {
            get
            {
                var dynamicParam = new DynamicParameters();
                Parameters.ForEach(p => dynamicParam.Add(p.ParameterName, p.Value));
                return dynamicParam;
            }
        }

        public const string PROJECTION = "$(projection)";
        public const string JOIN = "$(join)";
        public const string FILTER = "$(filter)";
        public const string GROUP = "$(group)";
        public const string SORT = "$(sort)";
        public const string PAGING = "$(paging)";

        public DynamicSql()
        {
            Parameters = new List<SqlParameter>();
        }

        public static DynamicSql DeepClone(DynamicSql src)
        {
            return new DynamicSql
            {
                DynamicForm = src.DynamicForm,
                Parameters = src.Parameters.ToList()
            };
        }

        public string AddAutoIncrParam(object val)
        {
            var paramName = Parameters.Count.ToString();
            Parameters.Add(new SqlParameter(paramName, val));
            return paramName;
        }

        public ListDataParameters AddAutoIncrSqlInParam<T>(IEnumerable<T> vals)
        {
            var paramName = Parameters.Count.ToString();
            var listDataParams = vals.GetDataParameters(paramName);
            var listSqlParams = listDataParams
                .Parameters.Select(p => new SqlParameter(p.Name, p.Value));
            Parameters.AddRange(listSqlParams);
            return listDataParams;
        }

    }
}
