using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPWeb_Demo2.Controllers
{

    public class ContactoController : Controller
    {

        private readonly ContactoManager contactoManager = new ContactoManager();

        [HttpGet]
        public IActionResult Inicio() => View(this.contactoManager.getListaContactos());

        /*
         * 
         * Unicamente muestra la vista crear
         * 
         */

        [HttpGet]
        public IActionResult Crear() => View();

        /*
         * 
         * Unicamente muestra la vista eliminar
         * 
         */

        [HttpGet]
        public IActionResult Eliminar(int? id)
        {
            if (id != null)
            {
                Contacto contacto = this.contactoManager.getContacto(id);
                return View(contacto);
            } else return RedirectToAction("Inicio", "Contacto");
        }

        /*
         * 
         * Unicamente muestra la vista editar 
         * 
         */

        [HttpGet]
        public IActionResult Editar(int? id)
        {
            if (id == null) return RedirectToAction("Inicio", "Contacto");

            Contacto c = this.contactoManager.getContacto(id);

            return View(c);
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de borrar 
         * 
         */

        [HttpPost]
        public IActionResult Crear(string nombre, string correo)
        {
            if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(correo))
            {
                contactoManager.addContacto(nombre, correo);

                return RedirectToAction("Inicio", "Contacto");
            }
            return View();
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de editar 
         * 
         */

        [HttpPost]
        public IActionResult Editar(int id, string nombre, string correo)
        {
            Contacto contacto = this.contactoManager.getContacto(id);
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo))
            {
                if (contacto.nombre != nombre || contacto.correo != correo)
                {
                    this.contactoManager.updateContacto(id, nombre, correo);
                    return RedirectToAction("Inicio", "Contacto");
                }
                else return View(contacto);
            }
            else return View(contacto);
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de eliminar 
         * 
         */

        [HttpPost]
        public IActionResult Eliminar(int id) 
        {
            Contacto contacto = this.contactoManager.getContacto(id);
            this.contactoManager.removeContacto(id);
            return RedirectToAction("Inicio", "Contacto");
        }

    }

}
