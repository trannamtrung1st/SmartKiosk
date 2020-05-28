using Newtonsoft.Json;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class TokenInfo
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("claims")]
        public IDictionary<string, IEnumerable<string>> Claims { get; }

        public TokenInfo() { }
        public TokenInfo(ClaimsPrincipal principal)
        {
            UserId = principal.Identity.Name;

            Claims = new Dictionary<string, IEnumerable<string>>(principal.Claims.GroupBy(c => c.Type)
                .Select(group => new KeyValuePair<string, IEnumerable<string>>(
                    group.Key, group.Select(c => c.Value).ToList())));
        }
    }

    public class AuthorizationGrantModel //Resource Owner Password Credentials Grant
    {
        [JsonProperty("username")]
        public string username { get; set; }
        //REQUIRED.  The resource owner username.

        [JsonProperty("password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        //REQUIRED.  The resource owner password.

        [JsonProperty("grant_type")]
        public string grant_type { get; set; }
        //REQUIRED. 
        //- password: grant username and password
        //- refresh_token: grant refresh_token

        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
        //OPTIONAL.  The refresh_token

        [JsonProperty("scope")]
        public string scope { get; set; }
        //OPTIONAL.  The scope of the access request as described by
        [JsonProperty("fcm_token")]
        public string fcm_token { get; set; }
    }

    public class AddRolesToUserModel
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("roles")]
        public List<string> roles { get; set; }
    }

    public class RemoveRolesFromUserModel
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("roles")]
        public List<string> roles { get; set; }
    }

    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool remember_me { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Required]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirm_password { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string full_name { get; set; }
    }

    public class TokenResponseModel
    {
        [JsonProperty("user_id")]
        public string user_id { get; set; }
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("token_type")]
        public string token_type { get; set; }
        [JsonProperty("expires_utc")]
        public string expires_utc { get; set; }
        [JsonProperty("issued_utc")]
        public string issued_utc { get; set; }
        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
        [JsonProperty("roles")]
        public IEnumerable<string> roles { get; set; }
    }

    #region Query
    public class UserInfoModel : MappingModel<AppUser>
    {
        public UserInfoModel()
        {
        }

        public UserInfoModel(AppUser entity) : base(entity)
        {
        }

        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonProperty("current_logged_in")]
        public bool CurrentLoggedIn { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<AppRoleModel> Roles { get; set; }
    }

    public class AppUserQueryResult : AppUser
    {
        public new ICollection<AppUserRoleQueryRow> UserRoles { get; set; }
    }

    public class AppUserRelationship : AppUser, IDapperRelationship
    {
        public string GetTableName() => AppUser.TBL_NAME;
    }

    public class AppUserQueryRow
    {
        public AppUserQueryResult AppUser { get; set; }
    }

    public class AppUserQueryProjection
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
        public const string ROLES = "roles";

        private const string U = AppUser.TBL_NAME;
        private const string R = AppRole.TBL_NAME;
        private const string UR = AppUserRole.TBL_NAME;
        public static readonly IDictionary<string, string> Projections =
            new Dictionary<string, string>()
            {
                {
                    INFO,$"{U}.{nameof(AppUser.Id)}," +
                    $"{U}.{nameof(AppUser.UserName)}," +
                    $"{U}.{nameof(AppUser.PhoneNumber)}," +
                    $"{U}.{nameof(AppUser.FullName)}"
                }
            };

        public static readonly IDictionary<string, string> Joins =
            new Dictionary<string, string>();

        private static readonly PartialResult infoResult =
            new PartialResult(key: INFO, type: typeof(AppUserQueryResult), splitOn: $"{nameof(AppUser.Id)}");
        public static readonly IDictionary<string, PartialResult> Results =
             new Dictionary<string, PartialResult>()
             {
                 {
                     INFO, infoResult
                 },
             };

        public static readonly IDictionary<string, string> Extras =
            new Dictionary<string, string>()
            {
                {
                    ROLES, $"SELECT {UR}.{nameof(AppUserRole.UserId)}," +
                    $"{UR}.{nameof(AppUserRole.RoleId)}," +
                    $"{R}.{nameof(AppRole.Id)} as [{R}.{nameof(AppRole.Id)}]," +
                    $"{R}.{nameof(AppRole.Name)} as [{R}.{nameof(AppRole.Name)}]," +
                    $"{R}.{nameof(AppRole.DisplayName)} as [{R}.{nameof(AppRole.DisplayName)}]," +
                    $"{R}.{nameof(AppRole.RoleType)} as [{R}.{nameof(AppRole.RoleType)}]" +
                    $" FROM {UR}\n" +
                    $"INNER JOIN ({AppUserQueryPlaceholder.USER_SUB_QUERY}) AS {U} " +
                    $"ON {U}.{nameof(AppUser.Id)}={UR}.{nameof(AppUserRole.UserId)}\n" +
                    $"INNER JOIN {R} AS {R} " +
                    $"ON {R}.{nameof(AppRole.Id)}={UR}.{nameof(AppUserRole.RoleId)}\n"
                }
            };
    }

    public class AppUserQuerySort
    {
        public const string USERNAME = "username";
        private const string DEFAULT = "a" + USERNAME;
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

    public class AppUserQueryFilter
    {
        public string id { get; set; }
    }

    public class AppUserQueryPaging
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

    public class AppUserQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    public class AppUserQueryPlaceholder
    {
        public const string USER_SUB_QUERY = "$(user_sub_query)";
    }
    #endregion
}
