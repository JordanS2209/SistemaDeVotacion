using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class CreationController : Controller
    {
        // NIVEL 1: Dashboard Principal
        public IActionResult Index()
        {
            return View();
        }

        // NIVEL 2: Listado de Roles
        public IActionResult ListRoles()
        {
            var roles = Crud<Rol>.GetAll();
            return View(roles);
        }

        // NIVEL 3: Formulario para crear (Vista)
        public IActionResult CreateRol()
        {
            return View();
        }

        // ACCIÓN: Procesar Creación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRol(Rol nuevoRol)
        {
            if (ModelState.IsValid)
            {
                Crud<Rol>.Create(nuevoRol);
                return RedirectToAction(nameof(ListRoles));
            }
            return View(nuevoRol);
        }
    }
}