namespace PsStore.Application.Interfaces.RedisCache
{
    public interface IRedisCacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetASync<T>(string key, T value, DateTime? expirationTime = null);
    }
}
