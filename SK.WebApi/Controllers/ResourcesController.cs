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
    [Route(ApiEndpoint.RESOURCE_API)]
    [ApiController]
    [InjectionFilter]
    public class ResourcesController : BaseController
    {
        [Inject]
        private readonly ResourceService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]ResourceQueryFilter filter,
            [FromQuery]ResourceQuerySort sort,
            [FromQuery]ResourceQueryProjection projection,
            [FromQuery]ResourceQueryPaging paging,
            [FromQuery]ResourceQueryOptions options)
        {
            var validationResult = _service.ValidateGetResources(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryResourceDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.LocationManager)]
        [HttpPost("")]
        public async Task<IActionResult> Create(CreateResourceModel model)
        {
            var validationResult = _service.ValidateCreateResource(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var metadata = GetFileDestinationMetadata();
            var entity = await _service.CreateResourceAsync(model, metadata);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.LocationManager)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, UpdateResourceModel model)
        {
            var entity = _service.Resources.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateResource(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            using (var trans = context.Database.BeginTransaction())
            {
                await _service.UpdateResourceTransactionAsync(entity, model);
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

    }
}