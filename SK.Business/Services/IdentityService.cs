using Dapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SK.Business.Models;
using SK.Business.Queries;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using static Dapper.SqlMapper;

namespace SK.Business.Services
{
    public class IdentityService : Service
    {
        [Inject]
        private readonly UserManager<AppUser> _userManager;
        [Inject]
        private readonly SignInManager<AppUser> _signInManager;
        [Inject]
        private readonly RoleManager<AppRole> _roleManager;

        public IdentityService(ServiceInjection inj) : base(inj)
        {
        }

        #region Role
        public IQueryable<AppRole> Roles
        {
            get
            {
                return _roleManager.Roles;
            }
        }

        public AppRole GetRoleByName(string name)
        {
            return Roles.FirstOrDefault(r => r.Name == name);
        }

        public async Task<IdentityResult> RemoveRoleAsync(AppRole entity)
        {
            return await _roleManager.DeleteAsync(entity);
        }

        protected void PrepareCreate(AppRole entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        public async Task<IdentityResult> CreateRoleAsync(CreateRoleModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            var result = await _roleManager.CreateAsync(entity);
            return result;
        }

        public async Task<IdentityResult> UpdateRoleAsync(AppRole entity,
            UpdateRoleModel model)
        {
            model.CopyTo(entity);
            var result = await _roleManager.UpdateAsync(entity);
            return result;
        }

        public ValidationResult ValidateGetProfile(
            ClaimsPrincipal principal)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateGetRoles(
            ClaimsPrincipal principal)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateCreateRole(
            ClaimsPrincipal principal, CreateRoleModel model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateUpdateRole(
            ClaimsPrincipal principal, UpdateRoleModel model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateDeleteRole(
            ClaimsPrincipal principal, AppRole entity)
        {
            return ValidationResult.Pass();
        }
        #endregion

        #region User
        public AppUser ConvertToUser(RegisterModel model)
        {
            var entity = new AppUser { UserName = model.username, FullName = model.full_name };
            return entity;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> PasswordSignInAsync(
            string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await _signInManager.PasswordSignInAsync(userName: username,
                   password,
                   isPersistent, lockoutOnFailure);
            return result;
        }

        public async Task SignInAsync(AppUser user, AuthenticationProperties props)
        {
            await _signInManager.SignInAsync(user: user, authenticationProperties: props);
        }

        public List<Claim> GetExtraClaims(AppUser entity)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(AppClaimType.UserName, entity.UserName));
            return claims;
        }

        public async Task SignInWithExtraClaimsAsync(AppUser entity, bool isPersistent)
        {
            var extraClaims = GetExtraClaims(entity);
            //SignInWithClaimsAsync: for additional claims
            await _signInManager.SignInWithClaimsAsync(user: entity,
                isPersistent: isPersistent, extraClaims);
        }

        protected void PrepareCreate(AppUser entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public object GetUserProfile(AppUser entity)
        {
            return new
            {
                full_name = entity.FullName,
                email = entity.Email,
                id = entity.Id,
                phone = entity.PhoneNumber
            };
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser entity)
        {
            return await _userManager.UpdateAsync(entity);
        }

        public async Task<IdentityResult> AddRolesForUserAsync(AppUser entity, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(entity, roles);
        }

        public async Task<IdentityResult> RemoveUserFromRolesAsync(AppUser entity, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(entity, roles);
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser entity, string password)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity, password);
            return result;
        }

        public async Task<IdentityResult> CreateUserWithRolesTransactionAsync(AppUser entity, string password,
            IEnumerable<string> roles = null)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity, password);
            if (!result.Succeeded)
                return result;
            if (roles != null)
                result = await _userManager.AddToRolesAsync(entity, roles);
            return result;
        }

        public ValidationResult ValidateLogin(
            ClaimsPrincipal principal, AuthorizationGrantModel model)
        {
            return ValidationResult.Pass();
        }

