using Aspire.Hosting;
using IdentityModel;

var builder = DistributedApplication.CreateBuilder(args);


//var sqlIndentity = builder.AddSqlServer("sql")
//                 .AddDatabase("db", "MultiAppServer.AppHost.Users");

//var redis = builder.AddRedis("redis");

//var rabbitMq = builder.AddRabbitMQ("eventbus");

//var user = builder.AddProject<Projects.BlazorIdentityUsers>("BlazorIdentityusers").WithReference(sqlIndentity).WithReference(messaging);

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

var identity = builder.AddProject<Projects.BlazorIdentity>("blazorwebidentity", launchProfileName)
    .WithExternalHttpEndpoints();

var file = builder.AddProject<Projects.BlazorWebApiFiles>("blazorfiles", launchProfileName);

var webApp = builder.AddProject<Projects.WebApp>("webapp", launchProfileName);

var identityEndpoint = identity.GetEndpoint(launchProfileName);

var user = builder.AddProject<Projects.BlazorApiUser>("blazorapiuser");


identity.WithEnvironment("IdentityApiClient", identityEndpoint).WithEnvironment("FileApiClient", file.GetEndpoint(launchProfileName)).WithEnvironment("UsersApiClient", user.GetEndpoint(launchProfileName));

file.WithEnvironment("Identity__Url", identityEndpoint).WithEnvironment("CallBackUrl", file.GetEndpoint(launchProfileName));

user.WithEnvironment("Identity__Url", identityEndpoint).WithEnvironment("CallBackUrl", user.GetEndpoint(launchProfileName));


webApp.WithReference(identity).WithReference(file).WithEnvironment("CallBackUrl", webApp.GetEndpoint(launchProfileName)).WithEnvironment("IdentityUrl", identityEndpoint)
    .WithEnvironment("IdentityApiClient", identityEndpoint).WithReference(user);

identity.WithEnvironment("WebAppClient", webApp.GetEndpoint(launchProfileName));

builder.Build().Run();

static bool ShouldUseHttpForEndpoints()
{
    const string EnvVarName = "ESHOP_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}
