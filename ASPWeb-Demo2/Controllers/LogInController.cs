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

        private readonly SesionCache sessionCache;
        private readonly UsuarioManager usuarioManager;

        public LogInController(IMemoryCache memoryCache)
        {
            this.sessionCache = new SesionCache(memoryCache);
            this.usuarioManager = new UsuarioManager(memoryCache);
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
                    return RedirectToAction("Inicio", "Contacto");
                }
                else return View();
            }
            else return View();
        }

        /*
         * Es el metodo que lleva acabo la accion de registro
         */

        [HttpPost]
        public async Task<IActionResult> Registrar(string nombre, string correo, string contrasena)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(contrasena))
            {
                if (this.verifyContrasena(contrasena))
                {
                    Usuario usuario = new Usuario();

                    int id = await this.generateNumber();

                    usuario.setIdUsuario(id);
                    usuario.setNombre(nombre);
                    usuario.setContrasena(contrasena);
                    usuario.setCorreo(correo);
                    usuario.setIpv4(this.getIpv4Adress());

                    var task = this.getUsuarioManager().Add(usuario);
                    await task;

                    if (task.IsCompletedSuccessfully)
                    {
                        task.Dispose();
                        return RedirectToAction("Login", "LogIn");
                    }
                    else return View();
                }
                else return View();
            }
            else return View();
        } 


        private bool verifyContrasena(string input) => input.Length >= 8 && input.Any(c => char.IsUpper(c)) && input.Any(c => char.IsDigit(c));

        public SesionCache GetSesionCache() => this.sessionCache;

        private UsuarioManager getUsuarioManager() => this.usuarioManager;

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

        private async Task<int> generateNumber()
        {
            int number = new Random().Next(1000, 5000);
            if (!this.getUsuarioManager().GetAll().Any(x => x.getIdUsuario() == number)) return number;
            else generateNumber();
            return 0;
        }

    }
}
