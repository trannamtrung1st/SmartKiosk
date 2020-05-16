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
            QueryScheduleDetail(provider);
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

    }
}
