using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Cache
{
    public abstract class ICache<T> where T : class
    {

        public IMemoryCache memoryCache;

        public string key { get; }

        public ICache(IMemoryCache memoryCache, string key)
        {
            this.memoryCache = memoryCache;
            this.key = key;
        }

        public abstract T? GetFromCache();
        public abstract Task SetFromCache(T value);

        public async Task RemoveFromCache()
        {
            this.GetMemoryCache().Remove(this.key);
        }

        public IMemoryCache GetMemoryCache() => this.memoryCache;

    }
}
