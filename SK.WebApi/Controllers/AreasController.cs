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
    [Route(ApiEndpoint.AREA_API)]
    [ApiController]
    [InjectionFilter]
    public class AreasController : BaseController
    {
        [Inject]
        private readonly AreaService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]AreaQueryFilter filter,
            [FromQuery]AreaQuerySort sort,
            [FromQuery]AreaQueryProjection projection,
            [FromQuery]AreaQueryPaging paging,
            [FromQuery]AreaQueryOptions options)
        {
            var validationResult = _service.ValidateGetAreas(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryAreaDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(new AppResultBuilder().NotFound());
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPost("")]
        public IActionResult Create(CreateAreaModel model)
        {
            var validationResult = _service.ValidateCreateArea(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateArea(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.AREA_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateAreaModel model)
        {
            var entity = _service.Areas.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateArea(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateArea(entity, model);
            context.SaveChanges();
            return NoContent();
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPatch("{id}/archived")]
        public IActionResult ChangeArchivedState(int id, ChangeArchivedStateModel model)
        {
            var entity = _service.Areas.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateChangeArchivedState(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.ChangeArchivedState(entity, model);
            context.SaveChanges();
            return NoContent();
        }

    }
}