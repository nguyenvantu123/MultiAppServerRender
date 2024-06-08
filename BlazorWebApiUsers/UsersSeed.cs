
using BlazorWebApi.Users.Data;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.RoleConst;
using eShop.Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace eShop.Identity.API;

public class UsersSeed(ILogger<UsersSeed> logger, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IDbSeeder<ApplicationDbContext>
{
    public async Task SeedAsync(ApplicationDbContext context)
    {

        logger.LogInformation("Seeding database");

        var adminRole = new ApplicationRole
        {
            Name = Constants.SuperAdministratorRole,
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

        var basicRole = new ApplicationRole
        {
            Name = Constants.BasicUserRole,
            Description = "User role",
            CreatedBy = "Super Admin",
            CreatedOn = DateTime.UtcNow,
            LastModifiedBy = "Super Admin",
            LastModifiedOn = DateTime.UtcNow,
            Id = Guid.NewGuid()
        };

        var basicRoleInDb = await roleManager.FindByNameAsync(Constants.BasicUserRole);
        if (basicRoleInDb == null)
        {
            await roleManager.CreateAsync(basicRole);
            logger.LogInformation("Seeded Basic User Role.");
        }

        var alice = await userManager.FindByNameAsync("alice");

        if (alice == null)
        {
            alice = new ApplicationUser
            {
                UserName = "alice",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
                CardHolderName = "Alice Smith",
                CardNumber = "4012888888881881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Expiration = "12/24",
                Id = Guid.NewGuid(),
                LastName = "Smith",
                Name = "Alice",
                PhoneNumber = "1234567890",
                ZipCode = "98052",
                State = "WA",
                Street = "15703 NE 61st Ct",
                SecurityNumber = "123"
            };

            var result = userManager.CreateAsync(alice, "Pass123$").Result;

            await userManager.AddToRoleAsync(alice, Constants.SuperAdministratorRole);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("alice created");
            }
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("alice already exists");
            }
        }

        var bob = await userManager.FindByNameAsync("bob");

        if (bob == null)
        {
            bob = new ApplicationUser
            {
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true,
                CardHolderName = "Bob Smith",
                CardNumber = "4012888888881881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Expiration = "12/24",
                Id = Guid.NewGuid(),
                LastName = "Smith",
                Name = "Bob",
                PhoneNumber = "1234567890",
                ZipCode = "98052",
                State = "WA",
                Street = "15703 NE 61st Ct",
                SecurityNumber = "456"
            };

            var result = await userManager.CreateAsync(bob, "Pass123$");

            await userManager.AddToRoleAsync(bob, Constants.BasicUserRole);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("bob created");
            }
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("bob already exists");
            }
        }
    }
}
