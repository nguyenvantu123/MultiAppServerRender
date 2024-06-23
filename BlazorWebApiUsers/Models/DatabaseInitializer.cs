
using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using BlazorBoilerplate.Infrastructure.Storage;
using BlazorBoilerplate.Infrastructure.Storage.DataModels;
using BlazorBoilerplate.Infrastructure.Storage.Permissions;
using BlazorWebApi.Users;
using BlazorWebApi.Users.Data;
using BlazorWebApi.Users.Models;
using BlazorWebApi.Users.RoleConst;
using Finbuckle.MultiTenant;
using IdentitySample;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace BlazorBoilerplate.Storage
{
    internal sealed class DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
     : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";

        private readonly ActivitySource _activitySource = new(ActivitySourceName);

        public UserManager<ApplicationUser> _userManager;

        public RoleManager<ApplicationRole> _roleManager;

        public ApplicationDbContext _context;

        public TenantStoreDbContext _tenantStoreDbContext;


        public EntityPermissions _entityPermissions;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //var dbContext = scope.ServiceProvider.GetRequiredService<UserDBContext>();

            using (var scope = serviceProvider.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                _userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                _roleManager = (RoleManager<ApplicationRole>)scope.ServiceProvider.GetService(typeof(RoleManager<ApplicationRole>));
                //Resolve ASP .NET Core Identity with DI help
                _context = (ApplicationDbContext)scope.ServiceProvider.GetService(typeof(ApplicationDbContext));
                _tenantStoreDbContext = (TenantStoreDbContext)scope.ServiceProvider.GetService(typeof(TenantStoreDbContext));
                _entityPermissions = (EntityPermissions)scope.ServiceProvider.GetService(typeof(EntityPermissions));

                #region application db 
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var strategy = dbContext.Database.CreateExecutionStrategy();

                using var activity = _activitySource.StartActivity("Migrating identity database", ActivityKind.Client);

                var sw = Stopwatch.StartNew();

                await strategy.ExecuteAsync(() => dbContext.Database.MigrateAsync(cancellationToken));
                #endregion

                #region tenantdb
                var tenantdbContext = scope.ServiceProvider.GetRequiredService<TenantStoreDbContext>();

                var tenantStrategy = tenantdbContext.Database.CreateExecutionStrategy();

                using var tenantActivity = _activitySource.StartActivity("Migrating tenant database", ActivityKind.Client);

                var swTenant = Stopwatch.StartNew();

                await tenantStrategy.ExecuteAsync(() => tenantdbContext.Database.MigrateAsync(cancellationToken));
                #endregion

                await EnsureAdminIdentitiesAsync();

                //Seed blazorboilerplate sample data
                await SeedDemoDataAsync();

                logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
                    sw.ElapsedMilliseconds + swTenant.ElapsedMilliseconds);

                //InitializeDatabaseAsync(userManager, roleManager);
            }
        }


        //private async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        //{
        //    //    logger.LogInformation("Seeding database");

        //    //    var adminRole = new ApplicationRole
        //    //    {
        //    //        Name = Constant.SuperAdministratorRole,
        //    //        Description = "Administrator role with full permissions",
        //    //        CreatedBy = "Super Admin",
        //    //        CreatedOn = DateTime.UtcNow,
        //    //        LastModifiedBy = "Super Admin",
        //    //        LastModifiedOn = DateTime.UtcNow,
        //    //        Id = Guid.NewGuid(),
        //    //    };
        //    //    var adminRoleInDb =
        //    //        await roleManager.FindByNameAsync(adminRole.Name);

        //    //    if (adminRoleInDb == null)
        //    //    {
        //    //        await roleManager.CreateAsync(adminRole);
        //    //        // adminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.SuperAdministratorRole);
        //    //        logger.LogInformation("Seeded Administrator Role.");
        //    //    }

        //    //    //Check if User Exists
        //    //    var superUser = new ApplicationUser
        //    //    {
        //    //        FirstName = "Super",
        //    //        LastName = "Admin",
        //    //        Email = "superadmin@gmail.com",
        //    //        UserName = "adminstrator",
        //    //        EmailConfirmed = true,
        //    //        PhoneNumberConfirmed = true,
        //    //        CreatedOn = DateTime.Now,
        //    //        IsActive = true,
        //    //        CreatedBy = "Super Admin",
        //    //        LastModifiedBy = "Super Admin",
        //    //        LastModifiedOn = DateTime.UtcNow,
        //    //        Id = Guid.NewGuid()
        //    //    };
        //    //    var superUserInDb = await userManager.FindByEmailAsync(superUser.Email);
        //    //    if (superUserInDb == null)
        //    //    {
        //    //        await userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
        //    //        var result = await userManager.AddToRoleAsync(superUser, Constant.SuperAdministratorRole);
        //    //        if (result.Succeeded)
        //    //        {
        //    //            logger.LogInformation("Seeded Default SuperAdmin User.");
        //    //        }
        //    //        else
        //    //        {
        //    //            foreach (var error in result.Errors)
        //    //            {
        //    //                logger.LogError(error.Description);
        //    //            }
        //    //        }
        //    //    }

        //    //    var basicRole = new ApplicationRole
        //    //    {
        //    //        Name = Constant.BasicUserRole,
        //    //        Description = "User role",
        //    //        CreatedBy = "Super Admin",
        //    //        CreatedOn = DateTime.UtcNow,
        //    //        LastModifiedBy = "Super Admin",
        //    //        LastModifiedOn = DateTime.UtcNow,
        //    //        Id = Guid.NewGuid()
        //    //    };
        //    //    var basicRoleInDb = await roleManager.FindByNameAsync(Constant.BasicUserRole);
        //    //    if (basicRoleInDb == null)
        //    //    {
        //    //        await roleManager.CreateAsync(basicRole);
        //    //        logger.LogInformation("Seeded Basic User Role.");
        //    //    }
        //    //}
        //}

        private async Task SeedDemoDataAsync()
        {
            if ((await _userManager.FindByNameAsync(DefaultUserNames.User)) == null)
            {
                await CreateUserAsync(DefaultUserNames.User, UserConstants.DefaultPassword, "User", "Blazor", "user@blazorboilerplate.com", "+1 (123) 456-7890");
            }

            if (_tenantStoreDbContext.TenantInfo.Count() < 2)
            {
                _tenantStoreDbContext.TenantInfo.Add(new AppTenantInfo() { Id = "tenant1", Identifier = "tenant1.local", Name = "Microsoft Inc." });
                _tenantStoreDbContext.TenantInfo.Add(new AppTenantInfo() { Id = "tenant2", Identifier = "tenant2.local", Name = "Contoso Corp." });

                _tenantStoreDbContext.SaveChanges();
            }

            ApplicationUser user = await _userManager.FindByNameAsync(DefaultUserNames.User);

            if (!_context.UserProfiles.Any())
                _context.UserProfiles.Add(new UserProfile
                {
                    UserId = user.Id,
                    ApplicationUser = user,
                    Count = 2,
                    IsNavOpen = true,
                    LastPageVisited = "/dashboard",
                    IsNavMinified = false,
                    LastUpdatedDate = DateTime.Now
                });

            if (!_context.ApiLogs.Any())
            {
                _context.ApiLogs.AddRange(
                new ApiLogItem
                {
                    RequestTime = DateTime.Now,
                    ResponseMillis = 30,
                    StatusCode = 200,
                    Method = "Get",
                    Path = "/api/seed",
                    QueryString = "",
                    RequestBody = "",
                    ResponseBody = "",
                    IPAddress = "::1",
                    ApplicationUserId = user.Id
                },
                new ApiLogItem
                {
                    RequestTime = DateTime.Now,
                    ResponseMillis = 30,
                    StatusCode = 200,
                    Method = "Get",
                    Path = "/api/seed",
                    QueryString = "",
                    RequestBody = "",
                    ResponseBody = "",
                    IPAddress = "::1",
                    ApplicationUserId = user.Id
                }
            );
            }

            _context.SaveChanges();
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string firstName, string lastName, string email, string phoneNumber, string[] roles = null)
        {

            ApplicationUser applicationUser = await _userManager.FindByNameAsync(userName);

            if (applicationUser == null)
            {
                applicationUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var result = _userManager.CreateAsync(applicationUser, password).Result;

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                result = _userManager.AddClaimsAsync(applicationUser, new Claim[]{
                                new Claim(ClaimTypes.Name, userName),
                                new Claim(ClaimTypes.GivenName, firstName),
                                new Claim(ClaimTypes.Surname, lastName),
                                new Claim(ClaimTypes.Email, email),
                                new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.trueString, ClaimValueTypes.Boolean),
                                new Claim(ClaimTypes.HomePhone, phoneNumber)
                            }).Result;

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                //add claims version of roles
                if (roles != null)
                {
                    foreach (var role in roles.Distinct())
                    {
                        await _userManager.AddClaimAsync(applicationUser, new Claim($"Is{role}", ClaimValues.trueString));
                    }

                    ApplicationUser user = await _userManager.FindByNameAsync(applicationUser.UserName);

                    try
                    {
                        result = await _userManager.AddToRolesAsync(user, roles.Distinct());
                    }
                    catch
                    {
                        await _userManager.DeleteAsync(user);
                        throw;
                    }

                    if (!result.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                    }
                }
                //}

            }

            return applicationUser;

        }

        public async Task EnsureAdminIdentitiesAsync()
        {
            await EnsureRoleAsync(DefaultRoleNames.Administrator, _entityPermissions.GetAllPermissionValues());
            await CreateUserAsync(DefaultUserNames.Administrator, "admin123", "Admin", "Blazor", "admin@blazorboilerplate.com", "+1 (123) 456-7890", new string[] { DefaultRoleNames.Administrator });

            ApplicationRole adminRole = await _roleManager.FindByNameAsync(DefaultRoleNames.Administrator);
            var AllClaims = _entityPermissions.GetAllPermissionValues().Distinct();
            var RoleClaims = (await _roleManager.GetClaimsAsync(adminRole)).Select(c => c.Value).ToList();
            var NewClaims = AllClaims.Except(RoleClaims);

            foreach (string claim in NewClaims)
                await _roleManager.AddClaimAsync(adminRole, new Claim(ApplicationClaimTypes.Permission, claim));

            var DeprecatedClaims = RoleClaims.Except(AllClaims);
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (string claim in DeprecatedClaims)
                foreach (var role in roles)
                    await _roleManager.RemoveClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, claim));

            logger.LogInformation("Inbuilt account generation completed");
        }

        private async Task EnsureRoleAsync(string roleName, string[] claims)
        {
            if ((await _roleManager.FindByNameAsync(roleName)) == null)
            {
                if (claims == null)
                    claims = Array.Empty<string>();

                string[] invalidClaims = claims.Where(c => _entityPermissions.GetPermissionByValue(c) == null).ToArray();
                if (invalidClaims.Any())
                    throw new Exception("The following claim types are invalid: " + string.Join(", ", invalidClaims));

                ApplicationRole applicationRole = new(roleName);

                var result = await _roleManager.CreateAsync(applicationRole);

                ApplicationRole role = await _roleManager.FindByNameAsync(applicationRole.Name);

                foreach (string claim in claims.Distinct())
                {
                    result = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByValue(claim)));

                    if (!result.Succeeded)
                        await _roleManager.DeleteAsync(role);
                }
            }
        }
    }


}

