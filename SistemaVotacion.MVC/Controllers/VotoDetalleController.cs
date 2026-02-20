using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class VotoDetalleController : Controller
    {
        private readonly HttpClient _client;
        private readonly SistemaVotacion.Servicios.Interfaces.IPdfService _pdfService;
        private readonly SistemaVotacion.Servicios.Interfaces.IChartService _chartService;
        private readonly ILogger<VotoDetalleController> _logger;

        public VotoDetalleController(
            SistemaVotacion.Servicios.Interfaces.IPdfService pdfService, 
            SistemaVotacion.Servicios.Interfaces.IChartService chartService,
            ILogger<VotoDetalleController> logger)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/")
            };
            _pdfService = pdfService;
            _chartService = chartService;
            _logger = logger;
        }

        

        [HttpGet]
        public async Task<IActionResult> GetChartImage(int idProceso, int? idProvincia = null)
        {
            try
            {
                var query = $"api/Resultados/eleccion-general?idProceso={idProceso}";
                if (idProvincia.HasValue && idProvincia.Value > 0) query += $"&idProvincia={idProvincia}";

                var response = await _client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    
                    byte[] imageBytes = _chartService.GeneratePieChart(data);
                    if (imageBytes != null)
                    {
                        return File(imageBytes, "image/png");
                    }
                }
                return NotFound();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPDF(int idProceso, string tipo = "general", int? idProvincia = null)
        {
            byte[] pdfBytes = null;
            try
            {
                if (tipo == "general")
                {
                    var query = $"api/Resultados/eleccion-general?idProceso={idProceso}";
                    if (idProvincia.HasValue && idProvincia.Value > 0) query += $"&idProvincia={idProvincia}";

                    var response = await _client.GetAsync(query);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
   
                        var data = JsonConvert.DeserializeObject<List<SistemaVotacion.Modelos.ResultadoGeneralDto>>(json);
                        pdfBytes = _pdfService.GenerarPdfResultadosGeneral(data, idProvincia);
                    }
                }
                else if (tipo == "consulta")
                {

                     var query = $"api/Resultados/consulta-popular?idProceso={idProceso}";
                     var response = await _client.GetAsync(query);
                     if (response.IsSuccessStatusCode)
                     {
                         var json = await response.Content.ReadAsStringAsync();
                         dynamic data = JsonConvert.DeserializeObject(json);
                         pdfBytes = _pdfService.GenerarPdfConsultaPopular(data);
                     }
                }

                if (pdfBytes != null)
                {
                    return File(pdfBytes, "application/pdf", $"Resultados_{tipo}.pdf");
                }
                else
                {
                    TempData["Error"] = "No se pudieron obtener datos para el PDF o el tipo de reporte es incorrecto.";

                if (tipo == "general") return RedirectToAction(nameof(EleccionGeneral), new { idProceso, idProvincia });
                if (tipo == "consulta") return RedirectToAction(nameof(ConsultaPopular), new { idProceso });
                return RedirectToAction(nameof(Index));
            }
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exportando PDF");
                TempData["Error"] = "Error PDF Service: " + ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "");
                if (tipo == "general") return RedirectToAction(nameof(EleccionGeneral), new { idProceso, idProvincia });
                if (tipo == "consulta") return RedirectToAction(nameof(ConsultaPopular), new { idProceso });
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> Index()
        {
            // Tratar de obtener el proceso activo automáticamente
            try
            {
                var response = await _client.GetAsync("api/ProcesosElectorales/activo");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic proceso = JsonConvert.DeserializeObject(json);
                    
                    int id = proceso.id;
                    int tipo = proceso.idTipoProceso;

                    if (tipo == 2) // Consulta Popular
                        return RedirectToAction(nameof(ConsultaPopular), new { idProceso = id });
                    
                    // Por defecto (General o Seccionales)
                    return RedirectToAction(nameof(EleccionGeneral), new { idProceso = id });
                }
            }
            catch { }

            return View("ProcesoNoDisponible"); 
        }

        public async Task<IActionResult> ConsultaPopular(int idProceso)
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"];

            if (idProceso <= 0)
            {
                 // Intentar buscar activo si llega 0
                 var activo = await GetIdProcesoActivo();
                 if (activo > 0) return RedirectToAction(nameof(ConsultaPopular), new { idProceso = activo });
                 
                 return View("ProcesoNoDisponible");
            }

            string resultado = "[]";
            try
            {
                resultado = await _client.GetStringAsync($"api/Resultados/consulta-popular?idProceso={idProceso}");
            }
            catch { }

            ViewBag.Resultado = resultado;
            ViewBag.IdProceso = idProceso;
            return View();
        }

        public async Task<IActionResult> EleccionGeneral(int idProceso, int? idProvincia = null)
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"];

            if (idProceso <= 0)
            {
                 // Intentar buscar activo si llega 0
                 var activo = await GetIdProcesoActivo();
                 if (activo > 0) return RedirectToAction(nameof(EleccionGeneral), new { idProceso = activo });

                 return View("ProcesoNoDisponible");
            }

            string resultado = "[]";
            try
            {
                var query = $"api/Resultados/eleccion-general?idProceso={idProceso}";
                if (idProvincia.HasValue && idProvincia.Value > 0)
                {
                    query += $"&idProvincia={idProvincia.Value}";
                }

                var response = await _client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    resultado = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    ViewBag.Error = $"Error API ({response.StatusCode}): {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error de conexión con API: {ex.Message}";
            }

            try
            {
                var responseProv = await _client.GetAsync("api/Provincias");
                if (responseProv.IsSuccessStatusCode)
                {
                    var jsonProv = await responseProv.Content.ReadAsStringAsync();
                    ViewBag.Provincias = JsonConvert.DeserializeObject<List<SistemaVotacion.Modelos.Provincia>>(jsonProv);
                }
            }
            catch { }

            // DEBUG FETCH para mi error de de api resultados 
            try 
            {
                var debugResp = await _client.GetAsync($"api/Resultados/debug?idProceso={idProceso}");
                if (debugResp.IsSuccessStatusCode)
                {
                    ViewBag.Debug = await debugResp.Content.ReadAsStringAsync();
                }
            } catch {}

            ViewBag.Resultado = resultado;
            ViewBag.IdProceso = idProceso;
            ViewBag.IdProvincia = idProvincia;

            return View();
        }

        private async Task<int> GetIdProcesoActivo()
        {
            try
            {
                var response = await _client.GetAsync("api/ProcesosElectorales/activo");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic proceso = JsonConvert.DeserializeObject(json);
                    return (int)proceso.id;
                }
            }
            catch { }
            return 0;
        }
    }
}