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
    [Route(ApiEndpoint.SCHEDULE_API)]
    [ApiController]
    [InjectionFilter]
    public class SchedulesController : BaseController
    {
        [Inject]
        private readonly ScheduleService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]ScheduleQueryFilter filter,
            [FromQuery]ScheduleQuerySort sort,
            [FromQuery]ScheduleQueryProjection projection,
            [FromQuery]ScheduleQueryPaging paging,
            [FromQuery]ScheduleQueryOptions options)
        {
            var validationResult = _service.ValidateGetSchedules(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryScheduleDynamic(
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
        public IActionResult Create(CreateScheduleModel model)
        {
            var validationResult = _service.ValidateCreateSchedule(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateSchedule(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.SCHEDULE_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateScheduleModel model)
        {
            var entity = _service.Schedules.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateSchedule(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateSchedule(entity, model);
            context.SaveChanges();
            return NoContent();
        }

    }
}