//        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string firstName, string lastName, string email, string phoneNumber, string[] roles = null)
//        {

//            try
//            {
//                ApplicationUser applicationUser = await _userManager.FindByNameAsync(userName);

//                if (applicationUser == null)
//                {
//                    applicationUser = new ApplicationUser
//                    {
//                        UserName = userName,
//                        Email = email,
//                        PhoneNumber = phoneNumber,
//                        FirstName = firstName,
//                        LastName = lastName,
//                        EmailConfirmed = true
//                    };

//                    var result = _userManager.CreateAsync(applicationUser, password).Result;

//                    if (!result.Succeeded)
//                        throw new Exception(result.Errors.First().Description);

//                    result = _userManager.AddClaimsAsync(applicationUser, new Claim[]{
//                        new Claim(ClaimTypes.Name, userName),
//                        new Claim(ClaimTypes.GivenName, firstName),
//                        new Claim(ClaimTypes.Surname, lastName),
//                        new Claim(ClaimTypes.Email, email),
//                        new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.trueString, ClaimValueTypes.Boolean),
//                        new Claim(ClaimTypes.HomePhone, phoneNumber)
//                    }).Result;

//                    if (!result.Succeeded)
//                        throw new Exception(result.Errors.First().Description);

//                    //add claims version of roles
//                    if (roles != null)
//                    {
//                        foreach (var role in roles.Distinct())
//                        {
//                            await _userManager.AddClaimAsync(applicationUser, new Claim($"Is{role}", ClaimValues.trueString));
//                        }

