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
        public IList<CreateResourceContentModel> Contents { get; set; }

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

    public class UpdateResourceModel
    {
        [JsonProperty("info")]
        public UpdateResourceInfoModel Info { get; set; }
        [JsonProperty("category_ids")]
        public IList<int> CategoryIds { get; set; }
        [JsonProperty("new_contents")]
        public IList<CreateResourceContentModel> NewResourceContents { get; set; }
        [JsonProperty("update_contents")]
        public IList<UpdateResourceContentModel> UpdateResourceContents { get; set; }
        [JsonProperty("delete_content_langs")]
        public IList<string> DeleteResourceContentLangs { get; set; }

    }

    public class UpdateResourceInfoModel : MappingModel<Resource>
    {
        public UpdateResourceInfoModel()
        {
        }

        public UpdateResourceInfoModel(Resource src) : base(src)
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
    }

    public class UpdateResourceContentModel : MappingModel<ResourceContent>
    {
        public UpdateResourceContentModel()
        {
        }

        public UpdateResourceContentModel(ResourceContent entity) : base(entity)
        {
        }

        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }


    #region Query
    public class ResourceRelationship : Resource, IDapperRelationship
    {
        public string GetTableName() => nameof(Resource);
    }

    public class ResourceQueryResult : Resource
    {
        public new ICollection<CateOfResQueryRow> CategoriesOfResources { get; set; }
    }

    public class ResourceQueryRow
    {
        public ResourceQueryResult Resource { get; set; }
        public ResourceContentRelationship Content { get; set; }
        public OwnerRelationship Owner { get; set; }
        public AreaRelationship Area { get; set; }
        public LocationRelationship Location { get; set; }
    }

    public class ResourceQueryProjection
    {
        private const string DEFAULT = INFO + "," + CONTENT;
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
        public const string CONTENT = "content";
        public const string CONTENT_CONTENT = "content.content";
        public const string LOCATION = "location";
        public const string OWNER = "owner";
        public const string AREA = "area";
        public const string CATEGORIES = "categories";

        private const string R = nameof(Resource);
        private const string RC = nameof(ResourceContent);
        private const string O = nameof(Owner);
        private const string A = nameof(Area);
        private const string L = nameof(Location);
        //extras
        private const string C = nameof(CategoriesOfResources);
        private const string E = nameof(EntityCategory);
        private const string EC = nameof(EntityCategoryContent);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{R}.{nameof(Resource.Id)}," +
                    $"{R}.{nameof(Resource.Archived)}," +
                    $"{R}.{nameof(Resource.AreaId)}," +
                    $"{R}.{nameof(Resource.BuildingId)}," +
                    $"{R}.{nameof(Resource.Code)}," +
                    $"{R}.{nameof(Resource.FloorId)}," +
                    $"{R}.{nameof(Resource.ImageUrl)}," +
                    $"{R}.{nameof(Resource.LocationId)}," +
                    $"{R}.{nameof(Resource.LogoUrl)}," +
                    $"{R}.{nameof(Resource.OwnerId)}," +
                    $"{R}.{nameof(Resource.Phone)}," +
                    $"{R}.{nameof(Resource.TypeId)}"
                },
                {
                    CONTENT,
                    $"{RC}.{nameof(ResourceContent.Id)} as [{RC}.{nameof(ResourceContent.Id)}]," +
                    $"{RC}.{nameof(ResourceContent.Description)} as [{RC}.{nameof(ResourceContent.Description)}]," +
                    $"{RC}.{nameof(ResourceContent.Lang)} as [{RC}.{nameof(ResourceContent.Lang)}]," +
                    $"{RC}.{nameof(ResourceContent.Name)} as [{RC}.{nameof(ResourceContent.Name)}]"
                },
                {
                    CONTENT_CONTENT,
                    $"{RC}.{nameof(ResourceContent.Content)} as [{RC}.{nameof(ResourceContent.Content)}]"
                },
                {
                    OWNER,
                    $"{O}.{nameof(Owner.Id)} as [{O}.{nameof(Owner.Id)}]," +
                    $"{O}.{nameof(Owner.Name)} as [{O}.{nameof(Owner.Name)}]," +
                    $"{O}.{nameof(Owner.Code)} as [{O}.{nameof(Owner.Code)}]"
                },
                {
                    AREA,
                    $"{A}.{nameof(Area.Id)} as [{A}.{nameof(Area.Id)}]," +
                    $"{A}.{nameof(Area.Name)} as [{A}.{nameof(Area.Name)}]," +
                    $"{A}.{nameof(Area.Code)} as [{A}.{nameof(Area.Code)}]"
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
                    CONTENT, $"LEFT JOIN (SELECT * FROM {RC} " +
                    $"{ResourceQueryPlaceholder.RES_CONTENT_FILTER}) as {RC} " +
                    $"ON {RC}.{nameof(ResourceContent.ResourceId)}={R}.{nameof(Resource.Id)}"
                },
                {
                    OWNER, $"INNER JOIN {O} " +
                    $"ON {O}.{nameof(Owner.Id)}={R}.{nameof(Resource.OwnerId)}"
                },
                {
                    AREA, $"INNER JOIN {A} " +
                    $"ON {A}.{nameof(Area.Id)}={R}.{nameof(Resource.AreaId)}"
                },
                {
                    LOCATION, $"INNER JOIN {L} " +
                    $"ON {L}.{nameof(Location.Id)}={R}.{nameof(Resource.LocationId)}"
                },
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, new PartialResult(key: INFO, type: typeof(ResourceQueryResult),
                         splitOn: $"{nameof(Resource.Id)}")
                 },
                 {
                     CONTENT, new PartialResult(key: CONTENT, type: typeof(ResourceContentRelationship),
                         splitOn: $"{RC}.{nameof(ResourceContent.Id)}")
                 },
                 {
                     OWNER, new PartialResult(key: OWNER, type: typeof(OwnerRelationship),
                         splitOn: $"{O}.{nameof(Owner.Id)}")
                 },
                 {
                     AREA, new PartialResult(key: AREA, type: typeof(AreaRelationship),
                         splitOn: $"{A}.{nameof(Area.Id)}")
                 },
                 {
                     LOCATION, new PartialResult(key: LOCATION, type: typeof(LocationRelationship),
                         splitOn: $"{L}.{nameof(Location.Id)}")
                 },
             };

        public static readonly IDictionary<string, string> Extras =
            new Dictionary<string, string>()
            {
                {
                    CATEGORIES, $"SELECT " +
                    $"{C}.{nameof(CategoriesOfResources.CategoryId)}," +
                    $"{C}.{nameof(CategoriesOfResources.ResourceId)}," +
                    $"{E}.{nameof(EntityCategory.Id)} as [{E}.{nameof(EntityCategory.Id)}]," +
                    $"{E}.{nameof(EntityCategory.Archived)} as [{E}.{nameof(EntityCategory.Archived)}]," +
                    $"{EC}.{nameof(EntityCategoryContent.Id)} as [{EC}.{nameof(EntityCategoryContent.Id)}]," +
                    $"{EC}.{nameof(EntityCategoryContent.Lang)} as [{EC}.{nameof(EntityCategoryContent.Lang)}]," +
                    $"{EC}.{nameof(EntityCategoryContent.Name)} as [{EC}.{nameof(EntityCategoryContent.Name)}]" +
                    $" FROM {C}\n" +
                    $"INNER JOIN ({ResourceQueryPlaceholder.RES_SUB_QUERY}) AS {R} " +
                    $"ON {C}.{nameof(CategoriesOfResources.ResourceId)}=" +
                    $"{R}.{nameof(Resource.Id)}\n" +
                    $"INNER JOIN (SELECT * FROM {E} " +
                    $"WHERE {E}.{nameof(EntityCategory.Archived)}=0) AS {E} " +
                    $"ON {C}.{nameof(CategoriesOfResources.CategoryId)}=" +
                    $"{E}.{nameof(EntityCategory.Id)}\n" +
                    $"INNER JOIN (SELECT * FROM {EC} " +
                    $"{ResourceQueryPlaceholder.CATE_CONTENT_FILTER}) AS {EC} " +
                    $"ON {E}.{nameof(EntityCategory.Id)}=" +
                    $"{EC}.{nameof(EntityCategoryContent.CategoryId)}\n"
                }
            };
    }

    public class ResourceQuerySort
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

    public class ResourceQueryFilter
    {
        public int? id { get; set; }
        public int? owner_id { get; set; }
        public int? loc_id { get; set; }
        public string name_contains { get; set; }
        public string lang { get; set; }
        //0: false, 1: true, 2: both => default: false
        public byte archived { get; set; }
    }

    public class ResourceQueryPaging
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

    public class ResourceQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class ResourceQueryPlaceholder
    {
        public const string CATE_CONTENT_FILTER = "$(cate_content_filter)";
        public const string RES_CONTENT_FILTER = "$(res_content_filter)";
        public const string RES_SUB_QUERY = "$(res_sub_query)";
    }
    #endregion
}
