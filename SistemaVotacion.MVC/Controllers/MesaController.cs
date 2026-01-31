using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace SistemaVotacion.MVC.Controllers
{
    public class MesaController : Controller
    {
        private readonly HttpClient _client;

        public MesaController()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/")
            };
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BuscarVotante(string numeroIdentificacion)
        {
            if (string.IsNullOrWhiteSpace(numeroIdentificacion))
            {
                ViewBag.Error = "Debe ingresar la cédula.";
                return View("Index");
            }

            var response = await _client.GetAsync(
                $"api/padrones/estado-votante-identificacion/{numeroIdentificacion}"
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "El votante no se encuentra en el padrón.";
                return View("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);

            bool haVotado = data.haVotado;
            bool procesoActivo = data.procesoActivo;
            string codigoAcceso = data.codigoAcceso;
            int padronId = data.padronId;


            if (!procesoActivo)
            {
                ViewBag.Error = "No existe un proceso electoral activo.";
                return View("Index");
            }


            if (haVotado)
            {
                ViewBag.YaVoto = true;
                return View("Index");
            }


            ViewBag.PadronId = padronId;
            ViewBag.CodigoAcceso = codigoAcceso;
            ViewBag.MostrarVentanaCodigo = true;

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Habilitar(int padronId)
        {
            var response = await _client.PostAsync(
                $"api/padrones/habilitar/{padronId}",
                null
            );

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                ViewBag.Error = string.IsNullOrWhiteSpace(msg)
                    ? "No se pudo habilitar la votación."
                    : msg;

                return View("Index");
            }

            var jsonCodigo = await response.Content.ReadAsStringAsync();
            dynamic codigo = JsonConvert.DeserializeObject(jsonCodigo);

            ViewBag.Codigo = codigo.codigoAcceso;

            return View("CodigoGenerado");
        }
    }
}