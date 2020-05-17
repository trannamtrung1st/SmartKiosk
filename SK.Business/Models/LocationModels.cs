using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateLocationModel : MappingModel<Location>
    {
        public CreateLocationModel()
        {
        }

        public CreateLocationModel(Location src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }


    #region Query
    public class LocationQueryRow
    {
        public Location Location { get; set; }
    }

    public class LocationQueryProjection
    {
        private const string DEFAULT = INFO;
        private string _fields = DEFAULT;
        public string fields
        {
            get
            {
                return _fields;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _fields = value;
                    _fieldsArr = value.Split(',').OrderBy(v => v).ToArray();
                }
            }
        }

        private string[] _fieldsArr = DEFAULT.Split(',');
        public string[] GetFieldsArr()
        {
            return _fieldsArr;
        }

        //---------------------------------------

        public const string INFO = "info";
        public const string SELECT = "select";

        private const string L = nameof(Location);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{L}.{nameof(Location.Id)},{L}.{nameof(Location.Code)}," +
                    $"{L}.{nameof(Location.Name)}," +
                    $"{L}.{nameof(Location.Address)}," +
                    $"{L}.{nameof(Location.Description)}," +
                    $"{L}.{nameof(Location.Archived)}"
                },
                {
                    SELECT,$"{L}.{nameof(Location.Id)},{L}.{nameof(Location.Code)}," +
                    $"{L}.{nameof(Location.Name)}"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>();

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(Location), splitOn: $"{nameof(Location.Id)}");
        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, infoResult
                 },
                 {
                     SELECT, infoResult
                 }
             };
    }

    public class LocationQuerySort
    {
        public const string NAME = "name";
        private const string DEFAULT = "a" + NAME;
        private string _sorts = DEFAULT;
        public string sorts
        {
            get
            {
                return _sorts;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _sorts = value;
                    _sortsArr = value.Split(',');
                }
            }
        }

        public string[] _sortsArr = DEFAULT.Split(',');
        public string[] GetSortsArr()
        {
            return _sortsArr;
        }

    }

    public class LocationQueryFilter
    {
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class LocationQueryPaging
    {
        private int _page = 1;
        public int page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value > 0 ? value : _page;
            }
        }

        private int _limit = 10;
        public int limit
        {
            get
            {
                return _limit;
            }
            set
            {
                if (value >= 1 && value <= 100)
                    _limit = value;
            }
        }
    }

    public class LocationQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class LocationQueryPlaceholder
    {
    }

    public class LocationRelationship : Location, IDapperRelationship
    {
        public string GetTableName() => nameof(Location);
    }
    #endregion
}
