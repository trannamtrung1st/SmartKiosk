using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class CreateOwnerModel : MappingModel<Owner>
    {
        public CreateOwnerModel()
        {
        }

        public CreateOwnerModel(Owner src) : base(src)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public class UpdateOwnerModel : MappingModel<Owner>
    {
        public UpdateOwnerModel()
        {
        }

        public UpdateOwnerModel(Owner src) : base(src)
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    #region Query
    public class OwnerQueryRow
    {
        public Owner Owner { get; set; }
    }

    public class OwnerQueryProjection
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
        public const string SELECT = "select";

        private const string O = nameof(Owner);
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{O}.{nameof(Owner.Id)},{O}.{nameof(Owner.Code)}," +
                    $"{O}.{nameof(Owner.Name)}," +
                    $"{O}.{nameof(Owner.Phone)}," +
                    $"{O}.{nameof(Owner.Description)}," +
                    $"{O}.{nameof(Owner.Archived)}"
                },
                {
                    SELECT,$"{O}.{nameof(Owner.Id)},{O}.{nameof(Owner.Code)}," +
                    $"{O}.{nameof(Owner.Name)}"
                },
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>();

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(Owner), splitOn: $"{nameof(Owner.Id)}");
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

    public class OwnerQuerySort
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

    public class OwnerQueryFilter
    {
        public int? id { get; set; }
        public string name_contains { get; set; }
    }

    public class OwnerQueryPaging
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

    public class OwnerQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class OwnerQueryPlaceholder
    {
    }

    public class OwnerRelationship : Owner, IDapperRelationship
    {
        public string GetTableName() => nameof(Owner);
    }

    #endregion
}
