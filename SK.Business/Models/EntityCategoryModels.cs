﻿using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{

    public class CreateEntityCategoryModel : MappingModel<EntityCategory>
    {
        public CreateEntityCategoryModel()
        {
        }

        public CreateEntityCategoryModel(EntityCategory entity) : base(entity)
        {
        }

        [JsonProperty("contents")]
        public IList<CreateEntityCategoryContentModel> Contents { get; set; }

    }

    public class CreateEntityCategoryContentModel : MappingModel<EntityCategoryContent>
    {
        public CreateEntityCategoryContentModel()
        {
        }

        public CreateEntityCategoryContentModel(EntityCategoryContent entity) : base(entity)
        {
        }

        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class UpdateEntityCategoryModel : MappingModel<EntityCategory>
    {
        public UpdateEntityCategoryModel()
        {
        }

        public UpdateEntityCategoryModel(EntityCategory entity) : base(entity)
        {
        }

        [JsonProperty("new_contents")]
        public IList<CreateEntityCategoryContentModel> NewEntityCategoryContents { get; set; }
        [JsonProperty("update_contents")]
        public IList<UpdateEntityCategoryContentModel> UpdateEntityCategoryContents { get; set; }
        [JsonProperty("delete_content_langs")]
        public IList<string> DeleteEntityCategoryContentLangs { get; set; }

    }

    public class UpdateEntityCategoryContentModel : MappingModel<EntityCategoryContent>
    {
        public UpdateEntityCategoryContentModel()
        {
        }

        public UpdateEntityCategoryContentModel(EntityCategoryContent entity) : base(entity)
        {
        }

        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    #region Query
    public class EntityCategoryQueryResult : EntityCategory
    {
        public new ICollection<CateOfResQueryRow> CategoriesOfResources { get; set; }
    }

    public class EntityCategoryRelationship : EntityCategory, IDapperRelationship
    {
        public string GetTableName() => nameof(EntityCategory);
    }

    public class EntityCategoryQueryRow
    {
        public EntityCategoryQueryResult EntityCategory { get; set; }
        public EntityCategoryContentRelationship Content { get; set; }
    }

    public class EntityCategoryQueryProjection
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
        public const string RESOURCES = "resources";

        private const string E = nameof(EntityCategory);
        private const string C = nameof(EntityCategoryContent);
        private const string CR = nameof(CategoriesOfResources);
        private const string R = nameof(Resource);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{E}.{nameof(EntityCategory.Id)}," +
                    $"{E}.{nameof(EntityCategory.Archived)}"
                },
                {
                    CONTENT,
                    $"{C}.{nameof(EntityCategoryContent.Id)} as [{C}.{nameof(EntityCategoryContent.Id)}]," +
                    $"{C}.{nameof(EntityCategoryContent.Lang)} as [{C}.{nameof(EntityCategoryContent.Lang)}]," +
                    $"{C}.{nameof(EntityCategoryContent.Name)} as [{C}.{nameof(EntityCategoryContent.Name)}]"
                }
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    CONTENT, $"LEFT JOIN (SELECT * FROM {C} " +
                    $"{EntityCategoryQueryPlaceholder.EC_CONTENT_FILTER}) as {C} " +
                    $"ON {C}.{nameof(EntityCategoryContent.CategoryId)}={E}.{nameof(EntityCategory.Id)}"
                },
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, new PartialResult(key: INFO, type: typeof(EntityCategoryQueryResult),
                         splitOn: $"{nameof(EntityCategory.Id)}")
                 },
                 {
                     CONTENT, new PartialResult(key: CONTENT, type: typeof(EntityCategoryContentRelationship),
                         splitOn: $"{C}.{nameof(EntityCategoryContent.Id)}")
                 },
             };

        public static readonly IDictionary<string, string> Extras =
            new Dictionary<string, string>()
            {
                {
                    RESOURCES, $"SELECT {CR}.{nameof(CategoriesOfResources.CategoryId)}," +
                    $"{CR}.{nameof(CategoriesOfResources.ResourceId)}," +
                    $"{R}.{nameof(Resource.Id)} as [{R}.{nameof(Resource.Id)}]," +
                    $"{R}.{nameof(Resource.Code)} as [{R}.{nameof(Resource.Code)}]," +
                    $"{R}.{nameof(Resource.ImageUrl)} as [{R}.{nameof(Resource.ImageUrl)}]," +
                    $"{R}.{nameof(Resource.LogoUrl)} as [{R}.{nameof(Resource.LogoUrl)}]" +
                    $" FROM {nameof(CategoriesOfResources)}\n" +
                    $"INNER JOIN ({EntityCategoryQueryPlaceholder.EC_SUB_QUERY}) AS {E} " +
                    $"ON {CR}.{nameof(CategoriesOfResources.CategoryId)}={E}.{nameof(EntityCategory.Id)}\n"+
                    $"INNER JOIN (SELECT * FROM {R} {EntityCategoryQueryPlaceholder.RESOURCES_FILTER}) AS {R} " +
                    $"ON {R}.{nameof(Resource.Id)}={CR}.{nameof(CategoriesOfResources.ResourceId)}\n"
                }
            };
    }

    public class EntityCategoryQuerySort
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

    public class EntityCategoryQueryFilter
    {
        public int? floor_id { get; set; }
        public int? id { get; set; }
        public string name_contains { get; set; }
        public string lang { get; set; }
        //0: false, 1: true, 2: both => default: false
        public byte archived { get; set; }
    }

    public class EntityCategoryQueryPaging
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

    public class EntityCategoryQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class EntityCategoryQueryPlaceholder
    {
        public const string RESOURCES_FILTER = "$(resources_filter)";
        public const string EC_SUB_QUERY = "$(ec_sub_query)";
        public const string EC_CONTENT_FILTER = "$(ec_content_filter)";
    }
    #endregion
}
