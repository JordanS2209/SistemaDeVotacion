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


        //ROLES

        public IActionResult ListRoles()
        {
            try
            {
                var roles = Crud<Rol>.GetAll();
                return View(roles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<Rol>());
            }
            
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
        public IActionResult DeleteRol(Rol rol) 
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



        //DIGNIDADES

        public IActionResult ListDignidades()
        {
            try
            {
                var dignidades = Crud<Dignidad>.GetAll();
                return View(dignidades);
            }
            catch (Exception ex)
            {
                // Esto te dirá en la consola de Visual Studio exactamente qué falló
                Console.WriteLine("Error en ListDignidades: " + ex.Message);

                // Enviamos una lista vacía para que la vista no se rompa, 
                // pero mostramos el error en un ViewBag
                ViewBag.Error = "No se pudo conectar con la API de Dignidades.";
                return View(new List<Dignidad>());
            }
        }


        public IActionResult CreateDignidad()
        {
            return View();
        }

        // ACCIÓN: Procesar Creación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDignidad(Dignidad nuevaDignidad)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Intentamos crear el rol
                    Crud<Dignidad>.Create(nuevaDignidad);
                    return RedirectToAction(nameof(ListDignidades));
                }
                catch (Exception ex)
                {
                    // Si la API falla, agregamos el error al modelo
                    // El primer parámetro vacío "" significa que es un error de todo el formulario
                    ModelState.AddModelError("", "Error al conectar con la API: " + ex.Message);
                }
            }

            // Si llegamos aquí, es porque el modelo no era válido o hubo una excepción
            return View(nuevaDignidad);
        }

        public IActionResult DeleteDignidad(int id)
        {
            try
            {
                // Buscamos el rol específico para mostrarlo en la vista de confirmación
                var dignidad = Crud<Dignidad>.GetById(id);

                if (dignidad == null)
                {
                    return NotFound();
                }

                return View(dignidad);
            }
            catch (Exception ex)
            {
                // Si hay error de conexión con la API
                TempData["Error"] = "Error al obtener el rol: " + ex.Message;
                return RedirectToAction(nameof(ListDignidades));
            }
        }

        // ACCIÓN: Ejecutar Borrado (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDignidad(Dignidad dignidad)
        {
            try
            {
                // Usamos el Id del modelo para eliminarlo en la API
                Crud<Dignidad>.Delete(dignidad.Id);
                return RedirectToAction(nameof(ListDignidades));
            }
            catch (Exception ex)
            {
                // Si falla (por ejemplo, si el rol está siendo usado por un usuario)
                ViewBag.Error = "No se pudo eliminar el rol. Detalles: " + ex.Message;
                return View(dignidad); // Regresamos a la vista de confirmación con el error
            }
        }

    }
}