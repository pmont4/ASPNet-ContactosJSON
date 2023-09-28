using ASPWeb_Demo2.Controllers.Cache;
using ASPWeb_Demo2.Models;
using ASPWeb_Demo2.Util;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Managers
{

    public class ContactoManager : IManager<Contacto>
    {

        private volatile JsonUtils jsonUtils;
        private volatile ContactosCache contactosCache;

        public ContactoManager(IMemoryCache memoryCache)
        {
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

        public bool Add(Contacto value)
        {
            List<Contacto>? list = this.GetAll();
            list.Add(value);

            this.getContactosCache().RemoveFromCache();
            return this.GetJsonUtils().updateJson(JsonUtils.CONTACT_FILE_LINK, list.OrderBy(x => x.idcontacto).ToList());
        }

        public bool Remove(object identifier)
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
                        this.getContactosCache().RemoveFromCache();
                        return this.GetJsonUtils().updateJson(JsonUtils.CONTACT_FILE_LINK, list.OrderBy(x => x.idcontacto).ToList());
                    }
                    else return false;
                }
                else return false;

            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool Update(object identifier, Contacto NewValue)
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
                        this.getContactosCache().RemoveFromCache();
                        return this.GetJsonUtils().updateJson(JsonUtils.CONTACT_FILE_LINK, list.OrderBy(x => x.idcontacto).ToList());
                    }
                    else return false;
                }
                else return false;
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private JsonUtils GetJsonUtils()
        {
            if (this.jsonUtils == null)
            {
                this.jsonUtils = new JsonUtils();
            }
            return this.jsonUtils;
        }

        private ContactosCache getContactosCache() => this.contactosCache;

    }

}
