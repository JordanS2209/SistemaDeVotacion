using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;

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

        // TIPO DE PROCESOS - CRUD
        //index
        public IActionResult ListTipoProcesos()
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
        public IActionResult DetailsTipoProcesos(int id)
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
        public IActionResult CreateTipoProcesos()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTipoProcesos(TipoProceso nuevoTipoProceso)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<TipoProceso>.Create(nuevoTipoProceso);
                    return RedirectToAction(nameof(ListTipoProcesos));
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
                    return RedirectToAction(nameof(ListTipoProcesos));
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
            var tipoProcesoData = Crud<TipoProceso>.GetById(id);
            return View(tipoProcesoData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTipoProceso(int id, TipoProceso tipoProcesoData)
        {
            try
            {
                Crud<TipoProceso>.Delete(id);
                return RedirectToAction(nameof(ListTipoProcesos));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(tipoProcesoData);
            }
        }

        //// CADIDATOS - CRUD
        //// Helper para obtener las listas
        //private List<SelectListItem> GetListas()
        //{
        //    Crud<Lista>.EndPoint = "https://localhost:7202/api/Listas/simple";
        //    var listas = Crud<Lista>.GetAll();
        //    return listas.Select(l => new SelectListItem
        //    {
        //        Value = l.Id.ToString(),
        //        Text = l.NombreLista
        //    }).ToList();
        //}

        //// Helper para obtener las dignidades
        //private List<SelectListItem> GetDignidades()
        //{
        //    Crud<Dignidad>.EndPoint = "https://localhost:7202/api/Dignidades/simple";

        //    var dignidades = Crud<Dignidad>.GetAll();
        //    return dignidades.Select(d => new SelectListItem
        //    {
        //        Value = d.Id.ToString(),
        //        Text = d.NombreDignidad
        //    }).ToList();
        //}

        public IActionResult ListCandidato()
        {
            try
            {
                // Ya no necesitas configurar el EndPoint aquí
                var candidatoData = Crud<Candidato>.GetAll();

                // Poblar los dropdowns
                //ViewBag.Listas = Crud<Lista>.GetAll();
                //ViewBag.Dignidades = Crud<Dignidad>.GetAll();

                return View(candidatoData);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al conectar con la API: " + ex.Message;
                //ViewBag.Listas = Crud<Lista>.GetAll();
                //ViewBag.Dignidades = Crud<Dignidad>.GetAll();
                return View(new List<Candidato>());
            }
        }


        public IActionResult FiltrarCandidatos(string nombre, int? lista, int? dignidad)
        {
            try
            {

                var queryParams = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(nombre))
                    queryParams.Add("nombre", nombre);

                if (lista.HasValue)
                    queryParams.Add("lista", lista.Value.ToString());

                if (dignidad.HasValue)
                    queryParams.Add("dignidad", dignidad.Value.ToString());

                // Generar la URL final con los parámetros
                string url = QueryHelpers.AddQueryString($"{Crud<Candidato>.EndPoint}/filtrar", queryParams);

                // Llamar al endpoint de filtrado
                var candidatos = Crud<Candidato>.GetCustom(url);

                // Poblar los dropdowns para que se mantengan en la vista
                ViewBag.Listas = Crud<Lista>.GetAll();
                ViewBag.Dignidades = Crud<Dignidad>.GetAll();

                return View("ListCandidato", candidatos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al filtrar candidatos: " + ex.Message;
                ViewBag.Listas = Crud<Lista>.GetAll();
                ViewBag.Dignidades = Crud<Dignidad>.GetAll();
                return View("ListCandidato", new List<Candidato>());
            }
        }
        public IActionResult DetailsCandidato(int id)
        {
            try
            {
                var candidatoData = Crud<Candidato>.GetById(id);
                if (candidatoData == null)
                    return NotFound();

                return View(candidatoData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }

        public IActionResult CreateCandidato()
        {
            ViewBag.Listas = Crud<Lista>.GetAll();
            ViewBag.Dignidades = Crud<Dignidad>.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCandidato(Candidato nuevoCandidato)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var candidatoCreado = Crud<Candidato>.Create(nuevoCandidato);
                    return RedirectToAction(nameof(ListCandidato));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear candidato: " + ex.Message);
                }
            }

            ViewBag.Listas = Crud<Lista>.GetAll();
            ViewBag.Dignidades = Crud<Dignidad>.GetAll();
            return View(nuevoCandidato);
        }


        public IActionResult EditCandidato(int id)
        {
            try
            {
                var candidatoData = Crud<Candidato>.GetById(id);
                if (candidatoData == null)
                    return NotFound();

                ViewBag.Listas = Crud<Lista>.GetAll();
                ViewBag.Dignidades = Crud<Dignidad>.GetAll();
                return View(candidatoData);
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
            if (id != candidatoData.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del candidato.");
            }

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

            ViewBag.Listas = Crud<Lista>.GetAll();
            ViewBag.Dignidades = Crud<Dignidad>.GetAll();
            return View(candidatoData);
        }


        public IActionResult DeleteCandidato(int id)
        {
            var candidatoData = Crud<Candidato>.GetById(id);
            if (candidatoData == null)
                return NotFound();

            return View(candidatoData);
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
                var preguntaConsultaData = Crud<PreguntaConsulta>.GetAll();
                return View(preguntaConsultaData);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<object>());
            }

        }
        public IActionResult DetailsPreguntaConsulta(int id)
        {
            try
            {
                var preguntaConsultaData = Crud<PreguntaConsulta>.GetById(id);
                return View(preguntaConsultaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreatePreguntaConsulta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePreguntaConsulta(PreguntaConsulta nuevoPreguntaConsulta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<PreguntaConsulta>.Create(nuevoPreguntaConsulta);
                    return RedirectToAction(nameof(ListPreguntaConsulta));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);

                }
            }
            return View(nuevoPreguntaConsulta);

        }
        public IActionResult EditPreguntaConsulta(int id)
        {
            try
            {
                var preguntaConsultaData = Crud<PreguntaConsulta>.GetById(id);
                return View(preguntaConsultaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(id);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPreguntaConsulta(int id, PreguntaConsulta preguntaConsultaData)
        {
            try
            {
                Crud<PreguntaConsulta>.Update(id, preguntaConsultaData);
                return RedirectToAction(nameof(ListPreguntaConsulta));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(preguntaConsultaData);
            }
        }

        public IActionResult DeletePreguntaConsulta(int id)
        {
            var preguntaConsultaData = Crud<PreguntaConsulta>.GetById(id);
            return View(preguntaConsultaData);
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
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(preguntaConsultaData);
            }
        }
        // PROCESO ELECTORAL - CRUD
        public IActionResult ListProcesoElectoral()
        {
            try
            {
                var procesoElectoralData = Crud<ProcesoElectoral>.GetAll();
                return View(procesoElectoralData);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(new List<object>());
            }

        }
        public IActionResult DetailsProcesoElectoral(int id)
        {
            try
            {
                var procesoElectoralData = Crud<ProcesoElectoral>.GetById(id);
                return View(procesoElectoralData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateProcesoElectoral()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProcesoElectoral(ProcesoElectoral nuevoProcesoElectoral)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<ProcesoElectoral>.Create(nuevoProcesoElectoral);
                    return RedirectToAction(nameof(ListProcesoElectoral));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);

                }
            }
            return View(nuevoProcesoElectoral);

        }
        public IActionResult EditProcesoElectoral(int id)
        {
            try
            {
                var procesoElectoralData = Crud<ProcesoElectoral>.GetById(id);
                return View(procesoElectoralData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(id);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProcesoElectoral(int id, ProcesoElectoral procesoElectoralData)
        {
            try
            {
                Crud<ProcesoElectoral>.Update(id, procesoElectoralData);
                return RedirectToAction(nameof(ListProcesoElectoral));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(procesoElectoralData);
            }
        }

        public IActionResult DeleteProcesoElectoral(int id)
        {
            var procesoElectoralData = Crud<ProcesoElectoral>.GetById(id);
            return View(procesoElectoralData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProcesoElectoral(int id, ProcesoElectoral procesoElectoralData)
        {
            try
            {
                Crud<ProcesoElectoral>.Delete(id);
                return RedirectToAction(nameof(ListProcesoElectoral));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(procesoElectoralData);
            }
        }
        // Lista-Crud
        public IActionResult ListLista()
        {
            try
            {
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
                var ListaData = Crud<Lista>.GetById(id);
                return View(ListaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View();
            }
        }
        public IActionResult CreateLista()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLista(Lista nuevoLista)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Crud<Lista>.Create(nuevoLista);
                    return RedirectToAction(nameof(ListLista));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);

                }
            }
            return View(nuevoLista);

        }
        public IActionResult EditLista(int id)
        {
            try
            {
                var ListaData = Crud<Lista>.GetById(id);
                return View(ListaData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al conectar con la API: " + ex.Message;
                return View(id);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLista(int id, Lista ListaData)
        {
            try
            {
                Crud<Lista>.Update(id, ListaData);
                return RedirectToAction(nameof(ListLista));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(ListaData);
            }
        }

        public IActionResult DeleteLista(int id)
        {
            var ListaData = Crud<Lista>.GetById(id);
            return View(ListaData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLista(int id, Lista ListaData)
        {
            try
            {
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
