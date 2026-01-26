using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    public class BoletasController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var listas = Crud<Lista>.GetAll();

                if (listas == null || !listas.Any())
                {
                    return View("ProcesoNoDisponible");
                }

                return View(listas);
            }
            catch (Exception ex)
            {
                // Aquí puedes loguear el error si quieres
                ViewBag.Error = ex.Message;
                return View("ProcesoNoDisponible");
            }
        }
    }
}