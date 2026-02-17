using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SistemaVotacion.MVC.Controllers
{
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
                var procesoElectoral = Crud<ProcesoElectoral>.GetById(id);
                if (procesoElectoral == null)
                    return NotFound();

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


        // CADIDATOS - CRUD
        // Helper para obtener las listas
        private List<SelectListItem> GetListas()
        {
            Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
            return Crud<Lista>.GetAll()
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.NombreLista
                })
                .ToList();
        }

        // Helper para obtener las dignidades
        private List<SelectListItem> GetDignidades()
        {
            return Crud<Dignidad>.GetAll()
               .Select(d => new SelectListItem
               {
                   Value = d.Id.ToString(),
                   Text = d.NombreDignidad
               })
               .ToList();
        }


        public IActionResult ListCandidato()
        {
            try
            {
                var candidatos = Crud<Candidato>.GetAll();
                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();
                return View(candidatos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();
                return View(new List<Candidato>());
            }
        }


        public IActionResult FiltrarCandidatos(string nombre, int? lista, int? dignidad)
        {
            try
            {
                var candidatos = Crud<Candidato>.GetAll();

                // Filtrado en memoria
                if (!string.IsNullOrEmpty(nombre))
                    candidatos = candidatos
                        .Where(c => c.NombreCandidato.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (lista.HasValue)
                    candidatos = candidatos.Where(c => c.IdLista == lista.Value).ToList();

                if (dignidad.HasValue)
                    candidatos = candidatos.Where(c => c.IdDignidad == dignidad.Value).ToList();

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();

                return View("ListCandidato", candidatos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al filtrar candidatos: " + ex.Message;
                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();
                return View("ListCandidato", new List<Candidato>());
            }
        }


        public IActionResult DetailsCandidato(int id)
        {
            try
            {
                var candidato = Crud<Candidato>.GetById(id);
                if (candidato == null) return NotFound();
                return View(candidato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        public IActionResult CreateCandidato()
        {
            ViewBag.Listas = GetListas();
            ViewBag.Dignidades = GetDignidades();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCandidato(Candidato nuevoCandidato)
        {

            if (string.IsNullOrEmpty(nuevoCandidato.NombreCandidato))
                ModelState.AddModelError("", "El nombre del candidato es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Candidato>.Create(nuevoCandidato);
                    return RedirectToAction(nameof(ListCandidato));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear: " + ex.Message);
                }
            }

            ViewBag.Listas = GetListas();
            ViewBag.Dignidades = GetDignidades();
            return View(nuevoCandidato);
        }


        public IActionResult EditCandidato(int id)
        {
            try
            {
                var candidato = Crud<Candidato>.GetById(id);
                if (candidato == null) return NotFound();

                ViewBag.Listas = GetListas();
                ViewBag.Dignidades = GetDignidades();
                return View(candidato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCandidato(int id, Candidato candidatoData)
        {

            if (string.IsNullOrEmpty(candidatoData.NombreCandidato))
                ModelState.AddModelError("", "El nombre del candidato es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Candidato>.Update(id, candidatoData);
                    return RedirectToAction(nameof(ListCandidato));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            ViewBag.Listas = GetListas();
            ViewBag.Dignidades = GetDignidades();
            return View(candidatoData);
        }


        public IActionResult DeleteCandidato(int id)
        {
            try
            {
                var candidato = Crud<Candidato>.GetById(id);
                if (candidato == null) return NotFound();
                return View(candidato);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCandidato(int id, Candidato candidatoData)
        {
            try
            {
                Crud<Candidato>.Delete(id);
                return RedirectToAction(nameof(ListCandidato));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(candidatoData);
            }
        }

        //OPCION CONSULTA - CRUD
        // LISTAR

        // Helper para opciones Sí/No
        private List<SelectListItem> GetOpcionesConsulta()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "Sí", Text = "Sí" },
            new SelectListItem { Value = "No", Text = "No" }
        };
        }

        // Helper para preguntas
        private List<SelectListItem> GetPreguntasConsulta()
        {
            return Crud<PreguntaConsulta>.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.TextoPregunta
                })
                .ToList();
        }
        public IActionResult ListOpcionConsulta()
        {
            try
            {
                var opcionConsultaData = Crud<OpcionConsulta>.GetAll();
                return View(opcionConsultaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<OpcionConsulta>());
            }
        }

        // DETALLES
        public IActionResult DetailsOpcionConsulta(int id)
        {
            try
            {
                var opcion = Crud<OpcionConsulta>.GetById(id);
                if (opcion == null)
                    return NotFound();

                return View(opcion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        

        // CREAR (GET)
        public IActionResult CreateOpcionConsulta()
        {
            ViewBag.Valores = GetOpcionesConsulta();
            ViewBag.Preguntas = GetPreguntasConsulta();
            return View();
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOpcionConsulta(OpcionConsulta nuevaOpcion)
        {
            if (string.IsNullOrWhiteSpace(nuevaOpcion.TextoOpcion))
                ModelState.AddModelError("", "El texto de la opción es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<OpcionConsulta>.Create(nuevaOpcion);
                    return RedirectToAction(nameof(ListOpcionConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear: " + ex.Message);
                }
            }

            ViewBag.Pregunta = GetPreguntasConsulta();
            ViewBag.Valores = GetOpcionesConsulta();
            return View(nuevaOpcion);
        }

        // EDITAR (GET)
        public IActionResult EditOpcionConsulta(int id)
        {
            try
            {
                var opcion = Crud<OpcionConsulta>.GetById(id);
                if (opcion == null)
                    return NotFound();

                ViewBag.Pregunta = GetPreguntasConsulta();
                ViewBag.Valores = GetOpcionesConsulta();
                return View(opcion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOpcionConsulta(int id, OpcionConsulta opcionConsultaData)
        {
            if (string.IsNullOrWhiteSpace(opcionConsultaData.TextoOpcion))
                ModelState.AddModelError("", "El texto de la opción es obligatorio.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<OpcionConsulta>.Update(id, opcionConsultaData);
                    return RedirectToAction(nameof(ListOpcionConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            ViewBag.Pregunta = GetPreguntasConsulta();
            ViewBag.Valores = GetOpcionesConsulta();
            return View(opcionConsultaData);
        }

        // ELIMINAR (GET)
        public IActionResult DeleteOpcionConsulta(int id)
        {
            try
            {
                var opcion = Crud<OpcionConsulta>.GetById(id);
                if (opcion == null)
                    return NotFound();

                return View(opcion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        // ELIMINAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOpcionConsulta(int id, OpcionConsulta opcionConsultaData)
        {
            try
            {
                Crud<OpcionConsulta>.Delete(id);
                return RedirectToAction(nameof(ListOpcionConsulta));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(opcionConsultaData);
            }
        }

        //PREGUNTA CONSULTA - CRUD
        public IActionResult ListPreguntaConsulta()
        {
            try
            {
                var preguntas = Crud<PreguntaConsulta>.GetAll();
                return View(preguntas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<PreguntaConsulta>());
            }

        }
        public IActionResult DetailsPreguntaConsulta(int id)
        {
            try
            {
                var pregunta = Crud<PreguntaConsulta>.GetById(id);
                if (pregunta == null) return NotFound();

                return View(pregunta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreatePreguntaConsulta()
        {
            ViewBag.ProcesosElectorales = GetProcesosElectorales();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePreguntaConsulta(PreguntaConsulta nuevoPreguntaConsulta)
        {
            // Validación mínima
            if (nuevoPreguntaConsulta.NumeroPregunta <= 0)
                ModelState.AddModelError("", "El número de la pregunta debe ser mayor que 0.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<PreguntaConsulta>.Create(nuevoPreguntaConsulta);
                    return RedirectToAction(nameof(ListPreguntaConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear: " + ex.Message);
                }
            }

            ViewBag.ProcesosElectorales = GetProcesosElectorales();
            return View(nuevoPreguntaConsulta);

        }
        public IActionResult EditPreguntaConsulta(int id)
        {
            try
            {
                var pregunta = Crud<PreguntaConsulta>.GetById(id);
                if (pregunta == null) return NotFound();

                ViewBag.ProcesosElectorales = GetProcesosElectorales();
                return View(pregunta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPreguntaConsulta(int id, PreguntaConsulta preguntaConsultaData)
        {
            if (preguntaConsultaData.NumeroPregunta <= 0)
                ModelState.AddModelError("", "El número de la pregunta debe ser mayor que 0.");

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<PreguntaConsulta>.Update(id, preguntaConsultaData);
                    return RedirectToAction(nameof(ListPreguntaConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            ViewBag.ProcesosElectorales = GetProcesosElectorales();
            return View(preguntaConsultaData);
        }

        public IActionResult DeletePreguntaConsulta(int id)
        {
            try
            {
                var pregunta = Crud<PreguntaConsulta>.GetById(id);
                if (pregunta == null) return NotFound();

                return View(pregunta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePreguntaConsulta(int id, PreguntaConsulta preguntaConsultaData)
        {
            try
            {
                Crud<PreguntaConsulta>.Delete(id);
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(preguntaConsultaData);
            }
        }

        // Lista-Crud
        private List<SelectListItem> GetProcesosElectorales()
        {

            return Crud<ProcesoElectoral>.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.NombreProceso
                })
                .ToList();
        }
        public IActionResult ListLista()
        {
            try
            {
                Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                var ListaData = Crud<Lista>.GetAll();
                return View(ListaData);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<object>());
            }

        }
        public IActionResult DetailsLista(int id)
        {
            try
            {
                Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                var lista = Crud<Lista>.GetById(id);
                if (lista == null)
                    return NotFound();

                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener la lista: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateLista()
        {
            Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
            ViewBag.Procesos = GetProcesosElectorales();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLista(Lista nuevaLista)
        {
            if (string.IsNullOrWhiteSpace(nuevaLista.NombreLista))
            {
                ModelState.AddModelError("", "El nombre de la lista es obligatorio.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                    Crud<Lista>.Create(nuevaLista);
                    return RedirectToAction(nameof(ListLista));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear lista: " + ex.Message);
                }
            }

            ViewBag.Procesos = GetProcesosElectorales();
            return View(nuevaLista);
        }
        public IActionResult EditLista(int id)
        {
            try
            {
                Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                var lista = Crud<Lista>.GetById(id);
                if (lista == null)
                    return NotFound();

                ViewBag.Procesos = GetProcesosElectorales();
                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener lista: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLista(int id, Lista ListaData)
        {
            if (string.IsNullOrWhiteSpace(ListaData.NombreLista))
            {
                ModelState.AddModelError("", "El nombre de la lista es obligatorio.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                    Crud<Lista>.Update(id, ListaData);
                    return RedirectToAction(nameof(ListLista));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar lista: " + ex.Message);
                }
            }

            ViewBag.Procesos = GetProcesosElectorales();
            return View(ListaData);
        }

        public IActionResult DeleteLista(int id)
        {
            try
            {
                Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                var lista = Crud<Lista>.GetById(id);
                if (lista == null)
                    return NotFound();

                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener lista: " + ex.Message;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLista(int id, Lista ListaData)
        {
            try
            {
                Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas";
                Crud<Lista>.Delete(id);
                return RedirectToAction(nameof(ListLista));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(ListaData);
            }
        }
        // Multimedia-Crud
        private List<SelectListItem> GetCandidatos()
        {
            return Crud<Candidato>.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.NombreCandidato // Cambia según tu propiedad de nombre
                })
                .ToList();
        }

       
        public async Task<IActionResult> ListMultimedia()
        {
            try
            {

                var multimediaData = await _multimediaService.GetAllAsync();
                return View(multimediaData);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<Multimedia>());
            }

        }
        public async Task<IActionResult> DetailsMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                return View(multimediaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateMultimedia()
        {
            ViewBag.Candidatos = GetCandidatos();
            ViewBag.Listas = GetListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultimedia(IFormFile file, int idCandidato, int idLista, string? descripcion)
        {
            if (file == null || idCandidato == 0 || idLista == 0)
            {
                ModelState.AddModelError("", "Archivo, Candidato y Lista son obligatorios.");
                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();
                return View();
            }

            try
            {
                await _multimediaService.UploadAsync(file, idCandidato, idLista, descripcion);
                return RedirectToAction(nameof(ListMultimedia));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();
                return View();
            }
        }

        public async Task<IActionResult> EditMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                ViewBag.Candidatos = GetCandidatos();
                ViewBag.Listas = GetListas();
                return View(multimediaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMultimedia(int id, Multimedia multimediaData)
        {
            try
            {
                await _multimediaService.UpdateAsync(id, multimediaData);
                return RedirectToAction(nameof(ListMultimedia));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(multimediaData);
            }
        }

        public async Task<IActionResult> DeleteMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                if (multimediaData == null)
                    return NotFound();
                return View(multimediaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultimedia(int id, Multimedia multimediaData)
        {
            try
            {
                await _multimediaService.DeleteAsync(id);
                return RedirectToAction(nameof(ListMultimedia));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(multimediaData);
            }
        }
    }
}
