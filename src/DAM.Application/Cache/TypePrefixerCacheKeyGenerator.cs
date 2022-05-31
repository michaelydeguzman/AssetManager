using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DAM.Application.Cache
{
    public class TypePrefixerCacheKeyGenerator : ICacheKeyGenerator
    {
        /// <summary>
        /// Creates a {type}_{id} cache key based on the state of the object instance
        /// </summary>
        public string CreateKey(object key, object instance)
        {
            if (instance is Type)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", instance, key);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", instance.GetType(), key);
        }

        /// <summary>
        /// Creates a {type}_{id} cache key based on the state of object instance T
        /// </summary>
        public string CreateKey<T>(object key, T instance)
        {
            if (instance is Type)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", instance, key);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", typeof(T), key);
        }
    }
}
