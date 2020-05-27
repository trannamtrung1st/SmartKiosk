using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SK.Business;
using SK.Business.Services;
using SK.Data.Models;
using TNT.Core.Helpers.DI;

namespace SK.WebAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            configuration.Bind("BusinessSettings", Business.Settings.Instance);
            configuration.Bind("WebAdminSettings", WebAdmin.Settings.Instance);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });
            services.AddServiceInjection();
            var connStr = Configuration.GetConnectionString("DataContext");
#if TEST
            connStr = connStr.Replace("{envConfig}", ".Test");
#else
            connStr = connStr.Replace("{envConfig}", "");
#endif
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(connStr);
            });
            Data.Global.Init(services);
            Business.Global.Init(services);
            #region OAuth
            //for some default Identity configuration
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<DataContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            #endregion
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.AccessDeniedPath = Routing.ACCESS_DENIED;
                options.ExpireTimeSpan = TimeSpan.FromHours(
                    WebAdmin.Settings.Instance.CookiePersistentHours);
                options.LoginPath = Routing.LOGIN;
                options.LogoutPath = Routing.LOGOUT;
                options.ReturnUrlParameter = "return_url";
                options.SlidingExpiration = true;
                options.Events.OnValidatePrincipal = async (c) =>
                {
                    var identity = c.Principal.Identity as ClaimsIdentity;
                    //extra claims will be expired after amount of time
                    if (identity.FindFirst(AppClaimType.UserName)?.Value == null)
                    {
                        var identityService = c.HttpContext.RequestServices.GetRequiredService<IdentityService>();
                        var entity = await identityService.GetUserByUserNameAsync(identity.Name);
                        var extraClaims = identityService.GetExtraClaims(entity);
                        identity.AddClaims(extraClaims);
                        c.ShouldRenew = true;
                    }
                    await SecurityStampValidator.ValidatePrincipalAsync(c);
                };
            });
            services.AddControllers();
            services.AddRazorPages(options =>
            {
                var allowAnnonymousPages = new[] {
                    "/AccessDenied", "/Error", "/Status", "/Identity/Login",
#if DEBUG
                    "/Identity/Register"
#endif
                };
                var authorizeFolders = new[] { "/" };
                var authorizeLocationFolders = new[] { "/" };
                options.Conventions
                    .AddAreaPageRoute("Location", "/Dashboard/Index", Routing.LOCATION_DASHBOARD)
                    .AddAreaPageRoute("Location", "/Resource/Index", Routing.LOCATION_RESOURCE)
                    .AddAreaPageRoute("Location", "/Resource/Create", Routing.LOCATION_RESOURCE_CREATE)
                    .AddAreaPageRoute("Location", "/Post/Index", Routing.LOCATION_POST)
                    .AddAreaPageRoute("Location", "/Post/Detail", Routing.LOCATION_POST_DETAIL)
                    .AddAreaPageRoute("Location", "/Post/Create", Routing.LOCATION_POST_CREATE)
                    .AddAreaPageRoute("Location", "/Building/Index", Routing.LOCATION_BUILDING)
                    .AddAreaPageRoute("Location", "/Building/Create", Routing.LOCATION_BUILDING_CREATE)
                    .AddAreaPageRoute("Location", "/Floor/Index", Routing.LOCATION_FLOOR)
                    .AddAreaPageRoute("Location", "/Floor/Create", Routing.LOCATION_FLOOR_CREATE)
                    .AddAreaPageRoute("Location", "/Floor/Detail", Routing.LOCATION_FLOOR_DETAIL)
                    .AddAreaPageRoute("Location", "/Area/Index", Routing.LOCATION_AREA)
                    .AddAreaPageRoute("Location", "/Area/Create", Routing.LOCATION_AREA_CREATE)
                    .AddAreaPageRoute("Location", "/Device/Index", Routing.LOCATION_DEVICE)
                    .AddAreaPageRoute("Location", "/Config/Index", Routing.LOCATION_CONFIG)
                    .AddAreaPageRoute("Location", "/Config/Create", Routing.LOCATION_CONFIG_CREATE)
                    .AddAreaPageRoute("Location", "/Config/Detail", Routing.LOCATION_CONFIG_DETAIL)
                    .AddAreaPageRoute("Location", "/Config/ScreenSaver", Routing.LOCATION_CONFIG_SSP)
                    .AddAreaPageRoute("Location", "/Config/Overview", Routing.LOCATION_CONFIG_OVERVIEW)
                    .AddAreaPageRoute("Location", "/Config/Contact", Routing.LOCATION_CONFIG_CONTACT)
                    .AddAreaPageRoute("Location", "/Schedule/Index", Routing.LOCATION_SCHEDULE)
                    .AddAreaPageRoute("Location", "/Schedule/Create", Routing.LOCATION_SCHEDULE_CREATE)
                    .AddPageRoute("/Post/Detail", Routing.POST_DETAIL)
                    .AddPageRoute("/ResType/Detail", Routing.RES_TYPE_DETAIL)
                    .AddPageRoute("/EtCate/Detail", Routing.ENTITY_CATE_DETAIL)
                    .AddPageRoute("/Owner/Detail", Routing.OWNER_DETAIL);
                foreach (var f in authorizeFolders)
                    options.Conventions.AuthorizeFolder(f);
                foreach (var f in authorizeLocationFolders)
                    options.Conventions.AuthorizeAreaFolder("Location", f);
                foreach (var p in allowAnnonymousPages)
                    options.Conventions.AllowAnonymousToPage(p);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(Routing.ERROR);
            app.UseStatusCodePagesWithReExecute(Routing.STATUS, "?code={0}");
            #region Globalization and Localization
            var supportedCultures = Business.Settings.Instance.SupportedCultures;
            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                // Formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                options.SupportedUICultures = supportedCultures;

                //Sử dụng cho Route culture convention
                //options.RequestCultureProviders.Insert(0, new AppRequestCultureProvider());
            });
            #endregion
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
