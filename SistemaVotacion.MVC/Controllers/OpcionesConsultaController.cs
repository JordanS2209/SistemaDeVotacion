using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    public class OpcionesConsultaController : Controller
    {
        // GET: OpcionesConsultaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: OpcionesConsultaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OpcionesConsultaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OpcionesConsultaController/Create
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

        // GET: OpcionesConsultaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OpcionesConsultaController/Edit/5
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

        // GET: OpcionesConsultaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OpcionesConsultaController/Delete/5
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
