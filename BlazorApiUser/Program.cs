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

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.AddRabbitMQ("Eventbus");

builder.AddSqlServerDbContext<ApplicationDbContext>("FileDb");

builder.AddDefaultAuthentication();

// Configure mediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
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
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseRouting();
app.UseAntiforgery();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

var users = app.NewVersionedApi("Users");

users.MapUsersApiV1()
      .RequireAuthorization();

//var roles = app.NewVersionedApi("Roles");

//roles.MapRolesApiV1()
//      .RequireAuthorization();

app.UseDefaultOpenApi();
app.UseHttpsRedirection();

app.Run();
