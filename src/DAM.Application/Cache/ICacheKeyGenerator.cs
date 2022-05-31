using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Cache
{
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// Creates a cache key based on the state of the object instance
        /// </summary>
        string CreateKey(object key, object instance);


        /// <summary>
        /// Creates a cache key based on the state of the typed object instance
        /// </summary>
        string CreateKey<T>(object key, T instance);
    }
}
