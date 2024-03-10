using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiAppServer.ServiceDefaults.Configurations;
<<<<<<< HEAD
=======
using MultiAppServer.ServiceDefaults.Wrapper;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
using Newtonsoft.Json;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
<<<<<<< HEAD
using Wrapper;
=======
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
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
            http.UseServiceDiscovery();
        });

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

    public static IHostApplicationBuilder ConfigureJwtBearToken(this IHostApplicationBuilder builder)
    {

        var cs = GetAppConfiguration(builder);

        var key = Encoding.UTF8.GetBytes(cs.Secret);
        builder.Services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
            bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                //bearer.TokenValidationParameters.ValidateLifetime = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        //if (!string.IsNullOrEmpty(accessToken) &&
                        //    (path.StartsWithSegments(ApplicationConstants.SignalR.HubUrl)))
                        //{
                        //    // Read the token out of the query string
                        //    context.Token = accessToken;
                        //}
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = c =>
                    {

                        var endpoint = c.HttpContext.GetEndpoint();

                        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
<<<<<<< HEAD
                        {
                            // if endpoint has AllowAnonymous doesn't validate the token expiration
                            return Task.CompletedTask;
                        }

                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.OK;
                            c.Response.ContentType = "application/json";

                            var result = new ResultBase<bool>(401, "The Token is expired.");
                            return c.Response.WriteAsJsonAsync(result);
                        }
                        else
                        {
#if DEBUG
                            c.NoResult();
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
#else
                                                                                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                                                                            c.Response.ContentType = "application/json";
                                                                                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                                                                            return c.Response.WriteAsync(result);
#endif
                        }
                        //return Task.CompletedTask;
=======
                        {
                            // if endpoint has AllowAnonymous doesn't validate the token expiration
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        if (!context.Response.HasStarted)
                        {
<<<<<<< HEAD
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            //var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized.", 401));
=======
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b

                            var result = new ResultBase<bool>(401, "You are not Authorized.");
                            return context.Response.WriteAsJsonAsync(result);
                            //return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = new ResultBase<bool>(403, "You are not authorized to access this resource.");

                        //var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource.", 403));
                        return context.Response.WriteAsJsonAsync(result);
                    },
                };
            });

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
                metrics.AddRuntimeInstrumentation()
                       .AddBuiltInMeters();
            })
            .WithTracing(tracing =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // We want to view all traces in development
                    tracing.SetSampler(new AlwaysOnSampler());
                }

                tracing.AddAspNetCoreInstrumentation()
                       .AddGrpcClientInstrumentation()
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
            builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
        }

        // Uncomment the following lines to enable the Prometheus exporter (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
        // builder.Services.AddOpenTelemetry()
        //    .WithMetrics(metrics => metrics.AddPrometheusExporter());

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.Exporter package)
        // builder.Services.AddOpenTelemetry()
        //    .UseAzureMonitor();

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Uncomment the following line to enable the Prometheus endpoint (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
        // app.MapPrometheusScrapingEndpoint();

        // All health checks must pass for app to be considered ready to accept traffic after starting
        app.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }

    private static MeterProviderBuilder AddBuiltInMeters(this MeterProviderBuilder meterProviderBuilder) =>
        meterProviderBuilder.AddMeter(
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            "System.Net.Http");
}