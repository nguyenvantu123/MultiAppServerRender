using BlazorBoilerplate.Constants;
using BlazorWebApi.Users.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Polly;
using System.Globalization;
using System.Security.Claims;
using WebApp.Pages;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp.Pages
{
    [AllowAnonymous]
    public class AddTokenViewModel : PageModel
    {

        public async Task<IActionResult> OnPostAsync(SaveTokenModel loginParameters)
        {

            var tokens = new List<AuthenticationToken>
             {
                 new AuthenticationToken {Name = OpenIdConnectParameterNames.AccessToken, Value = loginParameters.Access_Token},
                 new AuthenticationToken {Name = OpenIdConnectParameterNames.RefreshToken, Value = loginParameters.Refresh_Token}
             };

            tokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = loginParameters.Expire_In.ToString("o", CultureInfo.InvariantCulture)
            });

            var prop = new AuthenticationProperties();

            prop.StoreTokens(tokens);
            prop.IsPersistent = true;

            var identity = new ClaimsIdentity();

            identity = new ClaimsIdentity(null, "Server authentication", "name", "role");

            await HttpContext.SignInAsync(new ClaimsPrincipal(identity), prop);

            return LocalRedirect(Url.Content($"~{loginParameters.ReturnUrl}"));
        }
    }
}
