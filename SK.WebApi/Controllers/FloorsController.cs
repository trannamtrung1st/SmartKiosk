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
    [Route(ApiEndpoint.FLOOR_API)]
    [ApiController]
    [InjectionFilter]
    public class FloorsController : BaseController
    {
        [Inject]
        private readonly FloorService _service;
        [Inject]
        private readonly FileService _fileService;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]FloorQueryFilter filter,
            [FromQuery]FloorQuerySort sort,
            [FromQuery]FloorQueryProjection projection,
            [FromQuery]FloorQueryPaging paging,
            [FromQuery]FloorQueryOptions options)
        {
            var validationResult = _service.ValidateGetFloors(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryFloorDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPost("")]
        public IActionResult Create(CreateFloorModel model)
        {
            var validationResult = _service.ValidateCreateFloor(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateFloor(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.FLOOR_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateFloorModel model)
        {
            var entity = _service.Floors.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateFloor(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateFloor(entity, model);
            context.SaveChanges();
            return NoContent();
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPut("{id}/floor_plan")]
        public async Task<IActionResult> UpdateFloorPlan(int id, UpdateFloorPlanModel model)
        {
            var entity = _service.Floors.Id(id).FirstOrDefault();
            if (entity == null) return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateFloorPlan(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var metadata = GetFileDestinationMetadata();
            var file = await _fileService.GetFileAsync(model.File, metadata);
            var ext = file.Extension;
            if (!ext.Equals(".svg"))
                return BadRequest(ValidationResult.Fail(
                    new AppResultBuilder().FailValidation(mess: "Invalid file extension")));
            await _service.UpdateFloorPlanSvgAsync(entity, model, file);
            context.SaveChanges();
            return NoContent();
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPatch("{id}/floor_plan")]
        public IActionResult UpdateFloorPlanConfig(int id, [FromBody]string svg)
        {
            var entity = _service.Floors.Id(id).FirstOrDefault();
            if (entity == null) return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateFloorPlanConfig(
                User, entity, svg);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateFloorPlanSvg(entity, svg);
            context.SaveChanges();
            return NoContent();
        }

    }
}