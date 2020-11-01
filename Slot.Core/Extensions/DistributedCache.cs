using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slot.Core.Extensions
{
    public static class DistributedCache
    {
        private static string MakeKey<T>(string key)
        {
            var t = typeof(T);
            return string.Concat(t.FullName, key);
        }

        public static T Get<T>(this IDistributedCache cache, string key) where T : class
        {
            try
            {
                return cache.Get(MakeKey<T>(key)).FromByteArray<T>();
            }
            catch (Exception ex)
            {
                // TODO how to log the exception?
                return null;
            }
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            try
            {
                var result = await cache.GetAsync(MakeKey<T>(key));
                return result.FromByteArray<T>();
            }
            catch (Exception ex)
            {
                // TODO how to log the exception?
                return null;
            }
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            cache.Set(MakeKey<T>(key), value.ToByteArray(), options);
        }

        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            return cache.SetAsync(MakeKey<T>(key), value.ToByteArray(), options);
        }

        public static void Remove<T>(this IDistributedCache cache, string key)
        {
            cache.Remove(MakeKey<T>(key));
        }

        public static Task RemoveAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default(CancellationToken))
        {
            return cache.RemoveAsync(MakeKey<T>(key), token);
        }
    }
}
