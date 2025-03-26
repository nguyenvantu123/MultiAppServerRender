using Duende.AccessTokenManagement.OpenIdConnect;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Shared
{
    public class InMemoryUserTokenStore : IUserTokenStore
    {
        private readonly ConcurrentDictionary<string, UserToken> _tokens = new ConcurrentDictionary<string, UserToken>();

        public Task StoreTokenAsync(string userName, UserToken token)
        {
            _tokens[userName] = token;
            return Task.CompletedTask;
        }

        public Task<UserToken> GetTokenAsync(string userName)
        {
            _tokens.TryGetValue(userName, out var token);
            return Task.FromResult(token);
        }

        public Task ClearTokenAsync(string userName)
        {
            _tokens.TryRemove(userName, out _);
            return Task.CompletedTask;
        }

        public Task StoreTokenAsync(ClaimsPrincipal user, UserToken token, UserTokenRequestParameters? parameters = null)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentException("User does not have a NameIdentifier claim.");
            }

            _tokens[userId] = token;
            return Task.CompletedTask;
        }

        public Task<UserToken> GetTokenAsync(ClaimsPrincipal user, UserTokenRequestParameters? parameters = null)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentException("User does not have a NameIdentifier claim.");
            }

            _tokens.TryGetValue(userId, out var token);
            return Task.FromResult(token);
        }

        public Task ClearTokenAsync(ClaimsPrincipal user, UserTokenRequestParameters? parameters = null)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentException("User does not have a NameIdentifier claim.");
            }

            _tokens.TryRemove(userId, out _);
            return Task.CompletedTask;
        }
    }
}