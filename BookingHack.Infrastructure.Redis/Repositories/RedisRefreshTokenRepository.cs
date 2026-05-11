using BookingHack.Application.Repositories;
using StackExchange.Redis;

namespace BookingHack.Infrastructure.Redis.Repositories;

public class RedisRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IConnectionMultiplexer _redis;

    public RedisRefreshTokenRepository(IConnectionMultiplexer redis)
        => _redis = redis;

    private static RedisKey TokenKey(string hash) => $"rt:{hash}";
    private static RedisKey UserKey(string userId) => $"rt:user:{userId}";

    public async Task<string?> GetUserIdAsync(string tokenHash)
    {
        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(TokenKey(tokenHash));
        return value.HasValue ? (string?)value : null;
    }

    public async Task StoreAsync(string tokenHash, string userId, TimeSpan expiry)
    {
        var db = _redis.GetDatabase();

        await db.StringSetAsync(TokenKey(tokenHash), userId, expiry);
        await db.SetAddAsync(UserKey(userId), tokenHash);
        await db.KeyExpireAsync(UserKey(userId), expiry);
    }

    public async Task DeleteAsync(string tokenHash)
    {
        var db = _redis.GetDatabase();
        var userId = (string?)await db.StringGetAsync(TokenKey(tokenHash));

        await db.KeyDeleteAsync(TokenKey(tokenHash));

        if (userId is not null)
            await db.SetRemoveAsync(UserKey(userId), tokenHash);
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        var db = _redis.GetDatabase();
        var hashes = await db.SetMembersAsync(UserKey(userId));

        if (hashes.Length == 0) return;

        var tokenKeys = hashes
            .Where(h => h.HasValue)
            .Select(h => TokenKey((string)h!))
            .ToArray();

        await db.KeyDeleteAsync(tokenKeys);
        await db.KeyDeleteAsync(UserKey(userId));
    }
}
