using ASPWeb_Demo2.Controllers.Cache;
using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers
{
    public class ContactoController : Controller
    {

        private SesionCache sesionCache;
        private ContactosCache contactosCache;

        private volatile ContactoManager contactoManager;
        private volatile UsuarioManager usuarioManager;

        private IMemoryCache memoryCache;

        public ContactoController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Inicio()
        {
            if (this.GetSesionCache().GetFromCache() != null)
            {
                List<Contacto>? model = null;
                if (!(this.GetContactosCache().GetFromCache() is null))
                {
                    model = this.GetContactosCache().GetFromCache();
                }
                else
                {
                    model = this.GetContactoManager().getListaContactos();
                    this.GetContactosCache().SetFromCache(model);
                }
                return View(model);
            }
            else return RedirectToAction("Login", "LogIn");
        }


        /*
         * 
         * Unicamente muestra la vista crear
         * 
         */

        [HttpGet]
        public IActionResult Crear() => this.GetSesionCache().GetFromCache() != null ? View() : RedirectToAction("Login", "LogIn");

        /*
         * 
         * Unicamente muestra la vista eliminar
         * 
         */

        [HttpGet]
        public IActionResult Eliminar(int? id)
        {
            if (this.GetSesionCache().GetFromCache() != null)
            {
                if (id != null)
                {
                    Contacto contacto = this.GetContactoManager().getContacto(id);
                    return View(contacto);
                }
                else return RedirectToAction("Inicio", "Contacto");
            } else return RedirectToAction("Login", "LogIn");
        }

        /*
         * 
         * Unicamente muestra la vista editar 
         * 
         */

        [HttpGet]
        public IActionResult Editar(int? id)
        { 
            if (this.GetSesionCache().GetFromCache() != null)
            {
                if (id == null) return RedirectToAction("Inicio", "Contacto");
                Contacto c = this.GetContactoManager().getContacto(id);
                return View(c);
            } else return RedirectToAction("Login", "LogIn");
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de borrar 
         * 
         */

        [HttpPost]
        public IActionResult Crear(string nombre, string correo)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo))
            {
                if (this.GetContactoManager().addContacto(nombre, correo))
                {
                    Console.WriteLine(this.getUsuarioManager().updateRegistroUsuario(this.RegistroFormato("crear", nombre)));
                    return RedirectToAction("Inicio", "Contacto");
                } 
            } else return View();

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
            Contacto contacto = this.GetContactoManager().getContacto(id);
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo))
            {
                if (contacto.nombre != nombre || contacto.correo != correo)
                {
                    if (this.GetContactoManager().updateContacto(id, nombre, correo))
                    {
                        Console.WriteLine(this.getUsuarioManager().updateRegistroUsuario(this.RegistroFormato("editar", nombre)));
                    }
                    return RedirectToAction("Inicio", "Contacto");
                } else return View(contacto);
            } else return View(contacto);
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de eliminar 
         * 
         */

        [HttpPost]
        public IActionResult Eliminar(int id) 
        {
            Contacto contacto = this.GetContactoManager().getContacto(id);
            if (this.GetContactoManager().removeContacto(id))
            {
                Console.WriteLine(this.getUsuarioManager().updateRegistroUsuario(this.RegistroFormato("eliminar", id)));
            }
            return RedirectToAction("Inicio", "Contacto");
        }

        private string RegistroFormato(string accion, object identificador)
        {
            if (identificador.GetType() == typeof(string) || identificador.GetType() == typeof(int))
            {
                switch (accion)
                {
                    case "crear": return ("ha creado al usuario " + identificador);
                    case "editar": return ("ha editado al usuario " + identificador);
                    case "eliminar": return ("ha eliminado al usuario " + identificador);
                }
            }
            return "";
        }

        private ContactoManager GetContactoManager()
        {
            if (this.contactoManager == null)
            {
                contactoManager = new ContactoManager(this.memoryCache);
            }
            return contactoManager;
        }

        private UsuarioManager getUsuarioManager()
        {
            if (this.usuarioManager == null)
            {
                usuarioManager = new UsuarioManager(this.memoryCache);
            }
            return usuarioManager;
        }

        private SesionCache GetSesionCache()
        {
            if (sesionCache == null)
            {
                sesionCache = new SesionCache(this.memoryCache);
                return sesionCache;
            }
            return sesionCache;
        }

        private ContactosCache GetContactosCache()
        {
            if (contactosCache == null)
            {
                contactosCache = new ContactosCache(this.memoryCache);
                return contactosCache;
            }
            return contactosCache;
        }

    }

}