        public ValidationResult ValidateRegister(
            ClaimsPrincipal principal, RegisterModel model)
        {
            return ValidationResult.Pass();
        }
        #endregion

        #region OAuth
        public TokenResponseModel GenerateTokenResponse(ClaimsPrincipal principal,
            AuthenticationProperties properties, string scope = null)
        {
            #region Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.Default.GetBytes(JWT.SECRET_KEY);
            var issuer = JWT.ISSUER;
            var audience = JWT.AUDIENCE;
            var identity = principal.Identity as ClaimsIdentity;
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, principal.Identity.Name));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = properties.IssuedUtc?.UtcDateTime,
                Expires = properties.ExpiresUtc?.UtcDateTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                NotBefore = properties.IssuedUtc?.UtcDateTime
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            #endregion
            var resp = new TokenResponseModel();
            resp.user_id = identity.Name;
            resp.access_token = tokenString;
            resp.token_type = "bearer";
            if (properties.ExpiresUtc != null)
                resp.expires_utc = properties.ExpiresUtc?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (properties.IssuedUtc != null)
                resp.issued_utc = properties.IssuedUtc?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            #region Handle scope
            if (scope != null)
            {
                var scopes = scope.Split(' ');
                foreach (var s in scopes)
                {
                    switch (s)
                    {
                        case AppOAuthScope.ROLES:
                            resp.roles = identity.FindAll(identity.RoleClaimType)
                                .Select(c => c.Value).ToList();
                            break;
                    }
                }
            }
            #endregion
            #region Refresh Token
            key = Encoding.Default.GetBytes(JWT.REFRESH_SECRET_KEY);
            issuer = JWT.REFRESH_ISSUER;
            audience = JWT.REFRESH_AUDIENCE;
            var id = identity.Name;
            identity = new ClaimsIdentity(
                identity.Claims.Where(c => c.Type == identity.NameClaimType),
                identity.AuthenticationType);

            var refresh_expires = (properties.Parameters["refresh_expires"]
                as DateTimeOffset?)?.UtcDateTime;
            tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = properties.IssuedUtc?.UtcDateTime,
                Expires = refresh_expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                NotBefore = properties.IssuedUtc?.UtcDateTime
            };

            token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);
            resp.refresh_token = tokenString;
            #endregion
            return resp;
        }

        public ClaimsPrincipal ValidateRefreshToken(string tokenStr)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken secToken;
                var param = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JWT.REFRESH_ISSUER,
                    ValidAudience = JWT.REFRESH_AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(JWT.REFRESH_SECRET_KEY)),
                    ClockSkew = TimeSpan.Zero
                };
                return tokenHandler.ValidateToken(tokenStr, param, out secToken);
            }
            catch (Exception) { }
            return null;
        }

        public async Task<AppUser> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
                return null;
            return user;
        }

        public async Task<ClaimsIdentity> GetIdentityAsync(AppUser entity, string scheme)
        {
            var identity = new ClaimsIdentity(scheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.Id));
            var claims = await _userManager.GetClaimsAsync(entity);
            var roles = await _userManager.GetRolesAsync(entity);
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
            identity.AddClaims(claims);
            return identity;
        }

        //for IdentityCookie
        public async Task<ClaimsPrincipal> GetApplicationPrincipalAsync(AppUser entity)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(entity);
            var identity = principal.Identity as ClaimsIdentity;
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.Id));
            var claims = GetExtraClaims(entity);
            var roles = await _userManager.GetRolesAsync(entity);
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
            identity.AddClaims(claims);
            return principal;
        }
        #endregion

        #region External Login
        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            return await _signInManager.GetExternalLoginInfoAsync(expectedXsrf);
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            return _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
        }

        public async Task<AppUser> AuthenticateExternalAsync(string provider, string providerKey)
        {
            var user = await _userManager.FindByLoginAsync(provider, providerKey);
            return user;
        }

        public async Task<IdentityResult> AddLoginToUserAsync(AppUser entity, ExternalLoginInfo info)
        {
            var result = await _userManager.AddLoginAsync(entity, info);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unexpected error occurred adding external login for user with ID '{entity.Id}'.");
            return result;
        }
        #endregion

        #region Query AppUser
        public IDictionary<string, object> GetAppUserDynamic(
            AppUserQueryRow row, AppUserQueryProjection projection,
            AppUserQueryOptions options, string currentAccId = null)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AppUserQueryProjection.INFO:
                        {
                            var entity = row.AppUser;
                            obj["username"] = entity.UserName;
                            obj["full_name"] = entity.FullName;
                            obj["phone_number"] = entity.PhoneNumber;
                            obj["current_logged_in"] = entity.Id == currentAccId;
                        }
                        break;
                    case AppUserQueryProjection.ROLES:
                        {
                            var entities = row.AppUser.UserRoles?
                                .Select(o => o.Role);
                            if (entities != null)
                                obj["roles"] = entities.Select(o => new
                                {
                                    name = o.Name,
                                    display_name = o.DisplayName,
                                    role_type = o.RoleType
                                }).ToList();
                        }
                        break;
                }
            }
            return obj;
        }

        public QueryResult<IDictionary<string, object>> GetAppUserDynamic(
            IEnumerable<AppUserQueryRow> rows, AppUserQueryProjection projection,
            AppUserQueryOptions options, int? totalCount = null, string currentAccId = null)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetAppUserDynamic(o, projection, options, currentAccId);
                list.Add(obj);
            }
            var resp = new QueryResult<IDictionary<string, object>>();
            resp.Results = list;
            if (options.count_total)
                resp.TotalCount = totalCount;
            return resp;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAppUserDynamic(
            AppUserQueryProjection projection,
            AppUserQueryOptions options,
            AppUserQueryFilter filter = null,
            AppUserQuerySort sort = null,
            AppUserQueryPaging paging = null,
            string currentAccId = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = AppUserQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !AppUserQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            query = query.SqlExtras(projection);
            var multipleResult = await conn.QueryMultipleAsync(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            using (multipleResult)
            {
                var queryResult = multipleResult.Read(
                    types: query.GetTypesArr(),
                    map: (objs) => ProcessMultiResults(query, objs),
                    splitOn: string.Join(',', query.GetSplitOns()));
                var extraKeys = projection.GetFieldsArr()
                    .Where(f => AppUserQueryProjection.Extras.ContainsKey(f));
                IEnumerable<AppUserRoleQueryRow> userRoles = null;
                foreach (var key in extraKeys)
                {
                    switch (key)
                    {
                        case AppUserQueryProjection.ROLES:
                            userRoles = GetAppUserRoleQueryResult(multipleResult);
                            break;
                    }
                }
                ProcessExtras(queryResult, userRoles);
                if (options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    var singleResult = GetAppUserDynamic(single, projection, options, currentAccId);
                    return new QueryResult<IDictionary<string, object>>()
                    {
                        SingleResult = singleResult
                    };
                }
                if (options.count_total) totalCount = await countTask;
                var result = GetAppUserDynamic(queryResult, projection, options, totalCount, currentAccId);
                return result;
            }
        }

        public async Task<QueryResult<AppUserQueryRow>> QueryAppUser(
            AppUserQueryFilter filter = null,
            AppUserQuerySort sort = null,
            AppUserQueryProjection projection = null,
            AppUserQueryPaging paging = null,
            AppUserQueryOptions options = null)
        {
            var conn = context.Database.GetDbConnection();
            var openConn = conn.OpenAsync();
            var query = AppUserQuery.CreateDynamicSql();
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
                if (paging != null && (!options.load_all || !AppUserQueryOptions.IsLoadAllAllowed))
                    query = query.SqlSelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = conn.ExecuteScalarAsync<int>(
                        sql: countQuery.PreparedForm,
                        param: countQuery.DynamicParameters);
                #endregion
            }
            if (projection != null) query = query.SqlExtras(projection);
            var multipleResult = await conn.QueryMultipleAsync(
                sql: query.PreparedForm,
                param: query.DynamicParameters);
            using (multipleResult)
            {
                var queryResult = multipleResult.Read(
                    types: query.GetTypesArr(),
                    map: (objs) => ProcessMultiResults(query, objs),
                    splitOn: string.Join(',', query.GetSplitOns()));
                if (projection != null)
                {
                    var extraKeys = projection.GetFieldsArr()
                        .Where(f => AppUserQueryProjection.Extras.ContainsKey(f));
                    IEnumerable<AppUserRoleQueryRow> userRoles = null;
                    foreach (var key in extraKeys)
                    {
                        switch (key)
                        {
                            case AppUserQueryProjection.ROLES:
                                userRoles = GetAppUserRoleQueryResult(multipleResult);
                                break;
                        }
                    }
                    ProcessExtras(queryResult, userRoles);
                }
                if (options != null && options.single_only)
                {
                    var single = queryResult.SingleOrDefault();
                    if (single == null) return null;
                    return new QueryResult<AppUserQueryRow>
                    {
                        SingleResult = single
                    };
                }
                if (options != null && options.count_total) totalCount = await countTask;
                return new QueryResult<AppUserQueryRow>
                {
                    Results = queryResult,
                    TotalCount = totalCount
                };
            }
        }

        private AppUserQueryRow ProcessMultiResults(DynamicSql query, object[] objs)
        {
            var row = new AppUserQueryRow();
            for (var i = 0; i < query.MultiResults.Count; i++)
            {
                var r = query.MultiResults[i];
                switch (r.Key)
                {
                    case AppUserQueryProjection.INFO: row.AppUser = objs[i] as AppUserQueryResult; break;
                }
            }
            return row;
        }

        private IEnumerable<AppUserRoleQueryRow> GetAppUserRoleQueryResult(GridReader multipleResult)
        {
            var rows = multipleResult.Read(
                types: new[] { typeof(AppUserRole), typeof(AppRoleRelationship) },
                map: (objs) =>
                {
                    var row = new AppUserRoleQueryRow();
                    row.UserRole = objs[0] as AppUserRole;
                    row.Role = objs[1] as AppRoleRelationship;
                    return row;
                }, splitOn: $"{AppRole.TBL_NAME}.{nameof(AppRole.Id)}")
                .ToList();
            return rows;
        }

        private void ProcessExtras(IEnumerable<AppUserQueryRow> entities,
            IEnumerable<AppUserRoleQueryRow> userRoles)
        {
            var contentMaps = userRoles?.GroupByAppUser().ToDictionary(o => o.Key);
            foreach (var e in entities)
            {
                var entity = e.AppUser;
                if (contentMaps != null && contentMaps.ContainsKey(entity.Id))
                    entity.UserRoles = contentMaps[entity.Id].ToList();
            }
        }
        #endregion

        #region Validation
        public ValidationResult ValidateGetAppUsers(
            ClaimsPrincipal principal,
            AppUserQueryFilter filter,
            AppUserQuerySort sort,
            AppUserQueryProjection projection,
            AppUserQueryPaging paging,
            AppUserQueryOptions options)
        {
            return ValidationResult.Pass();
        }
        #endregion

        #region Device
        public void LoginDevice(Device entity,
            string oldFcmToken, string newFcmToken)
        {
            if (oldFcmToken != null)
            {
                FirebaseMessaging.DefaultInstance.SendAsync(new Message
                {
                    Topic = entity.Id,
                    Data = new Dictionary<string, string>()
                    {
                        {"action", "logout"}
                    }
                });
                var unsubResp = FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(
                        new List<string> { oldFcmToken }, entity.Id).Result;
            }
            var subResp = FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
                new List<string> { newFcmToken }, entity.Id).Result;
            if (subResp.Errors.Count > 0)
                throw new Exception("Can not manage topic subscription");
        }
        #endregion

    }
}
