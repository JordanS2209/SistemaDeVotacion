using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    // [Authorize(Roles = "Admin,SuperAdmin")]
    public class GeografiaController : Controller
    {
        // NIVEL 1: Dashboard de Geografía (Los "cuadritos")
        public IActionResult Index()
        {
            return View();
        }


        // SECCIÓN: PROVINCIAS

        public IActionResult ListProvincias()
        {
            try
            {
                var provincias = Crud<Provincia>.GetAll();
                return View(provincias);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<Provincia>());
            }
        }

        public IActionResult CreateProvincia()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProvincia(Provincia nuevaProvincia)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Provincia>.Create(nuevaProvincia);
                    return RedirectToAction(nameof(ListProvincias));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            return View(nuevaProvincia);
        }

        public IActionResult DeleteProvincia(int id)
        {
            try
            {
                var provincia = Crud<Provincia>.GetById(id);
                if (provincia == null)
                {
                    return NotFound();
                }

                return View(provincia);

            }
            catch (Exception ex)
            {

                // Si hay error de conexión con la API
                TempData["Error"] = "Error al obtener el rol: " + ex.Message;
                return RedirectToAction(nameof(ListProvincias));
            }
        }

        [HttpPost, ActionName("DeleteProvincia")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProvinciaConfirmed(int id)
        {
            try
            {
                Crud<Provincia>.Delete(id);
                return RedirectToAction(nameof(ListProvincias));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se puede eliminar. Verifique si tiene ciudades asociadas.";
                return View(Crud<Provincia>.GetById(id));
            }
        }

        // SECCIÓN: CIUDADES

        public IActionResult ListCiudades()
        {
            try
            {
                var ciudades = Crud<Ciudad>.GetAll();
                return View(ciudades);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return View(new List<Ciudad>());
            }
        }

        public IActionResult CreateCiudad()
        {
            try
            {
                // Obtenemos la lista de provincias para el dropdown
                var provincias = Crud<Provincia>.GetAll();
                ViewBag.Provincias = provincias;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudieron cargar las provincias: " + ex.Message;
                return RedirectToAction(nameof(ListCiudades));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCiudad(Ciudad nuevaCiudad)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Ciudad>.Create(nuevaCiudad);
                    return RedirectToAction(nameof(ListCiudades));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar en la API: " + ex.Message);
                }
            }

            // Si hubo un error, RECARGAMOS las provincias antes de devolver la vista
            ViewBag.Provincias = Crud<Provincia>.GetAll();
            return View(nuevaCiudad);
        }

        public IActionResult DeleteCiudad(int id)
        {
            try
            {
                var ciudad = Crud<Ciudad>.GetById(id);
                if (ciudad == null)
                {
                    return NotFound();
                }

                return View(ciudad);

            }
            catch (Exception ex)
            {

                // Si hay error de conexión con la API
                TempData["Error"] = "Error al obtener el rol: " + ex.Message;
                return RedirectToAction(nameof(ListCiudades));
            }
        }

        [HttpPost, ActionName("DeleteCiudad")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCiudadConfirmed(int id)
        {
            try
            {
                Crud<Ciudad>.Delete(id);
                return RedirectToAction(nameof(ListCiudades));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al eliminar la ciudad.";
                return View(Crud<Ciudad>.GetById(id));
            }
        }

        // SECCIÓN: PARROQUIAS

        public IActionResult ListParroquias()
        {
            try
            {
                var parroquias = Crud<Parroquia>.GetAll();
                return View(parroquias);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return View(new List<Parroquia>());
            }
        }

        public IActionResult CreateParroquia()
        {
            try
            {
                // Obtenemos la lista de provincias para el dropdown
                var ciudad = Crud<Ciudad>.GetAll();
                ViewBag.Ciudad = ciudad;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudieron cargar las provincias: " + ex.Message;
                return RedirectToAction(nameof(ListCiudades));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateParroquia(Parroquia nuevaParroquia)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Parroquia>.Create(nuevaParroquia);
                    return RedirectToAction(nameof(ListParroquias));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            ViewBag.Ciudad = Crud<Ciudad>.GetAll();
            return View(nuevaParroquia);
        }

        public IActionResult DeleteParroquia(int id)
        {
            try
            {
                var parroquia = Crud<Parroquia>.GetById(id);
                if (parroquia == null)
                {
                    return NotFound();
                }

                return View(parroquia);

            }
            catch (Exception ex)
            {

                // Si hay error de conexión con la API
                TempData["Error"] = "Error al obtener el rol: " + ex.Message;
                return RedirectToAction(nameof(ListParroquias));
            }
        }

        [HttpPost, ActionName("DeleteParroquia")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteParroquiaConfirmed(int id)
        {
            try
            {
                Crud<Parroquia>.Delete(id);
                return RedirectToAction(nameof(ListParroquias));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al eliminar la parroquia.";
                return View(Crud<Parroquia>.GetById(id));
            }
        }
    }
}