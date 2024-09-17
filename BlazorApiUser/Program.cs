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
using MediatR;
using System.Reflection;
using BlazorApiUser.IntegrationEvents;
using IntegrationEventLogEF.Services;
using BlazorIdentity.Users.Constants;
using BlazorIdentity.Users.Data;
using Finbuckle.MultiTenant;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

builder.AddSqlServerDbContext<TenantStoreDbContext>("Identitydb");

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
    .WithStaticStrategy(DefaultTenant.DefaultTenantId);

builder.AddDefaultAuthentication();

//builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<ApplicationDbContext>>();

//builder.Services.AddScoped<IUserIntegrationEventService, UserIntegrationEventService>();

builder.AddRabbitMQ("ConnectionStrings:Eventbus");

// Configure mediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
    //cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    //cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    //cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});


//builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
//.AddRoles<ApplicationRole>()
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddSignInManager()
//.AddDefaultTokenProviders();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    //Disable account confirmation.
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer().AddAspNetIdentity<ApplicationUser>();
//builder.Services.AddAntiforgery();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

builder.Services.AddSingleton<EntityPermissions>();

builder.Services.AddAntiforgery();

var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfig());
});
IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

var users = app.NewVersionedApi("Users");

users.MapUsersApiV1()
      .RequireAuthorization();

var roles = app.NewVersionedApi("Roles");

roles.MapRolesApiV1()
      .RequireAuthorization();


app.UseRouting();
app.UseAntiforgery();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();



app.UseDefaultOpenApi();
app.UseHttpsRedirection();

app.Run();
