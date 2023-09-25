using ASPWeb_Demo2.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Cache
{
    public class SesionCache : ICache<Usuario>
    {
        private readonly IMemoryCache memoryCache; 

        public SesionCache(IMemoryCache memoryCache) 
            : base (memoryCache, "user")
        {   
            this.memoryCache = memoryCache;
        }

        public override Usuario? GetFromCache()
        {
            Usuario? usuario;
            bool exists = this.memoryCache.TryGetValue(key, out usuario);
            if (exists) return usuario;
            return null;
        }

        public override void SetFromCache(Usuario value)
        {
            if (!this.memoryCache.TryGetValue(key, out var usuario))
            {
                if (!(value is null))
                {
                    this.memoryCache.Set(key, value);
                }
            }
        }

    }
}
