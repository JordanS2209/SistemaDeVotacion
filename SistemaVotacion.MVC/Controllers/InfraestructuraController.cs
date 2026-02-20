using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVotacion.MVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class InfraestructuraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
