using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateFloorModel : MappingModel<Floor>
    {
        public CreateFloorModel()
        {
        }

        public CreateFloorModel(Floor entity) : base(entity)
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
        [JsonProperty("building_id")]
        public int BuildingId { get; set; }
    }

    public class UpdateFloorModel : MappingModel<Floor>
    {
        public UpdateFloorModel()
        {
        }

        public UpdateFloorModel(Floor src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class UpdateFloorPlanModel
    {
        [JsonProperty("file")]
        public FileDestination File { get; set; }
    }

    #region Query

    public class FloorQueryRow
    {
        public Floor Floor { get; set; }
        public BuildingRelationship Building { get; set; }
    }

    public class FloorQueryProjection
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
        public const string BUILDING = "building";
        public const string FLOOR_PLAN = "floor_plan";

        private const string F = nameof(Floor);
        private const string B = nameof(Building);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{F}.{nameof(Floor.Id)},{F}.{nameof(Floor.Code)}," +
                    $"{F}.{nameof(Floor.Name)}," +
                    $"{F}.{nameof(Floor.Description)}," +
                    $"{F}.{nameof(Floor.BuildingId)}," +
                    $"{F}.{nameof(Floor.LocationId)}"
                },
                {
                    SELECT,$"{F}.{nameof(Floor.Id)},{F}.{nameof(Floor.Code)}," +
                    $"{F}.{nameof(Floor.Name)}"
                },
                {
                    FLOOR_PLAN,$"{F}.{nameof(Floor.FloorPlanSvg)}"
                },
                {
                    BUILDING,$"{B}.{nameof(Location.Id)} as [{B}.{nameof(Location.Id)}]," +
                    $"{B}.{nameof(Location.Code)} as [{B}.{nameof(Location.Code)}]," +
                    $"{B}.{nameof(Location.Name)} as [{B}.{nameof(Location.Name)}]"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    BUILDING, $"INNER JOIN {B} " +
                    $"ON {B}.{nameof(Building.Id)}={F}.{nameof(Floor.BuildingId)}"
                },
            };

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(Floor), splitOn: $"{nameof(Floor.Id)}");
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
                     BUILDING, new PartialResult(key: BUILDING, type: typeof(BuildingRelationship),
                         splitOn: $"{B}.{nameof(Building.Id)}")
                 },
             };
    }

    public class FloorQuerySort
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

    public class FloorQueryFilter
    {
        public int? loc_id { get; set; }
        public int? building_id { get; set; }
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class FloorQueryPaging
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

    public class FloorQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class FloorQueryPlaceholder
    {
    }

    public class FloorRelationship : Floor, IDapperRelationship
    {
        public string GetTableName() => nameof(Floor);
    }
    #endregion
}
