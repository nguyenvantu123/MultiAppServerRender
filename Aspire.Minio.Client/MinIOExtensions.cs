using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Minio;
using Minio.AspNetCore.HealthChecks;
using System;
using System.Diagnostics;
using System.Net;

namespace Aspire.Minio.Client
{
    public static class MinIoExtensions
    {

        // public static void AddMinIoClient(this IHostApplicationBuilder builder, string connectionName, Action<MinIOClientSettings>? configureSettings = null)
        //    => AddMinIoClient(builder, connectionName, configureSettings);

        //public static void AddMinIOClient(this IHostApplicationBuilder builder, string configurationSectionName, Action<MinIOClientSettings>? configureSettings = null)
        // => AddMinIoClient(builder, configurationSectionName, configureSettings);


        private static void AddMinIoClient(
         IHostApplicationBuilder builder,
         string configurationSectionName,
         Action<MinIoClientSettings>? configureSettings)
        {
            ArgumentNullException.ThrowIfNull(builder);

            var configSection = builder.Configuration.GetSection(configurationSectionName);

            var settings = new MinIoClientSettings();
            configSection.Bind(settings);


            builder.Services.AddMinio(configureClient => configureClient
              .WithEndpoint(settings.Endpoint)
              .WithCredentials(settings.AccessKey, settings.SecretKey));

            if (settings.HealthChecks)
            {
                builder.TryAddHealthCheck(new HealthCheckRegistration(
                                    $"MinIO.Client_{configurationSectionName}",
                                   sp =>
                                   {
                                       try
                                       {
                                           // if the IConnection can't be resolved, make a health check that will fail
                                           var client = sp.GetRequiredService<MinioClient>();
                                           var options = new MinioHealthCheck(client);
                                           return options;
                                       }
                                       catch (Exception ex)
                                       {
                                           return new FailedHealthCheck(ex);
                                       }
                                   },
                                   failureStatus: default,
                                   tags: default));
            }
        }

        private sealed class FailedHealthCheck(Exception ex) : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
            }
        }
    }
}
