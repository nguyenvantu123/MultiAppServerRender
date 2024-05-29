using Aspire.Minio.Client;
using Aspire.MongoDb.Driver;
using Aspire.RabbitMQ.Client;
using BlazorWebApi.Users.Configurations;
using BlazorWebApi.Users.Domain.Models;
using BlazorWebApi.Users.Localization;
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
using BlazorWebApi.Users.Mapping;
using BlazorWebApi.Users.Repository;
using Microsoft.AspNetCore.Mvc;
using LazyCache.Providers;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bff MultiAppServer",
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
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

var config = builder.Configuration;

var applicationSettingsConfiguration = config.GetSection(nameof(AppConfiguration));

builder.Services.Configure<AppConfiguration>(applicationSettingsConfiguration);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(IdentityDbInitializer.ActivitySourceName));

builder.Services.AddProblemDetails();

builder.Services.AddAutoMapper(typeof(UserAutoMapper));

builder.AddSqlServerDbContext<UserDbContext>("identitydb");

builder.AddRabbitMQ("message");

builder.Services.AddSingleton<IdentityDbInitializer>();

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UserDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHealthChecks()
    .AddCheck<IdentityDbInitializerHealthCheck>("DbInitializer", null);

//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
//});

builder.Services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));

builder.Services.AddHostedService(sp => sp.GetRequiredService<IdentityDbInitializer>());

builder.Services
    .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
    .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

builder.Services.AddLazyCache();

builder.ConfigureJwtBearToken();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapDefaultControllerRoute();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
    });

    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

var cs = builder.GetAppConfiguration();
if (cs.BehindSSLProxy)
{
    app.UseCors();
    app.UseForwardedHeaders();
}

app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});

//#pragma warning disable ASP0014
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapRazorPages();
//    endpoints.MapControllers();
//    endpoints.MapFallbackToFile("index.html");
//});
//#pragma warning restore ASP0014

//app.MapRazorPages();

app.Run();

