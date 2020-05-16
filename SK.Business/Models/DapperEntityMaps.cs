using Dapper.FluentMap.Mapping;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class ManualPropertyMap : IPropertyMap
    {
        public string ColumnName { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public bool CaseSensitive { get; set; }

        public bool Ignored { get; set; }
    }

    public static class DapperMapProvider
    {
        public static RelationshipMap<T> From<T>(T ins)
            where T : class, IDapperRelationship
        {
            return new RelationshipMap<T>(ins.GetTableName());
        }
    }

    public class RelationshipMap<Entity> : EntityMap<Entity> where Entity : class
    {
        public RelationshipMap(string tblName)
        {
            var baseType = typeof(Entity);
            var props = baseType.GetProperties();
            foreach (var p in props)
            {
                PropertyMaps.Add(new ManualPropertyMap
                {
                    CaseSensitive = true,
                    ColumnName = $"{tblName}.{p.Name}",
                    Ignored = false,
                    PropertyInfo = p
                });
            }
        }
    }
}
