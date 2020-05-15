using Newtonsoft.Json;
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

        [JsonProperty("image")]
        public FileDestination Image { get; set; }
        [JsonProperty("contents")]
        public IList<CreatePostContentModel> PostContents { get; set; }

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
    }

    public class UpdatePostModel : MappingModel<Post>
    {
        public UpdatePostModel()
        {
        }

        public UpdatePostModel(Post entity) : base(entity)
        {
        }

        [JsonProperty("image")]
        public FileDestination Image { get; set; }
        [JsonProperty("new_contents")]
        public IList<CreatePostContentModel> NewPostContents { get; set; }
        [JsonProperty("update_contents")]
        public IList<UpdatePostContentModel> UpdatePostContents { get; set; }
        [JsonProperty("delete_content_ids")]
        public IList<int> DeletePostContentIds { get; set; }

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
    }

    #region Query
    public class PostWithContent
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ContentId { get; set; }
        public string ImageUrl { get; set; }
        public string Lang { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        #region Join
        public Post Post { get; set; }
        public PostContent PostContent { get; set; }
        #endregion
    }

    public class PostQueryProjection
    {
        private const string DEFAULT = INFO + "," + CONTENT + "," + CONTENT_OVERVIEW;
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
                    _fieldsArr = value.Split(',');
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
        public const string CONTENT_OVERVIEW = "content.overview";
        public const string CONTENT_CONTENT = "content.content";

        private const string P = nameof(Post);
        private const string C = nameof(PostContent);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{P}.{nameof(Post.Id)},{P}.{nameof(Post.CreatedTime)}," +
                    $"{P}.{nameof(Post.ImageUrl)}"
                },
                {
                    CONTENT_OVERVIEW,
                    $"{C}.{nameof(PostContent.Id)} " +
                    $"as [{C}.{nameof(PostContent.Id)}]," +
                    $"{C}.{nameof(PostContent.Lang)}," +
                    $"{C}.{nameof(PostContent.Title)}"
                },
                {
                    CONTENT_CONTENT,
                    $"{C}.{nameof(PostContent.Content)}"
                }
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>()
            {
                {
                    CONTENT, $"LEFT JOIN (SELECT * FROM {C} " +
                    $"{PostQueryPlaceholder.POST_CONTENT_FILTER}) as {C} " +
                    $"ON {C}.{nameof(PostContent.PostId)}={P}.{nameof(Post.Id)}"
                }
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
