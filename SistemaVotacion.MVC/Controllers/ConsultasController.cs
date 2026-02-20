using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    public class ConsultasController : Controller
    {
        // GET: /Consultas/Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(string cedula)
        {
            try
            {
                // 1. Configuramos el endpoint temporalmente para esta consulta
                string url = $"https://localhost:7202/api/Consultas/LugarVotacion/{cedula}";
                var usuario = Crud<Usuario>.GetSingle(url);
               

                if (usuario == null)
                {
                    TempData["Error"] = "Cédula no encontrada.";
                    return View("Index");
                }

                return View("Resultado", usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error de conexión: " + ex.Message;
                return View("Index");
            }
        }
    }
}