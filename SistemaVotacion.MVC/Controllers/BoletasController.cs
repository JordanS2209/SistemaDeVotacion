using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SistemaVotacion.MVC.Controllers
{
    public class BoletasController : Controller
    {
        [HttpGet]
        public IActionResult Index(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                TempData["Error"] = "Debe ingresar un código válido.";
                return RedirectToAction("IngresarCodigo", "Acceso");
            }

            ViewBag.Codigo = codigo;

            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7202/")
            };

            // 1️⃣ Validar código y obtener proceso
            var resp = client.GetAsync($"api/Padrones/validar-codigo/{codigo}").Result;
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = "Código inválido o ya usado.";
                return View(new List<Lista>());
            }

            dynamic data = Newtonsoft.Json.JsonConvert
                .DeserializeObject(resp.Content.ReadAsStringAsync().Result);

            int idProceso = data.idProceso;


            var listasResp = client.GetAsync($"api/Listas/por-proceso/{idProceso}").Result;
            if (!listasResp.IsSuccessStatusCode)
            {
                return View(new List<Lista>());
            }

            var listas = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<Lista>>(listasResp.Content.ReadAsStringAsync().Result);

            return View(listas);
        }


        [HttpPost]
        public IActionResult Index(int idLista, string codigo)
        {
            try
            {
                using var client = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:7202/")
                };

                // 1️⃣ Validar código
                var padronResp = client.GetAsync($"api/Padrones/validar-codigo/{codigo}").Result;
                if (!padronResp.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Código inválido o ya usado.";
                    return View("VotoRealizado");
                }

                dynamic padronData = Newtonsoft.Json.JsonConvert
                    .DeserializeObject(padronResp.Content.ReadAsStringAsync().Result);

                int idPadron = padronData.padronId;


                int idProceso = 1;
                int idJunta = 1;


                var voto = new
                {
                    IdTipoVoto = 1,
                    IdJunta = idJunta,
                    IdProceso = idProceso,
                    IdLista = idLista,
                    IdDignidad = 1,
                    IdOpcion = (int?)null,
                    IdPregunta = (int?)null
                };

                //envia voto a la api
                var votoResp = client.PostAsJsonAsync(
                    "api/VotoDetalles/registrar-voto",
                    voto
                ).Result;

                if (!votoResp.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudo registrar el voto.";
                    return View("VotoRealizado");
                }

                ViewBag.Exito = true;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("VotoRealizado");
        }



    }
}