using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RecintosElectoralesController : Controller
    {
        // GET: RecintosElectorales
        public IActionResult Index()
        {
            try
            {
                var recintos = Crud<RecintoElectoral>.GetAll();
                return View(recintos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar recintos: " + ex.Message;
                return View(new List<RecintoElectoral>());
            }
        }

        // GET: RecintosElectorales/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // POST: RecintosElectorales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RecintoElectoral recinto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<RecintoElectoral>.Create(recinto);
                    TempData["Success"] = "Recinto creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear: " + ex.Message;
                }
            }
            CargarCombos();
            return View(recinto);
        }

        // GET: RecintosElectorales/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                var recinto = Crud<RecintoElectoral>.GetById(id);
                if (recinto == null) return NotFound();

                CargarCombos();
                return View(recinto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar recinto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: RecintosElectorales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RecintoElectoral recinto)
        {
            if (id != recinto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<RecintoElectoral>.Update(id, recinto);
                    TempData["Success"] = "Recinto actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                     TempData["Error"] = "Error al actualizar: " + ex.Message;
                }
            }
            CargarCombos();
            return View(recinto);
        }

        // GET: RecintosElectorales/Delete/5
        public IActionResult Delete(int id)
        {
             try
            {
                var recinto = Crud<RecintoElectoral>.GetById(id);
                if (recinto == null) return NotFound();

                return View(recinto);
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Error al cargar recinto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: RecintosElectorales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                Crud<RecintoElectoral>.Delete(id);
                TempData["Success"] = "Recinto eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se puede eliminar (posiblemente en uso): " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private void CargarCombos()
        {
            var parroquias = Crud<Parroquia>.GetAll() ?? new List<Parroquia>();
            ViewBag.Parroquias = parroquias.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.NombreParroquia
            }).ToList();
        }
    }
}
