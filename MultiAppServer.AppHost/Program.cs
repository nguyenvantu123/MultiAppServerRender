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

var webhookApi = builder.AddProject<Projects.Webhooks_API>("webhooks-api", launchProfileName).WithEnvironment("Identity__Url", identityEndpoint);

var webhookClient = builder.AddProject<Projects.WebhookClient>("webhookclient", launchProfileName);
var identityApi = builder.AddProject<Projects.BlazorIdentityApi>("blazoridentityapi");

identity.WithEnvironment("FileApiClient", file.GetEndpoint(launchProfileName)).WithEnvironment("UsersApiClient", user.GetEndpoint(launchProfileName)).WithEnvironment("WebhooksClient", webhookClient.GetEndpoint(launchProfileName)).WithEnvironment("WebhookApiClient", webhookApi.GetEndpoint(launchProfileName)).WithEnvironment("IdentityApiClient", identityApi.GetEndpoint(launchProfileName));

file.WithEnvironment("Identity__Url", identityEndpoint).WithEnvironment("CallBackUrl", file.GetEndpoint(launchProfileName));

user.WithEnvironment("Identity__Url", identityEndpoint).WithEnvironment("CallBackUrl", user.GetEndpoint(launchProfileName));


webApp.WithReference(identity).WithReference(file).WithEnvironment("CallBackUrl", webApp.GetEndpoint(launchProfileName)).WithEnvironment("IdentityUrl", identityEndpoint).WithReference(user);

identity.WithEnvironment("WebAppClient", webApp.GetEndpoint(launchProfileName));


webhookClient.WithReference(identity).WithReference(webhookApi).WithEnvironment("IdentityUrl", identityEndpoint).WithEnvironment("CallBackUrl", webhookClient.GetEndpoint(launchProfileName));


identityApi.WithReference(identity).WithEnvironment("Identity__Url", identityEndpoint).WithEnvironment("CallBackUrl", identityApi.GetEndpoint(launchProfileName));


builder.Build().Run();

static bool ShouldUseHttpForEndpoints()
{
    const string EnvVarName = "ESHOP_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}
