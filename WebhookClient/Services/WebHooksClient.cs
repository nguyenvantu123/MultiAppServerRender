namespace WebhookClient.Services;

public class WebhooksClient(HttpClient client, ILogger<WebhooksClient> _logger)
{
    public Task<HttpResponseMessage> AddWebHookAsync(WebhookSubscriptionRequest payload)
    {

        return client.PostAsJsonAsync("/api/webhooks", payload);
    }

    public async Task<List<WebhookResponse>> LoadWebhooks(string accessToken)
    {
        _logger.LogError("day la access_token trong LoadWebhooks:" + accessToken);
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        return await client.GetFromJsonAsync<List<WebhookResponse>>("/api/webhooks") ?? [];
    }
}
