var builder = DistributedApplication.CreateBuilder(args);


var sqlIndentity = builder.AddSqlServer("sql")
                 .AddDatabase("db", "MultiAppServer.Users");

var messaging = builder.AddRabbitMQ("messaging");

var fileCache = builder.AddRedis("fileCache");

var user = builder.AddProject<Projects.BlazorWebApiUsers>("blazorwebapiusers").WithReference(sqlIndentity).WithReference(messaging);

var bff = builder.AddProject<Projects.BlazorWebApiBff>("blazorwebapibff").WithReference(user).WithReference(messaging); ;

var file = builder.AddProject<Projects.BlazorWebApiFiles>("blazorwebapifiles").WithReference(messaging);


builder.AddProject<Projects.WebApp>("webapp");


builder.Build().Run();
