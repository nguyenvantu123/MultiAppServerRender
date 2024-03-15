using Polly.Retry;
using Polly;

namespace BlazorWeb
{
    public static class RetryPolicies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
        }
    }
}