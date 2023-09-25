using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Cache
{
    public abstract class ICache<T> where T : class
    {

        private IMemoryCache memoryCache;

        public string key { get; }

        public ICache(IMemoryCache memoryCache, string key)
        {
            this.memoryCache = memoryCache;
            this.key = key;
        }

        public abstract T? GetFromCache();
        public abstract void SetFromCache(T value);

        public void RemoveFromCache()
        {
            this.GetMemoryCache.Remove(this.key);
        }

        private IMemoryCache GetMemoryCache => this.memoryCache;

    }
}
