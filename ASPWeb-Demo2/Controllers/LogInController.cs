using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPWeb_Demo2.Controllers
{
    public class LogInController : Controller
    {

        private UsuarioManager usuarioManager = new UsuarioManager();

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Registrar() => View();


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

        [HttpPost]
        public IActionResult Registrar(string nombre, string correo, string contrasena)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(contrasena))
            {
                if (this.verifyContrasena(contrasena))
                {
                    if (this.usuarioManager.addUsuario(nombre, correo, contrasena))
                    {
                        return RedirectToAction("Login", "LogIn");
                    }
                    else return View();
                } else return View();
            } else return View();
        }

        private bool verifyContrasena(string input) => input.Length >= 8 && input.Any(c => char.IsUpper(c)) && input.Any(c => char.IsDigit(c));

    }
}
