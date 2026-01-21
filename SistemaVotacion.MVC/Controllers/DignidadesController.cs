using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class DignidadesController : Controller
    {
        // GET: DignidadesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DignidadesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DignidadesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DignidadesController/Create
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

        // GET: DignidadesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DignidadesController/Edit/5
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

        // GET: DignidadesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DignidadesController/Delete/5
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
