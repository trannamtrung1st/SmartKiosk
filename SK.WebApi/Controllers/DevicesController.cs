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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SK.WebApi.Helpers;
using SK.Data.Models;

namespace SK.WebApi.Controllers
{
    [Route(ApiEndpoint.DEVICE_API)]
    [ApiController]
    [InjectionFilter]
    public class DevicesController : BaseController
    {
        [Inject]
        private readonly DeviceService _service;
        [Inject]
        private readonly IdentityService _identityService;
        [Inject]
        private readonly ScheduleService _scheduleService;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]DeviceQueryFilter filter,
            [FromQuery]DeviceQuerySort sort,
            [FromQuery]DeviceQueryProjection projection,
            [FromQuery]DeviceQueryPaging paging,
            [FromQuery]DeviceQueryOptions options)
        {
            var validationResult = _service.ValidateGetDevices(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryDeviceDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.DeviceManager)]
        [HttpPost("")]
        public async Task<IActionResult> Create(CreateDeviceModel model)
        {
            if (ModelState.IsValid)
            {
                var validCode = _service.GetActivationCode(model.username);
                var validationResult = _service.ValidateCreateDevice(User, model, validCode);
                if (!validationResult.Valid)
                    return BadRequest(validationResult.Result);
                //create user account
                var account = _service.MakeDeviceAccount(model);
                using (var trans = context.Database.BeginTransaction())
                {
                    var result = await _identityService.CreateUserWithRolesTransactionAsync(account, model.password,
                        new List<string>() { Data.RoleName.Device });
                    if (result.Succeeded)
                    {
                        //create device
                        var entity = _service.CreateDevice(model, account.Id);
                        context.SaveChanges();
                        trans.Commit();
                        return Created($"/{ApiEndpoint.DEVICE_API}?id={entity.Id}",
                            new AppResultBuilder().Success(entity.Id));
                    }
                    foreach (var err in result.Errors)
                        ModelState.AddModelError(err.Code, err.Description);
                }
            }
            var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
            return BadRequest(builder.Results);
        }

        [AppAuthorize(Roles = Data.RoleName.DeviceManager)]
        [HttpPatch("{id}")]
        public IActionResult Update(string id, UpdateDeviceModel model)
        {
            var entity = _service.Devices.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateDevice(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            _service.UpdateDevice(entity, model);
            context.SaveChanges();
            return NoContent();
        }

        [AppAuthorize(Roles = Data.RoleName.DeviceManager)]
        [HttpPatch("schedule")]
        public IActionResult SetScheduleForDevices(SetScheduleForDevicesModel model)
        {
            Schedule schedule = null;
            if (model.ScheduleId != null)
            {
                schedule = _scheduleService.Schedules.Id(model.ScheduleId.Value)
                    .FirstOrDefault();
                if (schedule == null)
                    return NotFound(new AppResultBuilder().NotFound());
            }
            _service.SetScheduleForDevices(schedule, model.DeviceIds);
            context.SaveChanges();
            return Ok();
        }


        [AppAuthorize(Roles = Data.RoleName.DeviceManager)]
        [HttpPost("trigger")]
        public async Task<IActionResult> Trigger(TriggerDevicesModel model)
        {
            var validationResult = _service.ValidateTriggerDevices(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var resp = await _service.TriggerDevicesAsync(model);
            return Ok(new AppResultBuilder().Success(resp));
        }

    }
}