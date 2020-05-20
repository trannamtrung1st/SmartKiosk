using AutoMapper;
using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SK.Business.Models;
using SK.Business.Services;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business
{
    public class Global
    {
        public static Random Random { get; private set; }
        public static IMapper Mapper { get; private set; }

        private static void InitAutoMapper()
        {
            //AutoMapper
            var mapConfig = new MapperConfiguration(cfg =>
            {
                var dType = typeof(IMappingModel);
                var modelTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(t => dType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                var maps = new Dictionary<Type, Type>();
                foreach (var t in modelTypes)
                {
                    var genArgs = t.BaseType?.GetGenericArguments().FirstOrDefault();
                    if (genArgs != null) cfg.CreateMap(genArgs, t).ReverseMap();
                }
            });
            Mapper = mapConfig.CreateMapper();
        }

        private static void InitDI(IServiceCollection services)
        {
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });

            //extra
            var baseServiceType = typeof(Service);
            var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => baseServiceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var t in serviceTypes)
                services.AddScoped(t);
        }

        private static void InitDapper()
        {
            FluentMapper.Initialize(cfg =>
            {
                var relType = typeof(IDapperRelationship);
                var modelTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(t => relType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                foreach (var t in modelTypes)
                    cfg.AddMap(DapperMapProvider.From((dynamic)Activator.CreateInstance(t)));
            });
        }

        public static void Init(IServiceCollection services)
        {
            Random = new Random();
            InitAutoMapper();
            InitDI(services);
            InitDapper();
        }

        public static void ParseFirebaseConfig(string secretFile)
        {
            var secretJson = File.ReadAllText(secretFile);
            Settings.Instance.FirebaseConfig = JsonConvert.DeserializeObject<FirebaseConfig>(secretJson);
        }

    }
}
