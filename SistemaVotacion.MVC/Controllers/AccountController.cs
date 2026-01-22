using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;

namespace SistemaVotacion.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Account
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (await _authService.Login(email, password))
            {
                // Redirigir al Dashboard principal
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Email o contraseña incorrectos.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = Crud<Rol>.GetAll();
            ViewBag.Generos = Crud<Genero>.GetAll();
            ViewBag.TiposIdentificacion = Crud<TipoIdentificacion>.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            string email,
            string nombre,
            string apellido,
            string password,
            DateTime fechaNac,
            string cedula,
            string codigoDactilar,
            int idRol,
            int idTipoIdent,
            int idGenero)
        {
            // 1. Limpieza y validaciones básicas
            email = email.Trim().ToLower();


            // 2. Verificar si el usuario ya existe (por Email o Cédula)
            var usuarios = Crud<Usuario>.GetAll();
            var existe = usuarios.Any(u => u.Email.ToLower() == email || u.NumeroIdentificacion == cedula);

            if (existe)
            {
                ViewBag.ErrorMessage = "El correo o número de identificación ya están registrados.";
                return View();
            }

            // 3. Llamar al servicio con todos los parámetros requeridos
            var exito = await _authService.Register(
                email,
                nombre,
                apellido,
                password,
                fechaNac,
                cedula,
                codigoDactilar,
                idRol,
                idTipoIdent,
                idGenero
            );

            if (exito)
            {
                // Al tener éxito, redirigimos al Login (que es el Index de este controlador)
                return RedirectToAction("Index", "Account");
            }

            ViewBag.ErrorMessage = "Ocurrió un error al procesar el registro.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Elimina la cookie de autenticación
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Account");
        }
    }
}