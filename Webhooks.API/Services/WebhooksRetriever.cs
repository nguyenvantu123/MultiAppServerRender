using Microsoft.EntityFrameworkCore;
using Webhooks.API.Infrastructure;
using Webhooks.API.Model;

namespace Webhooks.API.Services;

public class WebhooksRetriever(WebhooksContext db) : IWebhooksRetriever
{
    public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsOfType(WebhookType type)
    {
        return await db.Subscriptions.Where(s => s.Type == type).ToListAsync();
    }
}
