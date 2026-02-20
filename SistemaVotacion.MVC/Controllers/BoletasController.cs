using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "Votante, SuperAdmin, Delegado")] 
    public class BoletasController : Controller
    {
       
        [HttpGet]
        public IActionResult Index(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return RedirectToAction("IngresarCodigo", "Acceso");
            }

            try
            {
                // 1. RE-VALIDAR CÓDIGO Y ESTADO 
                var urlValidar = $"{Crud<Padron>.EndPoint}/validar-codigo/{codigo}";
                
              
                int padronId = 0;
                int procesoId = 0;

                using (var client = new HttpClient())
                {
                    var responseVal = client.GetAsync(urlValidar).Result;
                    if (!responseVal.IsSuccessStatusCode)
                    {
                        var msg = responseVal.Content.ReadAsStringAsync().Result;
                        TempData["Error"] = "Error de validación: " + msg;
                        return RedirectToAction("IngresarCodigo", "Acceso");
                    }

                    var jsonVal = responseVal.Content.ReadAsStringAsync().Result;
                    
                    // Usar un objeto dinámico con JObject para ser case-insensitive o DTO
                    var dataVal = Newtonsoft.Json.Linq.JObject.Parse(jsonVal);
                    
                    // Buscar propiedades ignorando mayúsculas/minúsculas
                    padronId = (int)(dataVal["padronId"] ?? dataVal["PadronId"] ?? 0);
                    procesoId = (int)(dataVal["idProceso"] ?? dataVal["IdProceso"] ?? 0);

                    if (padronId == 0 || procesoId == 0)
                    {
                         TempData["Error"] = "Error de formato en respuesta de validación.";
                         return RedirectToAction("IngresarCodigo", "Acceso");
                    }
                }

                // 2. OBTENER PROCESO
                var proceso = Crud<ProcesoElectoral>.GetById(procesoId);
                if (proceso == null || !EstaActivo(proceso))
                {
                    return View("ProcesoNoDisponible");
                }

                // 3. DETERMINAR TIPO DE PROCESO
                var tipoProceso = Crud<TipoProceso>.GetById(proceso.IdTipoProceso);
                string nombreTipo = tipoProceso?.NombreTipoProceso?.ToLower() ?? "";

                ViewBag.Codigo = codigo;
                ViewBag.FechaFinVotacion = proceso.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss");
                ViewBag.IdProceso = procesoId;
                ViewBag.IdPadron = padronId;

                // Despacho según tipo con normalización de nombres
                if (nombreTipo.Contains("consulta") || nombreTipo.Contains("referendum"))
                {
                    return CargarConsultaPopular(procesoId);
                }
                else
                {
                    return CargarEleccionCandidatos(procesoId);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la boleta: " + ex.Message;
                return RedirectToAction("IngresarCodigo", "Acceso");
            }
        }

        private IActionResult CargarEleccionCandidatos(int procesoId)
        {
            try 
            {
                // Obtenemos Listas del Proceso
                var url = $"{Crud<Lista>.EndPoint}/por-proceso/{procesoId}";
                var listas = Crud<Lista>.GetCustom(url);

                if (listas == null || !listas.Any())
                {
                    // Fallback si la API custom no devuelve nada o falla
                     var todas = Crud<Lista>.GetAll() ?? new List<Lista>();
                     listas = todas.Where(l => l.IdProceso == procesoId).ToList();
                }

                return View("Index", listas);
            }
            catch
            {
                return View("Index", new List<Lista>());
            }
        }

        private IActionResult CargarConsultaPopular(int procesoId)
        {
            try
            {
                // Cargar Preguntas del proceso
                var todasPreguntas = Crud<PreguntaConsulta>.GetAll() ?? new List<PreguntaConsulta>();
                var preguntas = todasPreguntas.Where(p => p.IdProceso == procesoId).ToList();

                // Cargar Opciones
                // Lo más eficiente sería un endpoint en API "GetCuestionario/{procesoId}", 
                // pero lo haremos con lo que tenemos.
                
                var todasOpciones = Crud<OpcionConsulta>.GetAll() ?? new List<OpcionConsulta>();
                // Solo opciones de las preguntas filtradas
                var preguntasIds = preguntas.Select(p => p.Id).ToList();
                var opciones = todasOpciones.Where(o => preguntasIds.Contains(o.IdPregunta)).ToList();

                ViewBag.Preguntas = preguntas;
                ViewBag.Opciones = opciones;

                return View("ConsultaPopular");
            }
            catch
            {
                return View("ProcesoNoDisponible");
            }
        }


        [HttpPost]
        public IActionResult Index(string codigo, int idLista)
        {
            // VOTO ELECCIÓN GENERAL
            if (string.IsNullOrWhiteSpace(codigo))
            {
                 // Error
                 return RedirectToAction("Index", new { codigo = codigo });
            }

            try
            {
                 var votoDto = new VotoGeneralDto
                 {
                     CodigoAcceso = codigo,
                     IdLista = idLista
                 };

                 var url = $"{Crud<VotoDetalle>.EndPoint}/votar-general";

                 using (var client = new HttpClient())
                 {
                     var json = JsonConvert.SerializeObject(votoDto);
                     var content = new StringContent(json, Encoding.UTF8, "application/json");
                     
                
                     var response = client.PostAsync(url, content).Result;

                     if (response.IsSuccessStatusCode)
                     {
                         return View("VotoRealizado");
                     }
                     else
                     {
                         var msg = response.Content.ReadAsStringAsync().Result;
                         if (string.IsNullOrWhiteSpace(msg))
                         {
                             msg = $"Error de API ({response.StatusCode})";
                         }
                         if (msg.Length > 500) msg = msg.Substring(0, 500);
                         TempData["Error"] = msg;
                         return RedirectToAction("IngresarCodigo", "Acceso");
                     }
                 }
            }
            catch (Exception ex)
            {
                 TempData["Error"] = ex.Message;
                 return RedirectToAction("IngresarCodigo", "Acceso");
            }
        }

        [HttpPost]
        public IActionResult Votar(string codigo, int IdProceso, int IdPadron, Dictionary<int, int> respuestas, bool EsVotoBlanco = false)
        {
            // VOTO CONSULTA POPULAR
            
            
            if (!EsVotoBlanco && (respuestas == null || !respuestas.Any()))
            {
                TempData["Error"] = "Debe seleccionar opciones.";
                return RedirectToAction("Index", new { codigo = codigo }); 
            }

            try
            {
                var listaRespuestas = new List<object>();
                foreach(var kvp in respuestas)
                {
                    listaRespuestas.Add(new { IdPregunta = kvp.Key, IdOpcion = kvp.Value });
                }

                var dto = new 
                {
                    IdProceso = IdProceso,
                    IdPadron = IdPadron,
                    CodigoAcceso = codigo,
                    Respuestas = listaRespuestas,
                    EsVotoBlanco = EsVotoBlanco
                };

                var url = $"{Crud<VotoDetalle>.EndPoint}/registrar-consulta";

                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(dto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return View("VotoRealizado");
                    }
                    else
                    {
                         // Manejo básico de error
                         var msg = response.Content.ReadAsStringAsync().Result;
                         if (string.IsNullOrWhiteSpace(msg))
                         {
                             msg = $"Error de API Consulta ({response.StatusCode})";
                         }
                         TempData["Error"] = msg;
                         return RedirectToAction("IngresarCodigo", "Acceso");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Excepción: " + ex.Message;
                return RedirectToAction("IngresarCodigo", "Acceso");
            }
        }

        private bool EstaActivo(ProcesoElectoral p)
        {
            var ahora = DateTime.Now;
            return ahora >= p.FechaInicio && ahora <= p.FechaFin;
        }
    }
}
