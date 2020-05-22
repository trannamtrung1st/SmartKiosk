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
    [Route(ApiEndpoint.ENTITY_CATE_API)]
    [ApiController]
    [InjectionFilter]
    public class EntityCategoriesController : BaseController
    {
        [Inject]
        private readonly EntityCategoryService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [AppAuthorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]EntityCategoryQueryFilter filter,
            [FromQuery]EntityCategoryQuerySort sort,
            [FromQuery]EntityCategoryQueryProjection projection,
            [FromQuery]EntityCategoryQueryPaging paging,
            [FromQuery]EntityCategoryQueryOptions options)
        {
            var validationResult = _service.ValidateGetEntityCategories(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryEntityCategoryDynamic(
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
        public IActionResult Create(CreateEntityCategoryModel model)
        {
            var validationResult = _service.ValidateCreateEntityCategory(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var entity = _service.CreateEntityCategory(model);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.ENTITY_CATE_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [AppAuthorize(Roles = Data.RoleName.BuildingManager)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEntityCategoryModel model)
        {
            var entity = _service.EntityCategories.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdateEntityCategory(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            using (var trans = context.Database.BeginTransaction())
            {
                await _service.UpdateEntityCategoryTransactionAsync(entity, model);
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

    }
}