//                        ApplicationUser user = await _userManager.FindByNameAsync(applicationUser.UserName);

//                        try
//                        {
//                            result = await _userManager.AddToRolesAsync(user, roles.Distinct());
//                        }
//                        catch
//                        {
//                            await _userManager.DeleteAsync(user);
//                            throw;
//                        }

//                        if (!result.Succeeded)
//                        {
//                            await _userManager.DeleteAsync(user);
//                        }
//                    }
//                    //}

//                }

//                return applicationUser;

//            }
//            catch (Exception ex)
//            {

//                throw;
//            }

//        }
//    public class DatabaseInitializer : IDatabaseInitializer
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly TenantStoreDbContext _tenantStoreDbContext;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<ApplicationRole> _roleManager;
//        private readonly EntityPermissions _entityPermissions;
//        private readonly ILogger _logger;

//        public DatabaseInitializer(
//            TenantStoreDbContext tenantStoreDbContext,
//            ApplicationDbContext context,
//            UserManager<ApplicationUser> userManager,
//            RoleManager<ApplicationRole> roleManager,
//            EntityPermissions entityPermissions,
//            ILogger<DatabaseInitializer> logger)
//        {
//            _tenantStoreDbContext = tenantStoreDbContext;
//            _context = context;
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _entityPermissions = entityPermissions;
//            _logger = logger;
//        }

