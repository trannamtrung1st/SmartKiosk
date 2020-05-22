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
    [Route(ApiEndpoint.RES_TYPE_API)]
    [ApiController]
    [InjectionFilter]
    public class ResourceTypesController : BaseController
    {
        [Inject]
        private readonly ResourceTypeService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]ResourceTypeQueryFilter filter,
            [FromQuery]ResourceTypeQuerySort sort,
            [FromQuery]ResourceTypeQueryProjection projection,
            [FromQuery]ResourceTypeQueryPaging paging,
            [FromQuery]ResourceTypeQueryOptions options)
        {
            var validationResult = _service.ValidateGetResourceTypes(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryResourceTypeDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.AppManager)]
        [HttpPost("")]
        public IActionResult Create(CreateResourceTypeModel model)
        {
            var validationResult = _service.ValidateCreateResourceType(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateResourceType(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.RES_TYPE_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.AppManager)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, UpdateResourceTypeModel model)
        {
            var entity = _service.ResourceTypes.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateResourceType(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            using (var trans = context.Database.BeginTransaction())
            {
                await _service.UpdateResourceTypeTransactionAsync(entity, model);
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

    }
}