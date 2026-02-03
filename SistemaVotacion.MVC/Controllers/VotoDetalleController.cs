using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVotacion.Modelos;
using System.Net.Http.Json;

namespace SistemaVotacion.MVC.Controllers
{
    public class VotoDetalleController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7202/");

            string data = "{}";
            string provincias = "[]";           

            try
            {
                data = await client.GetStringAsync("api/Resultados/resumen-general");
                provincias = await client.GetStringAsync("api/Provincias");
            }
            catch { }

            ViewBag.Data = data;
            ViewBag.Provincias = provincias;

            return View();
        }
    }
}