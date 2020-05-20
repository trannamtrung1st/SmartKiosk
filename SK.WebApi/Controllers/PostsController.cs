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

namespace SK.WebApi.Controllers
{
    [Route(ApiEndpoint.POST_API)]
    [ApiController]
    [InjectionFilter]
#if !DEBUG
    [Authorize]
#endif
    public class PostsController : BaseController
    {
        [Inject]
        private readonly PostService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]PostQueryFilter filter,
            [FromQuery]PostQuerySort sort,
            [FromQuery]PostQueryProjection projection,
            [FromQuery]PostQueryPaging paging,
            [FromQuery]PostQueryOptions options)
        {
            var validationResult = _service.ValidateGetPosts(
                User, filter, sort, projection, paging, options);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var result = await _service.QueryPostDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only)
            {
                if (result == null) return NotFound(new AppResultBuilder().NotFound());
                return Ok(new AppResultBuilder().Success(result.SingleResult));
            }
            return Ok(new AppResultBuilder().Success(result));
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(CreatePostModel model)
        {
            var validationResult = _service.ValidateCreatePost(User, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            var metadata = GetFileDestinationMetadata();
            var entity = await _service.CreatePostAsync(model, metadata);
            context.SaveChanges();
            return Created($"/{ApiEndpoint.POST_API}?id={entity.Id}",
                new AppResultBuilder().Success(entity.Id));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id,
            UpdatePostModel model)
        {
            var entity = _service.Posts.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(new AppResultBuilder().NotFound());
            var validationResult = _service.ValidateUpdatePost(User, entity, model);
            if (!validationResult.Valid)
                return BadRequest(validationResult.Result);
            using (var trans = context.Database.BeginTransaction())
            {
                var metadata = GetFileDestinationMetadata();
                await _service.UpdatePostTransactionAsync(entity, model, metadata);
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = _service.Posts.IdOnly().Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(new AppResultBuilder().NotFound());
                var validationResult = _service.ValidateDeletePost(User, entity);
                if (!validationResult.Valid)
                    return BadRequest(validationResult.Result);
                using (var trans = context.Database.BeginTransaction())
                {
                    await _service.DeletePostTransactionAsync(entity);
                    context.SaveChanges();
                    trans.Commit();
                }
                return NoContent();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
                return BadRequest(new AppResultBuilder().DependencyDeleteFail());
            }
        }

    }
}