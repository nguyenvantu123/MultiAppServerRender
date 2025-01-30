using System.Diagnostics;
using BlazorIdentity.Data;
using BlazorIdentity.Users.Constants;
using BlazorIdentity.Users.Data;
using BlazorIdentity.Users.Models;

namespace BlazorIdentity.Users
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
                var userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                var roleManager = (RoleManager<ApplicationRole>)scope.ServiceProvider.GetService(typeof(RoleManager<ApplicationRole>));


                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var strategy = dbContext.Database.CreateExecutionStrategy();

                using var activity = _activitySource.StartActivity("Migrating identity database", ActivityKind.Client);

                var sw = Stopwatch.StartNew();

                await strategy.ExecuteAsync(() => dbContext.Database.MigrateAsync(cancellationToken));

                await SeedAsync(userManager, roleManager);

                logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
                    sw.ElapsedMilliseconds);

                //InitializeDatabaseAsync(userManager, roleManager);
            }
        }


        private async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            logger.LogInformation("Seeding database");

            var adminRole = new ApplicationRole
            {
                Name = Constant.SuperAdministratorRole,
                Description = "Administrator role with full permissions",
                CreatedBy = "Super Admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "Super Admin",
                LastModifiedOn = DateTime.UtcNow,
                Id = Guid.NewGuid(),
            };
            var adminRoleInDb =
                await roleManager.FindByNameAsync(adminRole.Name);

            if (adminRoleInDb == null)
            {
                await roleManager.CreateAsync(adminRole);
                // adminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.SuperAdministratorRole);
                logger.LogInformation("Seeded Administrator Role.");
            }

            //Check if User Exists
            var superUser = new ApplicationUser
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
                Id = Guid.NewGuid()
            };
            var superUserInDb = await userManager.FindByEmailAsync(superUser.Email);
            if (superUserInDb == null)
            {
                await userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                var result = await userManager.AddToRoleAsync(superUser, Constant.SuperAdministratorRole);
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

            var basicRole = new ApplicationRole
            {
                Name = Constant.BasicUserRole,
                Description = "User role",
                CreatedBy = "Super Admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "Super Admin",
                LastModifiedOn = DateTime.UtcNow,
                Id = Guid.NewGuid()
            };
            var basicRoleInDb = await roleManager.FindByNameAsync(Constant.BasicUserRole);
            if (basicRoleInDb == null)
            {
                await roleManager.CreateAsync(basicRole);
                logger.LogInformation("Seeded Basic User Role.");
            }
        }
    }
}