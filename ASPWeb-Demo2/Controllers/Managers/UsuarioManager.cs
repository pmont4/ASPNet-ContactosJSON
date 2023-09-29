using ASPWeb_Demo2.Models;
using ASPWeb_Demo2.Util;
using System.Net.Sockets;
using System.Net;
using ASPWeb_Demo2.Controllers.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers.Managers
{
    public class UsuarioManager : IManager<Usuario>
    {

        private JsonUtils jsonUtils;
        private readonly SesionCache sesionCache;

        public UsuarioManager(IMemoryCache memoryCache) 
        {
            this.sesionCache = new SesionCache(memoryCache);
        }

        public List<Usuario>? GetAll() => this.getJsonUtils().deserealizeObjectFromJsonFile<List<Usuario>>(JsonUtils.USER_FILE_LINK);

        public Usuario? GetOne(object identifier)
        {
            try
            {
                string name = Convert.ToString(identifier);
                return this.GetAll().Where(u => u.getNombre() == name).FirstOrDefault();
            } catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task Add(Usuario value)
        {
            List<Usuario>? list = this.GetAll();
            var task = new Task(() =>
            { 
                list.Add(value);
            });
            task.Start();

            await task;

            if (task.IsCompletedSuccessfully)
            {
                this.getJsonUtils().updateJson(JsonUtils.USER_FILE_LINK, list.OrderBy(x => x.getIdUsuario()).ToList());
            }
        }

        public async Task Remove(object identifier)
        {
            try
            {
                int id = Convert.ToInt32(identifier);
                List<Usuario> list = this.GetAll();

                Usuario? toRemove = list.Where(u => u.getIdUsuario() == id).FirstOrDefault();
                if (toRemove != null)
                {
                    if (list.Remove(toRemove))
                    {
                        this.getJsonUtils().updateJson(JsonUtils.USER_FILE_LINK, list.OrderBy(x => x.getIdUsuario()).ToList());
                    }
                }

            } catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);  
            }
        }

        public async Task Update(object identifier, Usuario NewValue)
        {
            throw new NotImplementedException();
        }

        public bool verificar(string nombre, string contrasena) => this.GetAll().Any(u => u.getNombre() == nombre && u.getContrasena() == contrasena);

        public string crearSesion(Usuario usuario)
        {
            List<Usuario>? lista = this.GetAll();

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
                        }
                        else if (current_ipv4 != this.getIpv4Adress())
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
                                    if (this.getJsonUtils().updateJson(JsonUtils.USER_FILE_LINK, lista)) return "Lista actualizada con exito.";
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
                            if (this.getJsonUtils().updateJson(JsonUtils.USER_FILE_LINK, lista)) return "Lista actualizada con exito.";
                        }

                        lista.Add(usuario);
                        if (this.getJsonUtils().updateJson(JsonUtils.USER_FILE_LINK, lista)) return "Lista actualizada con exito.";
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
                Usuario? toUpdate = this.GetAll().Where(x => x.getIdUsuario() == cacheUser.getIdUsuario()).First();
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

                                        List<Usuario>? lista_actualizada = this.GetAll();
                                        Usuario? u = lista_actualizada.Where(u => u.getIdUsuario() == toUpdate.getIdUsuario()).First();
                                        if (lista_actualizada.Remove(u))
                                        {
                                            lista_actualizada.Add(toUpdate);

                                            if (this.getJsonUtils().updateJson(JsonUtils.USER_FILE_LINK, lista_actualizada.OrderBy(u => u.getIdUsuario()).ToList())) return ("Lista actualizada");
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
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private JsonUtils getJsonUtils()
        {
            if (this.jsonUtils == null)
            {
                this.jsonUtils = new JsonUtils();
            }
            return jsonUtils;
        }

        private SesionCache GetSesionCache() => this.sesionCache;

    }
}
