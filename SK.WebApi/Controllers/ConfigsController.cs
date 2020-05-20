using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.DI;
using System.Data.SqlClient;
using SK.Business.Services;
using SK.Business.Models;
using SK.Business.Queries;
using Microsoft.EntityFrameworkCore;
using SK.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using SK.WebApi.Filters;

namespace SK.WebApi.Controllers
{
    [Route(ApiEndpoint.CONFIG_API)]
    [ApiController]
    [InjectionFilter]
    public class ConfigsController : BaseController
    {
        [Inject]
        private readonly ConfigService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]ConfigQueryFilter filter,
            [FromQuery]ConfigQuerySort sort,
            [FromQuery]ConfigQueryProjection projection,
            [FromQuery]ConfigQueryPaging paging,
            [FromQuery]ConfigQueryOptions options)
        {
            var validationResult = _service.ValidateGetConfigs(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryConfigDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.ConfigManager)]
        [HttpPost("")]
        public IActionResult Create(CreateConfigModel model)
        {
            var validationResult = _service.ValidateCreateConfig(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateConfig(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.CONFIG_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.ConfigManager)]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateConfigModel model)
        {
            var entity = _service.Configs.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateConfig(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateConfig(entity, model);
            context.SaveChanges();
            return NoContent();
        }

    }
}