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

        public string crearSesion(Usuario usuario)
        {
            List<Usuario> lista = this.getListaUsuarios();
            if (lista.Count > 0)
            {
                foreach(Usuario u in lista)
                {
                    if (u.getNombre() == usuario.getNombre())
                    {
                        if (lista.Remove(u))
                        {
                            usuario = u;
                            if (u.getSesiones() != null)
                            {
                                List<Sesion> lista_sesiones_old = u.getSesiones();
                                if (lista_sesiones_old.Count > 0)
                                {
                                    if (!lista_sesiones_old.Any(s => s.getFecha() == DateTime.Now.ToString("dd/MM/yyyy")))
                                    {
                                        Sesion sesion = new Sesion();
                                        sesion.setIdSesion(lista_sesiones_old.Last().getIdSesion() + 1);
                                        sesion.setFecha(DateTime.Now.ToString("dd/MM/yyyy"));
                                        sesion.setRegistros(new List<string>());
                                        lista_sesiones_old.Add(sesion);
                                        usuario.setSesiones(lista_sesiones_old.OrderBy(x => x.getIdSesion()).ToList());

                                        lista.Add(usuario);
                                        if (this.updateJson(lista))
                                        {
                                            return "Lista actualizada con exito.";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<Sesion> nueva_lista = new List<Sesion>();
                                Sesion sesion = new Sesion();
                                sesion.setIdSesion(1);
                                sesion.setFecha(DateTime.Now.ToString("dd/MM/yyyy"));
                                sesion.setRegistros(new List<string>());
                                nueva_lista.Add(sesion);
                                usuario.setSesiones(nueva_lista.OrderBy(x => x.getIdSesion()).ToList());

                                lista.Add(usuario);
                                if (this.updateJson(lista))
                                {
                                    return "Lista actualizada con exito.";
                                }
                            }
                        }
                    }
                }
            }

            return "No se pudo actualizar la lista.";
        }

    }
}
