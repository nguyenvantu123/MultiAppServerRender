using Aspire.Minio.Client;
using Aspire.MongoDb.Driver;
using Aspire.RabbitMQ.Client;
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


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.Services.AddControllersWithViews();

builder.AddDefaultAuthentication();

//builder.Services.AddSwaggerGen(opt =>
//{
//    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
//    //opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    //{
//    //    In = ParameterLocation.Header,
//    //    Description = "Please enter token",
//    //    Name = "Authorization",
//    //    Type = SecuritySchemeType.Http,
//    //    BearerFormat = "JWT",
//    //    Scheme = "bearer"
//    //});

//    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            new string[]{}
//        }
//    });
//});

builder.Services.AddControllers(options =>
{
    var parameterTransformer = new SlugifyParameterTransformer();
    options.Conventions.Add(new CustomActionNameConvention(parameterTransformer));
    options.Conventions.Add(new RouteTokenTransformerConvention(parameterTransformer));
});

var identitySection = builder.Configuration.GetSection("Identity");

//builder.Services.AddAuthentication().AddJwtBearer(options =>
//{

//    //$"{configuration["IdentityApiClient"]}/swagger/oauth2-redirect.html"
//    var identityUrl = identitySection["IdentityApiClient"];
//    var audience = identitySection.GetRequiredValue("Audience");

//    options.Authority = identityUrl;
//    options.RequireHttpsMetadata = false;
//    options.Audience = audience;

//#if DEBUG
//    //Needed if using Android Emulator Locally. See https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-8.0#android
//    options.TokenValidationParameters.ValidIssuers = [identityUrl, "https://10.0.2.2:5243"];
//#else
//            options.TokenValidationParameters.ValidIssuers = [identityUrl];
//#endif

//    options.TokenValidationParameters.ValidateAudience = false;
//});

//builder.Services.AddAuthorization();


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

builder.AddRabbitMqEventBus("eventbus").AddEventBusSubscriptions();

builder.Services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.IsEssential = true;
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//    options.LoginPath = "/Account/Login";
//    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//    // ReturnUrlParameter requires
//    //using Microsoft.AspNetCore.Authentication.Cookies;
//    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
//    options.SlidingExpiration = true;

//    //options.EventsType = typeof(CookieEvents);

//    // Suppress redirect on API URLs in ASP.NET Core -> https://stackoverflow.com/a/56384729/54159
//    options.Events = new CookieAuthenticationEvents()
//    {
//        OnRedirectToAccessDenied = context =>
//        {
//            if (context.Request.Path.StartsWithSegments("/api"))
//            {
//                context.Response.StatusCode = Status403Forbidden;
//            }

//            return Task.CompletedTask;
//        },
//        OnRedirectToLogin = context =>
//        {
//            context.Response.StatusCode = Status401Unauthorized;
//            return Task.CompletedTask;
//        }
//    };
//});

#region Automapper
//Automapper to map DTO to Models https://www.c-sharpcorner.com/UploadFile/1492b1/crud-operations-using-automapper-in-mvc-application/
var automapperConfig = new MapperConfiguration(configuration =>
{
    configuration.AddProfile(new MappingModel());
});

var autoMapper = automapperConfig.CreateMapper();

builder.Services.AddSingleton(autoMapper);
#endregion

//builder.Services.AddScoped<ApplicationPersistenceManager>();

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

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//}).AddCookie(x =>
//{
//    x.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//});
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

//app.UseAuthentication();

app.MapDefaultControllerRoute();

app.UseDeveloperExceptionPage();
app.UseMultiTenant();
app.UseMiddleware<UserSessionMiddleware>();
app.UseDefaultOpenApi();
//app.UseMiddleware<APIResponseRequestLoggingMiddleware>();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{

    var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitializer>();
    databaseInitializer.SeedAsync().Wait();
}

app.Run();
