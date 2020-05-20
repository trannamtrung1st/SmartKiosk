using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SK.Business.Queries;
using SK.Business.Services;
using SK.Data;
using SK.Business.Models;
using SK.Data.Models;
using TNT.Core.Helpers.DI;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Globalization;

namespace SK.ConsoleClient
{
    class Program
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            Data.Global.Init(services);
            Business.Global.Init(services);
            services.AddServiceInjection();
            var provider = services.BuildServiceProvider();
            QueryResource(provider);
        }

        static void QueryDevice(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var deviceService = provider.GetRequiredService<DeviceService>();
                var results = deviceService.QueryDeviceDynamic(
                    projection: new DeviceQueryProjection()
                    {
                        fields =
                        $"{DeviceQueryProjection.ACCOUNT}," +
                        $"{DeviceQueryProjection.BUILDING}," +
                        $"{DeviceQueryProjection.INFO}," +
                        $"{DeviceQueryProjection.LOCATION}," +
                        $"{DeviceQueryProjection.FLOOR}," +
                        $"{DeviceQueryProjection.AREA}," +
                        $"{DeviceQueryProjection.SCHEDULE}"
                    },
                    options: new DeviceQueryOptions() { count_total = true },
                    filter: new DeviceQueryFilter(),
                    sort: new DeviceQuerySort()
                    {
                        sorts = "a" + DeviceQuerySort.NAME
                    },
                    paging: new DeviceQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query device data");
            }
        }

        static void QuerySchedule(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var scheduleService = provider.GetRequiredService<ScheduleService>();
                var results = scheduleService.QueryScheduleDynamic(
                    projection: new ScheduleQueryProjection()
                    {
                        fields =
                        $"{ScheduleQueryProjection.LOCATION}," +
                        $"{ScheduleQueryProjection.INFO}," +
                        $"{ScheduleQueryProjection.DETAILS}"
                    },
                    options: new ScheduleQueryOptions() { count_total = true },
                    filter: new ScheduleQueryFilter(),
                    sort: new ScheduleQuerySort()
                    {
                        sorts = "a" + ScheduleQuerySort.NAME
                    },
                    paging: new ScheduleQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query schedule data");
            }
        }

        static void QueryScheduleDetail(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<ScheduleDetailService>();
                var results = sdService.QueryScheduleDetailDynamic(
                    projection: new ScheduleDetailQueryProjection()
                    {
                        fields =
                        $"{ScheduleDetailQueryProjection.CONFIGS}," +
                        $"{ScheduleDetailQueryProjection.INFO}," +
                        $"{ScheduleDetailQueryProjection.SCHEDULE}"
                    },
                    options: new ScheduleDetailQueryOptions() { count_total = true },
                    filter: new ScheduleDetailQueryFilter(),
                    sort: new ScheduleDetailQuerySort()
                    {
                        sorts = "a" + ScheduleDetailQuerySort.START_TIME
                    },
                    paging: new ScheduleDetailQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query schedule detail data");
            }
        }

        static void QueryLocation(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<LocationService>();
                var results = sdService.QueryLocationDynamic(
                    projection: new LocationQueryProjection()
                    {
                        fields =
                        $"{LocationQueryProjection.INFO}"
                    },
                    options: new LocationQueryOptions() { count_total = true },
                    filter: new LocationQueryFilter(),
                    sort: new LocationQuerySort()
                    {
                        sorts = "a" + LocationQuerySort.NAME
                    },
                    paging: new LocationQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query location data");
            }
        }

        static void QueryBuilding(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<BuildingService>();
                var results = sdService.QueryBuildingDynamic(
                    projection: new BuildingQueryProjection()
                    {
                        fields =
                        $"{BuildingQueryProjection.INFO}," +
                        $"{BuildingQueryProjection.LOCATION}"
                    },
                    options: new BuildingQueryOptions() { count_total = true },
                    filter: new BuildingQueryFilter(),
                    sort: new BuildingQuerySort()
                    {
                        sorts = "a" + BuildingQuerySort.NAME
                    },
                    paging: new BuildingQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query building data");
            }
        }

        static void QueryFloor(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<FloorService>();
                var results = sdService.QueryFloorDynamic(
                    projection: new FloorQueryProjection()
                    {
                        fields =
                        $"{FloorQueryProjection.INFO}," +
                        $"{FloorQueryProjection.FLOOR_PLAN}," +
                        $"{FloorQueryProjection.BUILDING}"
                    },
                    options: new FloorQueryOptions() { count_total = true },
                    filter: new FloorQueryFilter(),
                    sort: new FloorQuerySort()
                    {
                        sorts = "a" + FloorQuerySort.NAME
                    },
                    paging: new FloorQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query floor data");
            }
        }

        static void QueryArea(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<AreaService>();
                var results = sdService.QueryAreaDynamic(
                    projection: new AreaQueryProjection()
                    {
                        fields =
                        $"{AreaQueryProjection.INFO}," +
                        $"{AreaQueryProjection.FLOOR}," +
                        $"{AreaQueryProjection.BUILDING}"
                    },
                    options: new AreaQueryOptions() { count_total = true },
                    filter: new AreaQueryFilter(),
                    sort: new AreaQuerySort()
                    {
                        sorts = "a" + AreaQuerySort.NAME
                    },
                    paging: new AreaQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query area data");
            }
        }

        static void QueryOwner(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<OwnerService>();
                var results = sdService.QueryOwnerDynamic(
                    projection: new OwnerQueryProjection()
                    {
                        fields =
                        $"{OwnerQueryProjection.INFO}"
                    },
                    options: new OwnerQueryOptions() { count_total = true },
                    filter: new OwnerQueryFilter(),
                    sort: new OwnerQuerySort()
                    {
                        sorts = "a" + OwnerQuerySort.NAME
                    },
                    paging: new OwnerQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query owner data");
            }
        }

        static void QueryPost(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<PostService>();
                var results = sdService.QueryPostDynamic(
                    projection: new PostQueryProjection()
                    {
                        fields =
                        $"{PostQueryProjection.INFO}," +
                        $"{PostQueryProjection.CONTENT}," +
                        $"{PostQueryProjection.OWNER}"
                    },
                    options: new PostQueryOptions() { count_total = true },
                    filter: new PostQueryFilter(),
                    sort: new PostQuerySort()
                    {
                        sorts = "a" + PostQuerySort.TITLE
                    },
                    paging: new PostQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query post data");
            }
        }

        static void QueryResourceType(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<ResourceTypeService>();
                var results = sdService.QueryResourceTypeDynamic(
                    projection: new ResourceTypeQueryProjection()
                    {
                        fields =
                        $"{ResourceTypeQueryProjection.INFO}," +
                        $"{ResourceTypeQueryProjection.CONTENT}"
                    },
                    options: new ResourceTypeQueryOptions() { count_total = true },
                    filter: new ResourceTypeQueryFilter()
                    {
                        lang = Lang.EN,
                        archived = 0
                    },
                    sort: new ResourceTypeQuerySort()
                    {
                        sorts = "a" + ResourceTypeQuerySort.NAME
                    },
                    paging: new ResourceTypeQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query resource type data");
            }
        }

        static void QueryEntityCategory(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<EntityCategoryService>();
                var results = sdService.QueryEntityCategoryDynamic(
                    projection: new EntityCategoryQueryProjection()
                    {
                        fields =
                        $"{EntityCategoryQueryProjection.INFO}," +
                        $"{EntityCategoryQueryProjection.CONTENT}"
                    },
                    options: new EntityCategoryQueryOptions() { count_total = true },
                    filter: new EntityCategoryQueryFilter()
                    {
                        lang = Lang.EN,
                        archived = 0
                    },
                    sort: new EntityCategoryQuerySort()
                    {
                        sorts = "a" + EntityCategoryQuerySort.NAME
                    },
                    paging: new EntityCategoryQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query entity category data");
            }
        }

        static void QueryResource(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var sdService = provider.GetRequiredService<ResourceService>();
                var results = sdService.QueryResourceDynamic(
                    projection: new ResourceQueryProjection()
                    {
                        fields =
                        $"{ResourceQueryProjection.INFO}," +
                        $"{ResourceQueryProjection.AREA}," +
                        $"{ResourceQueryProjection.CATEGORIES}," +
                        $"{ResourceQueryProjection.CONTENT}," +
                        $"{ResourceQueryProjection.CONTENT_CONTENT}," +
                        $"{ResourceQueryProjection.LOCATION}," +
                        $"{ResourceQueryProjection.OWNER}"
                    },
                    options: new ResourceQueryOptions() { count_total = true },
                    filter: new ResourceQueryFilter()
                    {
                        lang = Lang.EN,
                        archived = 0
                    },
                    sort: new ResourceQuerySort()
                    {
                        sorts = "a" + ResourceQuerySort.NAME
                    },
                    paging: new ResourceQueryPaging()
                    {
                        limit = 100,
                        page = 1
                    }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query resource data");
            }
        }
    }
}
