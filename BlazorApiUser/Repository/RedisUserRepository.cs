
using Shared.Models;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApiUser.Repositories;

public class RedisUserRepository(ILogger<RedisUserRepository> logger, IConnectionMultiplexer redis)
{
    private readonly IDatabase _database = redis.GetDatabase();

    // implementation:

    // - /basket/{id} "string" per unique basket
    private static RedisKey UserKeyPrefix = "/user/"u8.ToArray();
    // note on UTF8 here: library limitation (to be fixed) - prefixes are more efficient as blobs

    private static RedisKey GetUserKey(Guid userId) => UserKeyPrefix.Append(userId.ToString());

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _database.KeyDeleteAsync(GetUserKey(id));
    }

    public async Task<UserProfileViewModel> GetUserProfileAsync(Guid userId)
    {
        using var data = await _database.StringGetLeaseAsync(GetUserKey(userId));

        if (data is null || data.Length == 0)
        {
            return null;
        }
        return JsonSerializer.Deserialize(data.Span, UserSerializationContext.Default.UserProfileViewModel);
    }

    public async Task<UserProfileViewModel> UpdateUserProfileAsync(UserProfileViewModel userProfile)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(userProfile, UserSerializationContext.Default.UserProfileViewModel);
        var created = await _database.StringSetAsync(GetUserKey(userProfile.UserId), json);

        if (!created)
        {
            logger.LogInformation("Problem occurred persisting the item.");
            return null;
        }


        logger.LogInformation("User item persisted successfully.");
        return await GetUserProfileAsync(userProfile.UserId);
    }
}

[JsonSerializable(typeof(UserProfileViewModel))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class UserSerializationContext : JsonSerializerContext
{

}
