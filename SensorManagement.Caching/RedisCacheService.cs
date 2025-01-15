using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SensorManagement.Caching
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _cacheDB;

        private bool _disposed;

        public RedisCacheService(
            IAsyncPolicy retryPolicy,
            IOptions<RedisCacheOptions> options,
            ILogger<RedisCacheService> logger,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));

            var redisOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));

            _cacheDB = _connectionMultiplexer.GetDatabase();
            InitializeConnectionEvents();

            _logger.LogInformation("RedisCacheService initialized successfully.");
        }

        private void InitializeConnectionEvents()
        {
            _connectionMultiplexer.ConnectionFailed += (sender, e) =>
            {
                _logger.LogError("Redis connection failed. Endpoint: {Endpoint}, FailureType: {FailureType}",
                    e.EndPoint, e.FailureType);
            };

            _connectionMultiplexer.ConnectionRestored += (sender, e) =>
            {
                _logger.LogInformation("Redis connection restored. Endpoint: {Endpoint}", e.EndPoint);
            };
        }

        public async Task<T> GetAsync<T>(string key)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            try
            {
                _logger.LogInformation("Fetching data for key {Key}", key);
                var jsonData = await _cacheDB.StringGetAsync(key);

                if (string.IsNullOrEmpty(jsonData))
                {
                    _logger.LogInformation("No data found for key {Key}", key);
                    return default;
                }

                _logger.LogInformation("Deserializing data for key {Key}", key);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data for key {Key}", key);
                throw;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            await _semaphore.WaitAsync();
            try
            {
                var jsonData = JsonSerializer.Serialize(value);
                _logger.LogInformation("Serialized data for key {Key}: {JsonData}", key, jsonData);

                var result = await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Setting data for key {Key} in cache.", key);
                    return await _cacheDB.StringSetAsync(key, jsonData, expiration);
                });

                _logger.LogInformation("Set data for key {Key} succeeded: {Result}", key, result);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveAsync(string key)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            await _semaphore.WaitAsync();
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Removing key {Key} from cache.", key);
                    await _cacheDB.KeyDeleteAsync(key);
                });
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RedisCacheService));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _semaphore.Dispose();
                _connectionMultiplexer.Dispose();
            }

            _disposed = true;
        }
    }
}
