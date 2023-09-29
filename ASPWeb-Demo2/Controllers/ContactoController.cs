using ASPWeb_Demo2.Controllers.Cache;
using ASPWeb_Demo2.Controllers.Managers;
using ASPWeb_Demo2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ASPWeb_Demo2.Controllers
{

    public class ContactoController : Controller
    {

        private readonly SesionCache sesionCache;
        private readonly ContactosCache contactosCache;

        private readonly ContactoManager contactoManager;
        private readonly UsuarioManager usuarioManager;

        public ContactoController(IMemoryCache memoryCache)
        {
            this.sesionCache = new SesionCache(memoryCache);
            this.contactosCache = new ContactosCache(memoryCache);

            this.contactoManager = new ContactoManager(memoryCache);
            this.usuarioManager = new UsuarioManager(memoryCache);
        }

        [HttpGet]
        public async Task<IActionResult> Inicio()
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
                    model = this.GetContactoManager().GetAll();
                    var task = this.GetContactosCache().SetFromCache(model);
                    await task;

                    if (task.IsCompletedSuccessfully)
                    {
                        task.Dispose();
                    }
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
                    Contacto? contacto = this.GetContactoManager().GetOne(id);
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
                Contacto? c = this.GetContactoManager().GetOne(id);
                return View(c);
            } else return RedirectToAction("Login", "LogIn");
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de borrar 
         * 
         */

        [HttpPost]
        public async Task<IActionResult> Crear(string nombre, string correo)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo))
            {
                Contacto contacto = new Contacto();

                int id = await this.generateNumber();

                contacto.idcontacto = id;
                contacto.nombre = nombre;
                contacto.correo = correo;

                var task = this.GetContactoManager().Add(contacto);
                await task;

                if (task.IsCompletedSuccessfully)
                {
                    task.Dispose();
                    Console.WriteLine(this.getUsuarioManager().updateRegistroUsuario(this.RegistroFormato("crear", nombre)));
                    return RedirectToAction("Inicio", "Contacto");
                }
                else return View();
                
            } else return View();
        }

        /*
         * 
         * Es el metodo que lleva acabo la accion de editar 
         * 
         */

        [HttpPost]
        public async Task<IActionResult> Editar(int id, string nombre, string correo)
        {
            Contacto contacto = this.GetContactoManager().GetOne(id);
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(correo))
            {
                if (contacto.nombre != nombre || contacto.correo != correo)
                {
                    Contacto toSend = new Contacto(0, nombre, correo);

                    var task = this.GetContactoManager().Update(id, toSend);
                    await task;

                    if (task.IsCompletedSuccessfully)
                    {
                        task.Dispose();
                        Console.WriteLine(this.getUsuarioManager().updateRegistroUsuario(this.RegistroFormato("editar", nombre)));
                        return RedirectToAction("Inicio", "Contacto");
                    } 
                    else return View(contacto);
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
        public async Task<IActionResult> Eliminar(int id) 
        {
            var task = this.GetContactoManager().Remove(id);
            await task;

            if (task.IsCompletedSuccessfully)
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

        private async Task<int> generateNumber()
        {
            int number = new Random().Next(1000, 5000);
            if (this.GetContactoManager().GetAll().Count() == 0) return number;
            else if (this.GetContactoManager().GetAll().Where(x => x.idcontacto == number).FirstOrDefault() == null) return number;
            else generateNumber();
            return 0;
        }

        private ContactoManager GetContactoManager() => this.contactoManager;

        private UsuarioManager getUsuarioManager() => this.usuarioManager;

        private SesionCache GetSesionCache() => this.sesionCache;

        private ContactosCache GetContactosCache() => this.contactosCache;

    }

}
