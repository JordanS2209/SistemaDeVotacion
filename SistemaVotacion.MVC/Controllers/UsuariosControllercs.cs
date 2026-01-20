using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.MVC.Controllers
{
    public class UsuariosControllercs : Controller
    {
        // GET: UsuariosControllercs
        public ActionResult Index()
        {
            var data = Crud<Usuario>.ReadAll();
            var total = data.Data.Count;
            
            return View(data);
        }

        // GET: UsuariosControllercs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuariosControllercs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosControllercs/Create
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

        // GET: UsuariosControllercs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuariosControllercs/Edit/5
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

        // GET: UsuariosControllercs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuariosControllercs/Delete/5
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
