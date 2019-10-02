using EasyCaching.Core;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetBoilerplate.Caching
{
    public class DefaultCacheManager : ICacheManager
    {
        #region Fields

        private readonly IEasyCachingProvider _provider;
        private const int defaultCacheMinutes = 60;

        #endregion

        #region Ctor

        public DefaultCacheManager(IEasyCachingProvider provider)
        {
            _provider = provider;
        }

        #endregion

        #region Methods

        public async Task<T> GetAsync<T>(string key)
        {
            var serializedObject = await _provider.GetAsync<string>(key.ToLowerInvariant());
            if (!serializedObject.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(serializedObject.Value);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire)
        {
            return await GetAsync(key, defaultCacheMinutes, acquire);
        }

        public async Task<T> GetAsync<T>(string key, int cacheMinutes, Func<Task<T>> acquire)
        {
            var serializedObject = await _provider.GetAsync<string>(key.ToLowerInvariant());
            if (serializedObject.HasValue)
                return JsonSerializer.Deserialize<T>(serializedObject.Value);

            var result = await acquire();
            if (result != null && cacheMinutes > 0)
                await SetAsync(key, result, cacheMinutes);

            return result;
        }

        public async Task SetAsync(string key, object data, int cacheTime)
        {
            if (cacheTime <= 0)
                return;

            var serializedObject = JsonSerializer.Serialize(data);
            key = key.ToLowerInvariant();
            await _provider.SetAsync(key, serializedObject, TimeSpan.FromMinutes(cacheTime));
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public async Task<bool> IsSetAsync(string key)
        {
            key = key.ToLowerInvariant();
            return await _provider.ExistsAsync(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public async Task RemoveAsync(string key)
        {
            key = key.ToLowerInvariant();
            await _provider.RemoveAsync(key.ToLowerInvariant());
        }

        /// <summary>
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefix">String key prefix</param>
        public async Task RemoveByPatternAsync(string prefix)
        {
            prefix = prefix.ToLowerInvariant();
            await _provider.RemoveByPrefixAsync(prefix);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public async Task ClearAsync()
        {
            await _provider.FlushAsync();
        }

        #endregion
    }
}