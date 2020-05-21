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
    [Route(ApiEndpoint.LOCATION_API)]
    [ApiController]
    [InjectionFilter]
    public class LocationsController : BaseController
    {
        [Inject]
        private readonly LocationService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]LocationQueryFilter filter,
            [FromQuery]LocationQuerySort sort,
            [FromQuery]LocationQueryProjection projection,
            [FromQuery]LocationQueryPaging paging,
            [FromQuery]LocationQueryOptions options)
        {
            var validationResult = _service.ValidateGetLocations(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryLocationDynamic(
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
        public IActionResult Create(CreateLocationModel model)
        {
            var validationResult = _service.ValidateCreateLocation(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateLocation(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.LOCATION_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

    }
}