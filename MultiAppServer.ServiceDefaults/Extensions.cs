using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MultiAppServer.ServiceDefaults
{
    // Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
    // This project should be referenced by each service project in your solution.
    // To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
    public static partial class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

            // Uncomment the following to restrict the allowed schemes for service discovery.
            // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
            // {
            //     options.AllowedSchemes = ["https"];
            // });

            return builder;
        }

        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        //.AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
            //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
            //{
            //    builder.Services.AddOpenTelemetry()
            //       .UseAzureMonitor();
            //}

            return builder;
        }

        public static AppConfiguration GetAppConfiguration(this IHostApplicationBuilder builder)
        {
            var cs = builder.Configuration;

            var applicationSettingsConfiguration = cs.GetSection(nameof(AppConfiguration));

            //builder.Services.Configure<AppConfiguration>(applicationSettingsConfiguration);

            AppConfiguration config = applicationSettingsConfiguration.Get<AppConfiguration>() ?? new AppConfiguration();

            return config;
        }

        //        public static IHostApplicationBuilder ConfigureJwtBearToken(this IHostApplicationBuilder builder)
        //        {

        //            var cs = GetAppConfiguration(builder);

        //            var key = Encoding.UTF8.GetBytes(cs.Secret);
        //            builder.Services
        //                .AddAuthentication(authentication =>
        //                {
        //                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //                })
        //                .AddJwtBearer(
        //                bearer =>
        //                {
        //                    bearer.RequireHttpsMetadata = false;
        //                    bearer.SaveToken = true;
        //                    //bearer.TokenValidationParameters.ValidateLifetime = false;
        //                    bearer.TokenValidationParameters = new TokenValidationParameters
        //                    {
        //                        ValidateIssuerSigningKey = true,
        //                        IssuerSigningKey = new SymmetricSecurityKey(key),
        //                        ValidateIssuer = false,
        //                        ValidateAudience = false,
        //                        RoleClaimType = ClaimTypes.Role,
        //                        ClockSkew = TimeSpan.Zero
        //                    };
        //                    bearer.Events = new JwtBearerEvents
        //                    {
        //                        OnMessageReceived = context =>
        //                        {
        //                            var accessToken = context.Request.Query["access_token"];

        //                            // If the request is for our hub...
        //                            var path = context.HttpContext.Request.Path;
        //                            //if (!string.IsNullOrEmpty(accessToken) &&
        //                            //    (path.StartsWithSegments(ApplicationConstants.SignalR.HubUrl)))
        //                            //{
        //                            //    // Read the token out of the query string
        //                            //    context.Token = accessToken;
        //                            //}
        //                            return Task.CompletedTask;
        //                        },
        //                        OnAuthenticationFailed = c =>
        //                        {

        //                            var endpoint = c.HttpContext.GetEndpoint();

        //                            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        //                            {
        //                                // if endpoint has AllowAnonymous doesn't validate the token expiration
        //                                return Task.CompletedTask;
        //                            }

        //                            if (c.Exception is SecurityTokenExpiredException)
        //                            {
        //                                c.Response.StatusCode = (int)HttpStatusCode.OK;
        //                                c.Response.ContentType = "application/json";

        //                                var result = new ResultBase<bool> { StatusCode = 401, ErrorMessages = new List<string> { "The Token is expired." }, Success = false };
        //                                return c.Response.WriteAsJsonAsync(result);
        //                            }
        //                            else
        //                            {
        //#if DEBUG
        //                                c.NoResult();
        //                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //                                c.Response.ContentType = "text/plain";
        //                                return c.Response.WriteAsync(c.Exception.ToString());
        //#else
        //                                                                                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //                                                                                            c.Response.ContentType = "application/json";
        //                                                                                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
        //                                                                                            return c.Response.WriteAsync(result);
        //#endif

        //                            }

        //                            //return Task.CompletedTask;
        //                        },
        //                        OnChallenge = context =>
        //                        {
        //                            context.HandleResponse();

        //                            if (!context.Response.HasStarted)
        //                            {
        //                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //                                context.Response.ContentType = "application/json";

        //                                var result = new ResultBase<bool>
        //                                { StatusCode = 401, ErrorMessages = new List<string> { "You are not Authorized." }, Success = false };

        //                                return context.Response.WriteAsJsonAsync(result);
        //                                //return context.Response.WriteAsync(result);
        //                            }

        //                            return Task.CompletedTask;
        //                        },
        //                        OnForbidden = context =>
        //                        {
        //                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        //                            context.Response.ContentType = "application/json";


        //                            var result = new ResultBase<bool> { StatusCode = 403, ErrorMessages = new List<string> { "You are not authorized to access this resource." }, Success = false };

        //                            //var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource.", 403));
        //                            return context.Response.WriteAsJsonAsync(result);
        //                        },
        //                    };
        //                });

        //            return builder;
        //        }


        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Adding health checks endpoints to applications in non-development environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
            if (app.Environment.IsDevelopment())
            {
                // All health checks must pass for app to be considered ready to accept traffic after starting
                app.MapHealthChecks("/health");

                // Only health checks tagged with the "live" tag must pass for app to be considered alive
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            return app;
        }
    }
}
