using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPWeb_Demo2.Controllers
{
    public class ContactoController : Controller
    {

        private readonly ContactoManager contactoManager = new ContactoManager();

        public IActionResult Inicio()
        {
            return View(contactoManager.getListaContactos());
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre, string correo)
        {
            contactoManager.addContacto(nombre, correo);
            return View();
        }
    }

}
