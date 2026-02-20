using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RepresentantesController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var representantes = Crud<RepresentanteJunta>.GetAll();
                return View(representantes);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo cargar la lista de representantes: " + ex.Message;
                return View(new List<RepresentanteJunta>());
            }
        }

        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RepresentanteJunta representante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<RepresentanteJunta>.Create(representante);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al asignar representante: " + ex.Message);
                }
            }
            CargarCombos();
            return View(representante);
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var representante = Crud<RepresentanteJunta>.GetById(id);
                if (representante == null) return NotFound();

                CargarCombos();
                return View(representante);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RepresentanteJunta representante)
        {
            if (id != representante.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<RepresentanteJunta>.Update(id, representante);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            CargarCombos();
            return View(representante);
        }

        public IActionResult Delete(int id)
        {
            var rep = Crud<RepresentanteJunta>.GetById(id);
            if (rep == null) return NotFound();
            return View(rep);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                Crud<RepresentanteJunta>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        private void CargarCombos()
        {
            // Cargar Procesos
            var procesos = Crud<ProcesoElectoral>.GetAll();
            ViewBag.Procesos = procesos?.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.NombreProceso
            }).ToList();

            // Cargar Juntas
            var juntas = Crud<JuntaReceptora>.GetAll();
            ViewBag.Juntas = juntas?.Select(j => new SelectListItem
            {
                Value = j.Id.ToString(),
                Text = $"Junta {j.NumeroJunta} (ID: {j.Id})" 
            }).ToList();

            // Cargar Usuarios
            var usuarios = Crud<Usuario>.GetAll();
            ViewBag.Usuarios = usuarios?.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(), // Corrected from IdUsuario
                Text = $"{u.Nombres} {u.Apellidos} ({u.Email})" // Corrected from NombreUsuario and CorreoElectronico
            }).ToList();

            // Cargar Roles
            var roles = Crud<Rol>.GetAll();
            ViewBag.Roles = roles?.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(), 
                Text = r.NombreRol 
            }).ToList();
        }
    }
}
