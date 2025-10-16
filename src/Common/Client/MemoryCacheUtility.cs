using System.Runtime.Caching;

namespace Common.Client
{
    public static class MemoryCacheUtility
    {
        public static T AddOrGetExistingFromCache<T>(string key, Func<T> funcGetData)
        {
            try
            {
                if (MemoryCache.Default.Contains(key)) //opt-ing performance, saving two objects creation
                {
                    var lazy = MemoryCache.Default[key] as Lazy<T>;
                    if (lazy != null)
                    {
                        return lazy.Value;
                    }
                }
                var data = new Lazy<T>(funcGetData);

                CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddDays(7) };

                //do use AddOrGetExisting in concurrent context here
                var retVal = MemoryCache.Default.AddOrGetExisting(key, data, policy) as Lazy<T>;

                return (retVal ?? data).Value;
            }
            catch (Exception)
            {
                // Remove cache key if exception happens during retrieval.
                MemoryCache.Default.Remove(key);
                throw;
            }
        }

        public static void RemoveFromCache(string key)
        {
            try
            {
                if (MemoryCache.Default.Contains(key)) 
                {
                    MemoryCache.Default.Remove(key);
                }
            }
            catch (Exception)
            {
                // Remove cache key if exception happens during retrieval.
                MemoryCache.Default.Remove(key);
                throw;
            }
        }
    }
}
