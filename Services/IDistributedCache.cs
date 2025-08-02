using Microsoft.Extensions.Caching.Distributed;

namespace pharmacy_management.Services
{
    public class RedisCacheService(IDistributedCache cache)
    {
        private readonly IDistributedCache _cache = cache;

        public async Task CacheSetAsync(string key, string value, int expireMinutes = 10)
        {
            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expireMinutes)
            });
        }

        public async Task<string> CacheGetAsync(string key)
        {
            return await _cache.GetStringAsync(key)??string.Empty;
        }
    }

}