namespace Venice.Domain.Interfaces;

public interface ICacheRepository
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> IsHealthyAsync();
}
