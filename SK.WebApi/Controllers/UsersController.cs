﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SK.Business;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Business.Services;
using SK.Data;
using SK.Data.Helpers;
using SK.Data.Models;
using SK.WebApi.Filters;
using SK.WebApi.Helpers;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace SK.WebApi.Controllers
{

    [Route(ApiEndpoint.USER_API)]
    [ApiController]
    [InjectionFilter]
    public class UsersController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly IdentityService _service;
        [Inject]
        private readonly DeviceService _deviceService;

        #region OAuth
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromForm]AuthorizationGrantModel model)
        {
            var validationResult = _service.ValidateLogin(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            AppUser entity = null;
            switch (model.grant_type)
            {
                case "password":
                case null:
                    {
                        entity = await
                            _service.AuthenticateAsync(model.username, model.password);
                        if (entity == null)
                        {
                            return Unauthorized(new AppResultBuilder()
                                .Unauthorized(mess: "Invalid username or password"));
                        }
                    }
                    break;
                case "refresh_token":
                    {
                        var validResult = _service.ValidateRefreshToken(model.refresh_token);
                        if (validResult == null)
                        {
                            return Unauthorized(new AppResultBuilder()
                                .Unauthorized(mess: "Invalid refresh token"));
                        }
                        entity = await _service.GetUserByIdAsync(validResult.Identity.Name);
                        if (entity == null)
                        {
                            return Unauthorized(new AppResultBuilder()
                                .Unauthorized(mess: "Invalid user identity"));
                        }
                    }
                    break;
                default:
                    return BadRequest(new AppResultBuilder()
                        .Unsupported("Unsupported grant type"));
            }
            var identity =
                await _service.GetIdentityAsync(entity, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var utcNow = DateTime.UtcNow;
            var props = new AuthenticationProperties()
            {
                IssuedUtc = utcNow,
                ExpiresUtc = utcNow.AddHours(WebApi.Settings.Instance.TokenValidHours)
            };
            props.Parameters["refresh_expires"] = utcNow.AddHours(
                WebApi.Settings.Instance.RefreshTokenValidHours);
            var resp = _service.GenerateTokenResponse(principal, props, model.scope);
            _logger.CustomProperties(entity).Info("Login user");
            if (principal.IsInRole(RoleName.Device))
            {
                if (model.fcm_token == null)
                    return BadRequest(new AppResultBuilder()
                        .FailValidation(mess: "FCM Token is required for device login"));
                using (var trans = context.Database.BeginTransaction())
                {
                    var device = _deviceService.Devices.Id(entity.Id).FirstOrDefault();
                    var oldFcmToken = device.CurrentFcmToken;
                    _deviceService.ChangeDeviceToken(device, model.fcm_token, resp.access_token);
                    context.SaveChanges();
                    _service.LoginDevice(device, oldFcmToken, model.fcm_token);
                    trans.Commit();
                }
            }
            return Ok(resp);
        }
        #endregion

        [AppAuthorize(Roles = Data.RoleName.UserManager)]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]AppUserQueryFilter filter,
            [FromQuery]AppUserQuerySort sort,
            [FromQuery]AppUserQueryProjection projection,
            [FromQuery]AppUserQueryPaging paging,
            [FromQuery]AppUserQueryOptions options)
        {
            var validationResult = _service.ValidateGetAppUsers(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryAppUserDynamic(
                projection, options, filter, sort, paging, UserId);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [HttpGet("info")]
        [AppAuthorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var entity = await _service.GetUserByIdAsync(UserId);
            var roles = entity.UserRoles.Select(u => u.Role).ToList();
            var model = new UserInfoModel(entity)
            {
                Roles = roles.Select(r => new AppRoleModel(r)),
                CurrentLoggedIn = true
            };
            return Ok(new AppResultBuilder().Success(model));
        }

        [HttpGet("token-info")]
        [AppAuthorize]
        public IActionResult GetTokenInfo()
        {
            var resp = new TokenInfo(User);
            return Ok(new AppResultBuilder().Success(resp));
        }

        [HttpGet("profile")]
        [AppAuthorize]
        public async Task<IActionResult> GetProfile()
        {
            var validationResult = _service.ValidateGetProfile(User);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = await _service.GetUserByIdAsync(UserId);
            var data = _service.GetUserProfile(entity);
            return Ok(new AppResultBuilder().Success(data));
        }

        [HttpPost("")]
        public async Task<IActionResult> Register([FromForm]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var validationResult = _service.ValidateRegister(User, model);
                if (!validationResult.Valid)
                    return BadRequest(validationResult.Result);
                IdentityResult result;
                using (var trans = context.Database.BeginTransaction())
                {
                    var entity = _service.ConvertToUser(model);
                    result = await _service
                        .CreateUserWithRolesTransactionAsync(entity, model.password);
                    if (result.Succeeded)
                    {
                        trans.Commit();
                        _logger.CustomProperties(entity).Info("Register new user");
                        return NoContent();
                    }
                }
                foreach (var err in result.Errors)
                    ModelState.AddModelError(err.Code, err.Description);
            }
            var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
            return BadRequest(builder.Results);
        }

#if DEBUG
        #region Administration
        [HttpPost("role")]
        public async Task<IActionResult> AddRole(AddRolesToUserModel model)
        {
            var entity = await _service.GetUserByUserNameAsync(model.username);
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var result = await _service.AddRolesForUserAsync(entity, model.roles);
            if (result.Succeeded)
                return NoContent();
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(new AppResultBuilder().FailValidation(ModelState));
        }

        [HttpDelete("role")]
        public async Task<IActionResult> RemoveRole(RemoveRolesFromUserModel model)
        {
            var entity = await _service.GetUserByUserNameAsync(model.username);
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var result = await _service.RemoveUserFromRolesAsync(entity, model.roles);
            if (result.Succeeded)
                return NoContent();
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(new AppResultBuilder().FailValidation(ModelState));
        }
        #endregion
#endif
    }
}
