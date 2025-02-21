using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MultiAppServer.ServiceDefaults;

public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddAuthToken(this IHttpClientBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.TryAddTransient<HttpClientAuthorizationDelegatingHandler>();

        builder.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

        return builder;
    }

    private class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<HttpClientAuthorizationDelegatingHandler> _logger;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor, ILogger<HttpClientAuthorizationDelegatingHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor, HttpMessageHandler innerHandler, ILogger<HttpClientAuthorizationDelegatingHandler> logger) : base(innerHandler)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            if (_httpContextAccessor.HttpContext is HttpContext context)
            {
                var accessToken = await context.GetTokenAsync("access_token");

                if (accessToken is not null)
                {
                    if (request.Headers.Authorization?.Scheme != "Bearer")
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                }

                _logger.LogInformation("access_token:" + accessToken);
            }

            var data = await base.SendAsync(request, cancellationToken);

            return data;
        }
    }

}
