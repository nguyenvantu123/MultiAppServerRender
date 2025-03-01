namespace WebhookClient.Services;

//public class WebhooksClient( )
//{
//    public Task<HttpResponseMessage> AddWebHookAsync(WebhookSubscriptionRequest payload, string accessToken)
//    {
//        _logger.LogError("day la access_token trong LoadWebhooks:" + accessToken);
//        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
//        return client.PostAsJsonAsync("/api/webhooks", payload);
//    }

//    public async Task<List<WebhookResponse>> LoadWebhooks(string accessToken)
//    {
//        return await client.GetFromJsonAsync<List<WebhookResponse>>("/api/webhooks") ?? [];
//    }
//}

public class WebhooksClient
{
    private readonly HttpClient _httpClient;

    private ILogger<WebhooksClient> _logger;

    public WebhooksClient(IHttpClientFactory httpClientFactory, ILogger<WebhooksClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public Task<HttpResponseMessage> AddWebHookAsync(WebhookSubscriptionRequest payload, string accessToken)
    {
        return _httpClient.PostAsJsonAsync("/api/webhooks", payload);
    }

    public async Task<List<WebhookResponse>> LoadWebhooks(string accessToken)
    {
        return await _httpClient.GetFromJsonAsync<List<WebhookResponse>>("/api/webhooks") ?? [];
    }
}