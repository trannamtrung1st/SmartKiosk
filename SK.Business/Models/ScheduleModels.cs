using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateScheduleModel : MappingModel<Schedule>
    {
        public CreateScheduleModel()
        {
        }

        public CreateScheduleModel(Schedule src) : base(src)
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

    public class UpdateScheduleModel : MappingModel<Schedule>
    {
        public UpdateScheduleModel()
        {
        }

        public UpdateScheduleModel(Schedule src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    #region Query
    public class ScheduleQueryRow
    {
        public Schedule Schedule { get; set; }
        public LocationRelationship Location { get; set; }
    }

    public class ScheduleQueryProjection
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
        public const string DETAILS = "details";
        public const string LOCATION = "location";
        public const string SELECT = "select";

        private const string S = nameof(Schedule);
        private const string L = nameof(Location);
        private const string SD = nameof(ScheduleDetail);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{S}.{nameof(Schedule.Id)},{S}.{nameof(Schedule.Code)}," +
                    $"{S}.{nameof(Schedule.Name)}," +
                    $"{S}.{nameof(Schedule.Description)}," +
                    $"{S}.{nameof(Schedule.LocationId)}"
                },
                {
                    SELECT,$"{S}.{nameof(Schedule.Id)},{S}.{nameof(Schedule.Code)}," +
                    $"{S}.{nameof(Schedule.Name)}"
                },
                {
                    LOCATION,
                    $"{L}.{nameof(Location.Id)} as [{L}.{nameof(Location.Id)}]," +
                    $"{L}.{nameof(Location.Name)} as [{L}.{nameof(Location.Name)}]," +
                    $"{L}.{nameof(Location.Code)} as [{L}.{nameof(Location.Code)}]"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    LOCATION, $"INNER JOIN {L} " +
                    $"ON {L}.{nameof(Location.Id)}={S}.{nameof(Schedule.LocationId)}"
                }
            };

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(Schedule), splitOn: $"{nameof(Schedule.Id)}");
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
                     LOCATION, new PartialResult(key: LOCATION, typeof(LocationRelationship),
                         splitOn: $"{L}.{nameof(Location.Id)}")
                 }
             };

        public static readonly IDictionary<string, string> Extras =
            new Dictionary<string, string>()
            {
                {
                    DETAILS, $"SELECT {SD}.{nameof(ScheduleDetail.Id)}," +
                    $"{SD}.{nameof(ScheduleDetail.Name)}," +
                    $"{SD}.{nameof(ScheduleDetail.FromTime)}," +
                    $"{SD}.{nameof(ScheduleDetail.ToTime)}," +
                    $"{SD}.{nameof(ScheduleDetail.IsDefault)}," +
                    $"{SD}.{nameof(ScheduleDetail.ScheduleId)}" +
                    $" FROM {nameof(ScheduleDetail)}\n" +
                    $"INNER JOIN ({ScheduleQueryPlaceholder.SCHEDULE_SUB_QUERY}) AS {S} " +
                    $"ON {SD}.{nameof(ScheduleDetail.ScheduleId)}={S}.{nameof(Schedule.Id)}\n" +
                    $"ORDER BY {SD}.{nameof(ScheduleDetail.FromTime)}"
                }
            };

    }

    public class ScheduleQuerySort
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

    public class ScheduleQueryFilter
    {
        public int? loc_id { get; set; }
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class ScheduleQueryPaging
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

    public class ScheduleQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class ScheduleQueryPlaceholder
    {
        public const string SCHEDULE_SUB_QUERY = "$(schedule_sub_query)";
    }

    public class ScheduleRelationship : Schedule, IDapperRelationship
    {
        public string GetTableName() => nameof(Schedule);
    }
    #endregion
}
