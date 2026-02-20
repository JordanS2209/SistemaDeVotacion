
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class MesaController : Controller
    {
       
        public IActionResult ListJunta()
        {
            try
            {
                var juntas = Crud<JuntaReceptora>.GetAll();
                return View(juntas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con API: " + ex.Message;
                return View(new List<JuntaReceptora>());
            }
        }

        public IActionResult CreateJunta()
        {
            CargarCombosJunta();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateJunta(JuntaReceptora junta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<JuntaReceptora>.Create(junta);
                    return RedirectToAction(nameof(ListJunta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear junta: " + ex.Message);
                }
            }
            CargarCombosJunta();
            return View(junta);
        }

        public IActionResult EditJunta(int id)
        {
            try
            {
                var junta = Crud<JuntaReceptora>.GetById(id);
                if (junta == null) return NotFound();

                CargarCombosJunta();
                return View(junta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditJunta(int id, JuntaReceptora junta)
        {
            if (id != junta.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<JuntaReceptora>.Update(id, junta);
                    return RedirectToAction(nameof(ListJunta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            CargarCombosJunta();
            return View(junta);
        }

        public IActionResult DeleteJunta(int id)
        {
            try
            {
                var junta = Crud<JuntaReceptora>.GetById(id);
                if (junta == null) return NotFound();
                return View(junta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("DeleteJunta")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteJuntaConfirmed(int id)
        {
            try
            {
                Crud<JuntaReceptora>.Delete(id);
                return RedirectToAction(nameof(ListJunta));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al eliminar: " + ex.Message;
                return View();
            }
        }

        private void CargarCombosJunta()
        {
            ViewBag.Generos = GetGenerosList();
            ViewBag.Recintos = GetRecintosList();
        }

        private List<SelectListItem> GetGenerosList()
        {
            var generos = Crud<Genero>.GetAll() ?? new List<Genero>();
            return generos.Select(g => new SelectListItem
            {
                Value = g.IdGenero.ToString(),
                Text = g.DetalleGenero
            }).ToList();
        }

        private List<SelectListItem> GetRecintosList()
        {
            var recintos = Crud<RecintoElectoral>.GetAll() ?? new List<RecintoElectoral>();
            return recintos.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.NombreRecinto
            }).ToList();
        }
    }
}
