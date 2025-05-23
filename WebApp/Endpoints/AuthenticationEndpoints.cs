﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace WebApp.Endpoints;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/account/logout", async (HttpContext httpContext, IAntiforgery antiforgery) =>
        {
            //await antiforgery.ValidateRequestAsync(httpContext);
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        });

        return app;
    }
}
