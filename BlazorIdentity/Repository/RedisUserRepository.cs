using BlazorIdentity.Users.Models;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorIdentity.Repositories;

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

    public async Task<UserProfile> GetUserProfileAsync(Guid userId)
    {
        using var data = await _database.StringGetLeaseAsync(GetUserKey(userId));

        if (data is null || data.Length == 0)
        {
            return null;
        }
        return JsonSerializer.Deserialize(data.Span, UserSerializationContext.Default.UserProfile);
    }

    public async Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(userProfile, UserSerializationContext.Default.UserProfile);
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

[JsonSerializable(typeof(UserProfile))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class UserSerializationContext : JsonSerializerContext
{

}
