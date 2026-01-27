using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class AccesoController : Controller
    {
        [HttpGet]
        public IActionResult IngresarCodigo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IngresarCodigo(string codigo)
        {
            // Por ahora NO validamos nada
            // Cualquier código de 6 dígitos pasa

            return RedirectToAction("Index", "Boletas");
        }
    }
}