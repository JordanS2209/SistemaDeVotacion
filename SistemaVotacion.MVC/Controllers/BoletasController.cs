using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class BoletasController : Controller
    {
        [HttpGet]
        public IActionResult Index(string codigo)
        {
           
            if (string.IsNullOrWhiteSpace(codigo))
            {
                TempData["Error"] = "Debe ingresar un código de votación.";
                return RedirectToAction("IngresarCodigo", "Acceso");
            }

            
            ViewBag.Codigo = codigo;
            return View(); 
        }
    }
}