//        public virtual async Task SeedAsync()
//        {
//            //Apply EF Core migration
//            await MigrateAsync();

//            await EnsureAdminIdentitiesAsync();

//            //Seed blazorboilerplate sample data
//            await SeedDemoDataAsync();

//            _context.Database.ExecuteSqlRaw("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
//        }

//        private async Task MigrateAsync()
//        {
//            await _tenantStoreDbContext.Database.MigrateAsync();
//            await _context.Database.MigrateAsync();
//        }

//        private async Task SeedDemoDataAsync()
//        {
//            if ((await _userManager.FindByNameAsync(DefaultUserNames.User)) == null)
//            {
//                await CreateUserAsync(DefaultUserNames.User, UserConstants.DefaultPassword, "User", "Blazor", "user@blazorboilerplate.com", "+1 (123) 456-7890");
//            }

//            if (_tenantStoreDbContext.TenantInfo.Count() < 2)
//            {
//                _tenantStoreDbContext.TenantInfo.Add(new AppTenantInfo() { Id = "tenant1", Identifier = "tenant1.local", Name = "Microsoft Inc." });
//                _tenantStoreDbContext.TenantInfo.Add(new AppTenantInfo() { Id = "tenant2", Identifier = "tenant2.local", Name = "Contoso Corp." });

//                _tenantStoreDbContext.SaveChanges();
//            }

//            ApplicationUser user = await _userManager.FindByNameAsync(DefaultUserNames.User);

//            if (!_context.UserProfiles.Any())
//                _context.UserProfiles.Add(new UserProfile
//                {
//                    UserId = user.Id,
//                    ApplicationUser = user,
//                    Count = 2,
//                    IsNavOpen = true,
//                    LastPageVisited = "/dashboard",
//                    IsNavMinified = false,
//                    LastUpdatedDate = DateTime.Now
//                });

//            if (!_context.ApiLogs.Any())
//            {
//                _context.ApiLogs.AddRange(
//                new ApiLogItem
//                {
//                    RequestTime = DateTime.Now,
//                    ResponseMillis = 30,
//                    StatusCode = 200,
//                    Method = "Get",
//                    Path = "/api/seed",
//                    QueryString = "",
//                    RequestBody = "",
//                    ResponseBody = "",
//                    IPAddress = "::1",
//                    ApplicationUserId = user.Id
//                },
//                new ApiLogItem
//                {
//                    RequestTime = DateTime.Now,
//                    ResponseMillis = 30,
//                    StatusCode = 200,
//                    Method = "Get",
//                    Path = "/api/seed",
//                    QueryString = "",
//                    RequestBody = "",
//                    ResponseBody = "",
//                    IPAddress = "::1",
//                    ApplicationUserId = user.Id
//                }
//            );
//            }

//            _context.SaveChanges();
//        }

