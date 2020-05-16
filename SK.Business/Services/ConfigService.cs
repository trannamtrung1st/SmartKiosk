using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace SK.Business.Services
{
    public class ConfigService : Service
    {
        public ConfigService(ServiceInjection inj) : base(inj)
        {
        }


        #region Query Config
        public IDictionary<string, object> GetConfigDynamic(
            ConfigQueryRow row, ConfigQueryProjection projection,
            ConfigQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ConfigQueryProjection.INFO:
                        {
                            var entity = row.Config;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                            obj["description"] = entity.Description;
                            obj["location_id"] = entity.LocationId;
                        }
                        break;
                    case ConfigQueryProjection.SELECT:
                        {
                            var entity = row.Config;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case ConfigQueryProjection.DETAIL:
                        {
                            var entity = row.Config;
                            var playlist = GetScreenSaverConfig(entity);
                            var home = GetHomePageConfig(entity);
                            var peConfig = GetProgramEventConfig(entity);
                            var cConfig = GetContactConfig(entity);
                            obj["screen_saver_playlist"] = playlist;
                            obj["home_config"] = home;
                            obj["program_event_config"] = peConfig;
                            obj["contact_config"] = cConfig;
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetConfigDynamic(
            IEnumerable<ConfigQueryRow> rows, ConfigQueryProjection projection,
            ConfigQueryOptions options, int? totalCount = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetConfigDynamic(o, projection, options);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryConfigDynamic(
            ConfigQueryProjection projection,
            ConfigQueryOptions options,
            ConfigQueryFilter filter = null,
            ConfigQuerySort sort = null,
            ConfigQueryPaging paging = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ConfigQuery.CreateDynamicSql();
            #region General
            if (filter != null) query = query.SqlFilter(filter);
            query = query.SqlJoin(projection);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options.count_total) countQuery = query.SqlCount("*");
            query = query.SqlProjectFields(projection);
            #endregion
            await openConn;
            if (!options.single_only)
            {
                #region List query
                if (sort != null) query = query.SqlSort(sort);
                if (paging != null && (!options.load_all || !ConfigQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            var queryResult = await conn.QueryAsync(
                sql: query.PreparedForm,
                types: query.GetTypesArr(),
                map: (objs) => ProcessMultiResults(query, objs),
                splitOn: string.Join(',', query.GetSplitOns()),
                param: query.DynamicParameters);
            if (options.single_only)
            {
                var single = queryResult.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetConfigDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options.count_total) totalCount = await countTask;
            var result = GetConfigDynamic(queryResult, projection, options, totalCount);
            return result;
        }

        public async Task<QueryResult<ConfigQueryRow>> QueryConfig(
            ConfigQueryFilter filter = null,
            ConfigQuerySort sort = null,
            ConfigQueryProjection projection = null,
            ConfigQueryPaging paging = null,
            ConfigQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = ConfigQuery.CreateDynamicSql();
            #region General
            if (filter != null) query = query.SqlFilter(filter);
            if (projection != null) query = query.SqlJoin(projection);
            DynamicSql countQuery = null; int? totalCount = null; Task<int> countTask = null;
            if (options != null && options.count_total) countQuery = query.SqlCount("*");
            if (projection != null) query = query.SqlProjectFields(projection);
            #endregion
            await openConn;
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.SqlSort(sort);
                if (paging != null && (!options.load_all || !ConfigQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            var queryResult = await conn.QueryAsync(
                sql: query.PreparedForm,
                types: query.GetTypesArr(),
                map: (objs) => ProcessMultiResults(query, objs),
                splitOn: string.Join(',', query.GetSplitOns()),
                param: query.DynamicParameters);
            if (options != null && options.single_only)
            {
                var single = queryResult.SingleOrDefault();
                return new QueryResult<ConfigQueryRow>
                {
                    SingleResult = single
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            return new QueryResult<ConfigQueryRow>
            {
                Results = queryResult,
                TotalCount = totalCount
            };
        }

        private ConfigQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new ConfigQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case ConfigQueryProjection.INFO:
                    case ConfigQueryProjection.SELECT: row.Config = objs[i] as Config; break;
                }
            }
            return row;
        }
        #endregion

        public ScreenSaverConfig GetScreenSaverConfig(Config conf)
        {
            ScreenSaverConfig playlist = null;
            if (conf.ScreenSaverPlaylist != null)
                playlist = JsonConvert.DeserializeObject<ScreenSaverConfig>(conf.ScreenSaverPlaylist);
            if (playlist == null)
                playlist = new ScreenSaverConfig();
            return playlist;
        }

        public HomePageConfig GetHomePageConfig(Config conf)
        {
            HomePageConfig hpConf = null;
            if (conf.HomeConfig != null)
                hpConf = JsonConvert.DeserializeObject<HomePageConfig>(conf.HomeConfig);
            if (hpConf == null)
                hpConf = new HomePageConfig();
            return hpConf;
        }

        public PostsConfig GetProgramEventConfig(Config conf)
        {
            PostsConfig peConf = null;
            if (conf.ProgramEventConfig != null)
                peConf = JsonConvert.DeserializeObject<PostsConfig>(conf.ProgramEventConfig);
            if (peConf == null)
                peConf = new PostsConfig();
            return peConf;
        }

        public ContactConfig GetContactConfig(Config conf)
        {
            ContactConfig cConfig = null;
            if (conf.ContactConfig != null)
                cConfig = JsonConvert.DeserializeObject<ContactConfig>(conf.ContactConfig);
            if (cConfig == null)
                cConfig = new ContactConfig();
            return cConfig;
        }
    }
}
