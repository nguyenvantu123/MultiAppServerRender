using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using BlazorIdentity.Users;
using BlazorIdentity.Users.Data;
using BlazorIdentity.Users.Models;
using LazyCache;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using Finbuckle.MultiTenant.Abstractions;
using BlazorIdentity.Users.Configuration;
using BlazorIdentity.Users.Constants;

using Serilog.Extensions.Logging;
using Aspire.StackExchange.Redis;
using BlazorIdentity.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;
using AutoMapper;
using BlazorIdentity.Data;
using BlazorIdentityConfiguration.Constants;
using BlazorIdentityConfiguration.Interfaces;
using BlazorIdentityConfiguration;
using Microsoft.AspNetCore.Mvc.Localization;
using BlazorIdentity.Helpers.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.AddApplicationServices();

builder.AddRedis("Redis");

builder.AddRabbitMqEventBus("EventBus");

builder.Services.AddSingleton<RedisUserRepository>();


builder.Services.AddControllers(options =>
{
    var parameterTransformer = new SlugifyParameterTransformer();
    options.Conventions.Add(new CustomActionNameConvention(parameterTransformer));
    options.Conventions.Add(new RouteTokenTransformerConvention(parameterTransformer));
});

builder.Services.AddAuthorization();

builder.AddSqlServerDbContext<TenantStoreDbContext>("Identitydb");

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
    .WithStaticStrategy(DefaultTenant.DefaultTenantId);

builder.Services.AddRazorPages();

//builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver<AppTenantInfo>), typeof(TenantResolver<AppTenantInfo>), ServiceLifetime.Scoped));

builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver), sp => sp.GetRequiredService<ITenantResolver<AppTenantInfo>>(), ServiceLifetime.Scoped));

builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


builder.Services.AddIdentityServer()
.AddInMemoryIdentityResources(Config.GetResources())
.AddInMemoryApiScopes(Config.GetApiScopes())
.AddInMemoryApiResources(Config.GetApis())
.AddInMemoryClients(Config.GetClients(builder.Configuration))
.AddAspNetIdentity<ApplicationUser>()
// TODO: Not recommended for production - you need to store your key material somewhere secure
.AddDeveloperSigningCredential();

var configuration = builder.Configuration;
var rootConfiguration = new RootConfiguration();
configuration.GetSection(ConfigurationConsts.RegisterConfigurationKey).Bind(rootConfiguration.RegisterConfiguration);
configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);

builder.Services.AddSingleton(rootConfiguration);
builder.Services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory());
builder.Services.AddSingleton<IRootConfiguration, RootConfiguration>();
builder.Services.AddTransient<IViewLocalizer, ResourceViewLocalizer>();
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

//using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
//{

//    var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitializer>();
//    databaseInitializer.SeedAsync().Wait();
//}

app.Run();
