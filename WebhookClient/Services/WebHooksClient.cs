namespace WebhookClient.Services;

public class WebhooksClient(HttpClient client)
{
    public Task<HttpResponseMessage> AddWebHookAsync(WebhookSubscriptionRequest payload)
    {
        return client.PostAsJsonAsync("/api/webhooks", payload);
    }

    public async Task<IEnumerable<WebhookResponse>> LoadWebhooks()
    {

        return await new Task<IEnumerable<WebhookResponse>>(() => new List<WebhookResponse>());
        //return await client.GetFromJsonAsync<IEnumerable<WebhookResponse>>("/api/webhooks") ?? [];
    }
}
