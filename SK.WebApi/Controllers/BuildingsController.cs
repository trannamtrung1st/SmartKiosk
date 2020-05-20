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
    [Route(ApiEndpoint.BUILDING_API)]
    [ApiController]
    [InjectionFilter]
    public class BuildingsController : BaseController
    {
        [Inject]
        private readonly BuildingService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]BuildingQueryFilter filter,
            [FromQuery]BuildingQuerySort sort,
            [FromQuery]BuildingQueryProjection projection,
            [FromQuery]BuildingQueryPaging paging,
            [FromQuery]BuildingQueryOptions options)
        {
            var validationResult = _service.ValidateGetBuildings(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryBuildingDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(new AppResultBuilder().NotFound());
            return Ok(new AppResultBuilder().Success(result));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPost("")]
        public IActionResult Create(CreateBuildingModel model)
        {
            var validationResult = _service.ValidateCreateBuilding(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateBuilding(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.BUILDING_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

    }
}