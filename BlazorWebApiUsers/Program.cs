using Aspire.Minio.Client;
using Aspire.MongoDb.Driver;
using Aspire.RabbitMQ.Client;
using Grpc.Core;
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
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using BlazorWebApi.Users;
using BlazorWebApi.Users.Data;
using BlazorWebApi.Users.Models;
using Microsoft.AspNetCore.Mvc;
using LazyCache.Providers;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using ServiceDefaults;
using IdentityServer4.Services;
using eShop.Identity.API;
using eShop.Identity.API.Services;
using IdentityServer4.Stores;
using IdentityServer4.EntityFramework.Stores;
using AutoMapper.Configuration;
using BlazorBoilerplate.Storage;
using Finbuckle.MultiTenant;
using BlazorWebApi.Users.RoleConst;
using Breeze.AspNetCore;
using Breeze.Core;
using Newtonsoft.Json.Serialization;
using BlazorBoilerplate.Shared.Localizer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllersWithViews();

builder.Services.AddControllers();
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

builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

builder.AddSqlServerDbContext<TenantStoreDbContext>("Identitydb");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Users",
        Description = "MultiAppServer",
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://github.com/ignaciojvig/ChatAPI/blob/master/LICENSE")
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

//builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddScoped<ApplicationPersistenceManager>();

builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver<TenantInfo>), typeof(TenantResolver<TenantInfo>), ServiceLifetime.Scoped));

builder.Services.Replace(new ServiceDescriptor(typeof(ITenantResolver), sp => sp.GetRequiredService<ITenantResolver<TenantInfo>>(), ServiceLifetime.Scoped));


builder.Services.AddMultiTenant<TenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, TenantInfo>()
    .WithStaticStrategy(Settings.DefaultTenantId);

//services.AddScoped<LocalizationPersistenceManager>();

// Apply database migration automatically. Note that this approach is not
// recommended for production scenarios. Consider generating SQL scripts from
// migrations instead.

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = "null";
    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    // TODO: Remove this line in production.
    //options.KeyManagement.Enabled = false;
})
.AddInMemoryIdentityResources(Config.GetResources())
.AddInMemoryApiScopes(Config.GetApiScopes())
.AddInMemoryApiResources(Config.GetApis())
.AddInMemoryClients(Config.GetClients(builder.Configuration))
.AddAspNetIdentity<ApplicationUser>()
// TODO: Not recommended for production - you need to store your key material somewhere secure
.AddDeveloperSigningCredential();
//.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
builder.Services.AddTransient<IRedirectService, RedirectService>();

builder.Services.AddSingleton<CustomAuthService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);


builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});



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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();
//}

app.Run();

//var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Version = "v1",
//        Title = "Bff MultiAppServer",
//        Description = "MultiAppServer",
//        License = new OpenApiLicense
//        {
//            Name = "MIT",
//            Url = new Uri("https://github.com/ignaciojvig/ChatAPI/blob/master/LICENSE")
//        }
//    });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = @"JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var config = builder.Configuration;

//var applicationSettingsConfiguration = config.GetSection(nameof(AppConfiguration));

//builder.Services.Configure<AppConfiguration>(applicationSettingsConfiguration);

//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing => tracing.AddSource(IdentityDbInitializer.ActivitySourceName));

//builder.Services.AddProblemDetails();

//builder.Services.AddAutoMapper(typeof(UserAutoMapper));

//builder.AddSqlServerDbContext<UserDbContext>("identitydb");

//builder.AddRabbitMQ("message");

//builder.Services.AddSingleton<IdentityDbInitializer>();

//builder.Services.AddIdentity<User, Role>(options =>
//    {
//        options.Password.RequiredLength = 8;
//        options.Password.RequireDigit = true;
//        options.Password.RequireLowercase = true;
//        options.Password.RequireNonAlphanumeric = true;
//        options.Password.RequireUppercase = true;
//        options.User.RequireUniqueEmail = true;
//    })
//    .AddEntityFrameworkStores<UserDbContext>()
//    .AddDefaultTokenProviders();

//builder.Services.AddHealthChecks()
//    .AddCheck<IdentityDbInitializerHealthCheck>("DbInitializer", null);

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

//builder.Services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));

//builder.Services.AddHostedService(sp => sp.GetRequiredService<IdentityDbInitializer>());

//builder.Services
//    .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
//    .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

//builder.Services.AddApiVersioning(config =>
//{
//    config.DefaultApiVersion = new ApiVersion(1, 0);
//    config.AssumeDefaultVersionWhenUnspecified = true;
//    config.ReportApiVersions = true;
//});

//builder.Services.AddLazyCache();

//builder.ConfigureJwtBearToken();

//var app = builder.Build();

//app.MapDefaultEndpoints();

//app.MapDefaultControllerRoute();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
//        options.RoutePrefix = "swagger";
//        options.DisplayRequestDuration();
//    });

//    app.UseDeveloperExceptionPage();
//}


//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//var cs = builder.GetAppConfiguration();
//if (cs.BehindSSLProxy)
//{
//    app.UseCors();
//    app.UseForwardedHeaders();
//}

//app.MapControllers();

