    using Microsoft.AspNetCore.Mvc;
    using SistemaVotacion.ApiConsumer;
    using SistemaVotacion.Modelos;
    using Newtonsoft.Json;
    using System.Net;
    using System.Net.Http.Json;
    using System.Collections.Generic;
    using System.Linq;

    namespace SistemaVotacion.MVC.Controllers
    {
        public class BoletasController : Controller
        {
            // ================================
            // ENTRADA ÚNICA AL SISTEMA DE VOTO
            // ================================
            [HttpGet]
            public async Task<IActionResult> Index(string codigo)
            {
                try
                {
                    using var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:7202/")
                    };

                    var response = await client.GetAsync("api/ProcesosElectorales/activo");

                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return View("ProcesoNoDisponible");

                    if (!response.IsSuccessStatusCode)
                        return View("ProcesoNoDisponible");

                    var proceso = JsonConvert.DeserializeObject<ProcesoElectoral>(
                        await response.Content.ReadAsStringAsync()
                    );

                    if (proceso == null)
                        return View("ProcesoNoDisponible");

                    // REDIRECCIÓN SEGÚN TIPO DE PROCESO
                    return proceso.IdTipoProceso switch
                    {
                        1 => RedirigirAEleccionesGenerales(codigo),
                        2 => RedirectToAction("ConsultaPopular", new { codigo }),
                        3 => RedirectToAction("EleccionesSeccionales"),
                        _ => View("ProcesoNoDisponible")
                    };
                }
                catch
                {
                    return View("ProcesoNoDisponible");
                }
            }

            // ================================
            // ELECCIONES GENERALES
            // ================================
        
            // POST: Procesar Voto Generales
            [HttpPost]
            public async Task<IActionResult> Index(string codigo, int idLista)
            {
                if (string.IsNullOrWhiteSpace(codigo) || idLista <= 0)
                {
                    TempData["Error"] = "Datos de votación inválidos.";
                    return RedirectToAction("IngresarCodigo", "Acceso");
                }

                try
                {
                    var votoDto = new VotoGeneralDto
                    {
                        CodigoAcceso = codigo,
                        IdLista = idLista
                    };

                    using var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:7202/")
                    };

                    var response = await client.PostAsJsonAsync("api/VotoDetalles/votar-general", votoDto);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("VotoRealizado");
                    }
                    else
                    {
                        var msg = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error al registrar voto: {msg}";
                        return RedirectToAction("IngresarCodigo", "Acceso");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error de conexión con el servidor.";
                    return RedirectToAction("IngresarCodigo", "Acceso");
                }
            }

            private IActionResult RedirigirAEleccionesGenerales(string codigo)
            {
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    TempData["Error"] = "Debe ingresar un código válido.";
                    return RedirectToAction("IngresarCodigo", "Acceso");
                }

                ViewBag.Codigo = codigo;
                var listas = Crud<Lista>.GetAll();
                return View("Index", listas);
            }

            // ================================
            // ELECCIONES SECCIONALES
            // ================================
            [HttpGet]
            public IActionResult EleccionesSeccionales()
            {
                var listas = Crud<Lista>.GetAll();
                return View(listas);
            }

            // ================================
            // CONSULTA POPULAR
            // ================================
            [HttpGet]
            public async Task<IActionResult> ConsultaPopular(string codigo)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(codigo))
                    {
                        TempData["Error"] = "Debe ingresar un código válido.";
                        return RedirectToAction("IngresarCodigo", "Acceso");
                    }

                    using var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:7202/")
                    };

                    // 1️⃣ Obtener proceso activo
                    var procesoResp = await client.GetAsync("api/ProcesosElectorales/activo");
                    if (!procesoResp.IsSuccessStatusCode)
                        return View("ProcesoNoDisponible");

                    var proceso = JsonConvert.DeserializeObject<ProcesoElectoral>(
                        await procesoResp.Content.ReadAsStringAsync()
                    );

                    // 2️⃣ Validar que sea CONSULTA POPULAR
                    if (proceso == null || proceso.IdTipoProceso != 2)
                        return View("ProcesoNoDisponible");

                    // 3️⃣ Validar padrón por código
                    var padron = Crud<Padron>.GetAll()
                        .FirstOrDefault(p => p.CodigoAcceso == codigo && p.IdProceso == proceso.Id);

                    if (padron == null)
                        return View("ProcesoNoDisponible");

                    if (padron.HaVotado)
                        return View("VotoRealizado");

                    // 4️⃣ Obtener preguntas
                    var preguntasResp = await client.GetAsync(
                        $"api/PreguntasConsultas?procesoId={proceso.Id}"
                    );

                    var preguntas = preguntasResp.IsSuccessStatusCode
                        ? JsonConvert.DeserializeObject<List<PreguntaConsulta>>(
                            await preguntasResp.Content.ReadAsStringAsync())
                        : new List<PreguntaConsulta>();

                    // 4️⃣.1 Obtener opciones
                    var opcionesResp = await client.GetAsync(
                        $"api/OpcionesConsultas/por-proceso/{proceso.Id}"
                    );

                    var opciones = opcionesResp.IsSuccessStatusCode
                        ? JsonConvert.DeserializeObject<List<OpcionConsulta>>(
                            await opcionesResp.Content.ReadAsStringAsync())
                        : new List<OpcionConsulta>();

                    // 5️⃣ Enviar datos a la vista
                    ViewBag.IdProceso = proceso.Id;
                    ViewBag.IdPadron = padron.Id;
                    ViewBag.Preguntas = preguntas;
                    ViewBag.Opciones = opciones;

                    return View();
                }
                catch
                {
                    return View("ProcesoNoDisponible");
                }
            }

            // ================================
            // REGISTRAR VOTO (CONSULTA POPULAR)
            // ================================
            [HttpPost]
            public async Task<IActionResult> Votar(int idProceso, int idPadron, Dictionary<int, int> respuestas)
            {
                try
                {
                    if (respuestas == null || respuestas.Count == 0)
                        return View("VotoRealizado");

                    using var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:7202/")
                    };

                    var payload = new
                    {
                        idProceso,
                        idPadron,
                        respuestas = respuestas.Select(r => new
                        {
                            idPregunta = r.Key,
                            idOpcion = r.Value
                        }).ToList()
                    };

                    var resp = await client.PostAsJsonAsync(
                        "api/VotoDetalles/registrar-consulta",
                        payload
                    );

                    if (!resp.IsSuccessStatusCode)
                        return View("ProcesoNoDisponible");

                    return RedirectToAction("VotoRealizado");
                }
                catch
                {
                    return View("ProcesoNoDisponible");
                }
            }

            // ================================
            // CONFIRMACIÓN FINAL
            // ================================
            [HttpGet]
            public IActionResult VotoRealizado()
            {
                return View();
            }
        }
    }
