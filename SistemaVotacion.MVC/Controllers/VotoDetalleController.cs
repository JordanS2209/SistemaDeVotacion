using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SistemaVotacion.MVC.Controllers
{
    public class VotoDetalleController : Controller
    {
        private readonly HttpClient _client;

        public VotoDetalleController()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/")
            };
        }

        // ==========================
        // RESUMEN GENERAL
        // ==========================
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/")
            };

            try
            {
                var response = await client.GetAsync("api/Resultados/consulta-popular");

                if (!response.IsSuccessStatusCode)
                    return View("ProcesoNoDisponible");

                var json = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(json);

                // ============================
                // RESUMEN GENERAL
                // ============================
                int totalSi = 0;
                int totalNo = 0;
                int totalBlancos = 0;

                var preguntasLista = new List<object>();

                foreach (var p in data.resultados)
                {
                    totalSi += (int)p.totalSi;
                    totalNo += (int)p.totalNo;
                    totalBlancos += (int)p.totalBlancos;

                    preguntasLista.Add(new
                    {
                        id = p.idPregunta,
                        texto = p.textoPregunta,
                        si = p.totalSi,
                        no = p.totalNo,
                        blancos = p.totalBlancos,
                        ganador = p.ganador
                    });
                }

                ViewBag.Resumen = JsonConvert.SerializeObject(new
                {
                    si = totalSi,
                    no = totalNo,
                    blancos = totalBlancos
                });

                ViewBag.Preguntas = JsonConvert.SerializeObject(preguntasLista);

                ViewBag.IdProceso = data.idProceso;
                ViewBag.IdPadron = 1; // puedes quitar esto si no lo necesitas

                return View();
            }
            catch
            {
                return View("ProcesoNoDisponible");
            }
        }


        // ==========================
        // CONSULTA POPULAR
        // ==========================
        public async Task<IActionResult> ConsultaPopular()
        {
            string resultado = "{}";

            try
            {
                resultado = await _client.GetStringAsync("api/Resultados/consulta-popular");
            }
            catch { }

            ViewBag.Resultado = resultado;

            return View();
        }

        // ==========================
        // ELECCIÓN GENERAL
        // ==========================
        public async Task<IActionResult> EleccionGeneral()
        {
            string resultado = "{}";

            try
            {
                resultado = await _client.GetStringAsync("api/Resultados/eleccion-general");
            }
            catch { }

            ViewBag.Resultado = resultado;

            return View();
        }
    }
}