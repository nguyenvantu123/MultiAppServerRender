using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace WebApp.Services
{
    public class AuthStateRevalidation(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<IdentityOptions> options) : RevalidatingServerAuthenticationStateProvider(loggerFactory)
    {
        protected override TimeSpan RevalidationInterval => TimeSpan.FromHours(2);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            return await ValidateSecurityTimeStampAsync(userManager, authenticationState.User);
        }

        private async Task<bool> ValidateSecurityTimeStampAsync(UserManager<IdentityUser> userManager, ClaimsPrincipal claimsPrincipal)
        {
            var user = await userManager.GetUserAsync(claimsPrincipal);

            if (user is null) return false;

            var principalTimeStamp = claimsPrincipal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);

            var userStamp = await userManager.GetSecurityStampAsync(user);

            return principalTimeStamp == userStamp;
        }
    }
}
