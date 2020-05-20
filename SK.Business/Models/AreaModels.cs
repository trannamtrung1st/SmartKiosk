using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateAreaModel : MappingModel<Area>
    {
        public CreateAreaModel()
        {
        }

        public CreateAreaModel(Area src) : base(src)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("floor_id")]
        public int FloorId { get; set; }
        [JsonProperty("building_id")]
        public int BuildingId { get; set; }
        [JsonProperty("location_id")]
        public int LocationId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class UpdateAreaModel : MappingModel<Area>
    {
        public UpdateAreaModel()
        {
        }

        public UpdateAreaModel(Area src) : base(src)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class ChangeArchivedStateModel
    {
        [JsonProperty("archived")]
        public bool Archived { get; set; }
    }

    #region Query
    public class AreaQueryRow
    {
        public Area Area { get; set; }
        public FloorRelationship Floor { get; set; }
        public BuildingRelationship Building { get; set; }
    }

    public class AreaQueryProjection
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
        public const string FLOOR = "floor";
        public const string BUILDING = "building";

        private const string A = nameof(Area);
        private const string F = nameof(Floor);
        private const string B = nameof(Building);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{A}.{nameof(Area.Id)},{A}.{nameof(Area.Code)}," +
                    $"{A}.{nameof(Area.Name)}," +
                    $"{A}.{nameof(Area.Description)}," +
                    $"{A}.{nameof(Area.Archived)}," +
                    $"{A}.{nameof(Area.FloorId)}," +
                    $"{A}.{nameof(Area.BuildingId)}," +
                    $"{A}.{nameof(Area.LocationId)}"
                },
                {
                    SELECT,$"{A}.{nameof(Area.Id)},{A}.{nameof(Area.Code)}," +
                    $"{A}.{nameof(Area.Name)}"
                },
                {
                    FLOOR,
                    $"{F}.{nameof(Floor.Id)} as [{F}.{nameof(Floor.Id)}]," +
                    $"{F}.{nameof(Floor.Name)} as [{F}.{nameof(Floor.Name)}]," +
                    $"{F}.{nameof(Floor.Code)} as [{F}.{nameof(Floor.Code)}]"
                },
                {
                    BUILDING,
                    $"{B}.{nameof(Building.Id)} as [{B}.{nameof(Building.Id)}]," +
                    $"{B}.{nameof(Building.Name)} as [{B}.{nameof(Building.Name)}]," +
                    $"{B}.{nameof(Building.Code)} as [{B}.{nameof(Building.Code)}]"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    FLOOR, $"INNER JOIN {F} " +
                    $"ON {F}.{nameof(Floor.Id)}={A}.{nameof(Area.FloorId)}"
                },
                {
                    BUILDING, $"INNER JOIN {B} " +
                    $"ON {B}.{nameof(Building.Id)}={A}.{nameof(Area.BuildingId)}"
                },
            };

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(Area), splitOn: $"{nameof(Area.Id)}");
        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, infoResult
                 },
                 {
                     SELECT, infoResult
                 },
                 {
                     FLOOR, new PartialResult(key: FLOOR, typeof(FloorRelationship),
                         splitOn: $"{F}.{nameof(Floor.Id)}")
                 },
                 {
                     BUILDING, new PartialResult(key: BUILDING, typeof(BuildingRelationship),
                         splitOn: $"{B}.{nameof(Building.Id)}")
                 },
             };

    }

    public class AreaQuerySort
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

    public class AreaQueryFilter
    {
        public int? id { get; set; }
        public string name_contains { get; set; }
        public int? floor_id { get; set; }
        public int? loc_id { get; set; }
        //0: false, 1: true, 2: both => default: false
        public byte archived { get; set; }
    }

    public class AreaQueryPaging
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

    public class AreaQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class AreaQueryPlaceholder
    {
    }

    public class AreaRelationship : Area, IDapperRelationship
    {
        public string GetTableName() => nameof(Area);
    }
    #endregion
}
