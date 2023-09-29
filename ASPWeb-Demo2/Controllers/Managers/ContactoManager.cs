using ASPWeb_Demo2.Controllers.Cache;
using ASPWeb_Demo2.Models;
using ASPWeb_Demo2.Util;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Managers
{

    public class ContactoManager : IManager<Contacto>
    {

        private readonly JsonUtils jsonUtils;
        private readonly ContactosCache contactosCache;

        public ContactoManager(IMemoryCache memoryCache)
        {
            this.jsonUtils = new JsonUtils();
            this.contactosCache = new ContactosCache(memoryCache);
        }

        public List<Contacto>? GetAll() => this.GetJsonUtils().deserealizeObjectFromJsonFile<List<Contacto>>(JsonUtils.CONTACT_FILE_LINK);

        public Contacto? GetOne(object identifier)
        {
            try
            {
                int id = Convert.ToInt32(identifier);
                return this.GetAll().Where(c => c.idcontacto == id).FirstOrDefault();
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task Add(Contacto value)
        {
            List<Contacto>? list = this.GetAll();
            list.Add(value);

            var task = this.getContactosCache().RemoveFromCache(); 
            await task;

            if (task.IsCompletedSuccessfully)
            {
                this.GetJsonUtils().updateJson(JsonUtils.CONTACT_FILE_LINK, list.OrderBy(x => x.idcontacto).ToList());
            } 
        }

        public async Task Remove(object identifier)
        {
            try
            {
                int id = Convert.ToInt32(identifier);
                List<Contacto> list = this.GetAll();

                Contacto? toRemove = list.Where(c => c.idcontacto == id).FirstOrDefault();
                if (toRemove != null)
                {
                    if (list.Remove(toRemove))
                    {
                        var task = this.getContactosCache().RemoveFromCache();
                        await task;
                        if (task.IsCompletedSuccessfully)
                        {
                            this.GetJsonUtils().updateJson(JsonUtils.CONTACT_FILE_LINK, list.OrderBy(x => x.idcontacto).ToList());
                        }
                    }
                }

            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task Update(object identifier, Contacto NewValue)
        {
            try
            {
                int id = Convert.ToInt32(identifier);
                List<Contacto> list = this.GetAll();

                Contacto? toUpdate = list.Where(c => c.idcontacto == id).FirstOrDefault();
                if (toUpdate != null)
                {
                    Contacto Replace = NewValue;
                    Replace.idcontacto = toUpdate.idcontacto;

                    if (list.Remove(toUpdate))
                    {
                        list.Add(Replace);

                        var task = this.getContactosCache().RemoveFromCache();
                        await task;

                        if (task.IsCompletedSuccessfully)
                        {
                            this.GetJsonUtils().updateJson(JsonUtils.CONTACT_FILE_LINK, list.OrderBy(x => x.idcontacto).ToList());
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private JsonUtils GetJsonUtils() => this.jsonUtils;

        private ContactosCache getContactosCache() => this.contactosCache;

    }

}
