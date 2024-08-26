using MultiAppServer.ServiceDefaults;
using eShop.ServiceDefaults;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using BlazorApiUser.Behaviors;
using BlazorIdentity.Data;
using BlazorApiUser.Repository;
using BlazorApiUser.Commands.Users;
using BlazorIdentity.Users.Models;
using AutoMapper;
using BlazorApiUser.MapperProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.AddRabbitMQ("Eventbus");

builder.AddSqlServerDbContext<ApplicationDbContext>("FileDb");

builder.AddDefaultAuthentication();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddRoles<ApplicationRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer().AddAspNetIdentity<ApplicationUser>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddScoped<IFilesQueries, FilesQueries>();

//builder.Services.AddScoped<IRequestManager, RequestManager>();

//builder.Services.AddScoped<FileServices>();

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddTransient<IIdentityService, IdentityService>();

//builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<FileDbContext>>();
builder.Services.AddAntiforgery();

// Configure mediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});
//builder.Services.AddScoped<CreateRoleCommandHandler,  CreateRoleCommand>();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddSingleton<EntityPermissions>();
//builder.Services.AddSingleton<UserManager<ApplicationUser>>();
//builder.Services.AddSingleton<RoleManager<ApplicationRole>>();

var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfig());
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseRouting();
app.UseAntiforgery();
app.UseIdentityServer();
app.UseAuthorization();
app.UseAuthentication();
app.MapDefaultEndpoints();

var users = app.NewVersionedApi("Users");

users.MapUsersApiV1()
      .RequireAuthorization();

var roles = app.NewVersionedApi("Roles");

roles.MapRolesApiV1()
      .RequireAuthorization();

app.UseDefaultOpenApi();

app.Run();
