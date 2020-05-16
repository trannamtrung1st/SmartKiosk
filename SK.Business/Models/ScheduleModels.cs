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
    public class ScheduleRelationship : Schedule, IDapperRelationship
    {
        public string GetTableName() => nameof(Schedule);
    }
    #endregion
}
