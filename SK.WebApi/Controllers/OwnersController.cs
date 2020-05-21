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
    [Route(ApiEndpoint.OWNER_API)]
    [ApiController]
    [InjectionFilter]
    public class OwnersController : BaseController
    {
        [Inject]
        private readonly OwnerService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]OwnerQueryFilter filter,
            [FromQuery]OwnerQuerySort sort,
            [FromQuery]OwnerQueryProjection projection,
            [FromQuery]OwnerQueryPaging paging,
            [FromQuery]OwnerQueryOptions options)
        {
            var validationResult = _service.ValidateGetOwners(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryOwnerDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.OwnerManager)]
        [HttpPost("")]
        public IActionResult Create(CreateOwnerModel model)
        {
            var validationResult = _service.ValidateCreateOwner(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateOwner(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.OWNER_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.OwnerManager)]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateOwnerModel model)
        {
            var entity = _service.Owners.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateOwner(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateOwner(entity, model);
            context.SaveChanges();
            return NoContent();
        }

    }
}