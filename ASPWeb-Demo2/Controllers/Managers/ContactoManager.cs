using ASPWeb_Demo2.Models;
using Newtonsoft.Json;
using System.Net;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class ContactoManager
    {
        public ContactoManager() { }

        private volatile string linkToJsonFile = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\contactos.json";

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

        public Contacto? getContacto(int? id) => this.getListaContactos().Where(c => c.getIdContacto() == id).FirstOrDefault();
        
        public void addContacto(String nombre, String correo)
        {
            int id = 0;
            List<Contacto> contactos = this.getListaContactos();
            if (contactos.Count > 0) id = contactos.Last().getIdContacto() + 1;
            else id = 1;

            Contacto contacto = new Contacto(id, nombre, correo);
            contactos.Add(contacto);

            try
            {
                Task task = Task.Run(() => this.updateJson(contactos));
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
                if (c.getIdContacto() == id)
                {
                    if (lista.Remove(c))
                    {
                        var nuevaLista = lista.OrderBy(x => x.getIdContacto()).ToList();

                        try
                        {
                            Task task = Task.Run(() => this.updateJson(nuevaLista));
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
                if (c.getIdContacto() == id)
                {
                    if (lista.Remove(c))
                    {
                        Contacto contacto = c;

                        c.setNombre(nombre);
                        c.setCorreo(correo);

                        lista.Add(contacto);
                        var nuevaLista = lista.OrderBy(x => x.getIdContacto()).ToList();

                        try
                        {
                            Task task = Task.Run(() => this.updateJson(nuevaLista));
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

        private void updateJson<T>(List<T>? toWrite)
        {
            if (toWrite != null && toWrite.Count > 0)
            {
                var noDupes = toWrite.ToHashSet();
                var nuevaList = noDupes.ToList();

                File.WriteAllText(linkToJsonFile, string.Empty);

                string json = JsonConvert.SerializeObject(nuevaList, Formatting.Indented);
                using (StreamWriter writer = File.AppendText(linkToJsonFile))
                {
                    writer.Write(json);
                }
            }
        }

    }

}
