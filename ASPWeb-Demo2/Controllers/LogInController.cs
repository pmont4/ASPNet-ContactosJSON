using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPWeb_Demo2.Controllers
{
    public class LogInController : Controller
    {

        private UsuarioManager usuarioManager = new UsuarioManager();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registrar() { return View(); }


        [HttpPost]
        public IActionResult Login(string nombre, string contrasena, bool check)
        {
            Console.WriteLine(check);
            Usuario usuario = this.usuarioManager.getUsuario(nombre);
            if (usuario != null && !(string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(contrasena))) 
            {
                if (usuarioManager.verificar(nombre, contrasena))
                {
                    Task task = Task.Run(() => this.usuarioManager.updateFechaSesion(usuario));

                    return RedirectToAction("Inicio","Contacto");
                } else
                {
                    return View();
                }
            } else
            {
                return View();
            }
        }

    }
}
