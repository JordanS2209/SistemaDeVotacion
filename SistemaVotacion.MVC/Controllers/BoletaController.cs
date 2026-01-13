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

    }
}
