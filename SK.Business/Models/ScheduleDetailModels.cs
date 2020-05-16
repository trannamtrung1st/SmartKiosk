using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateScheduleDetailModel : MappingModel<ScheduleDetail>
    {
        public CreateScheduleDetailModel()
        {
        }

        public CreateScheduleDetailModel(ScheduleDetail src) : base(src)
        {
        }

        [JsonProperty("start_end_date_str")]
        public string StartEndDateStr { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("is_default")]
        public bool? IsDefault { get; set; }
        [JsonProperty("schedule_id")]
        public int? ScheduleId { get; set; }
    }

    public class UpdateScheduleDetailModel : MappingModel<ScheduleDetail>
    {
        public UpdateScheduleDetailModel()
        {
        }

        public UpdateScheduleDetailModel(ScheduleDetail src) : base(src)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("is_default")]
        public bool? IsDefault { get; set; }
        [JsonProperty("start_end_date_str")]
        public string StartEndDateStr { get; set; }
        [JsonProperty("week_configs")]
        public IEnumerable<CreateWeekConfigModel> WeekConfigs { get; set; }
    }

    public class CreateWeekConfigModel : MappingModel<ScheduleWeekConfig>
    {
        public CreateWeekConfigModel()
        {
        }

        public CreateWeekConfigModel(ScheduleWeekConfig src) : base(src)
        {
        }

        [JsonProperty("from_time")]
        public TimeSpan? FromTime { get; set; }
        [JsonProperty("to_time")]
        public TimeSpan? ToTime { get; set; }
        [JsonProperty("config_id")]
        public int ConfigId { get; set; }
        [JsonProperty("all_day")]
        public bool? AllDay { get; set; }
        [JsonProperty("from_day_of_week")]
        public int? FromDayOfWeek { get; set; }
        [JsonProperty("to_day_of_week")]
        public int? ToDayOfWeek { get; set; }
    }

    #region Query
    public class ScheduleDetailQueryRow
    {
        public ScheduleDetail ScheduleDetail { get; set; }
        public ScheduleRelationship Schedule { get; set; }
    }

    public class ScheduleDetailQueryProjection
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
        public const string CONFIGS = "configs";
        public const string SCHEDULE = "schedule";

        private const string SD = nameof(ScheduleDetail);
        private const string S = nameof(Schedule);
        private const string C = nameof(ScheduleWeekConfig);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{SD}.{nameof(ScheduleDetail.FromTime)}," +
                    $"{SD}.{nameof(ScheduleDetail.ToTime)}," +
                    $"{SD}.{nameof(ScheduleDetail.Id)}," +
                    $"{SD}.{nameof(ScheduleDetail.Name)}," +
                    $"{SD}.{nameof(ScheduleDetail.ScheduleId)}," +
                    $"{SD}.{nameof(ScheduleDetail.IsDefault)}"
                },
                {
                    SCHEDULE,
                    $"{S}.{nameof(Schedule.Id)} as [{S}.{nameof(Schedule.Id)}]," +
                    $"{S}.{nameof(Schedule.Name)} as [{S}.{nameof(Schedule.Name)}]," +
                    $"{S}.{nameof(Schedule.Code)} as [{S}.{nameof(Schedule.Code)}]"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    SCHEDULE, $"INNER JOIN {S} " +
                    $"ON {S}.{nameof(Schedule.Id)}={SD}.{nameof(ScheduleDetail.ScheduleId)}"
                }
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO,
                     new PartialResult(key: INFO, type: typeof(ScheduleDetail), splitOn: $"{nameof(ScheduleDetail.Id)}")
                 },
                 {
                     SCHEDULE, new PartialResult(key: SCHEDULE, typeof(ScheduleRelationship),
                         splitOn: $"{S}.{nameof(Schedule.Id)}")
                 }
             };

        public static readonly IDictionary<string, string> Extras =
            new Dictionary<string, string>()
            {
                {
                    CONFIGS, $"SELECT {C}.{nameof(ScheduleWeekConfig.Id)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.ToTime)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.ToDayOfWeek)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.FromTime)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.FromDayOfWeek)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.AllDay)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.ConfigId)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.ScheduleDetailId)}" +
                    $" FROM {nameof(ScheduleWeekConfig)}\n" +
                    $"INNER JOIN ({ScheduleDetailQueryPlaceholder.SCHEDULE_DETAIL_SUB_QUERY}) AS {SD} " +
                    $"ON {C}.{nameof(ScheduleWeekConfig.ScheduleDetailId)}={SD}.{nameof(ScheduleDetail.Id)}\n" +
                    $"ORDER BY {C}.{nameof(ScheduleWeekConfig.FromDayOfWeek)}," +
                    $"{C}.{nameof(ScheduleWeekConfig.FromTime)}"
                }
            };

    }

    public class ScheduleDetailQuerySort
    {
        public const string NAME = "name";
        public const string START_TIME = "start_time";
        private const string DEFAULT = "a" + START_TIME;
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

    public class ScheduleDetailQueryFilter
    {
        public int? schedule_id { get; set; }
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class ScheduleDetailQueryPaging
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

    public class ScheduleDetailQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class ScheduleDetailQueryPlaceholder
    {
        public const string SCHEDULE_DETAIL_SUB_QUERY = "$(schedule_detail_sub_query)";
    }

    public class ScheduleDetailRelationship : ScheduleDetail, IDapperRelationship
    {
        public string GetTableName() => nameof(ScheduleDetail);
    }
    #endregion
}
