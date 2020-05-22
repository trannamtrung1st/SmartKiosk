using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.General;

namespace SK.Business.Models
{
    public class AppResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("code")]
        public AppResultCode? Code { get; set; }

    }

    public class AppResultBuilder
    {

        public AppResultBuilder()
        {
            Results = new List<AppResult>();
        }

        [JsonProperty("results")]
        public List<AppResult> Results { get; }

        public AppResultBuilder Success(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.Success,
                Message = mess ?? AppResultCode.Success.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder Error(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.UnknownError,
                Message = mess ?? AppResultCode.UnknownError.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder DependencyDeleteFail(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.DependencyDeleteFail,
                Message = mess ?? AppResultCode.DependencyDeleteFail.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder FailValidation(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.FailValidation,
                Message = mess ?? AppResultCode.FailValidation.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder DefaultScheduleDetailExisted(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.DefaultScheduleDetailExisted,
                Message = mess ?? AppResultCode.DefaultScheduleDetailExisted.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder InvalidActivationCode(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.InvalidActivationCode,
                Message = mess ?? AppResultCode.InvalidActivationCode.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder NotFound(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.NotFound,
                Message = mess ?? AppResultCode.NotFound.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder Unsupported(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.Unsupported,
                Message = mess ?? AppResultCode.Unsupported.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder Unauthorized(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.Unauthorized,
                Message = mess ?? AppResultCode.Unauthorized.DisplayName(),
                Data = data,
            });
            return this;
        }

        public AppResultBuilder DuplicatedUsername(object data = null, string mess = null)
        {
            Results.Add(new AppResult
            {
                Code = AppResultCode.DuplicatedUsername,
                Message = mess ?? AppResultCode.DuplicatedUsername.DisplayName(),
                Data = data,
            });
            return this;
        }

    }
}
