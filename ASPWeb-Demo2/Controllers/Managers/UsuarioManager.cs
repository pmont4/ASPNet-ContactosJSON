using ASPWeb_Demo2.Models;
using ASPWeb_Demo2.Util;
using System.Net.Sockets;
using System.Net;
using ASPWeb_Demo2.Controllers.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class UsuarioManager
    {

        private string linkToJsonFile = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\usuarios.json";

        private volatile JsonUtils jsonUtils;
        private SesionCache sesionCache;

        public UsuarioManager(IMemoryCache memoryCache)
        {
            jsonUtils = new JsonUtils(linkToJsonFile);
            this.sesionCache = new SesionCache(memoryCache);
        }

        public List<Usuario>? getListaUsuarios() => this.getJsonUtils().deserealizeObjectFromJsonFile<List<Usuario>>();

        public Usuario? getUsuario(string? nombre) => this.getListaUsuarios().Where(u => u.getNombre() == nombre).FirstOrDefault();

        public bool addUsuario(string nombre, string correo, string contrasena)
        {
            int id = 0;
            if (this.getListaUsuarios().Count() > 0) id = this.getListaUsuarios().Last().getIdUsuario() + 1;
            else id = 1;

            Usuario usuario = new Usuario(id, nombre, correo, contrasena, this.getIpv4Adress(), null);

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
                    usuario = u;
                    if (lista.Remove(u))
                    {
                        string? current_ipv4 = usuario.getIpv4();

                        if (current_ipv4 == null)
                        {
                            usuario.setIpv4(this.getIpv4Adress());
                        } else if (current_ipv4 != this.getIpv4Adress())
                        {
                            usuario.setIpv4(this.getIpv4Adress());
                        }

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
                                    sesion.setRegistros(new List<Registro>());
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
                            sesion.setRegistros(new List<Registro>());
                            nueva_lista.Add(sesion);
                            usuario.setSesiones(nueva_lista.OrderBy(x => x.getIdSesion()).ToList());

                            lista.Add(usuario);
                            if (this.getJsonUtils().updateJson(lista)) return "Lista actualizada con exito.";
                        }

                        lista.Add(usuario);
                        if (this.getJsonUtils().updateJson(lista)) return "Lista actualizada con exito.";
                    }
                }
                else return "Usuario no encontrado.";

            }

            return "a";
        }


        public string updateRegistroUsuario(string mensaje)
        {
            Usuario? cacheUser = this.GetSesionCache().GetFromCache();
            if (cacheUser != null)
            {
                Usuario? toUpdate = this.getListaUsuarios().Where(x => x.getIdUsuario() == cacheUser.getIdUsuario()).First();
                if (toUpdate != null)
                {
                    List<Sesion>? lista_sesiones = toUpdate.getSesiones();
                    if (!lista_sesiones.Equals(null))
                    {
                        if (lista_sesiones.Count > 0)
                        {
                            Sesion? sesionToUpdate = lista_sesiones.Find(s => s.getFecha() == DateTime.Now.ToString("dd/MM/yyyy"));
                            if (lista_sesiones.Remove(sesionToUpdate))
                            {
                                if (!sesionToUpdate.Equals(null))
                                {
                                    List<Registro>? lista_registros = sesionToUpdate.getRegistros();
                                    if (!lista_registros.Equals(null))
                                    {
                                        Registro registro = new Registro();

                                        int id_registro = 1;
                                        if (lista_registros.Count > 0) id_registro = lista_registros.Last().getIdRegistro() + 1;

                                        registro.setIdRegistro(id_registro);
                                        registro.setFecha(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                        registro.setTexto(id_registro + " - " + mensaje);

                                        lista_registros.Add(registro);
                                        sesionToUpdate.setRegistros(lista_registros);
                                        lista_sesiones.Add(sesionToUpdate);

                                        toUpdate.setSesiones(lista_sesiones.OrderBy(s => s.getIdSesion()).ToList());

                                        List<Usuario>? lista_actualizada = this.getListaUsuarios();
                                        Usuario? u = lista_actualizada.Where(u => u.getIdUsuario() == toUpdate.getIdUsuario()).First();
                                        if (lista_actualizada.Remove(u))
                                        {
                                            lista_actualizada.Add(toUpdate);

                                            if (this.getJsonUtils().updateJson(lista_actualizada.OrderBy(u => u.getIdUsuario()).ToList())) return ("Lista actualizada");
                                        }
                                    }
                                    else return ("Lista no actualizada, se retorno nulo en la lista de registros.");
                                }
                                else return ("Lista no actualizada, se retorno nulo en la lista de sesiones.");
                            }
                        }
                    }
                }
                else
                {
                    return ("Lista no actualizada, el usuario actual fue nulo.");
                }
                return ("Lista no actualizada.");
            }
            return ("No se pudo actualizar la lista");
        }

        private string? getIpv4Adress()
        {
            try
            {
                string? toReturn = null;

                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        toReturn = ip.ToString();
                    }
                }

                return toReturn;
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        private JsonUtils getJsonUtils() => this.jsonUtils;
        private SesionCache GetSesionCache() => this.sesionCache;

    }
}
