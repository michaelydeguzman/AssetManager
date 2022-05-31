using DAM.Application.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Infrastructure.Caching
{
    public class CacheAsideProvider : ICacheProvider
    { 
        public CacheAsideProvider(IDistributedCache distributedCache, ICacheKeyGenerator cacheKeyGenerator)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _cacheKeyGenerator = cacheKeyGenerator ?? throw new ArgumentNullException(nameof(cacheKeyGenerator));
        }

        #region Protected

        /// <summary>
        /// The appropriate ICacheKeyGenerator instance.
        /// </summary>
        private readonly ICacheKeyGenerator _cacheKeyGenerator;

        /// <summary>
        /// The appropriate IDistributedCache instance.
        /// </summary>
        private readonly IDistributedCache _distributedCache;

        #endregion

        #region Methods

        /// <summary>
        /// Evicts the object from the cache based on the key
        /// </summary>
        public void Evict<T>(object key)
        {
            EvictAsync<T>(key, _cacheKeyGenerator).RunSynchronously();
        }

        /// <summary>
        /// Evicts the object from the cache based on the key
        /// </summary>
        async public Task EvictAsync<T>(object key)
        {
            await EvictAsync<T>(key, _cacheKeyGenerator);
        }

        public void Evict<T>(object key, ICacheKeyGenerator keyGenerator)
        {
            EvictAsync<T>(key, keyGenerator).RunSynchronously();
        }

        /// <summary>
        /// Evicts the object from the cache based on the key
        /// </summary>
        async public Task EvictAsync<T>(object key, ICacheKeyGenerator keyGenerator)
        {
            var ck = keyGenerator.CreateKey(key, typeof(T));

            await _distributedCache.RemoveAsync(ck);
        } 

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        public T Get<T>(object key) where T : class
        {
            return GetAsync<T>(key, _cacheKeyGenerator).Result;
        }

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        async public Task<T> GetAsync<T>(object key) where T : class
        {
            return await GetAsync<T>(key, _cacheKeyGenerator);
        }

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        async public Task<T> GetAsync<T>(object key, ICacheKeyGenerator keyGenerator) where T : class
        { 
            var ck = keyGenerator.CreateKey(key, typeof(T));

            var cachedValue = await _distributedCache.GetStringAsync(ck);  

            return cachedValue != null ? JsonConvert.DeserializeObject<T>(cachedValue) : null;
        }

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        public T Get<T>(object key, Func<T> loader) where T : class
        {
            return GetAsync(key, loader, _cacheKeyGenerator).Result;
        }

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        async public Task<T> GetAsync<T>(object key, Func<T> loader) where T : class
        {
            return await GetAsync(key, loader, _cacheKeyGenerator);
        }

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        async public Task<T> GetAsync<T>(object key, Func<T> loader, ICacheKeyGenerator keyGenerator) where T : class
        {
            var instance = await GetAsync<T>(key, keyGenerator);

            var isEmpty = false;

            var enumerable = instance as IEnumerable;

            if (enumerable != null)
            {
                isEmpty = !HasItems(enumerable);
            }

            if (instance == null || isEmpty)
            {
                instance = loader.Invoke();

                try
                {
                    SaveAsync(key, instance, keyGenerator);
                }
                catch (Exception ex)
                {
                    //if (Logger.IsNotNull())
                    //{
                    //    var msg = "Error saving instance {0} to cache".FormatInvariantCulture(instance.ToString());
                    //    Logger.Error(ex.AsLogMessage(msg));
                    //}
                }
            }

            return instance;
        }

        /// <summary>
        /// Save an object to the cache using the given key and keyGenerator
        /// </summary>
        async public void Save<T>(object key, T value) where T : class
        {
            SaveAsync(key, value, _cacheKeyGenerator); 
        } 

        /// <summary>
        /// Save an object to the cache using the given key and keyGenerator
        /// </summary>
        async public void SaveAsync<T>(object key, T value, ICacheKeyGenerator keyGenerator) where T : class
        { 
            var ck = keyGenerator.CreateKey(key, value);

            await _distributedCache.SetStringAsync(ck, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(8, 0, 0) });  
        }  

        #endregion 

        private bool HasItems(IEnumerable source)
        {
            return source != null &&
                   source.GetEnumerator() != null &&
                   source.GetEnumerator().MoveNext();
        }
    }
}
