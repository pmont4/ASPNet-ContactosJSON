using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPWeb_Demo2.Controllers
{
    public class LogInController : Controller
    {

        private UsuarioManager usuarioManager = new UsuarioManager();

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


        /*
         * Es el metodo que lleva acabo la accion de LogIn
         */

        [HttpPost]
        public IActionResult Login(string nombre, string contrasena, bool check)
        {
            Usuario usuario = this.usuarioManager.getUsuario(nombre);
            if (usuario != null && !(string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(contrasena))) 
            {
                if (usuarioManager.verificar(nombre, contrasena))
                {
                    Console.WriteLine(this.usuarioManager.crearSesion(usuario));

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
                    if (this.usuarioManager.addUsuario(nombre, correo, contrasena))
                    {
                        return RedirectToAction("Login", "LogIn");
                    } else return View();
                } else return View();
            } else return View();
        }

        private bool verifyContrasena(string input) => input.Length >= 8 && input.Any(c => char.IsUpper(c)) && input.Any(c => char.IsDigit(c));

    }
}
