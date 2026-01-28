using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SistemaVotacion.MVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly HttpClient _client;

        public AccesoController()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/") 
            };
        }

        [HttpGet]
        public IActionResult IngresarCodigo()
        {
            if (TempData["Error"] is string tempError && !string.IsNullOrWhiteSpace(tempError))
            {
                ViewBag.Error = tempError;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IngresarCodigo(string codigo)
        {
            
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 6)
            {
                ViewBag.Error = "Debe ingresar un código de 6 dígitos.";
                return View();
            }

           
            var response = await _client.GetAsync($"api/padrones/validar-codigo/{codigo}");

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                ViewBag.Error = string.IsNullOrWhiteSpace(msg)
                    ? "Código no válido."
                    : msg;

                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);
            int padronId = data.padronId;

            
            return RedirectToAction("Index", "Boletas", new { codigo });
        }
    }
}