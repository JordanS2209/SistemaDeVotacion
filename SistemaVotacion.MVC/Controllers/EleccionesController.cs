using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class EleccionesController : Controller
    {
        private readonly IMultimediaService _multimediaService;


        public EleccionesController(IMultimediaService multimediaService)
        {
            _multimediaService = multimediaService;
        }
        public IActionResult Index()
        {
            return View();
        }
        // tipo electoral - crud 
        private List<SelectListItem> GetProcesosElectorales()
        {
            try
            {
                var procesos = Crud<ProcesoElectoral>.GetAll() ?? new List<ProcesoElectoral>();
                return procesos
                    .OrderByDescending(p => p.FechaInicio)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.NombreProceso })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        private List<SelectListItem> GetListasSelect()
        {
            try
            {
                var listas = Crud<Lista>.GetAll() ?? new List<Lista>();
                return listas
                    .OrderBy(l => l.NumeroLista)
                    .ThenBy(l => l.NombreLista)
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = $"{l.NumeroLista} - {l.NombreLista}" })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        private List<SelectListItem> GetDignidades()
        {
            try
            {
                var dignidades = Crud<Dignidad>.GetAll() ?? new List<Dignidad>();
                return dignidades
                    .OrderBy(d => d.NombreDignidad)
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.NombreDignidad })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        private List<SelectListItem> GetCandidatos()
        {
            try
            {
                var candidatos = Crud<Candidato>.GetAll() ?? new List<Candidato>();
                return candidatos
                    .OrderBy(c => c.NombreCandidato)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.NombreCandidato })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }
        private List<SelectListItem> GetTipoProceso()
        {

            try
            {
                var tipos = Crud<TipoProceso>.GetAll();

                // Por si JsonConvert devolviera null 
                if (tipos == null) return new List<SelectListItem>();

                return tipos
                    .OrderBy(tp => tp.NombreTipoProceso)
                    .Select(tp => new SelectListItem
                    {
                        Value = tp.Id.ToString(),
                        Text = tp.NombreTipoProceso
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetTipoProceso(): " + ex.Message);
                return new List<SelectListItem>();
            }
        }
        private bool EstaActivo(ProcesoElectoral p)
        {
            var ahora = DateTime.Now;
            return ahora >= p.FechaInicio && ahora <= p.FechaFin;
        }

        private bool EstaCerrado(ProcesoElectoral p)
        {
            return DateTime.Now > p.FechaFin;
        }

        private void CargarDashboardProcesoElectoral()
        {
            // Esta vista (CreateProcesoElectoral) actúa como Dashboard para:
            // - Crear TipoProceso
            // - Crear ProcesoElectoral
            // - Listar ambos
            // Esta vista actúa como “Dashboard” (TipoProceso + ProcesoElectoral)
            try
            {
                var tipos = Crud<TipoProceso>.GetAll();
                var procesos = Crud<ProcesoElectoral>.GetAll();

                bool existenTipos = tipos != null && tipos.Any();
                ViewBag.ExistenTipos = existenTipos;

                if (existenTipos)
                    ViewBag.TipoProceso = GetTipoProceso();

                ViewBag.ListaTiposProceso = tipos ?? new List<TipoProceso>();
                ViewBag.ListaProcesosElectorales = procesos ?? new List<ProcesoElectoral>();
            }
            catch (Exception ex)
            {
                ViewBag.ExistenTipos = false;
                ViewBag.TipoProceso = new List<SelectListItem>();
                ViewBag.ListaTiposProceso = new List<TipoProceso>();
                ViewBag.ListaProcesosElectorales = new List<ProcesoElectoral>();
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
            }
        }

        public IActionResult DetailsProcesoElectoral(int id)
        {
            try
            {
                // 1) Obtener el proceso
                var procesoElectoral = Crud<ProcesoElectoral>.GetById(id);
                if (procesoElectoral == null)
                    return NotFound();

                // 2) Obtener listas por proceso
                var url = $"{Crud<Lista>.EndPoint}/por-proceso/{id}";
                var listas = Crud<Lista>.GetCustom(url) ?? new List<Lista>();

                // 3) Pasar listas a la vista LISTAS PRO PROCESO
                ViewBag.ListasDelProceso = listas;

                return View("ProcesoElectoral/DetailsProcesoElectoral", procesoElectoral);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }
        }


        public IActionResult CreateProcesoElectoral()
        {
            CargarDashboardProcesoElectoral();
            return View("ProcesoElectoral/CreateProcesoElectoral");
        }

        //para ver exactamente mis errores no entendia que estaba haciendo mal
        private string ObtenerDetalleBadRequestDesdeApi<TPayload>(string url, TPayload payload)
        {
            try
            {
                using var client = new HttpClient();

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = client.PostAsync(url, content).Result;

                var body = response.Content.ReadAsStringAsync().Result;

                // Recortamos para que no sea infinito en TempData
                if (!string.IsNullOrWhiteSpace(body) && body.Length > 1200)
                    body = body.Substring(0, 1200) + "...";

                return $"HTTP {(int)response.StatusCode} {response.StatusCode}. Detalle API: {body}";
            }
            catch (Exception ex)
            {
                return "No se pudo leer el detalle del error desde la API: " + ex.Message;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProcesoElectoral(ProcesoElectoral nuevoProcesoElectoral)
        {
            var hoy = DateTime.Now.Date;

            if (nuevoProcesoElectoral == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }

            // Validaciones 
            if (string.IsNullOrWhiteSpace(nuevoProcesoElectoral.NombreProceso))
                ModelState.AddModelError(nameof(ProcesoElectoral.NombreProceso), "El nombre del proceso es obligatorio.");

            if (nuevoProcesoElectoral.IdTipoProceso <= 0)
                ModelState.AddModelError(nameof(ProcesoElectoral.IdTipoProceso), "Debe seleccionar un Tipo de Proceso.");

            if (nuevoProcesoElectoral.FechaInicio == default)
                ModelState.AddModelError(nameof(ProcesoElectoral.FechaInicio), "La fecha de inicio es obligatoria.");

            if (nuevoProcesoElectoral.FechaFin == default)
                ModelState.AddModelError(nameof(ProcesoElectoral.FechaFin), "La fecha de fin es obligatoria.");

            if (nuevoProcesoElectoral.FechaInicio != default && nuevoProcesoElectoral.FechaFin != default)
            {
                if (nuevoProcesoElectoral.FechaInicio.Date >= nuevoProcesoElectoral.FechaFin.Date)
                    ModelState.AddModelError("", "La fecha de inicio debe ser menor a la fecha de fin.");

                if (nuevoProcesoElectoral.FechaFin.Date < hoy)
                    ModelState.AddModelError(nameof(ProcesoElectoral.FechaFin), "No se puede crear un proceso con fecha fin pasada.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los datos del proceso electoral.";
                CargarDashboardProcesoElectoral();
                return View("ProcesoElectoral/CreateProcesoElectoral", nuevoProcesoElectoral);
            }

            try
            {
                nuevoProcesoElectoral.NombreProceso = nuevoProcesoElectoral.NombreProceso.Trim();

                Crud<ProcesoElectoral>.Create(nuevoProcesoElectoral);

                TempData["Success"] = "Proceso electoral creado correctamente.";
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("BadRequest", StringComparison.OrdinalIgnoreCase))
                {
                    var detalle = ObtenerDetalleBadRequestDesdeApi(Crud<ProcesoElectoral>.EndPoint, nuevoProcesoElectoral);
                    TempData["Error"] = "La API rechazó la solicitud. " + detalle;
                }
                else
                {
                    TempData["Error"] = "Error al crear proceso electoral: " + ex.Message;
                }

                //  IMPORTANTÍSIMO: recargar combos/listas y devolver la vista con el modelo
                CargarDashboardProcesoElectoral();
                return View("ProcesoElectoral/CreateProcesoElectoral", nuevoProcesoElectoral);
            }
        }




        public IActionResult EditProcesoElectoral(int id)
        {
            try
            {
                var procesoElectoral = Crud<ProcesoElectoral>.GetById(id);
                if (procesoElectoral == null)
                    return NotFound();

                // Regla UX (igual la API lo bloquea): no editar si activo/cerrado
                if (EstaActivo(procesoElectoral))
                {
                    TempData["Error"] = "No se puede editar un proceso activo.";
                    return RedirectToAction(nameof(CreateProcesoElectoral));
                }

                if (EstaCerrado(procesoElectoral))
                {
                    TempData["Error"] = "No se puede editar un proceso cerrado.";
                    return RedirectToAction(nameof(CreateProcesoElectoral));
                }

                ViewBag.TipoProceso = GetTipoProceso();
                return View("ProcesoElectoral/EditProcesoElectoral", procesoElectoral);


            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProcesoElectoral(int id, ProcesoElectoral procesoElectoral)
        {
            if (procesoElectoral == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }

            if (string.IsNullOrWhiteSpace(procesoElectoral.NombreProceso))
                ModelState.AddModelError(nameof(ProcesoElectoral.NombreProceso), "NombreProceso es obligatorio.");

            if (procesoElectoral.FechaInicio >= procesoElectoral.FechaFin)
                ModelState.AddModelError("", "La fecha de inicio debe ser menor a la fecha de fin.");

            if (ModelState.IsValid)
            {
                try
                {
                    // Revalidación contra estado actual
                    var actual = Crud<ProcesoElectoral>.GetById(id);
                    if (actual == null)
                        return NotFound();

                    if (EstaActivo(actual))
                    {
                        TempData["Error"] = "No se puede editar un proceso activo.";
                        return RedirectToAction(nameof(CreateProcesoElectoral));
                    }

                    if (EstaCerrado(actual))
                    {
                        TempData["Error"] = "No se puede editar un proceso cerrado.";
                        return RedirectToAction(nameof(CreateProcesoElectoral));
                    }

                    procesoElectoral.NombreProceso = procesoElectoral.NombreProceso.Trim();

                    Crud<ProcesoElectoral>.Update(id, procesoElectoral);
                    TempData["Success"] = "Proceso electoral actualizado correctamente.";
                    return RedirectToAction(nameof(CreateProcesoElectoral));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar proceso electoral: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos del proceso electoral.";
            }

            ViewBag.TipoProceso = GetTipoProceso();
            return View("ProcesoElectoral/EditProcesoElectoral", procesoElectoral);


        }


        public IActionResult ActivarProcesoElectoral(int id)
        {
            try
            {
                var url = $"{Crud<ProcesoElectoral>.EndPoint}/Activar/{id}";
                var resultado = Crud<ProcesoElectoral>.PutCustom(url);

                if (!resultado)
                    TempData["Error"] = "No se pudo activar el proceso electoral.";
                else
                    TempData["Success"] = "Proceso activado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al activar proceso: " + ex.Message;
            }

            return RedirectToAction(nameof(CreateProcesoElectoral));
        }

        public IActionResult CerrarProcesoElectoral(int id)
        {

            try
            {
                var url = $"{Crud<ProcesoElectoral>.EndPoint}/Cerrar/{id}";
                var resultado = Crud<ProcesoElectoral>.PutCustom(url);

                if (!resultado)
                    TempData["Error"] = "No se pudo cerrar el proceso electoral.";
                else
                    TempData["Success"] = "Proceso cerrado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cerrar proceso: " + ex.Message;
            }

            return RedirectToAction(nameof(CreateProcesoElectoral));
        }

        // TIPO DE PROCESOS - CRUD
        //index


        public IActionResult DetailsTipoProceso(int id)
        {
            try
            {
                var tipoProcesoData = Crud<TipoProceso>.GetById(id);
                if (tipoProcesoData == null)
                    return NotFound();

                return View("TipoProceso/DetailsTipoProceso", tipoProcesoData);

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }

        }
        public IActionResult CreateTipoProceso()
        {
            return RedirectToAction(nameof(CreateProcesoElectoral));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTipoProceso(TipoProceso nuevoTipoProceso)
        {
            if (nuevoTipoProceso == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }

            if (string.IsNullOrWhiteSpace(nuevoTipoProceso.NombreTipoProceso))
                ModelState.AddModelError(nameof(TipoProceso.NombreTipoProceso), "NombreTipoProceso es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    nuevoTipoProceso.NombreTipoProceso = nuevoTipoProceso.NombreTipoProceso.Trim();
                    if (!string.IsNullOrWhiteSpace(nuevoTipoProceso.Descripcion))
                        nuevoTipoProceso.Descripcion = nuevoTipoProceso.Descripcion.Trim();

                    Crud<TipoProceso>.Create(nuevoTipoProceso);
                    TempData["Success"] = "Tipo de Proceso creado correctamente.";
                    return RedirectToAction(nameof(CreateProcesoElectoral));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear Tipo de Proceso: " + ex.Message;
                    return RedirectToAction(nameof(CreateProcesoElectoral));
                }
            }

            TempData["Error"] = "Revisa los datos del Tipo de Proceso.";
            return RedirectToAction(nameof(CreateProcesoElectoral));
        }
        public IActionResult EditTipoProceso(int id)
        {
            try
            {
                var tipoProcesoData = Crud<TipoProceso>.GetById(id);
                if (tipoProcesoData == null)
                    return NotFound();

                return View("TipoProceso/EditTipoProceso", tipoProcesoData);

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTipoProceso(int id, TipoProceso tipoProcesoData)
        {
            if (tipoProcesoData == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }

            if (string.IsNullOrWhiteSpace(tipoProcesoData.NombreTipoProceso))
                ModelState.AddModelError(nameof(TipoProceso.NombreTipoProceso), "NombreTipoProceso es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    tipoProcesoData.NombreTipoProceso = tipoProcesoData.NombreTipoProceso.Trim();
                    if (!string.IsNullOrWhiteSpace(tipoProcesoData.Descripcion))
                        tipoProcesoData.Descripcion = tipoProcesoData.Descripcion.Trim();

                    Crud<TipoProceso>.Update(id, tipoProcesoData);
                    TempData["Success"] = "Tipo de Proceso actualizado correctamente.";
                    return RedirectToAction(nameof(CreateProcesoElectoral));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar Tipo de Proceso: " + ex.Message;
                    return View("TipoProceso/EditTipoProceso", tipoProcesoData);
                }
            }

            TempData["Error"] = "Revisa los datos del Tipo de Proceso.";
            return View("TipoProceso/EditTipoProceso", tipoProcesoData);
        }


        public IActionResult DeleteTipoProceso(int id)
        {
            try
            {
                var tipo = Crud<TipoProceso>.GetById(id);
                if (tipo == null)
                    return NotFound();

                return View("TipoProceso/_DeleteTipoProceso", tipo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTipoProceso(int id, TipoProceso tipoProcesoData)
        {
            try
            {
                Crud<TipoProceso>.Delete(id);
                TempData["Success"] = "Tipo de Proceso eliminado correctamente.";
                return RedirectToAction(nameof(CreateProcesoElectoral));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar Tipo de Proceso: " + ex.Message;
                return View("TipoProceso/_DeleteTipoProceso", tipoProcesoData);
            }
        }


        // =======================
        // CANDIDATOS - CRUD
        // =======================



        // Helper para obtener las listas
        private List<SelectListItem> GetListas()
        {
            try
            {

                var listas = Crud<Lista>.GetAll() ?? new List<Lista>();

                return listas
                    .OrderBy(l => l.NumeroLista)
                    .ThenBy(l => l.NombreLista)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = $"{l.NumeroLista} - {l.NombreLista}"
                    })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        public IActionResult ListCandidato()
        {
            try
            {


                var candidatos = Crud<Candidato>.GetAll() ?? new List<Candidato>();

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/ListCandidato", candidatos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/ListCandidato", new List<Candidato>());
            }
        }

        public IActionResult FiltrarCandidatos(string nombre, int? lista, int? dignidad)
        {
            try
            {


                var candidatos = Crud<Candidato>.GetAll() ?? new List<Candidato>();

                // Filtrado en memoria
                if (!string.IsNullOrWhiteSpace(nombre))
                    candidatos = candidatos
                        .Where(c => (c.NombreCandidato ?? "").Contains(nombre, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (lista.HasValue)
                    candidatos = candidatos.Where(c => c.IdLista == lista.Value).ToList();

                if (dignidad.HasValue)
                    candidatos = candidatos.Where(c => c.IdDignidad == dignidad.Value).ToList();

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/ListCandidato", candidatos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al filtrar candidatos: " + ex.Message;

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/ListCandidato", new List<Candidato>());
            }
        }

        public IActionResult DetailsCandidato(int id)
        {
            try
            {


                var candidato = Crud<Candidato>.GetById(id);
                if (candidato == null) return NotFound();

                return View("Candidato/DetailsCandidato", candidato);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener el candidato: " + ex.Message;
                return RedirectToAction(nameof(ListCandidato));
            }
        }

        public IActionResult CreateCandidato()
        {
            ViewBag.Listas = GetListas();
            ViewBag.Dignidades = GetDignidades();
            return View("Candidato/CreateCandidato");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCandidato(Candidato nuevoCandidato)
        {
            // Validaciones mínimas
            if (nuevoCandidato == null)
                ModelState.AddModelError("", "Datos inválidos.");

            if (nuevoCandidato != null && string.IsNullOrWhiteSpace(nuevoCandidato.NombreCandidato))
                ModelState.AddModelError(nameof(Candidato.NombreCandidato), "El nombre del candidato es obligatorio.");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los datos del candidato.";
                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();
                return View("Candidato/CreateCandidato", nuevoCandidato);
            }

            try
            {


                nuevoCandidato.NombreCandidato = nuevoCandidato.NombreCandidato.Trim();

                Crud<Candidato>.Create(nuevoCandidato);

                TempData["Success"] = "Candidato creado correctamente.";
                return RedirectToAction(nameof(ListCandidato));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear candidato: " + ex.Message;

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/CreateCandidato", nuevoCandidato);
            }
        }

        public IActionResult EditCandidato(int id)
        {
            try
            {


                var candidato = Crud<Candidato>.GetById(id);
                if (candidato == null) return NotFound();

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/EditCandidato", candidato);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener candidato: " + ex.Message;
                return RedirectToAction(nameof(ListCandidato));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCandidato(int id, Candidato candidatoData)
        {
            if (candidatoData == null)
                ModelState.AddModelError("", "Datos inválidos.");

            if (candidatoData != null && string.IsNullOrWhiteSpace(candidatoData.NombreCandidato))
                ModelState.AddModelError(nameof(Candidato.NombreCandidato), "El nombre del candidato es obligatorio.");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los datos del candidato.";
                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();
                return View("Candidato/EditCandidato", candidatoData);
            }

            try
            {

                candidatoData.NombreCandidato = candidatoData.NombreCandidato.Trim();

                Crud<Candidato>.Update(id, candidatoData);

                TempData["Success"] = "Candidato actualizado correctamente.";
                return RedirectToAction(nameof(ListCandidato));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar candidato: " + ex.Message;

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("Candidato/EditCandidato", candidatoData);
            }
        }

        public IActionResult DeleteCandidato(int id)
        {
            try
            {

                var candidato = Crud<Candidato>.GetById(id);
                if (candidato == null) return NotFound();

                return View("Candidato/DeleteCandidato", candidato);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener candidato: " + ex.Message;
                return RedirectToAction(nameof(ListCandidato));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCandidato(int id, Candidato candidatoData)
        {
            try
            {

                Crud<Candidato>.Delete(id);

                TempData["Success"] = "Candidato eliminado correctamente.";
                return RedirectToAction(nameof(ListCandidato));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar candidato: " + ex.Message;
                return View("Candidato/DeleteCandidato", candidatoData);
            }
        }


        public IActionResult ListLista()
        {
            try
            {
                var listaData = Crud<Lista>.GetAll() ?? new List<Lista>();
                return View("Lista/ListLista", listaData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return View("Lista/ListLista", new List<Lista>());
            }
        }

        public IActionResult DetailsLista(int id)
        {
            try
            {
                // Traer la lista

                var lista = Crud<Lista>.GetById(id);
                if (lista == null) return NotFound();

                // Traer candidatos de esa lista

                var url = $"{Crud<Candidato>.EndPoint}/por-lista/{id}";
                var candidatos = Crud<Candidato>.GetCustom(url) ?? new List<Candidato>();

                // 3) Mandar candidatos a la vista (sin modificar modelos)
                ViewBag.CandidatosDeLaLista = candidatos;

                return View("Lista/DetailsLista", lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener la lista: " + ex.Message;
                return RedirectToAction(nameof(ListLista));
            }
        }



        public IActionResult CreateLista()
        {
            ViewBag.Procesos = GetProcesosElectorales();
            return View("Lista/CreateLista");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLista(Lista nuevaLista)
        {
            if (nuevaLista == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListLista));
            }

            if (string.IsNullOrWhiteSpace(nuevaLista.NombreLista))
                ModelState.AddModelError(nameof(Lista.NombreLista), "El nombre de la lista es obligatorio.");

            if (nuevaLista.NumeroLista <= 0)
                ModelState.AddModelError(nameof(Lista.NumeroLista), "El número de lista debe ser mayor que 0.");

            if (nuevaLista.IdProceso <= 0)
                ModelState.AddModelError(nameof(Lista.IdProceso), "Debe seleccionar un proceso electoral.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Lista>.Create(nuevaLista);
                    TempData["Success"] = "Lista creada correctamente.";
                    return RedirectToAction(nameof(ListLista));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear lista: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos de la lista.";
            }

            ViewBag.Procesos = GetProcesosElectorales();
            return View("Lista/CreateLista", nuevaLista);
        }

        public IActionResult EditLista(int id)
        {
            try
            {
                var lista = Crud<Lista>.GetById(id);
                if (lista == null) return NotFound();

                ViewBag.Procesos = GetProcesosElectorales();
                return View("Lista/EditLista", lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener lista: " + ex.Message;
                return RedirectToAction(nameof(ListLista));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLista(int id, Lista listaData)
        {
            if (listaData == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListLista));
            }

            if (string.IsNullOrWhiteSpace(listaData.NombreLista))
                ModelState.AddModelError(nameof(Lista.NombreLista), "El nombre de la lista es obligatorio.");

            if (listaData.NumeroLista <= 0)
                ModelState.AddModelError(nameof(Lista.NumeroLista), "El número de lista debe ser mayor que 0.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Lista>.Update(id, listaData);
                    TempData["Success"] = "Lista actualizada correctamente.";
                    return RedirectToAction(nameof(ListLista));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar lista: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos de la lista.";
            }

            ViewBag.Procesos = GetProcesosElectorales();
            return View("Lista/EditLista", listaData);
        }

        public IActionResult DeleteLista(int id)
        {
            try
            {
                var lista = Crud<Lista>.GetById(id);
                if (lista == null) return NotFound();

                return View("Lista/DeleteLista", lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener lista: " + ex.Message;
                return RedirectToAction(nameof(ListLista));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLista(int id, Lista listaData)
        {
            try
            {
                Crud<Lista>.Delete(id);
                TempData["Success"] = "Lista eliminada correctamente.";
                return RedirectToAction(nameof(ListLista));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return View("Lista/DeleteLista", listaData);
            }
        }




        // =======================
        // MULTIMEDIA - CRUD
        // Views/Elecciones/Multimedia
        // =======================

        public async Task<IActionResult> ListMultimedia()
        {
            try
            {
                var multimediaData = await _multimediaService.GetAllAsync();
                return View("Multimedia/ListMultimedia", multimediaData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return View("Multimedia/ListMultimedia", new List<Multimedia>());
            }
        }

        public async Task<IActionResult> DetailsMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                if (multimediaData == null) return NotFound();

                return View("Multimedia/DetailsMultimedia", multimediaData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(ListMultimedia));
            }
        }

        public IActionResult CreateMultimedia()
        {
            ViewBag.Candidatos = GetCandidatos();
            ViewBag.Listas = GetListas();
            return View("Multimedia/CreateMultimedia");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultimedia(IFormFile file, int? idCandidato, int? idLista, string? descripcion)
        {
            // Archivo obligatorio
            if (file == null || file.Length == 0)
                ModelState.AddModelError("file", "El archivo es obligatorio.");

            // exactamente uno
            var tieneCandidato = idCandidato.HasValue && idCandidato.Value > 0;
            var tieneLista = idLista.HasValue && idLista.Value > 0;

            if (tieneCandidato == tieneLista)
            {
                ModelState.AddModelError("idCandidato", "Selecciona solo uno: Candidato o Lista.");
                ModelState.AddModelError("idLista", "Selecciona solo uno: Candidato o Lista.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los datos del archivo multimedia.";

                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();

                // Para repintar lo que el usuario ya eligió (opcional pero recomendado)
                ViewData["SelectedCandidato"] = idCandidato;
                ViewData["SelectedLista"] = idLista;
                ViewData["DescripcionValue"] = descripcion;

                return View("Multimedia/CreateMultimedia");
            }

            try
            {
                await _multimediaService.UploadAsync(
                    file,
                    tieneCandidato ? idCandidato : null,
                    tieneLista ? idLista : null,
                    descripcion
                );

                TempData["Success"] = "Multimedia subida correctamente.";
                return RedirectToAction(nameof(ListMultimedia));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al subir multimedia: " + ex.Message;

                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();
                return View("Multimedia/CreateMultimedia");
            }
        }


        public async Task<IActionResult> EditMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                if (multimediaData == null) return NotFound();

                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();
                return View("Multimedia/EditMultimedia", multimediaData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(ListMultimedia));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMultimedia(int id, Multimedia multimediaData)
        {
            try
            {
                await _multimediaService.UpdateAsync(id, multimediaData);
                TempData["Success"] = "Multimedia actualizada correctamente.";
                return RedirectToAction(nameof(ListMultimedia));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar multimedia: " + ex.Message;

                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();
                return View("Multimedia/EditMultimedia", multimediaData);
            }
        }

        public async Task<IActionResult> DeleteMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                if (multimediaData == null) return NotFound();

                return View("Multimedia/DeleteMultimedia", multimediaData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(ListMultimedia));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultimedia(int id, Multimedia multimediaData)
        {
            try
            {
                await _multimediaService.DeleteAsync(id);
                TempData["Success"] = "Multimedia eliminada correctamente.";
                return RedirectToAction(nameof(ListMultimedia));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar multimedia: " + ex.Message;
                return View("Multimedia/DeleteMultimedia", multimediaData);
            }
        }

        // =======================
        // PREGUNTA CONSULTA - CRUD
        // =======================

        public IActionResult ListPreguntaConsulta()
        {
            try
            {
                var preguntas = Crud<PreguntaConsulta>.GetAll() ?? new List<PreguntaConsulta>();
                return View("PreguntaConsulta/ListPreguntaConsulta", preguntas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return View("PreguntaConsulta/ListPreguntaConsulta", new List<PreguntaConsulta>());
            }
        }

        public IActionResult DetailsPreguntaConsulta(int id)
        {
            try
            {
                var pregunta = Crud<PreguntaConsulta>.GetById(id);
                if (pregunta == null) return NotFound();

                // Cargar opciones asociadas
                 var url = $"{Crud<OpcionConsulta>.EndPoint}/por-pregunta/{id}";
                 var opciones = Crud<OpcionConsulta>.GetCustom(url) ?? new List<OpcionConsulta>();
                 ViewBag.OpcionesDeLaPregunta = opciones;

                return View("PreguntaConsulta/DetailsPreguntaConsulta", pregunta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener pregunta: " + ex.Message;
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }
        }

        public IActionResult CreatePreguntaConsulta()
        {
            ViewBag.Procesos = GetProcesosElectorales();
            return View("PreguntaConsulta/CreatePreguntaConsulta");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePreguntaConsulta(PreguntaConsulta nuevaPregunta)
        {
            if (nuevaPregunta == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }

            if (string.IsNullOrWhiteSpace(nuevaPregunta.TextoPregunta))
                ModelState.AddModelError(nameof(PreguntaConsulta.TextoPregunta), "El texto de la pregunta es obligatorio.");

            if (nuevaPregunta.IdProceso <= 0)
                ModelState.AddModelError(nameof(PreguntaConsulta.IdProceso), "Debe seleccionar un proceso electoral.");
            
             if (nuevaPregunta.NumeroPregunta <= 0)
                ModelState.AddModelError(nameof(PreguntaConsulta.NumeroPregunta), "El número de pregunta debe ser mayor a 0.");


            if (ModelState.IsValid)
            {
                try
                {
                    Crud<PreguntaConsulta>.Create(nuevaPregunta);
                    TempData["Success"] = "Pregunta de consulta creada correctamente.";
                    return RedirectToAction(nameof(ListPreguntaConsulta));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear pregunta: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos de la pregunta.";
            }

            ViewBag.Procesos = GetProcesosElectorales();
            return View("PreguntaConsulta/CreatePreguntaConsulta", nuevaPregunta);
        }

        public IActionResult EditPreguntaConsulta(int id)
        {
            try
            {
                var pregunta = Crud<PreguntaConsulta>.GetById(id);
                if (pregunta == null) return NotFound();

                ViewBag.Procesos = GetProcesosElectorales();
                return View("PreguntaConsulta/EditPreguntaConsulta", pregunta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener pregunta: " + ex.Message;
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPreguntaConsulta(int id, PreguntaConsulta preguntaData)
        {
            if (preguntaData == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }

            if (string.IsNullOrWhiteSpace(preguntaData.TextoPregunta))
                ModelState.AddModelError(nameof(PreguntaConsulta.TextoPregunta), "El texto de la pregunta es obligatorio.");

            if (preguntaData.IdProceso <= 0)
                ModelState.AddModelError(nameof(PreguntaConsulta.IdProceso), "Debe seleccionar un proceso electoral.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<PreguntaConsulta>.Update(id, preguntaData);
                    TempData["Success"] = "Pregunta actualizada correctamente.";
                    return RedirectToAction(nameof(ListPreguntaConsulta));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar pregunta: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos de la pregunta.";
            }

            ViewBag.Procesos = GetProcesosElectorales();
            return View("PreguntaConsulta/EditPreguntaConsulta", preguntaData);
        }

        public IActionResult DeletePreguntaConsulta(int id)
        {
            try
            {
                var pregunta = Crud<PreguntaConsulta>.GetById(id);
                if (pregunta == null) return NotFound();

                return View("PreguntaConsulta/DeletePreguntaConsulta", pregunta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePreguntaConsulta(int id, PreguntaConsulta preguntaData)
        {
            try
            {
                Crud<PreguntaConsulta>.Delete(id);
                TempData["Success"] = "Pregunta eliminada correctamente.";
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar pregunta: " + ex.Message;
                return View("PreguntaConsulta/DeletePreguntaConsulta", preguntaData);
            }
        }
    
        // =======================
        // TIPO VOTO - CRUD
        // =======================

        public IActionResult ListTipoVoto()
        {
            try
            {
                var tipos = Crud<TipoVoto>.GetAll() ?? new List<TipoVoto>();
                return View("TipoVoto/ListTipoVoto", tipos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return View("TipoVoto/ListTipoVoto", new List<TipoVoto>());
            }
        }

        public IActionResult CreateTipoVoto()
        {
            return View("TipoVoto/CreateTipoVoto");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTipoVoto(TipoVoto nuevoTipo)
        {
            if (nuevoTipo == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListTipoVoto));
            }

            if (string.IsNullOrWhiteSpace(nuevoTipo.NombreTipo))
                ModelState.AddModelError(nameof(TipoVoto.NombreTipo), "El nombre del tipo de voto es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<TipoVoto>.Create(nuevoTipo);
                    TempData["Success"] = "Tipo de voto creado correctamente.";
                    return RedirectToAction(nameof(ListTipoVoto));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear tipo de voto: " + ex.Message;
                }
            }

            return View("TipoVoto/CreateTipoVoto", nuevoTipo);
        }

        public IActionResult EditTipoVoto(int id)
        {
            try
            {
                var tipo = Crud<TipoVoto>.GetById(id);
                if (tipo == null) return NotFound();

                return View("TipoVoto/EditTipoVoto", tipo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener tipo de voto: " + ex.Message;
                return RedirectToAction(nameof(ListTipoVoto));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTipoVoto(int id, TipoVoto tipoData)
        {
            if (tipoData == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListTipoVoto));
            }

            if (string.IsNullOrWhiteSpace(tipoData.NombreTipo))
                ModelState.AddModelError(nameof(TipoVoto.NombreTipo), "El nombre del tipo de voto es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<TipoVoto>.Update(id, tipoData);
                    TempData["Success"] = "Tipo de voto actualizado correctamente.";
                    return RedirectToAction(nameof(ListTipoVoto));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar tipo de voto: " + ex.Message;
                }
            }

            return View("TipoVoto/EditTipoVoto", tipoData);
        }

        public IActionResult DeleteTipoVoto(int id)
        {
            try
            {
                var tipo = Crud<TipoVoto>.GetById(id);
                if (tipo == null) return NotFound();

                return View("TipoVoto/DeleteTipoVoto", tipo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(ListTipoVoto));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTipoVoto(int id, TipoVoto tipoData)
        {
            try
            {
                Crud<TipoVoto>.Delete(id);
                TempData["Success"] = "Tipo de voto eliminado correctamente.";
                return RedirectToAction(nameof(ListTipoVoto));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar tipo de voto: " + ex.Message;
                return View("TipoVoto/DeleteTipoVoto", tipoData);
            }
        }

        // =======================
        // OPCIÓN CONSULTA - CRUD
        // =======================

        private List<SelectListItem> GetPreguntasSelect()
        {
            try
            {
                var preguntas = Crud<PreguntaConsulta>.GetAll() ?? new List<PreguntaConsulta>();
                return preguntas
                    .OrderBy(p => p.NumeroPregunta)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"P{p.NumeroPregunta}: {p.TextoPregunta}" })
                    .ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        public IActionResult ListOpcionConsulta()
        {
            try
            {
                var opciones = Crud<OpcionConsulta>.GetAll() ?? new List<OpcionConsulta>();
                return View("OpcionConsulta/ListOpcionConsulta", opciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return View("OpcionConsulta/ListOpcionConsulta", new List<OpcionConsulta>());
            }
        }

        public IActionResult DetailsOpcionConsulta(int id)
        {
            try
            {
                var opcion = Crud<OpcionConsulta>.GetById(id);
                if (opcion == null) return NotFound();

                return View("OpcionConsulta/DetailsOpcionConsulta", opcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener opción: " + ex.Message;
                return RedirectToAction(nameof(ListOpcionConsulta));
            }
        }

        public IActionResult CreateOpcionConsulta()
        {
            ViewBag.Preguntas = GetPreguntasSelect();
            return View("OpcionConsulta/CreateOpcionConsulta");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOpcionConsulta(OpcionConsulta nuevaOpcion)
        {
            if (nuevaOpcion == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListOpcionConsulta));
            }

            if (string.IsNullOrWhiteSpace(nuevaOpcion.TextoOpcion))
                ModelState.AddModelError(nameof(OpcionConsulta.TextoOpcion), "El texto de la opción es obligatorio.");

            if (nuevaOpcion.IdPregunta <= 0)
                ModelState.AddModelError(nameof(OpcionConsulta.IdPregunta), "Debe seleccionar una pregunta.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<OpcionConsulta>.Create(nuevaOpcion);
                    TempData["Success"] = "Opción de consulta creada correctamente.";
                    return RedirectToAction(nameof(ListOpcionConsulta));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear opción: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos de la opción.";
            }

            ViewBag.Preguntas = GetPreguntasSelect();
            return View("OpcionConsulta/CreateOpcionConsulta", nuevaOpcion);
        }

        public IActionResult EditOpcionConsulta(int id)
        {
            try
            {
                var opcion = Crud<OpcionConsulta>.GetById(id);
                if (opcion == null) return NotFound();

                ViewBag.Preguntas = GetPreguntasSelect();
                return View("OpcionConsulta/EditOpcionConsulta", opcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener opción: " + ex.Message;
                return RedirectToAction(nameof(ListOpcionConsulta));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOpcionConsulta(int id, OpcionConsulta opcionData)
        {
            if (opcionData == null)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(ListOpcionConsulta));
            }

            if (string.IsNullOrWhiteSpace(opcionData.TextoOpcion))
                ModelState.AddModelError(nameof(OpcionConsulta.TextoOpcion), "El texto de la opción es obligatorio.");

            if (opcionData.IdPregunta <= 0)
                ModelState.AddModelError(nameof(OpcionConsulta.IdPregunta), "Debe seleccionar una pregunta.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<OpcionConsulta>.Update(id, opcionData);
                    TempData["Success"] = "Opción actualizada correctamente.";
                    return RedirectToAction(nameof(ListOpcionConsulta));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar opción: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Revisa los datos de la opción.";
            }

            ViewBag.Preguntas = GetPreguntasSelect();
            return View("OpcionConsulta/EditOpcionConsulta", opcionData);
        }

        public IActionResult DeleteOpcionConsulta(int id)
        {
            try
            {
                var opcion = Crud<OpcionConsulta>.GetById(id);
                if (opcion == null) return NotFound();

                return View("OpcionConsulta/DeleteOpcionConsulta", opcion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al conectar con la API: " + ex.Message;
                return RedirectToAction(nameof(ListOpcionConsulta));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOpcionConsulta(int id, OpcionConsulta opcionData)
        {
            try
            {
                Crud<OpcionConsulta>.Delete(id);
                TempData["Success"] = "Opción eliminada correctamente.";
                return RedirectToAction(nameof(ListOpcionConsulta));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar opción: " + ex.Message;
                return View("OpcionConsulta/DeleteOpcionConsulta", opcionData);
            }
        }
    }
}
