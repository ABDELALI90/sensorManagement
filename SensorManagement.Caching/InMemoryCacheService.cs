using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace SensorManagement.Caching
{
    // In-Memory implementation with improved error handling
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryCacheService> _logger;

        public InMemoryCacheService(IMemoryCache cache, ILogger<InMemoryCacheService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                _cache.Set(key, value, expiration);
                _logger.LogTrace("Successfully set value in memory cache for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set value in memory cache for key: {Key}", key);
                throw;
            }
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                _cache.TryGetValue(key, out T? value);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get value from memory cache for key: {Key}", key);
                throw;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove value from memory cache for key: {Key}", key);
                throw;
            }
        }
    }
}
