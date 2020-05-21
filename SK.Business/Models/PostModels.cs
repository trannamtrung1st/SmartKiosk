using Newtonsoft.Json;
using SK.Data;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreatePostModel : MappingModel<Post>
    {
        public CreatePostModel()
        {
        }

        public CreatePostModel(Post entity) : base(entity)
        {
        }

        [JsonProperty("type")]
        public PostType Type { get; set; }
        [JsonProperty("visible_time")]
        public DateTime? VisibleTime { get; set; }
        [JsonProperty("location_id")]
        public int LocationId { get; set; }
        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }
        [JsonProperty("image")]
        public FileDestination Image { get; set; }
        [JsonProperty("contents")]
        public IList<CreatePostContentModel> Contents { get; set; }

    }

    public class CreatePostContentModel : MappingModel<PostContent>
    {
        public CreatePostContentModel()
        {
        }

        public CreatePostContentModel(PostContent entity) : base(entity)
        {
        }

        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class UpdatePostModel
    {
        [JsonProperty("info")]
        public UpdatePostInfoModel Info { get; set; }
        [JsonProperty("image")]
        public FileDestination Image { get; set; }
        [JsonProperty("new_contents")]
        public IList<CreatePostContentModel> NewPostContents { get; set; }
        [JsonProperty("update_contents")]
        public IList<UpdatePostContentModel> UpdatePostContents { get; set; }
        [JsonProperty("delete_content_ids")]
        public IList<int> DeletePostContentIds { get; set; }

    }

    public class UpdatePostInfoModel : MappingModel<Post>
    {
        public UpdatePostInfoModel()
        {
        }

        public UpdatePostInfoModel(Post src) : base(src)
        {
        }

        [JsonProperty("visible_time")]
        public DateTime? VisibleTime { get; set; }
    }

    public class UpdatePostContentModel : MappingModel<PostContent>
    {
        public UpdatePostContentModel()
        {
        }

        public UpdatePostContentModel(PostContent entity) : base(entity)
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    #region Query
    public class PostQueryRow
    {
        public Post Post { get; set; }
        public PostContentRelationship Content { get; set; }
        public OwnerRelationship Owner { get; set; }
    }

    public class PostQueryProjection
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
        public const string OWNER = "owner";

        private const string P = nameof(Post);
        private const string C = nameof(PostContent);
        private const string O = nameof(Owner);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{P}.{nameof(Post.Id)},{P}.{nameof(Post.CreatedTime)}," +
                    $"{P}.{nameof(Post.VisibleTime)}," +
                    $"{P}.{nameof(Post.Archived)}," +
                    $"{P}.{nameof(Post.LocationId)}," +
                    $"{P}.{nameof(Post.OwnerId)}," +
                    $"{P}.{nameof(Post.Type)}," +
                    $"{P}.{nameof(Post.ImageUrl)}"
                },
                {
                    CONTENT,
                    $"{C}.{nameof(PostContent.Id)} as [{C}.{nameof(PostContent.Id)}]," +
                    $"{C}.{nameof(PostContent.Lang)} as [{C}.{nameof(PostContent.Lang)}]," +
                    $"{C}.{nameof(PostContent.Description)} as [{C}.{nameof(PostContent.Description)}]," +
                    $"{C}.{nameof(PostContent.Title)} as [{C}.{nameof(PostContent.Title)}]"
                },
                {
                    CONTENT_CONTENT,
                    $"{C}.{nameof(PostContent.Content)} as [{C}.{nameof(PostContent.Content)}]"
                },
                {
                    OWNER,
                    $"{O}.{nameof(Owner.Id)} as [{O}.{nameof(Owner.Id)}]," +
                    $"{O}.{nameof(Owner.Name)} as [{O}.{nameof(Owner.Name)}]," +
                    $"{O}.{nameof(Owner.Code)} as [{O}.{nameof(Owner.Code)}]"
                }
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    CONTENT, $"LEFT JOIN (SELECT * FROM {C} " +
                    $"{PostQueryPlaceholder.POST_CONTENT_FILTER}) as {C} " +
                    $"ON {C}.{nameof(PostContent.PostId)}={P}.{nameof(Post.Id)}"
                },
                {
                    OWNER, $"INNER JOIN {O} " +
                    $"ON {O}.{nameof(Owner.Id)}={P}.{nameof(Post.OwnerId)}"
                },
            };

        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, new PartialResult(key: INFO, type: typeof(Post),
                         splitOn: $"{nameof(Post.Id)}")
                 },
                 {
                     CONTENT, new PartialResult(key: CONTENT, type: typeof(PostContentRelationship),
                         splitOn: $"{C}.{nameof(PostContent.Id)}")
                 },
                 {
                     OWNER, new PartialResult(key: OWNER, type: typeof(OwnerRelationship),
                         splitOn: $"{O}.{nameof(Owner.Id)}")
                 },
             };
    }

    public class PostQuerySort
    {
        public const string TITLE = "title";
        public const string CREATED_TIME = "created_time";
        private const string DEFAULT = "d" + CREATED_TIME;
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

    public class PostQueryFilter
    {
        public int? id { get; set; }
        public int[] ids { get; set; }
        public int? not_eq_id { get; set; }
        public string title_contains { get; set; }
        public string lang { get; set; }
        //0: false, 1: true, 2: both => default: false
        public byte archived { get; set; }
    }

    public class PostQueryPaging
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

    public class PostQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class PostQueryPlaceholder
    {
        public const string POST_CONTENT_FILTER = "$(post_content_filter)";
    }
    #endregion
}
