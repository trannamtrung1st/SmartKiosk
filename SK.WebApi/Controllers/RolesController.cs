#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SK.Business;
using SK.Business.Models;
using SK.Business.Services;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace SK.WebApi.Controllers
{
    [Route(ApiEndpoint.ROLE_API)]
    [ApiController]
    [InjectionFilter]
    public class RolesController : BaseController
    {
        [Inject]
        private readonly IdentityService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet("")]
        public IActionResult GetRoles()
        {
            var validationResult = _service.ValidateGetRoles(User);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = _service.Roles.ToList();
            return Ok(new AppResultBuilder().Success(result));
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            var validationResult = _service.ValidateCreateRole(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.CreateRoleAsync(model);
            if (result.Succeeded)
                return Ok(new AppResultBuilder().Success(result));
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(new AppResultBuilder()
                .FailValidation(ModelState));
        }

        [HttpPatch("{name}")]
        public async Task<IActionResult> UpdateRole(string name, UpdateRoleModel model)
        {
            var validationResult = _service.ValidateUpdateRole(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.GetRoleByName(name);
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var result = await _service.UpdateRoleAsync(entity, model);
            if (result.Succeeded)
                return Ok(new AppResultBuilder().Success(entity));
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(new AppResultBuilder()
                .FailValidation(ModelState));
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> RemoveRole(string name)
        {
            var entity = _service.GetRoleByName(name);
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateDeleteRole(User, entity);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.RemoveRoleAsync(entity);
            if (result.Succeeded)
                return Ok(new AppResultBuilder().Success(result));
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(new AppResultBuilder()
                .FailValidation(ModelState));
        }
    }
}
#endif