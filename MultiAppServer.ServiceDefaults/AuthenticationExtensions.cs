using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MultiAppServer.ServiceDefaults;
using Newtonsoft.Json;
using System.Net;

namespace eShop.ServiceDefaults;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {

        var services = builder.Services;
        var configuration = builder.Configuration;

        // {
        //   "Identity": {
        //     "Url": "http://identity",
        //     "Audience": "basket"
        //    }
        // }

        var identitySection = configuration.GetSection("Identity");

        if (!identitySection.Exists())
        {
            // No identity section, so no authentication
            return services;
        }

        // prevent from mapping "sub" claim to nameidentifier.
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
        {
            var identityUrl = identitySection.GetRequiredValue("Url");
            var audience = identitySection.GetRequiredValue("Audience");

            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = audience;

            options.Events = new JwtBearerEvents
            {
                //OnMessageReceived = context =>
                //{
                //    var accessToken = context.Request.Query["access_token"];

                //    // If the request is for our hub...
                //    var path = context.HttpContext.Request.Path;
                //    if (!string.IsNullOrEmpty(accessToken) &&
                //        (path.StartsWithSegments(ApplicationConstants.SignalR.HubUrl)))
                //    {
                //        // Read the token out of the query string
                //        context.Token = accessToken;
                //    }
                //    return Task.CompletedTask;
                //},
                OnAuthenticationFailed = c =>
                {
                    if (c.Exception is SecurityTokenExpiredException)
                    {
                        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("The Token is expired."));
                        return c.Response.WriteAsync(result);
                    }
                    else
                    {
#if DEBUG
                        c.NoResult();
                        c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
#endif
                    }
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized."));
                        return context.Response.WriteAsync(result);
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource."));
                    return context.Response.WriteAsync(result);
                },
            };

        });

        services.AddAuthorization();
        return services;
    }
}
