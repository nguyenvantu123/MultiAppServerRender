using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using System.Security.Claims;
using WebApp.Interfaces;

namespace WebApp
{
    public class CustomAuthenticationStateProvider : ServerAuthenticationStateProvider
    {
        private readonly AccountApiClient _ac;

        public CustomAuthenticationStateProvider(AccountApiClient ac)
        {
            _ac = ac;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState = await base.GetAuthenticationStateAsync();
            var user = authState.User;

            // Get your custom data - in this case some roles


            // add some new identities to the Claims Principal
            //user.AddIdentity(new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Role, "Admin") }));

            // return the modified principal
            return await Task.FromResult(new AuthenticationState(user));
        }
    }
}