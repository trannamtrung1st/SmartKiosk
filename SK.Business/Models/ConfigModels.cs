using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateConfigModel : MappingModel<Config>
    {
        public CreateConfigModel()
        {
        }

        public CreateConfigModel(Config entity) : base(entity)
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

    public class UpdateConfigModel
    {
        [JsonProperty("info")]
        public UpdateConfigInfoModel Info { get; set; }
        [JsonProperty("ssp_config")]
        public ScreenSaverPlaylist SSP { get; set; }
        [JsonProperty("home_page_config")]
        public HomePageConfig HomePage { get; set; }
        [JsonProperty("pe_config")]
        public PostsConfig ProgramEvent { get; set; }
        [JsonProperty("contact_config")]
        public ContactConfig Contact { get; set; }
    }

    public class UpdateConfigInfoModel : MappingModel<Config>
    {
        public UpdateConfigInfoModel()
        {
        }

        public UpdateConfigInfoModel(Config src) : base(src)
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
    public class ConfigQueryRow
    {
        public Config Config { get; set; }
    }

    public class ConfigQueryProjection
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
        public const string DETAIL = "detail";
        public const string SELECT = "select";

        private const string C = nameof(Config);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{C}.{nameof(Config.Id)},{C}.{nameof(Config.Code)}," +
                    $"{C}.{nameof(Config.Name)}," +
                    $"{C}.{nameof(Config.Description)}," +
                    $"{C}.{nameof(Config.LocationId)}"
                },
                {
                    SELECT,$"{C}.{nameof(Config.Id)},{C}.{nameof(Config.Code)}," +
                    $"{C}.{nameof(Config.Name)}"
                },
                {
                    DETAIL,
                    $"{C}.{nameof(Config.ScreenSaverPlaylist)}," +
                    $"{C}.{nameof(Config.ContactConfig)}," +
                    $"{C}.{nameof(Config.HomeConfig)}," +
                    $"{C}.{nameof(Config.MapConfig)}," +
                    $"{C}.{nameof(Config.NotiConfig)}," +
                    $"{C}.{nameof(Config.ProgramEventConfig)}"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>();

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(Config), splitOn: $"{nameof(Config.Id)}");
        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, infoResult
                 },
                 {
                     SELECT, infoResult
                 }
             };
    }

    public class ConfigQuerySort
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

    public class ConfigQueryFilter
    {
        public int? loc_id { get; set; }
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class ConfigQueryPaging
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

    public class ConfigQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class ConfigQueryPlaceholder
    {
    }
    #endregion
}
