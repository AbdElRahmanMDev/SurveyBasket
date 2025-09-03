
using Microsoft.Extensions.Caching.Distributed;

namespace SurveyBasket.API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var cachedValue=await _distributedCache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(cachedValue))
                return null;

            return System.Text.Json.JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            await _distributedCache.SetStringAsync(key, System.Text.Json.JsonSerializer.Serialize(value), cancellationToken);
        }

        public async Task Remove(string key, CancellationToken cancellationToken = default)
        {
           await _distributedCache.RemoveAsync(key,cancellationToken);
        }

     
    }
}
