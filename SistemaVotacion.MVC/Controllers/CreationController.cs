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
                try
                {
                    // Intentamos crear el rol
                    Crud<Rol>.Create(nuevoRol);
                    return RedirectToAction(nameof(ListRoles));
                }
                catch (Exception ex)
                {
                    // Si la API falla, agregamos el error al modelo
                    // El primer parámetro vacío "" significa que es un error de todo el formulario
                    ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                }
            }

            // Si llegamos aquí, es porque el modelo no era válido o hubo una excepción
            return View(nuevoRol);
        }

        // NIVEL 4: Confirmación de Borrado (GET)
        public IActionResult DeleteRol(int id)
        {
            try
            {
                // Buscamos el rol específico para mostrarlo en la vista de confirmación
                var rol = Crud<Rol>.GetById(id);

                if (rol == null)
                {
                    return NotFound();
                }

                return View(rol);
            }
            catch (Exception ex)
            {
                // Si hay error de conexión con la API
                TempData["Error"] = "Error al obtener el rol: " + ex.Message;
                return RedirectToAction(nameof(ListRoles));
            }
        }

        // ACCIÓN: Ejecutar Borrado (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRol(Rol rol) // El formulario envía el objeto completo
        {
            try
            {
                // Usamos el Id del modelo para eliminarlo en la API
                Crud<Rol>.Delete(rol.Id);
                return RedirectToAction(nameof(ListRoles));
            }
            catch (Exception ex)
            {
                // Si falla (por ejemplo, si el rol está siendo usado por un usuario)
                ViewBag.Error = "No se pudo eliminar el rol. Detalles: " + ex.Message;
                return View(rol); // Regresamos a la vista de confirmación con el error
            }
        }
    }
}