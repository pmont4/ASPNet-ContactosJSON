using ASPWeb_Demo2.Models;
using ASPWeb_Demo2.Util;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class UsuarioManager
    {

        private string linkToJsonFile = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\usuarios.json";

        private volatile JsonUtils jsonUtils;

        public UsuarioManager()
        {
            jsonUtils = new JsonUtils(linkToJsonFile);
        }

        public List<Usuario>? getListaUsuarios() => this.getJsonUtils().deserealizeObjectFromJsonFile<List<Usuario>>();

        public Usuario? getUsuario(string? nombre) => this.getListaUsuarios().Where(u => u.getNombre() == nombre).FirstOrDefault();

        public bool addUsuario(string nombre, string correo, string contrasena)
        {
            int id = 0;
            if (this.getListaUsuarios().Count() > 0) id = this.getListaUsuarios().Last().getIdUsuario() + 1;
            else id = 1;

            Usuario usuario = new Usuario(id, nombre, correo, contrasena, null);

            List<Usuario> lista = this.getListaUsuarios();
            lista.Add(usuario);

            return this.getJsonUtils().updateJson(lista.OrderBy(x => x.getIdUsuario()).ToList());
        }

        public bool verificar(string nombre, string contrasena) => this.getListaUsuarios().Any(u => u.getNombre() == nombre && u.getContrasena() == contrasena);

        public string crearSesion(Usuario usuario)
        {
            List<Usuario>? lista = this.getListaUsuarios();

            if (lista.Count > 0)
            {
                Usuario? u = lista.Where(x => x.getNombre() == usuario.getNombre()).FirstOrDefault();
                if (u != null)
                {
                    if (lista.Remove(u))
                    {
                        usuario = u;
                        if (u.getSesiones() != null)
                        {
                            List<Sesion>? lista_sesiones_old = u.getSesiones();
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
                                    if (this.getJsonUtils().updateJson(lista)) return "Lista actualizada con exito.";
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
                            if (this.getJsonUtils().updateJson(lista)) return "Lista actualizada con exito.";
                        }
                    }
                }
                else return "Usuario no encontrado.";

            }

            return "";
        }

        private JsonUtils getJsonUtils() => this.jsonUtils;

    }
}
