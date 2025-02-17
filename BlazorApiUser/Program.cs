using MultiAppServer.ServiceDefaults;
using eShop.ServiceDefaults;
using BlazorApiUser.Behaviors;
using BlazorApiUser.Repository;
using BlazorApiUser.Commands.Users;
using AutoMapper;
using BlazorApiUser.MapperProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using BlazorApiUser.IntegrationEvents;
using IntegrationEventLogEF.Services;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BlazorApiUser.Filter;
using BlazorApiUser.Apis;
using Aspire.StackExchange.Redis;
using MultiAppServer.EventBus.Abstractions;
using Shared;
using Aspire.Pomelo.EntityFrameworkCore.MySql;
using BlazorApiUser.Repositories;
using BlazorApiUser.Db;
using BlazorApiUser.Models;
using BlazorApiUser.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedis("Redis");

builder.AddRabbitMqEventBus("EventBus");

builder.AddServiceDefaults();

builder.Services.AddSingleton<RedisUserRepository>();

//builder.AddMySqlDataSource("Identitydb");

//builder.AddMySqlDataSource("Identitydb");
builder.AddMySqlDbContext<ApplicationDbContext>("Identitydb");

builder.AddMySqlDbContext<TenantStoreDbContext>("Identitydb");

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
    .WithStaticStrategy(DefaultTenant.DefaultTenantId);

builder.AddDefaultAuthentication();

builder.Services.AddAntiforgery();

//builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<ApplicationDbContext>>();

//builder.Services.AddScoped<IUserIntegrationEventService, UserIntegrationEventService>();

// Configure mediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
    //cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    //cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    //cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    //Disable account confirmation.
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddIdentityServer().AddAspNetIdentity<ApplicationUser>();
//builder.Services.AddAntiforgery();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddSingleton<EntityPermissions>();

//builder.Services.AddAntiforgery();

var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfig());
});
IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddMvc(options =>
//{
//    options.Filters.Add(new MyExceptionFilterAttribute());
//    options.Filters.Add(new MyActionFilterAttribute());
//}).AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//});

var app = builder.Build();
//app.UsePathBase("/api");
var users = app.NewVersionedApi("Users");

users.MapUsersApiV1()
      .RequireAuthorization();

var roles = app.NewVersionedApi("Roles");

roles.MapRolesApiV1()
      .RequireAuthorization();

var tenants = app.NewVersionedApi("Tenants");

tenants.MapTenantsApiV1()
      .RequireAuthorization();

app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

app.UseDefaultOpenApi();
app.UseHttpsRedirection();

app.Run();
