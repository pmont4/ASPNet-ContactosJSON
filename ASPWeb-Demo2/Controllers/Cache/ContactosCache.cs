using ASPWeb_Demo2.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Cache
{
    public class ContactosCache : ICache<List<Contacto>>
    {

        public ContactosCache(IMemoryCache memoryCache)
            : base(memoryCache, "contacto") { }

        public override List<Contacto>? GetFromCache()
        {
            List<Contacto>? list;
            if (this.GetMemoryCache().TryGetValue(key, out list)) return list;
            return null;
        }

        public override void SetFromCache(List<Contacto> value)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20),
                Priority = CacheItemPriority.Normal
            };

            if (!this.GetMemoryCache().TryGetValue(key, out var list)) 
            {
                if (!(value is null))
                {
                    this.GetMemoryCache().Set(key, value, cacheOptions);
                }
            }

        }
    }
}
