using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TNT.Core.Helpers.Data;

namespace SK.Business.Models
{
    public class PartialResult
    {
        public PartialResult(string key, Type type, string splitOn)
        {
            Key = key;
            Type = type;
            SplitOn = splitOn;
        }
        public string Key { get; set; }
        public string SplitOn { get; set; }
        public Type Type { get; set; }
    }

    public class DynamicSql
    {
        public string SortClause { get; set; }
        public string DynamicForm { get; set; }
        public string PreparedForm
        {
            get
            {
                var prepared = DynamicForm;
                if (!prepared.Contains(PAGING) && SortClause == null)
                    SortClause = "ORDER BY (SELECT 1)";
                if (SortClause != null)
                    prepared = prepared.Replace(SORT, SortClause);
                var r = new Regex("\\$\\(.+?\\)");
                prepared = r.Replace(prepared, "");
                return prepared;
            }
        }
        public string PreparedViewForm
        {
            get
            {
                var prepared = DynamicForm;
                if (!prepared.Contains(PAGING))
                {
                    if (SortClause == null)
                        SortClause = "ORDER BY (SELECT 1)";
                    if (SortClause != null)
                        prepared = prepared.Replace(SORT, SortClause);
                }
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
        public List<PartialResult> MultiResults { get; set; }

        public const string PROJECTION = "$(projection)";
        public const string JOIN = "$(join)";
        public const string FILTER = "$(filter)";
        public const string GROUP = "$(group)";
        public const string SORT = "$(sort)";
        public const string PAGING = "$(paging)";

        public DynamicSql()
        {
            Parameters = new List<SqlParameter>();
            MultiResults = new List<PartialResult>();
        }

        public IEnumerable<string> GetSplitOns()
        {
            var splitOns = MultiResults.Where(r => r.SplitOn != null).Select(r => r.SplitOn);
            return splitOns;
        }

        public Type[] GetTypesArr()
        {
            return MultiResults.Select(r => r.Type).ToArray();
        }

        public static DynamicSql DeepClone(DynamicSql src)
        {
            return new DynamicSql
            {
                DynamicForm = src.DynamicForm,
                MultiResults = src.MultiResults.ToList(),
                Parameters = src.Parameters.ToList(),
                SortClause = src.SortClause
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