//        public async Task EnsureAdminIdentitiesAsync()
//        {
//            await EnsureRoleAsync(DefaultRoleNames.Administrator, _entityPermissions.GetAllPermissionValues());
//            await CreateUserAsync(DefaultUserNames.Administrator, "admin123", "Admin", "Blazor", "admin@blazorboilerplate.com", "+1 (123) 456-7890", new string[] { DefaultRoleNames.Administrator });

//            ApplicationRole adminRole = await _roleManager.FindByNameAsync(DefaultRoleNames.Administrator);
//            var AllClaims = _entityPermissions.GetAllPermissionValues().Distinct();
//            var RoleClaims = (await _roleManager.GetClaimsAsync(adminRole)).Select(c => c.Value).ToList();
//            var NewClaims = AllClaims.Except(RoleClaims);

//            foreach (string claim in NewClaims)
//                await _roleManager.AddClaimAsync(adminRole, new Claim(ApplicationClaimTypes.Permission, claim));

//            var DeprecatedClaims = RoleClaims.Except(AllClaims);
//            var roles = await _roleManager.Roles.ToListAsync();

//            foreach (string claim in DeprecatedClaims)
//                foreach (var role in roles)
//                    await _roleManager.RemoveClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, claim));

//            _logger.LogInformation("Inbuilt account generation completed");
//        }

//        private async Task EnsureRoleAsync(string roleName, string[] claims)
//        {
//            if ((await _roleManager.FindByNameAsync(roleName)) == null)
//            {
//                if (claims == null)
//                    claims = Array.Empty<string>();

//                string[] invalidClaims = claims.Where(c => _entityPermissions.GetPermissionByValue(c) == null).ToArray();
//                if (invalidClaims.Any())
//                    throw new Exception("The following claim types are invalid: " + string.Join(", ", invalidClaims));

//                ApplicationRole applicationRole = new(roleName);

//                var result = await _roleManager.CreateAsync(applicationRole);

//                ApplicationRole role = await _roleManager.FindByNameAsync(applicationRole.Name);

//                foreach (string claim in claims.Distinct())
//                {
//                    result = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByValue(claim)));

//                    if (!result.Succeeded)
//                        await _roleManager.DeleteAsync(role);
//                }
//            }
//        }

//        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string firstName, string lastName, string email, string phoneNumber, string[] roles = null)
//        {

//            try
//            {
//                ApplicationUser applicationUser = await _userManager.FindByNameAsync(userName);

//                if (applicationUser == null)
//                {
//                    applicationUser = new ApplicationUser
//                    {
//                        UserName = userName,
//                        Email = email,
//                        PhoneNumber = phoneNumber,
//                        FirstName = firstName,
//                        LastName = lastName,
//                        EmailConfirmed = true
//                    };

//                    var result = _userManager.CreateAsync(applicationUser, password).Result;

//                    if (!result.Succeeded)
//                        throw new Exception(result.Errors.First().Description);

//                    result = _userManager.AddClaimsAsync(applicationUser, new Claim[]{
//                        new Claim(ClaimTypes.Name, userName),
//                        new Claim(ClaimTypes.GivenName, firstName),
//                        new Claim(ClaimTypes.Surname, lastName),
//                        new Claim(ClaimTypes.Email, email),
//                        new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.trueString, ClaimValueTypes.Boolean),
//                        new Claim(ClaimTypes.HomePhone, phoneNumber)
//                    }).Result;

//                    if (!result.Succeeded)
//                        throw new Exception(result.Errors.First().Description);

//                    //add claims version of roles
//                    if (roles != null)
//                    {
//                        foreach (var role in roles.Distinct())
//                        {
//                            await _userManager.AddClaimAsync(applicationUser, new Claim($"Is{role}", ClaimValues.trueString));
//                        }

//                        ApplicationUser user = await _userManager.FindByNameAsync(applicationUser.UserName);

//                        try
//                        {
//                            result = await _userManager.AddToRolesAsync(user, roles.Distinct());
//                        }
//                        catch
//                        {
//                            await _userManager.DeleteAsync(user);
//                            throw;
//                        }

//                        if (!result.Succeeded)
//                        {
//                            await _userManager.DeleteAsync(user);
//                        }
//                    }
//                    //}

//                }

//                return applicationUser;

//            }
//            catch (Exception ex)
//            {

//                throw;
//            }

//        }
//    }
//}