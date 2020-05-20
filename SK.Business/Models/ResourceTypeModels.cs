using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{

    public class CreateResourceTypeModel : MappingModel<ResourceType>
    {
        public CreateResourceTypeModel()
        {
        }

        public CreateResourceTypeModel(ResourceType entity) : base(entity)
        {
        }

        [JsonProperty("contents")]
        public IList<CreateResourceTypeContentModel> Contents { get; set; }

    }

    public class CreateResourceTypeContentModel : MappingModel<ResourceTypeContent>
    {
        public CreateResourceTypeContentModel()
        {
        }

        public CreateResourceTypeContentModel(ResourceTypeContent entity) : base(entity)
        {
        }

        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class UpdateResourceTypeModel : MappingModel<ResourceType>
    {
        public UpdateResourceTypeModel()
        {
        }

        public UpdateResourceTypeModel(ResourceType entity) : base(entity)
        {
        }

        [JsonProperty("new_contents")]
        public IList<CreateResourceTypeContentModel> NewResourceTypeContents { get; set; }
        [JsonProperty("update_contents")]
        public IList<UpdateResourceTypeContentModel> UpdateResourceTypeContents { get; set; }
        [JsonProperty("delete_content_ids")]
        public IList<int> DeleteResourceTypeContentIds { get; set; }

    }

    public class UpdateResourceTypeContentModel : MappingModel<ResourceTypeContent>
    {
        public UpdateResourceTypeContentModel()
        {
        }

        public UpdateResourceTypeContentModel(ResourceTypeContent entity) : base(entity)
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    #region Query
    public class ResourceTypeQueryRow
    {
        public ResourceType ResourceType { get; set; }
        public ResourceTypeContentRelationship Content { get; set; }
    }

    public class ResourceTypeQueryProjection
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

        private const string RT = nameof(ResourceType);
        private const string C = nameof(ResourceTypeContent);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{RT}.{nameof(ResourceType.Id)}," +
                    $"{RT}.{nameof(ResourceType.Archived)}"
                },
                {
                    CONTENT,
                    $"{C}.{nameof(ResourceTypeContent.Id)} as [{C}.{nameof(ResourceTypeContent.Id)}]," +
                    $"{C}.{nameof(ResourceTypeContent.Lang)} as [{C}.{nameof(ResourceTypeContent.Lang)}]," +
                    $"{C}.{nameof(ResourceTypeContent.Name)} as [{C}.{nameof(ResourceTypeContent.Name)}]"
                }
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    CONTENT, $"LEFT JOIN (SELECT * FROM {C} " +
                    $"{ResourceTypeQueryPlaceholder.RES_TYPE_CONTENT_FILTER}) as {C} " +
                    $"ON {C}.{nameof(ResourceTypeContent.ResourceTypeId)}={RT}.{nameof(ResourceType.Id)}"
                },
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, new PartialResult(key: INFO, type: typeof(ResourceType),
                         splitOn: $"{nameof(ResourceType.Id)}")
                 },
                 {
                     CONTENT, new PartialResult(key: CONTENT, type: typeof(ResourceTypeContentRelationship),
                         splitOn: $"{C}.{nameof(ResourceTypeContent.Id)}")
                 },
             };
    }

    public class ResourceTypeQuerySort
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

    public class ResourceTypeQueryFilter
    {
        public int? id { get; set; }
        public string name_contains { get; set; }
        public string lang { get; set; }
        //0: false, 1: true, 2: both => default: false
        public byte archived { get; set; }
    }

    public class ResourceTypeQueryPaging
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

    public class ResourceTypeQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class ResourceTypeQueryPlaceholder
    {
        public const string RES_TYPE_CONTENT_FILTER = "$(res_type_content_filter)";
    }
    #endregion
}
