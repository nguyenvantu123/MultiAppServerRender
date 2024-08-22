using MultiAppServer.ServiceDefaults;
using eShop.ServiceDefaults;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using BlazorApiUser.Behaviors;
using BlazorIdentity.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.AddRabbitMQ("Eventbus");

builder.AddSqlServerDbContext<ApplicationDbContext>("FileDb");

builder.AddDefaultAuthentication();

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


var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);


var app = builder.Build();

app.UseAuthentication();


app.UseRouting();
app.UseAntiforgery();
app.UseAuthorization();
app.MapDefaultEndpoints();

var files = app.NewVersionedApi("Users");

files.MapUsersApiV1()
      .RequireAuthorization();

app.UseDefaultOpenApi();

app.Run();
