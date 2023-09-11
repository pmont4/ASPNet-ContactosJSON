using ASPWeb_Demo2.Models;
using Newtonsoft.Json;
using System.Net;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class ContactoManager
    {
        public ContactoManager() { }

        private const string linkToJsonFile = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\contactos.json";

        public List<Contacto>? getListaContactos()
        {
            WebRequest request = WebRequest.Create(linkToJsonFile);
            WebResponse response = request.GetResponse();
            
            using (Stream data = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    string responseServer = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<Contacto>>(responseServer);
                }
            }
        }

        public Contacto? getContacto(int id)
        {
            foreach(Contacto contacto in this.getListaContactos())
            {
                if (contacto.idcontacto == id)
                {
                    return contacto;
                }
            }
            return null;
        }
        
        public void addContacto(String nombre, String correo)
        {
            int id = 0;
            List<Contacto> contactos = this.getListaContactos();
            if (contactos.Count > 0) id = contactos.Last().idcontacto + 1;
            else id = 1;

            Contacto contacto = new Contacto(id, nombre, correo);
            contactos.Add(contacto);

            try
            {
                Task task = Task.Run(() => this.writeContactoJSONFile(contactos));
                task.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void removeContacto(int id)
        {
            List<Contacto> lista = getListaContactos();
            foreach (var c in lista)
            {
                if (c.idcontacto == id)
                {
                    if (lista.Remove(c))
                    {
                        try
                        {
                            Task task = Task.Run(() => this.writeContactoJSONFile(lista));
                            task.Start();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        break;
                    }
                }
            }
        }

        public void updateContacto(int id, string nombre, string correo)
        {
            List<Contacto> lista = getListaContactos();
            foreach (var c in lista)
            {
                if (c.idcontacto == id)
                {
                    if (lista.Remove(c))
                    {
                        Contacto contacto = new Contacto(id, nombre, correo);
                        lista.Add(contacto);

                        var noDupes = lista.OrderBy(x => x.idcontacto).ToHashSet();
                        var nuevaLista = noDupes.ToList();

                        try
                        {
                            Task task = Task.Run(() => this.writeContactoJSONFile(nuevaLista));
                            task.Start();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        break;
                    }
                }
            }
        }

        private async void writeContactoJSONFile(object? escritura)
        {
            File.WriteAllText(linkToJsonFile, string.Empty);

            string json = JsonConvert.SerializeObject(escritura, Formatting.Indented);
            using (StreamWriter writer = File.AppendText(linkToJsonFile))
            {
                writer.Write(json);
            }
        }
    }
}
