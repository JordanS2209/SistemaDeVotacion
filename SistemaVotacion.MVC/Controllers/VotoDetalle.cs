using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SistemaVotacion.MVC.Controllers
{
    public class VotoDetalle : Controller
    {
        // GET: VotoDetalle
        public async Task<ActionResult> Index()
        {
            List<dynamic> resultados = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7202/");
                var response = await client.GetAsync("api/Resultados/por-lista");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    resultados = JsonConvert.DeserializeObject<List<dynamic>>(json);
                }
            }
            ViewBag.Resultados = resultados;
            return View();
        }

        // GET: VotoDetalle/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VotoDetalle/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotoDetalle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VotoDetalle/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotoDetalle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VotoDetalle/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VotoDetalle/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
