using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.MVC.Models;
using System.Diagnostics;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var procesos = Crud<ProcesoElectoral>.GetAll();
                bool hayProcesoActivo = false;

                if (procesos != null && procesos.Any())
                {
                    var ahora = DateTime.Now;
                    hayProcesoActivo = procesos.Any(p => ahora >= p.FechaInicio && ahora <= p.FechaFin);
                }

                ViewBag.ProcesoActivo = hayProcesoActivo;
            }
            catch
            {
                ViewBag.ProcesoActivo = false;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
