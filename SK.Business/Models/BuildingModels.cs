using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateBuildingModel : MappingModel<Building>
    {
        public CreateBuildingModel()
        {
        }

        public CreateBuildingModel(Building src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("location_id")]
        public int LocationId { get; set; }
    }


    #region Query
    public class BuildingQueryRow
    {
        public Building Building { get; set; }
        public LocationRelationship Location { get; set; }
    }

    public class BuildingQueryProjection
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
        public const string LOCATION = "location";

        private const string B = nameof(Building);
        private const string L = nameof(Location);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{B}.{nameof(Building.Id)},{B}.{nameof(Building.Code)}," +
                    $"{B}.{nameof(Building.Name)}," +
                    $"{B}.{nameof(Building.Description)}," +
                    $"{B}.{nameof(Building.LocationId)}"
                },
                {
                    SELECT,$"{B}.{nameof(Building.Id)},{B}.{nameof(Building.Code)}," +
                    $"{B}.{nameof(Building.Name)}"
                },
                {
                    LOCATION,$"{L}.{nameof(Location.Id)} as [{L}.{nameof(Location.Id)}]," +
                    $"{L}.{nameof(Location.Code)} as [{L}.{nameof(Location.Code)}]," +
                    $"{L}.{nameof(Location.Name)} as [{L}.{nameof(Location.Name)}]"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    LOCATION, $"INNER JOIN {L} " +
                    $"ON {L}.{nameof(Location.Id)}={B}.{nameof(Building.LocationId)}"
                },
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, new PartialResult(key: INFO, type: typeof(Building),
                         splitOn: $"{nameof(Building.Id)}")
                 },
                 {
                     LOCATION, new PartialResult(key: LOCATION, type: typeof(LocationRelationship),
                         splitOn: $"{L}.{nameof(Location.Id)}")
                 },
             };
    }

    public class BuildingQuerySort
    {
        public const string NAME = "name";
        private const string DEFAULT = "d" + NAME;
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

    public class BuildingQueryFilter
    {
        public int? loc_id { get; set; }
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class BuildingQueryPaging
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

    public class BuildingQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class BuildingQueryPlaceholder
    {
    }

    public class BuildingRelationship : Building, IDapperRelationship
    {
        public string GetTableName() => nameof(Building);
    }
    #endregion
}
