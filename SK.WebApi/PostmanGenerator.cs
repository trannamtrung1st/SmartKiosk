using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using Newtonsoft.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using SK.Business.Models;
using SK.Data;
using TNT.Core.WebApi.Postman;
using SK.Business;

namespace SK.WebApi
{

    class PostmanGenerator
    {
        public static void Generate()
        {
            var apiVarName = "web_api_url";
            var apiVarHolder = apiVarName.ToPlaceholder();
            var random = new Random();

            #region PostsController
            var getPosts = new RequestItemBuilder()
                .Name("Get posts")
                .Method(HttpMethod.Get.Method)
                .Url($"{apiVarHolder}/{ApiEndpoint.POST_API}", new List<Query>
                {
                    new Query{ key = "lang", value = Lang.VI },
                    new Query{ key = "fields", value =
                        $"{PostQueryProjection.INFO}," +
                        $"{PostQueryProjection.CONTENT}" },
                    new Query{ key = "page", value = "1" },
                    new Query{ key = "limit", value = "50" },
                    new Query{ key = "sorts", value = $"d{PostQuerySort.TITLE}" },
                    new Query{ key = "count_total", value = "true" },
                    new Query{ key = "date_format", value = "dd/MM/yyyy hh:mm:yyyy" },
                    new Query{ key = "time_zone", value = AppTimeZone.Map[Lang.VI].StandardName },
                    new Query{ key = "culture", value = Lang.VI },
                })
                .Description("Get all posts")
                .Build();

            var createPostModel = new CreatePostModel
            {
                Contents = new List<CreatePostContentModel>
                {
                    new CreatePostContentModel
                    {
                        Lang = Lang.VI,
                        Title = "Bài viết " + Guid.NewGuid().ToString(),
                        Content = "Nội dung " + Guid.NewGuid().ToString(),
                    }
                }
            };
            var createPostJson = JsonConvert.SerializeObject(createPostModel, Formatting.Indented);
            var createPost = new RequestItemBuilder()
                .Name("Create post")
                .Method(HttpMethod.Post.Method)
                .Url($"{apiVarHolder}/{ApiEndpoint.POST_API}")
                .JsonBody(createPostJson)
                .Description("Create a post")
                .Build();

            var updatePostModel = new UpdatePostModel
            {
                NewPostContents = new List<CreatePostContentModel>
                {
                    new CreatePostContentModel
                    {
                        Lang = Lang.VI,
                        Title = "Bài viết " + Guid.NewGuid().ToString(),
                        Content = "Nội dung " + Guid.NewGuid().ToString(),
                    }
                },
                UpdatePostContents = new List<UpdatePostContentModel>
                {
                    new UpdatePostContentModel
                    {
                        Lang = "vi",
                        Title = "Bài viết " + Guid.NewGuid().ToString(),
                        Content = "Nội dung " + Guid.NewGuid().ToString(),
                    }
                },
                DeletePostContentLangs = new List<string>
                {
                    "en"
                }
            };
            var updatePostJson = JsonConvert.SerializeObject(updatePostModel, Formatting.Indented);
            var updatePost = new RequestItemBuilder()
                .Name("Update post")
                .Method(HttpMethod.Patch.Method)
                .Url($"{apiVarHolder}/{ApiEndpoint.POST_API}/1")
                .JsonBody(updatePostJson)
                .Description("Update a post")
                .Build();
            var updatePostFolder = new FolderItemBuilder()
                .Description("Update post")
                .Name("Update")
                .AddItems(updatePost)
                .Build();

            var deletePost = new RequestItemBuilder()
                .Name("Delete post")
                .Method(HttpMethod.Delete.Method)
                .Url($"{apiVarHolder}/{ApiEndpoint.POST_API}/1")
                .Description("Delete a post")
                .Build();

            var postFolder = new FolderItemBuilder()
                .Description("Posts Controller")
                .Name("Posts")
                .AddItems(getPosts,
                    createPost,
                    updatePostFolder,
                    deletePost)
                .Build();
            #endregion

            #region Collection
            var collection = new CollectionBuilder()
                .Info("SK.WebApi (Demo only)", "WebApi Walkthrough Collection")
                .AddStringVariables(new Variable
                {
                    key = "some-key",
                    value = "some-value",
                })
                .AddItems(postFolder)
                .Build();
            #endregion

            var json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            File.WriteAllText("./postman-collection.json", json);
        }
    }
}
