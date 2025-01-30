using BlazorIdentity.Users.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlazorIdentity.Authorization
{
    public class EmailVerifiedRequirement : IAuthorizationRequirement
    {
        public bool IsEmailVerified { get; private set; } //not used

        public EmailVerifiedRequirement(bool isEmailVerified)
        {
            IsEmailVerified = isEmailVerified;
        }
    }

    public class EmailVerifiedHandler : AuthorizationHandler<EmailVerifiedRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            EmailVerifiedRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ApplicationClaimTypes.EmailVerified))
            {
                var claim = context.User.FindFirst(c => c.Type == ApplicationClaimTypes.EmailVerified);
                var isEmailVerified = Convert.ToBoolean(claim.Value);

                if (isEmailVerified)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
