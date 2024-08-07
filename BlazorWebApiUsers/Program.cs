using Aspire.Minio.Client;
using Aspire.MongoDb.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using BlazorWebApi.Users;
using BlazorWebApi.Users.Data;
using BlazorWebApi.Users.Models;
using LazyCache;
using IdentityServer4.Services;
using Finbuckle.MultiTenant;
using Breeze.AspNetCore;
using Breeze.Core;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using Finbuckle.MultiTenant.Abstractions;
using BlazorWebApi.Users.Configuration;
using BlazorWebApi.Users.Services;
using BlazorWebApi.Users.Constants;
using AutoMapper;
using WebApp.Mapping;
using Microsoft.AspNetCore.Authorization;
using BlazorWebApi.Authorization;
using BlazorWebApi.Users.Middleware;
using BlazorWebApi.Middleware;
using BlazorBoilerplate.Server.Aop;
using Serilog.Extensions.Logging;
using eShop.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Google.Protobuf.WellKnownTypes;
using System.Reflection;
using static Microsoft.AspNetCore.Http.StatusCodes;
using BlazorWebApi.Users.Extensions;
using Aspire.StackExchange.Redis;
using BlazorWebApi.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Mvc.Authorization;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.AddApplicationServices();

builder.AddRedis("Redis");

builder.AddRabbitMQ("EventBus");

builder.Services.AddSingleton<RedisUserRepository>();

builder.Services.AddControllersWithViews();

builder.Services.AddControllers(options =>
{
    var parameterTransformer = new SlugifyParameterTransformer();
    options.Conventions.Add(new CustomActionNameConvention(parameterTransformer));
    options.Conventions.Add(new RouteTokenTransformerConvention(parameterTransformer));

    var policy = new AuthorizationPolicyBuilder()
       .RequireAuthenticatedUser()
       .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var identitySection = builder.Configuration.GetSection("Identity");


// No identity section, so no authentication
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var identityUrl = identitySection.GetRequiredValue("Url");
    var audience = identitySection.GetRequiredValue("Audience");

    options.Authority = identityUrl;
    options.RequireHttpsMetadata = false;
    options.Audience = audience;

#if DEBUG
    //Needed if using Android Emulator Locally. See https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-8.0#android
    options.TokenValidationParameters.ValidIssuers = [identityUrl, "https://10.0.2.2:5243"];
#else
            options.TokenValidationParameters.ValidIssuers = [identityUrl];
#endif

    options.TokenValidationParameters.ValidateAudience = false;
});

builder.Services.AddAuthorization();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
//builder.Services.AddSwaggerGen(c =>
//{
//    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity Api" });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
//                      Enter 'Bearer' [space] and then your token in the text input below.
//                      \r\n\r\nExample: 'Bearer 12345abcdef'",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            },
//                            Scheme = "oauth2",
//                            Name = "Bearer",
//                            In = ParameterLocation.Header,

//                        },
//                        new List<string>()
//                    }
//                });
//});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
builder.Services.AddTransient<IAuthorizationHandler, DomainRequirementHandler>();
builder.Services.AddTransient<IAuthorizationHandler, EmailVerifiedHandler>();
builder.Services.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();

builder.AddSqlServerDbContext<TenantStoreDbContext>("Identitydb");

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
    .WithStaticStrategy(DefaultTenant.DefaultTenantId);

builder.Services.AddScoped<BlazorWebApi.Users.Models.IUserSession, UserSessionApp>();

builder.Services.AddRazorPages();

//builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver<AppTenantInfo>), typeof(TenantResolver<AppTenantInfo>), ServiceLifetime.Scoped));

builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver), sp => sp.GetRequiredService<ITenantResolver<AppTenantInfo>>(), ServiceLifetime.Scoped));

builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

builder.Services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

#region Automapper
//Automapper to map DTO to Models https://www.c-sharpcorner.com/UploadFile/1492b1/crud-operations-using-automapper-in-mvc-application/
var automapperConfig = new MapperConfiguration(configuration =>
{
    configuration.AddProfile(new MappingModel());
});

var autoMapper = automapperConfig.CreateMapper();

builder.Services.AddSingleton(autoMapper);
#endregion

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


builder.Services.AddIdentityServer(options =>
{
    options.Authentication.CookieLifetime = TimeSpan.FromMinutes(5);

    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

})
.AddInMemoryIdentityResources(Config.GetResources())
.AddInMemoryApiScopes(Config.GetApiScopes())
.AddInMemoryApiResources(Config.GetApis())
.AddInMemoryClients(Config.GetClients(builder.Configuration))
.AddAspNetIdentity<ApplicationUser>()
// TODO: Not recommended for production - you need to store your key material somewhere secure
.AddDeveloperSigningCredential();

builder.Services.AddScoped<EntityPermissions>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
builder.Services.AddTransient<IRedirectService, RedirectService>();
builder.Services.AddTransient<IEmailFactory, EmailFactory>();
builder.Services.AddSingleton<CustomAuthService>();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
builder.Services.AddTransient<IAuthorizationHandler, DomainRequirementHandler>();
builder.Services.AddTransient<IAuthorizationHandler, EmailVerifiedHandler>();
builder.Services.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();

builder.Services.AddTransient<ApiResponseExceptionAspect>()
                .AddTransient<LogExceptionAspect>()
                .AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory());

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();
app.UseAuthentication();

app.MapDefaultControllerRoute();

app.UseDeveloperExceptionPage();
app.UseMultiTenant();
//app.UseMiddleware<UserSessionMiddleware>();

var files = app.NewVersionedApi("Files");

files.MapControllers()
      .RequireAuthorization();

app.UseDefaultOpenApi();

//app.UseSwagger();
//app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.Api v1"));

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{

    var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitializer>();
    databaseInitializer.SeedAsync().Wait();
}

app.Run();
