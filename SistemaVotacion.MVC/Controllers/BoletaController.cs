using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class BoletaController : Controller
    {
        // GET: BoletaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BoletaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BoletaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BoletaController/Create
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

        // GET: BoletaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BoletaController/Edit/5
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

        // GET: BoletaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BoletaController/Delete/5
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
