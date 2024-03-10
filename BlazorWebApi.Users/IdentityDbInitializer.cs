using BlazorWebApi.Data;
using BlazorWebApi.Identity.RoleConst;
using BlazorWebApi.Identity.UserConst;
using BlazorWebApi.Users.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace BlazorWebApi.Identity
{
    internal sealed class IdentityDbInitializer(IServiceProvider serviceProvider, ILogger<IdentityDbInitializer> logger)
     : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";

        private readonly ActivitySource _activitySource = new(ActivitySourceName);


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            //var dbContext = scope.ServiceProvider.GetRequiredService<UserDBContext>();

            using (var scope = serviceProvider.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var userManager = (UserManager<User>)scope.ServiceProvider.GetService(typeof(UserManager<User>));
                var roleManager = (RoleManager<UserRole>)scope.ServiceProvider.GetService(typeof(RoleManager<UserRole>));


                var dbContext = scope.ServiceProvider.GetRequiredService<UserDBContext>();

                var strategy = dbContext.Database.CreateExecutionStrategy();

                using var activity = _activitySource.StartActivity("Migrating identity database", ActivityKind.Client);

                var sw = Stopwatch.StartNew();

                await strategy.ExecuteAsync(() => dbContext.Database.MigrateAsync(cancellationToken));

                await SeedAsync(userManager, roleManager);

                logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

                //InitializeDatabaseAsync(userManager, roleManager);
            }
        }


        private async Task SeedAsync(UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            logger.LogInformation("Seeding database");

            var adminRole = new UserRole
            {
                Name = RoleConstants.SuperAdministratorRole,
                Description = "Administrator role with full permissions",
                CreatedBy = "Super Admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "Super Admin",
                LastModifiedOn = DateTime.UtcNow
            };
            var adminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.SuperAdministratorRole);
            if (adminRoleInDb == null)
            {
                await roleManager.CreateAsync(adminRole);
                adminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.SuperAdministratorRole);
                logger.LogInformation("Seeded Administrator Role.");
            }
            //Check if User Exists
            var superUser = new User
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "superadmin@gmail.com",
                UserName = "adminstrator",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedOn = DateTime.Now,
                IsActive = true,
                CreatedBy = "Super Admin",
                LastModifiedBy = "Super Admin",
                LastModifiedOn = DateTime.UtcNow,
                ProfilePictureDataUrl = ""
            };
            var superUserInDb = await userManager.FindByEmailAsync(superUser.Email);
            if (superUserInDb == null)
            {
                await userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                var result = await userManager.AddToRoleAsync(superUser, RoleConstants.SuperAdministratorRole);
                if (result.Succeeded)
                {
                    logger.LogInformation("Seeded Default SuperAdmin User.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        logger.LogError(error.Description);
                    }
                }
            }

            var basicRole = new UserRole
            {
                Name = RoleConstants.BasicUserRole,
                Description = "User role",
                CreatedBy = "Super Admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "Super Admin",
                LastModifiedOn = DateTime.UtcNow
            };
            var basicRoleInDb = await roleManager.FindByNameAsync(RoleConstants.BasicUserRole);
            if (basicRoleInDb == null)
            {
                await roleManager.CreateAsync(basicRole);
                logger.LogInformation("Seeded Basic User Role.");
            }

        }
    }
}
