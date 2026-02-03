using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;
using System.Collections;
using System.Collections.Generic;

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
            return Crud<TipoProceso>.GetAll()
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.NombreTipoProceso
                })
                .ToList();
        }

        public IActionResult ListProcesoElectoral()
        {
            try
            {
                var procesoElectoral = Crud<ProcesoElectoral>.GetAll();
                return View(procesoElectoral);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<ProcesoElectoral>());
            }
        }
        public IActionResult DetailsProcesoElectoral(int id)
        {
            try
            {
                var procesoElectoral = Crud<ProcesoElectoral>.GetById(id);
                if (procesoElectoral == null)
                    return NotFound();

                return View(procesoElectoral);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateProcesoElectoral()
        {
            ViewBag.TipoProceso = GetTipoProceso();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProcesoElectoral(ProcesoElectoral nuevoProcesoElectoral)
        {
            // Validación lógica mínima
            if (nuevoProcesoElectoral.FechaInicio >= nuevoProcesoElectoral.FechaFin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser menor a la fecha de fin.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<ProcesoElectoral>.Create(nuevoProcesoElectoral);
                    return RedirectToAction(nameof(ListProcesoElectoral));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear: " + ex.Message);
                }
            }

            // IMPORTANTE: volver a cargar el dropdown
            ViewBag.TipoProceso = GetTipoProceso();
            return View(nuevoProcesoElectoral);
        }

        public IActionResult EditProcesoElectoral(int id)
        {
            try
            {
                var procesoElectoral = Crud<ProcesoElectoral>.GetById(id);
                if (procesoElectoral == null)
                    return NotFound();

                ViewBag.TipoProceso = GetTipoProceso();
                return View(procesoElectoral);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProcesoElectoral(int id, ProcesoElectoral procesoElectoral)
        {
            if (procesoElectoral.FechaInicio >= procesoElectoral.FechaFin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser menor a la fecha de fin.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Crud<ProcesoElectoral>.Update(id, procesoElectoral);
                    return RedirectToAction(nameof(ListProcesoElectoral));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            ViewBag.TipoProceso = GetTipoProceso();
            return View(procesoElectoral);
        }


        public IActionResult DeleteProcesoElectoral(int id)
        {
            try
            {
                var procesoElectoral = Crud<ProcesoElectoral>.GetById(id);
                if (procesoElectoral == null)
                    return NotFound();

                return View(procesoElectoral);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProcesoElectoral(int id, ProcesoElectoral procesoElectoral)
        {
            try
            {
                Crud<ProcesoElectoral>.Delete(id);
                return RedirectToAction(nameof(ListProcesoElectoral));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(procesoElectoral);
            }
        }






        // TIPO DE PROCESOS - CRUD
        //index
        public IActionResult ListTipoProceso()
        {
            try
            {
                var tipoProcesosData = Crud<TipoProceso>.GetAll();
                return View(tipoProcesosData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<TipoProceso>());
            }
        }
        public IActionResult DetailsTipoProceso(int id)
        {
            try
            {
                var tipoProcesoData = Crud<TipoProceso>.GetById(id);
                if (tipoProcesoData == null)
                    return NotFound();

                return View(tipoProcesoData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateTipoProceso()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTipoProceso(TipoProceso nuevoTipoProceso)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<TipoProceso>.Create(nuevoTipoProceso);
                    return RedirectToAction(nameof(ListTipoProceso));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear: " + ex.Message);
                }
            }
            return View(nuevoTipoProceso);
        }
        public IActionResult EditTipoProceso(int id)
        {
            try
            {
                var tipoProcesoData = Crud<TipoProceso>.GetById(id);
                if (tipoProcesoData == null)
                    return NotFound();

                return View(tipoProcesoData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTipoProceso(int id, TipoProceso tipoProcesoData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<TipoProceso>.Update(id, tipoProcesoData);
                    return RedirectToAction(nameof(ListTipoProceso));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            return View(tipoProcesoData);
        }

        public IActionResult DeleteTipoProceso(int id)
        {
            try
            {
                var tipo = Crud<TipoProceso>.GetById(id);
                if (tipo == null)
                    return NotFound();

                return View(tipo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTipoProceso(int id, TipoProceso tipoProcesoData)
        {
            try
            {
                Crud<TipoProceso>.Delete(id);
                return RedirectToAction(nameof(ListTipoProceso));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(tipoProcesoData);
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
                var opcionConsultaData = Crud<OpcionConsulta>.GetById(id);
                return View(opcionConsultaData);
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
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<OpcionConsulta>.Create(nuevaOpcion);
                    return RedirectToAction(nameof(ListOpcionConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            ViewBag.Valores = GetOpcionesConsulta();
            ViewBag.Preguntas = GetPreguntasConsulta();
            return View(nuevaOpcion);
        }

        // EDITAR (GET)
        public IActionResult EditOpcionConsulta(int id)
        {
            try
            {
                var opcionConsultaData = Crud<OpcionConsulta>.GetById(id);
                ViewBag.Valores = GetOpcionesConsulta();
                ViewBag.Preguntas = GetPreguntasConsulta();
                return View(opcionConsultaData);
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
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<OpcionConsulta>.Update(id, opcionConsultaData);
                    return RedirectToAction(nameof(ListOpcionConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            ViewBag.Valores = GetOpcionesConsulta();
            ViewBag.Preguntas = GetPreguntasConsulta();
            return View(opcionConsultaData);
        }

        // ELIMINAR (GET)
        public IActionResult DeleteOpcionConsulta(int id)
        {
            var opcionConsultaData = Crud<OpcionConsulta>.GetById(id);
            return View(opcionConsultaData);
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
                ModelState.AddModelError("", "Error: " + ex.Message);
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultimedia(IFormFile file, int idCandidato, int idLista, string? descripcion)
        {
            if (file != null)
            {
                try
                {
                    await _multimediaService.UploadAsync(file, idCandidato, idLista, descripcion);
                    return RedirectToAction(nameof(ListMultimedia));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            return View();

        }
        public async Task<IActionResult> EditMultimedia(int id)
        {
            try
            {
                var multimediaData = await _multimediaService.GetByIdAsync(id);
                return View(multimediaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(id);
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
            var multimediaData = await _multimediaService.GetByIdAsync(id);
            return View(multimediaData);
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
