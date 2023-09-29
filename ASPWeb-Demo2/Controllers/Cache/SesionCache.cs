using ASPWeb_Demo2.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Cache
{
    public class SesionCache : ICache<Usuario>
    { 

        public SesionCache(IMemoryCache memoryCache) 
            : base (memoryCache, "user") {}

        public override Usuario? GetFromCache()
        {
            Usuario? usuario;
            bool exists = this.GetMemoryCache().TryGetValue(key, out usuario);
            if (exists) return usuario;
            return null;
        }

        public override async Task SetFromCache(Usuario value)
        {
            if (!this.memoryCache.TryGetValue(key, out var usuario))
            {
                if (!(value is null))
                {
                    this.GetMemoryCache().Set(key, value);
                }
            }
        }

    }
}
