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
    [Route(ApiEndpoint.SCHEDULE_DETAIL_API)]
    [ApiController]
    [InjectionFilter]
    public class ScheduleDetailsController : BaseController
    {
        [Inject]
        private readonly ScheduleDetailService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]ScheduleDetailQueryFilter filter,
            [FromQuery]ScheduleDetailQuerySort sort,
            [FromQuery]ScheduleDetailQueryProjection projection,
            [FromQuery]ScheduleDetailQueryPaging paging,
            [FromQuery]ScheduleDetailQueryOptions options)
        {
            var validationResult = _service.ValidateGetScheduleDetails(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryScheduleDetailDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.ScheduleManager)]
        [HttpPost("")]
        public IActionResult Create(CreateScheduleDetailModel model)
        {
            var validationResult = _service.ValidateCreateScheduleDetail(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            if (model.IsDefault == true)
            {
                var isDefaultExisted = _service.ScheduleDetails
                    .BySchedule(model.ScheduleId.Value)
                    .IsDefault().Any();
                if (isDefaultExisted)
                    return BadRequest(ValidationResult.Fail(
                        new AppResultBuilder().DefaultScheduleDetailExisted()));
            }
            var entity = _service.CreateScheduleDetail(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.SCHEDULE_DETAIL_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.ScheduleManager)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, UpdateScheduleDetailModel model)
        {
            var entity = _service.ScheduleDetails.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateScheduleDetail(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            if (model.IsDefault == true)
            {
                var defaultEntity = _service.ScheduleDetails
                    .BySchedule(entity.ScheduleId)
                    .IsDefault().IdOnly().FirstOrDefault();
                if (defaultEntity?.Id != entity.Id)
                    return BadRequest(ValidationResult.Fail(
                        new AppResultBuilder().DefaultScheduleDetailExisted()));
            }
            using (var trans = context.Database.BeginTransaction())
            {
                await _service.UpdateScheduleDetailTransactionAsync(entity, model);
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

    }
}