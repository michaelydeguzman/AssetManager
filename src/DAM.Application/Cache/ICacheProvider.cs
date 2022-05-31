using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Application.Cache
{
    public interface ICacheProvider
    {  
        /// <summary>
        /// Evicts the object from the cache based on the key
        /// </summary>
        void Evict<T>(object key);

        /// <summary>
        /// Evicts the object from the cache based on the key
        /// </summary>
        Task EvictAsync<T>(object key);

        void Evict<T>(object key, ICacheKeyGenerator keyGenerator);

        /// <summary>
        /// Evicts the object from the cache based on the key
        /// </summary>
        Task EvictAsync<T>(object key, ICacheKeyGenerator keyGenerator);

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        T Get<T>(object key) where T : class;

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        Task<T> GetAsync<T>(object key) where T : class;

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        Task<T> GetAsync<T>(object key, ICacheKeyGenerator keyGenerator) where T : class;

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        T Get<T>(object key, Func<T> loader) where T : class;

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        Task<T> GetAsync<T>(object key, Func<T> loader) where T : class;

        /// <summary>
        /// Gets the object from cache.
        /// </summary>
        Task<T> GetAsync<T>(object key, Func<T> loader, ICacheKeyGenerator keyGenerator) where T : class;

        /// <summary>
        /// Save an object to the cache using the given key and keyGenerator
        /// </summary>
        void Save<T>(object key, T value) where T : class;

        /// <summary>
        /// Save an object to the cache using the given key and keyGenerator
        /// </summary>
        void SaveAsync<T>(object key, T value, ICacheKeyGenerator keyGenerator) where T : class; 
    }
}
