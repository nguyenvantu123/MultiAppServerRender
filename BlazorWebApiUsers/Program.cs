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
using WebApp.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

builder.Services.AddControllers(options =>
{
    var parameterTransformer = new SlugifyParameterTransformer();
    options.Conventions.Add(new CustomActionNameConvention(parameterTransformer));
    options.Conventions.Add(new RouteTokenTransformerConvention(parameterTransformer));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddMvc().AddNewtonsoftJson(opt =>
{
    // Set Breeze defaults for entity serialization
    var ss = JsonSerializationFns.UpdateWithDefaults(opt.SerializerSettings);
    if (ss.ContractResolver is DefaultContractResolver resolver)
    {
        resolver.NamingStrategy = null;  // remove json camelCasing; names are converted on the client.
    }
})   // Add Breeze exception filter to send errors back to the client
          .AddMvcOptions(o => { o.Filters.Add(new GlobalExceptionFilter()); })
          .AddViewLocalization().AddDataAnnotationsLocalization(options =>
          {
              options.DataAnnotationLocalizerProvider = (type, factory) =>
              {
                  return factory.Create(typeof(Global));
              };
          });

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

//builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver<AppTenantInfo>), typeof(TenantResolver<AppTenantInfo>), ServiceLifetime.Scoped));

builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver), sp => sp.GetRequiredService<ITenantResolver<AppTenantInfo>>(), ServiceLifetime.Scoped));

builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

builder.Services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Users",
        Description = "MultiAppServer.AppHost",
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://github.com/ignaciojvig/ChatAPI/blob/master/LICENSE")
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#region Automapper
//Automapper to map DTO to Models https://www.c-sharpcorner.com/UploadFile/1492b1/crud-operations-using-automapper-in-mvc-application/
var automapperConfig = new MapperConfiguration(configuration =>
{
    configuration.AddProfile(new MappingModel());
});

var autoMapper = automapperConfig.CreateMapper();

builder.Services.AddSingleton(autoMapper);
#endregion

builder.Services.AddScoped<ApplicationPersistenceManager>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


builder.Services.AddIdentityServer(options =>
{
    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

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
//.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();


builder.Services.AddScoped<AppTenantInfo>();
builder.Services.AddScoped<EntityPermissions>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
builder.Services.AddTransient<IRedirectService, RedirectService>();
builder.Services.AddTransient<IEmailFactory, EmailFactory>();
builder.Services.AddSingleton<CustomAuthService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((document, request) =>
        {
            var paths = document.Paths.ToDictionary(item => item.Key.ToLowerInvariant(), item => item.Value);
            document.Paths.Clear();
            foreach (var pathItem in paths)
            {
                document.Paths.Add(pathItem.Key, pathItem.Value);
            }
        });
    });
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();
//}

app.UseMultiTenant();
app.UseMiddleware<UserSessionMiddleware>();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{

    //var scopedService = serviceScope.ServiceProvider.GetRequiredService<AppTenantInfo>();

    var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitializer>();
    databaseInitializer.SeedAsync().Wait();
}

app.Run();

