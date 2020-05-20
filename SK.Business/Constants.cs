using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business
{
    public enum AppResultCode
    {
        [Display(Name = "Unknown error")]
        UnknownError = 1,
        [Display(Name = "Success")]
        Success = 2,
        [Display(Name = "Fail validation")]
        FailValidation = 3,
        [Display(Name = "Not found")]
        NotFound = 4,
        [Display(Name = "Unsupported")]
        Unsupported = 5,
        [Display(Name = "Can not delete because of dependencies")]
        DependencyDeleteFail = 6,
        [Display(Name = "Unauthorized")]
        Unauthorized = 7,
        [Display(Name = "Username has already existed")]
        DuplicatedUsername = 8

    }

    public class JWT
    {
        public const string ISSUER = "smartkiosk1st";
        public const string AUDIENCE = "smartkiosk1st";
        public const string SECRET_KEY = "ASDFOIPJJP812340-89ADSFPOUADSFH809-3152-798OHASDFHPOU1324-8ASDF";

        public const string REFRESH_ISSUER = "refresh_smartkiosk1st";
        public const string REFRESH_AUDIENCE = "refresh_smartkiosk1st";
        public const string REFRESH_SECRET_KEY = "FSPDIU2093T-ASDGPIOSDGPHASDG-EWRQWGWQEGWE-QWER-QWER13412-AQRQWR";
    }

    public static class AppClaimType
    {
        public const string UserName = "username";
    }

    public static class ApiEndpoint
    {
        public const string ROLE_API = "api/roles";
        public const string USER_API = "api/users";
        public const string POST_API = "api/posts";
        public const string AREA_API = "api/areas";
        public const string CONFIG_API = "api/configs";
        public const string BUILDING_API = "api/buildings";
        public const string FILE_API = "api/files";
        public const string ERROR = "error";
    }

    public static class AppTimeZone
    {
        public static IDictionary<string, TimeZoneInfo> Map =
            new Dictionary<string, TimeZoneInfo>()
            {
                { Lang.VI, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") },
                { Lang.EN, TimeZoneInfo.Utc },
            };
    }

    public static class ActivationCodeSecrect
    {
        public const string SECRET = "smartkiosk777777";
    }
}
