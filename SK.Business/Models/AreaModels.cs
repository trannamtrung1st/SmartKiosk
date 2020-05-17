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
        [JsonProperty("archived")]
        public bool Archived { get; set; }
    }

    #region Query
    public class AreaRelationship : Area, IDapperRelationship
    {
        public string GetTableName() => nameof(Area);
    }
    #endregion
}
