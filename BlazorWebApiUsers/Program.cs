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

builder.AddSqlServerDbContext<TenantStoreDbContext>("Identitydb");

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
    .WithStaticStrategy(Settings.DefaultTenantId);

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

//builder.Services.AddScoped<IUserStore<ApplicationUser>, MultiTenantUserStore>();


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
builder.Services.AddScoped<AppTenantInfo>();
builder.Services.AddScoped<EntityPermissions>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
builder.Services.AddTransient<IRedirectService, RedirectService>();
builder.Services.AddTransient<IEmailFactory, EmailFactory>();
builder.Services.AddSingleton<CustomAuthService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

//builder.Services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
//builder.Services.AddHostedService(sp => sp.GetRequiredService<DatabaseInitializer>());

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//var store = app.Services.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
//foreach (var tenant in await store.GetAllAsync())
//{
//    await using var db = new ApplicationDbContext(tenant);
//    await db.Database.MigrateAsync();
//}

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
//        Title = "Bff MultiAppServer.AppHost",
//        Description = "MultiAppServer.AppHost",
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

