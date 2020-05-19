using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{

    public class CreateResourceModel : MappingModel<Resource>
    {
        public CreateResourceModel()
        {
        }

        public CreateResourceModel(Resource entity) : base(entity)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("logo")]
        public FileDestination Logo { get; set; }
        [JsonProperty("image")]
        public FileDestination Image { get; set; }
        [JsonProperty("type_id")]
        public int TypeId { get; set; }
        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }
        [JsonProperty("location_id")]
        public int LocationId { get; set; }
        [JsonProperty("building_id")]
        public int BuildingId { get; set; }
        [JsonProperty("floor_id")]
        public int FloorId { get; set; }
        [JsonProperty("area_id")]
        public int AreaId { get; set; }
        [JsonProperty("category_ids")]
        public IList<int> CategoryIds { get; set; }
        [JsonProperty("contents")]
        public IList<CreateResourceContentModel> ResourceContents { get; set; }

    }

    public class CreateResourceContentModel : MappingModel<ResourceContent>
    {
        public CreateResourceContentModel()
        {
        }

        public CreateResourceContentModel(ResourceContent entity) : base(entity)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
    }

    public class UpdateResourceModel : MappingModel<Resource>
    {
        public UpdateResourceModel()
        {
        }

        public UpdateResourceModel(Resource entity) : base(entity)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("logo")]
        public FileDestination Logo { get; set; }
        [JsonProperty("image")]
        public FileDestination Image { get; set; }
        [JsonProperty("building_id")]
        public int BuildingId { get; set; }
        [JsonProperty("floor_id")]
        public int FloorId { get; set; }
        [JsonProperty("area_id")]
        public int AreaId { get; set; }
        [JsonProperty("category_ids")]
        public IList<int> CategoryIds { get; set; }
        [JsonProperty("new_contents")]
        public IList<CreateResourceContentModel> NewResourceContents { get; set; }
        [JsonProperty("update_contents")]
        public IList<UpdateResourceContentModel> UpdateResourceContents { get; set; }
        [JsonProperty("delete_content_ids")]
        public IList<int> DeleteResourceContentIds { get; set; }

    }

    public class UpdateResourceContentModel : MappingModel<ResourceContent>
    {
        public UpdateResourceContentModel()
        {
        }

        public UpdateResourceContentModel(ResourceContent entity) : base(entity)
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
