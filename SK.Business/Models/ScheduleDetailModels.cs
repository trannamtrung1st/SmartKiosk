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
    public class ScheduleDetailRelationship : ScheduleDetail, IDapperRelationship
    {
        public string GetTableName() => nameof(ScheduleDetail);
    }
    #endregion
}
