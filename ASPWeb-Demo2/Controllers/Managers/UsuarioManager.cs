using ASPWeb_Demo2.Models;
using Newtonsoft.Json;
using System.Net;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class UsuarioManager
    {

        public UsuarioManager() { }

        private string linkToJsonFile = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\usuarios.json";

        private bool updateJson<T>(List<T> toWrite)
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
                    return true;
                }
            }
            return false;
        }

        public List<Usuario>? getListaUsuarios()
        {
            WebRequest request = WebRequest.Create(linkToJsonFile);
            WebResponse response = request.GetResponse();

            using (Stream data = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    string responseServer = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<Usuario>>(responseServer);
                }
            }
        }

        public Usuario? getUsuario(string? nombre) => this.getListaUsuarios().Where(u => u.getNombre() == nombre).FirstOrDefault();

        public bool addUsuario(string nombre, string correo, string contrasena)
        {
            int id = 0;
            if (this.getListaUsuarios().Count() > 0) id = this.getListaUsuarios().Last().getIdUsuario() + 1;
            else id = 1;

            Usuario usuario = new Usuario(id, nombre, correo, contrasena, null);

            List<Usuario> lista = this.getListaUsuarios();
            lista.Add(usuario);

            return this.updateJson(lista.OrderBy(x => x.getIdUsuario()).ToList());
        }

        public bool verificar(string nombre, string contrasena) => this.getListaUsuarios().Any(u => u.getNombre() == nombre && u.getContrasena() == contrasena); 

        public void updateFechaSesion(Usuario usuario)
        {
            List<Usuario> lista = this.getListaUsuarios();

            if (!string.IsNullOrEmpty(usuario.getFechaSesion()))
            {
                if (DateTime.TryParse(usuario.getFechaSesion(), out DateTime fechaAnterior))
                {
                    DateTime fechaAhora = DateTime.Now;
                    if (fechaAhora.Day != fechaAnterior.Day)
                    {
                        foreach (Usuario u in lista)
                        {
                            if (u.getNombre() == usuario.getNombre())
                            {
                                Usuario toUpdate = u;
                                toUpdate.setFechaSesion(fechaAhora.ToString("dd/MM/yyyy"));

                                if (lista.Remove(u))
                                {
                                    lista.Add(toUpdate);
                                    if (this.updateJson(lista.OrderBy(x => x.getIdUsuario()).ToList())) ;
                                }

                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                string fechaNueva = DateTime.Now.ToString("dd/MM/yyyy");

                foreach (Usuario u in lista)
                {
                    if (u.getNombre() == usuario.getNombre())
                    {
                        Usuario toUpdate = u;
                        toUpdate.setFechaSesion(fechaNueva);

                        if (lista.Remove(u))
                        {
                            lista.Add(toUpdate);
                            if (this.updateJson(lista.OrderBy(x => x.getIdUsuario()).ToList())) ;
                        }

                        break;
                    }
                }
            }
        }

    }
}
