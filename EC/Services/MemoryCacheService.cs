using System.Runtime.Caching;

namespace EC.Services
{
    public class MemoryCacheService
    {
        static object lockObj = new object();
        static CacheItemPolicy policy = new CacheItemPolicy();
        static ObjectCache cache = MemoryCache.Default;

        public void SetToken(string token, string user)
        {
            lock (lockObj)
            {
                cache.Set(token, user, policy);
            }

        }

        public string GetUser(string token)
        {
            return cache[token] as string;
        }
    }
}