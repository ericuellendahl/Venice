using StackExchange.Redis;
using System.Text.Json;
using Venice.Domain.Interfaces;

namespace Venice.Infra.Cache;

public class RedisCacheRepository(IConnectionMultiplexer _muxer) : ICacheRepository
{
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var db = _muxer.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var db = _muxer.GetDatabase();
        var serializedValue = await db.StringGetAsync(key);
        if (serializedValue.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(serializedValue!);
    }

    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            var db = _muxer.GetDatabase();
            await db.PingAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
