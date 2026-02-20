using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AuditoriaController : Controller
    {
        private readonly HttpClient _client;

        public AuditoriaController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("SistemaVotacionAPI");
        }

        public IActionResult Index()
        {
            try
            {
                var actas = Crud<ActaAuditoria>.GetAll();
                return View(actas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo cargar las actas: " + ex.Message;
                return View(new List<ActaAuditoria>());
            }
        }

        public IActionResult Generar()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generar(int idProceso, int idJunta, string observaciones)
        {
            if (idProceso <= 0 || idJunta <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un Proceso y una Junta.");
            }

            if (ModelState.IsValid)
            {
                try
                {

                    int totalSufragantes = 0;
                    int votosUrna = 0;

                    var response = await _client.GetAsync($"api/Resultados/auditoria?idProceso={idProceso}&idJunta={idJunta}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);
                        

                        totalSufragantes = (int?)data.totalEmpadronados ?? (int?)data.TotalEmpadronados ?? 0;
                        votosUrna = (int?)data.totalSufragantes ?? (int?)data.TotalSufragantes ?? 0;
                    }
                    else
                    {
                         ModelState.AddModelError("", "No se pudo obtener la información de la Junta desde la API.");
                         CargarCombos();
                         return View();
                    }

                    var nuevaActa = new ActaAuditoria
                    {
                        IdProceso = idProceso,
                        IdJunta = idJunta,
                        FechaCierre = DateTime.Now,
                        Observaciones = observaciones ?? "Generado Automáticamente",
                        HashSeguridad = new Random().NextInt64(1000000000, 9999999999).ToString(),
                        TotalSufragantesPadron = totalSufragantes,
                        VotosEnUrna = votosUrna
                    };

                    Crud<ActaAuditoria>.Create(nuevaActa);
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al generar acta: " + ex.Message);
                }
            }
            CargarCombos();
            return View();
        }

        public IActionResult Details(int id)
        {
            var acta = Crud<ActaAuditoria>.GetById(id);
            if (acta == null) return NotFound();
            return View(acta);
        }

        private void CargarCombos()
        {
            var procesos = Crud<ProcesoElectoral>.GetAll();
            ViewBag.Procesos = procesos?.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.NombreProceso }).ToList();

            var juntas = Crud<JuntaReceptora>.GetAll();
            ViewBag.Juntas = juntas?.Select(j => new SelectListItem { Value = j.Id.ToString(), Text = $"Junta {j.NumeroJunta}" }).ToList();
        }
    }
}
