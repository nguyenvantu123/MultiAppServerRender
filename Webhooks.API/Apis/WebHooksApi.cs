﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MultiAppServer.ServiceDefaults;
using Webhooks.API.Extensions;
using Webhooks.API.Infrastructure;
using Webhooks.API.Model;
using Webhooks.API.Services;

namespace Webhooks.API;

public static class WebHooksApi
{
    public static RouteGroupBuilder MapWebHooksApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/webhooks").HasApiVersion(1.0);

        api.MapGet("/", async (WebhooksContext context, ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();
            var data = await context.Subscriptions.Where(s => s.UserId == userId.ToString()).ToListAsync();
            return TypedResults.Ok(data);
        });

        api.MapGet("/{id:int}", async Task<Results<Ok<WebhookSubscription>, NotFound<string>>> (
            WebhooksContext context,
            ClaimsPrincipal user,
            int id) =>
        {
            var userId = user.GetUserId();
            var subscription = await context.Subscriptions
                .SingleOrDefaultAsync(s => s.Id == id && s.UserId == userId.ToString());
            if (subscription != null)
            {
                return TypedResults.Ok(subscription);
            }
            return TypedResults.NotFound($"Subscriptions {id} not found");
        });

        api.MapPost("/", async Task<Results<Created, BadRequest<string>>> (
            WebhookSubscriptionRequest request,
            IGrantUrlTesterService grantUrlTester,
            WebhooksContext context,
            ClaimsPrincipal user) =>
        {
            var grantOk = await grantUrlTester.TestGrantUrl(request.Url, request.GrantUrl, request.Token ?? string.Empty);

            if (grantOk)
            {
                var subscription = new WebhookSubscription()
                {
                    Date = DateTime.UtcNow,
                    DestUrl = request.Url,
                    Token = request!.Token!,
                    Type = Enum.Parse<WebhookType>(request.Event, ignoreCase: true),
                    UserId = user.GetUserId().ToString()
                };

                context.Add(subscription);
                await context.SaveChangesAsync();

                return TypedResults.Created($"/api/webhooks/{subscription.Id}");
            }
            else
            {
                return TypedResults.BadRequest($"Invalid grant URL: {request.GrantUrl}");
            }
        })
        .ValidateWebhookSubscriptionRequest();

        api.MapDelete("/{id:int}", async Task<Results<Accepted, NotFound<string>>> (
            WebhooksContext context,
            ClaimsPrincipal user,
            int id) =>
        {
            var userId = user.GetUserId();
            var subscription = await context.Subscriptions.SingleOrDefaultAsync(s => s.Id == id && s.UserId == userId.ToString());

            if (subscription != null)
            {
                context.Remove(subscription);
                await context.SaveChangesAsync();
                return TypedResults.Accepted($"/api/webhooks/{subscription.Id}");
            }

            return TypedResults.NotFound($"Subscriptions {id} not found");
        });

        return api;
    }
}
