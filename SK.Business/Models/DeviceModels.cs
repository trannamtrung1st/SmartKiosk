using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class UpdateDeviceModel : MappingModel<Device>
    {
        public UpdateDeviceModel()
        {
        }

        public UpdateDeviceModel(Device src) : base(src)
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
        [JsonProperty("floor_id")]
        public int? FloorId { get; set; }
        [JsonProperty("area_id")]
        public int? AreaId { get; set; }
        [JsonProperty("lat")]
        public double? Lat { get; set; }
        [JsonProperty("lon")]
        public double? Lon { get; set; }
    }

    #region Query
    public class DeviceQueryRow
    {
        public Device Device { get; set; }
        public AreaRelationship Area { get; set; }
        public FloorRelationship Floor { get; set; }
        public BuildingRelationship Building { get; set; }
        public LocationRelationship Location { get; set; }
        public ScheduleRelationship Schedule { get; set; }
        public AppUserRelationship DeviceAccount { get; set; }
    }

    public class DeviceQueryProjection
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
        public const string AREA = "area";
        public const string FLOOR = "floor";
        public const string LOCATION = "location";
        public const string ACCOUNT = "account";
        public const string SCHEDULE = "schedule";
        public const string BUILDING = "building";

        private const string D = nameof(Device);
        private const string A = nameof(Area);
        private const string F = nameof(Floor);
        private const string L = nameof(Location);
        private const string U = AppUser.TBL_NAME;
        private const string B = nameof(Building);
        private const string S = nameof(Schedule);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{D}.{nameof(Device.Id)},{D}.{nameof(Device.Code)}," +
                    $"{D}.{nameof(Device.Name)},{D}.{nameof(Device.Description)}," +
                    $"{D}.{nameof(Device.AreaId)},{D}.{nameof(Device.BuildingId)}," +
                    $"{D}.{nameof(Device.FloorId)},{D}.{nameof(Device.LocationId)}," +
                    $"{D}.{nameof(Device.Lat)}," +
                    $"{D}.{nameof(Device.Lon)},{D}.{nameof(Device.ScheduleId)}"
                },
                {
                    AREA,$"{A}.{nameof(Area.Id)} as [{A}.{nameof(Area.Id)}]," +
                    $"{A}.{nameof(Area.Code)} as [{A}.{nameof(Area.Code)}]," +
                    $"{A}.{nameof(Area.Name)} as [{A}.{nameof(Area.Name)}]"
                },
                {
                    FLOOR,$"{F}.{nameof(Floor.Id)} as [{F}.{nameof(Floor.Id)}]," +
                    $"{F}.{nameof(Floor.Code)} as [{F}.{nameof(Floor.Code)}]," +
                    $"{F}.{nameof(Floor.Name)} as [{F}.{nameof(Floor.Name)}]"
                },
                {
                    BUILDING,$"{B}.{nameof(Building.Id)} as [{B}.{nameof(Building.Id)}]," +
                    $"{B}.{nameof(Building.Code)} as [{B}.{nameof(Building.Code)}]," +
                    $"{B}.{nameof(Building.Name)} as [{B}.{nameof(Building.Name)}]"
                },
                {
                    LOCATION,$"{L}.{nameof(Location.Id)} as [{L}.{nameof(Location.Id)}]," +
                    $"{L}.{nameof(Location.Code)} as [{L}.{nameof(Location.Code)}]," +
                    $"{L}.{nameof(Location.Name)} as [{L}.{nameof(Location.Name)}]"
                },
                {
                    SCHEDULE,$"{S}.{nameof(Schedule.Id)} as [{S}.{nameof(Schedule.Id)}]," +
                    $"{S}.{nameof(Schedule.Code)} as [{S}.{nameof(Schedule.Code)}]," +
                    $"{S}.{nameof(Schedule.Name)} as [{S}.{nameof(Schedule.Name)}]"
                },
                {
                    ACCOUNT,
                    $"{U}.{nameof(AppUser.Id)} as [{U}.{nameof(AppUser.Id)}]," +
                    $"{U}.{nameof(AppUser.UserName)} as [{U}.{nameof(AppUser.UserName)}]"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    AREA, $"LEFT JOIN {A} " +
                    $"ON {A}.{nameof(Area.Id)}={D}.{nameof(Device.AreaId)}"
                },
                {
                    FLOOR, $"LEFT JOIN {F} " +
                    $"ON {F}.{nameof(Floor.Id)}={D}.{nameof(Device.FloorId)}"
                },
                {
                    BUILDING, $"LEFT JOIN {B} " +
                    $"ON {B}.{nameof(Building.Id)}={D}.{nameof(Device.BuildingId)}"
                },
                {
                    LOCATION, $"LEFT JOIN {L} " +
                    $"ON {L}.{nameof(Location.Id)}={D}.{nameof(Device.LocationId)}"
                },
                {
                    SCHEDULE, $"LEFT JOIN {S} " +
                    $"ON {S}.{nameof(Schedule.Id)}={D}.{nameof(Device.ScheduleId)}"
                },
                {
                    ACCOUNT, $"INNER JOIN {U} " +
                    $"ON {U}.{nameof(AppUser.Id)}={D}.{nameof(Device.Id)}"
                },
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, new PartialResult(key: INFO, type: typeof(Device),
                         splitOn: $"{nameof(Device.Id)}")
                 },
                 {
                     AREA, new PartialResult(key: AREA, type: typeof(AreaRelationship),
                         splitOn: $"{A}.{nameof(Area.Id)}")
                 },
                 {
                     FLOOR, new PartialResult(key: FLOOR, type: typeof(FloorRelationship),
                         splitOn: $"{F}.{nameof(Floor.Id)}")
                 },
                 {
                     BUILDING, new PartialResult(key: BUILDING, type: typeof(BuildingRelationship),
                         splitOn: $"{B}.{nameof(Building.Id)}")
                 },
                 {
                     LOCATION, new PartialResult(key: LOCATION, type: typeof(LocationRelationship),
                         splitOn: $"{L}.{nameof(Location.Id)}")
                 },
                 {
                     SCHEDULE, new PartialResult(key: SCHEDULE, type: typeof(ScheduleRelationship),
                         splitOn: $"{S}.{nameof(Schedule.Id)}")
                 },
                 {
                     ACCOUNT, new PartialResult(key: ACCOUNT, type: typeof(AppUserRelationship),
                         splitOn: $"{U}.{nameof(AppUser.Id)}")
                 },
             };
    }

    public class DeviceQuerySort
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

    public class DeviceQueryFilter
    {
        public string id { get; set; }
        public string name_contains { get; set; }
        public int? loc_id { get; set; }
    }

    public class DeviceQueryPaging
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

    public class DeviceQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class DeviceQueryPlaceholder
    {
    }
    #endregion
}
