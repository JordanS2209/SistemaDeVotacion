using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class ActasAuditoriasController : Controller
    {
        // GET: ActasAuditoriasController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ActasAuditoriasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ActasAuditoriasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ActasAuditoriasController/Create
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

        // GET: ActasAuditoriasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ActasAuditoriasController/Edit/5
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

        // GET: ActasAuditoriasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ActasAuditoriasController/Delete/5
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
