using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class PadronesController : Controller
    {
        // GET: PadronesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PadronesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PadronesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PadronesController/Create
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

        // GET: PadronesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PadronesController/Edit/5
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

        // GET: PadronesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PadronesController/Delete/5
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
