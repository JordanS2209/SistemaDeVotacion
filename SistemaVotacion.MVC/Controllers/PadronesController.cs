using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class PadronesController : Controller
    {
        // GET: PadronController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PadronController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PadronController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PadronController/Create
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

        // GET: PadronController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PadronController/Edit/5
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

        // GET: PadronController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PadronController/Delete/5
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
