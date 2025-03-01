using Duende.AccessTokenManagement.OpenIdConnect;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace WebhookClient.Extensions
{
    public class ServerSideTokenStore : IUserTokenStore
    {
        private readonly ConcurrentDictionary<string, UserToken> _tokens = new();

        public Task StoreTokenAsync(ClaimsPrincipal user, UserToken token, UserTokenRequestParameters? parameters = null)
        {
            var userId = GetUserId(user);
            if (!string.IsNullOrEmpty(userId))
            {
                _tokens[userId] = token;
            }
            return Task.CompletedTask;
        }

        public Task<UserToken?> GetTokenAsync(ClaimsPrincipal user, UserTokenRequestParameters? parameters = null)
        {
            var userId = GetUserId(user);
            if (!string.IsNullOrEmpty(userId) && _tokens.TryGetValue(userId, out var token))
            {
                return Task.FromResult<UserToken?>(token);
            }
            return Task.FromResult<UserToken?>(null);
        }

        public Task ClearTokenAsync(ClaimsPrincipal user, UserTokenRequestParameters? parameters = null)
        {
            var userId = GetUserId(user);
            if (!string.IsNullOrEmpty(userId))
            {
                _tokens.TryRemove(userId, out _);
            }
            return Task.CompletedTask;
        }

        private static string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found.");
        }
    }
}