using ASPWeb_Demo2.Models;
using ASPWeb_Demo2.Util;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class ContactoManager
    {

        private volatile JsonUtils jsonUtils;

        public ContactoManager()
        {
            this.jsonUtils = new JsonUtils(linkToJsonFile);
        }

        private string linkToJsonFile = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\contactos.json";

        public List<Contacto>? getListaContactos() => this.GetJsonUtils().deserealizeObjectFromJsonFile<List<Contacto>>();

        public Contacto? getContacto(int? id) => this.getListaContactos().Where(c => c.idcontacto == id).FirstOrDefault();

        public bool addContacto(String nombre, String correo)
        {
            int id = 0;
            List<Contacto>? contactos = this.getListaContactos();
            if (contactos.Count > 0) id = contactos.Last().idcontacto + 1;
            else id = 1;

            Contacto contacto = new Contacto(id, nombre, correo);
            contactos.Add(contacto);

            return this.GetJsonUtils().updateJson(contactos.OrderBy(c => c.idcontacto).ToList());
        }

        public bool removeContacto(int id)
        {
            List<Contacto>? lista = this.getListaContactos();
            if (lista.Count > 0)
            {
                Contacto? c = lista.Where(x => x.idcontacto.Equals(id)).FirstOrDefault();
                if (c != null)
                {
                    if (lista.Remove(c))
                    {
                        List<Contacto> nueva = lista.OrderBy(x => x.idcontacto).ToList();
                        return this.GetJsonUtils().updateJson(nueva.OrderBy(c => c.idcontacto).ToList());
                    }
                    else return false;
                }
                else return false;
            }
            else return false;

        }

        public bool updateContacto(int id, string nombre, string correo)
        {
            List<Contacto>? lista = this.getListaContactos();
            Contacto? c = lista.Where(x => x.idcontacto.Equals(id)).FirstOrDefault();
            if (lista.Remove(c))
            {
                Contacto contacto = c;

                c.nombre = nombre;
                c.correo = correo;

                lista.Add(contacto);
                List<Contacto> nuevaLista = lista.OrderBy(x => x.idcontacto).ToList();

                return this.GetJsonUtils().updateJson(nuevaLista);
            }
            return false;

        }

        private JsonUtils GetJsonUtils() => this.jsonUtils; 

    }

}
