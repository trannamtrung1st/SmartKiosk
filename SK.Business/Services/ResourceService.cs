using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NLog.Fluent;
using SK.Business.Models;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.Data;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class ResourceService : Service
    {
        [Inject]
        private readonly FileService _fileService;

        public ResourceService(ServiceInjection inj) : base(inj)
        {
        }

        #region Create Resource
        protected void PrepareCreate(Resource entity)
        {
        }

        public async Task<Resource> CreateResourceAsync(CreateResourceModel model,
            FileDestinationMetadata metadata)
        {
            var entity = model.ToDest();
            if (model.Image != null)
                await SetResourceImageUrlAsync(entity, model.Image, metadata);
            if (model.Logo != null)
                await SetResourceImageUrlAsync(entity, model.Logo, metadata);
            AddResourceCategoriesToResource(model.CategoryIds, entity);
            PrepareCreate(entity);
            return context.Resource.Add(entity).Entity;
        }
        #endregion

        #region Update Resource
        private void CreateResourceContents(IList<CreateResourceContentModel> model, Resource entity)
        {
            var entities = model.Select(o =>
            {
                var content = o.ToDest();
                content.ResourceId = entity.Id;
                return content;
            }).ToList();
            context.ResourceContent.AddRange(entities);
        }

        private void UpdateResourceContents(IList<UpdateResourceContentModel> model)
        {
            foreach (var o in model)
            {
                var entity = new ResourceContent();
                entity.Id = o.Id;
                context.Attach(entity);
                o.CopyTo(entity);
            }
        }

        private void AddResourceCategoriesToResource(IList<int> ids, Resource entity)
        {
            var categories = ids.Select(id => new CategoriesOfResources { CategoryId = id })
                .ToList();
            entity.CategoriesOfResources = categories;
        }

        public async Task UpdateResourceTransactionAsync(Resource entity,
            UpdateResourceModel model,
            FileDestinationMetadata metadata = null)
        {
            if (model.NewResourceContents != null)
                CreateResourceContents(model.NewResourceContents, entity);
            if (model.UpdateResourceContents != null)
                UpdateResourceContents(model.UpdateResourceContents);
            if (model.DeleteResourceContentIds != null)
                await DeleteResourceContentByIdsAsync(model.DeleteResourceContentIds);
            if (model.CategoryIds != null)
            {
                await DeleteAllCategoriesOfResourceAsync(entity);
                AddResourceCategoriesToResource(model.CategoryIds, entity);
            }
            if (model.Image != null)
                await SetResourceImageUrlAsync(entity, model.Image, metadata);
            if (model.Logo != null)
                await SetResourceLogoUrlAsync(entity, model.Logo, metadata);
        }
        #endregion


        #region Delete Resource
        protected async Task<int> DeleteResourceContentByIdsAsync(IEnumerable<int> ids)
        {
            var parameters = ids.GetDataParameters("id");
            var sql = $"DELETE FROM {nameof(ResourceContent)} WHERE " +
                $"{nameof(ResourceContent.Id)} IN " +
                $"({parameters.Placeholder})";
            var sqlParams = parameters.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value));
            var result = await context.Database
                .ExecuteSqlRawAsync(sql, sqlParams);
            return result;
        }

        protected async Task<int> DeleteAllContentsOfResourceAsync(Resource entity)
        {
            var id = new SqlParameter("id", entity.Id);
            var sql = $"DELETE FROM {nameof(ResourceContent)} WHERE " +
                $"{nameof(ResourceContent.ResourceId)}={id.ParameterName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql, id);
            return result;
        }

        protected async Task<int> DeleteAllCategoriesOfResourceAsync(Resource entity)
        {
            var id = new SqlParameter("id", entity.Id);
            var sql = $"DELETE FROM {nameof(CategoriesOfResources)} WHERE " +
                $"{nameof(CategoriesOfResources.ResourceId)}={id.ParameterName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql, id);
            return result;
        }

        public async Task<Resource> DeleteResourceTransactionAsync(Resource entity)
        {
            await DeleteAllContentsOfResourceAsync(entity);
            return context.Resource.Remove(entity).Entity;
        }
        #endregion

        protected async Task SetResourceImageUrlAsync(Resource entity,
            FileDestination image,
            FileDestinationMetadata metadata)
        {
            var imageUrl = await _fileService.GetFileUrlAsync(image, metadata);
            entity.ImageUrl = imageUrl;
        }

        protected async Task SetResourceLogoUrlAsync(Resource entity,
            FileDestination logo,
            FileDestinationMetadata metadata)
        {
            var logoUrl = await _fileService.GetFileUrlAsync(logo, metadata);
            entity.LogoUrl = logoUrl;
        }

    }
}
