using ASPWeb_Demo2.Controllers.Cache;
using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Sockets;
using System.Net;

namespace ASPWeb_Demo2.Controllers
{         
    public class LogInController : Controller
    {

        private SesionCache sessionCache;
        private IMemoryCache memoryCache;

        private UsuarioManager usuarioManager;

        public LogInController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        /*
         * Unicamente muestra la vista LogIn
         */

        [HttpGet]
        public IActionResult Login() => View();

        /*
         * Unicamente muestra la vista registrar
         */

        [HttpGet]
        public IActionResult Registrar() => View();

        [HttpGet]
        public IActionResult Logout() 
        {
            this.GetSesionCache().RemoveFromCache();
            return View();
        } 


        /*
         * Es el metodo que lleva acabo la accion de LogIn
         */

        [HttpPost]
        public IActionResult Login(string nombre, string contrasena, bool check)
        {
            Usuario? usuario = this.getUsuarioManager().GetOne(nombre);
            if (usuario != null && !(string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(contrasena))) 
            {
                if (this.getUsuarioManager().verificar(nombre, contrasena))
                {
                    this.getUsuarioManager().crearSesion(usuario);
                    if (this.GetSesionCache().GetFromCache() == null)
                    {
                        this.GetSesionCache().SetFromCache(usuario);
                    }
                    return RedirectToAction("Inicio","Contacto");
                } else return View();
            } else return View();
        }

        /*
         * Es el metodo que lleva acabo la accion de registro
         */

        [HttpPost]
        public IActionResult Registrar(string nombre, string correo, string contrasena)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(contrasena))
            {
                if (this.verifyContrasena(contrasena))
                {
                    Usuario usuario = new Usuario();
                    int id;

                    if (this.getUsuarioManager().GetAll().Count() > 0) id = this.getUsuarioManager().GetAll().Last().getIdUsuario() + 1;
                    else id = 1;

                    usuario.setIdUsuario(id);
                    usuario.setNombre(nombre);
                    usuario.setContrasena(contrasena);
                    usuario.setCorreo(correo);
                    usuario.setIpv4(this.getIpv4Adress());

                    if (this.getUsuarioManager().Add(usuario))
                    {
                        return RedirectToAction("Login", "LogIn");
                    } else return View();
                } else return View();
            } else return View();
        }

        private bool verifyContrasena(string input) => input.Length >= 8 && input.Any(c => char.IsUpper(c)) && input.Any(c => char.IsDigit(c));

        public SesionCache GetSesionCache()
        {
            if (sessionCache == null)
            {
                sessionCache = new SesionCache(this.memoryCache);
                return sessionCache;
            }
            return sessionCache;
        }

        private UsuarioManager getUsuarioManager()
        {
            if (this.usuarioManager == null)
            {
                usuarioManager = new UsuarioManager(this.memoryCache);
            }
            return usuarioManager;
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

    }
}
