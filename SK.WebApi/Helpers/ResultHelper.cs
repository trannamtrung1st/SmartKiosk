using Microsoft.AspNetCore.Mvc.ModelBinding;
using SK.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebApi.Helpers
{
    public static class ResultHelper
    {
        public static AppResultBuilder MakeInvalidAccountRegistrationResults(ModelStateDictionary modelState)
        {
            var builder = new AppResultBuilder();
            if (modelState.ContainsKey("password")
                && modelState["password"].ValidationState == ModelValidationState.Invalid)
                builder.FailValidation(mess: "Invalid password");
            if (modelState.ContainsKey("confirm_password")
                && modelState["confirm_password"].ValidationState == ModelValidationState.Invalid)
                builder.FailValidation(mess: "The password and confirm password are not matched");
            if (modelState.ContainsKey("username")
                && modelState["username"].ValidationState == ModelValidationState.Invalid)
                builder.FailValidation(mess: "Invalid username");
            if (modelState.ContainsKey("DuplicateUserName")
                && modelState["DuplicateUserName"].ValidationState == ModelValidationState.Invalid)
                builder.DuplicatedUsername();
            return builder;
        }
    }
}
