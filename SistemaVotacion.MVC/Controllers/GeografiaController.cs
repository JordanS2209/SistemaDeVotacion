using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class GeografiaController : Controller
    {
        // GET: GeografiaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: GeografiaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GeografiaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GeografiaController/Create
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

        // GET: GeografiaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GeografiaController/Edit/5
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

        // GET: GeografiaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GeografiaController/Delete/5
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
