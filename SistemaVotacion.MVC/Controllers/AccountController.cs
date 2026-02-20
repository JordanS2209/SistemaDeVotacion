using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            // Buscamos al usuario en la base de datos
           
            var usuario = Crud<Usuario>.GetSingle($"{Crud<Usuario>.EndPoint}/ByEmail/{email}");

            // Si el usuario no existe, mensaje estándar por seguridad
            if (usuario == null)
            {
                ViewBag.ErrorMessage = "Credenciales incorrectas.";
                return View("Index");
            }

            // Opción A: Usar el operador ?? para dar un valor por defecto
            if (usuario.CuentaBloqueada == true)
            {
                ViewBag.ErrorMessage = "Cuenta bloqueada. Contacte al administrador.";
                return View("Index");
            }

            // 3. Intentamos la autenticación con el servicio
            bool loginExitoso = await _authService.Login(email, password);

            if (loginExitoso)
            {
                // Si entra con éxito, reseteamos los intentos a 0
                usuario.IntentosFallidos = 0;
                Crud<Usuario>.Update(usuario.Id, usuario); // Asegúrate que el campo sea IdUsuario o el que uses

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 4. Lógica de incremento de intentos fallidos
                usuario.IntentosFallidos++;

                if (usuario.IntentosFallidos >= 3)
                {
                    usuario.CuentaBloqueada = true;
                    ViewBag.ErrorMessage = "Cuenta bloqueada.";
                }
                else
                {
                    int restantes = 3 - usuario.IntentosFallidos;
                    ViewBag.ErrorMessage = $"Credenciales mal puestas. {restantes} intentos restantes.";
                }

                // 5. Guardamos el nuevo estado (intentos o bloqueo) mediante el Crud
                Crud<Usuario>.Update(usuario.Id, usuario);

                return View("Index");
            }
        }

        
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult Register()
        {
            CargarCombosRegister();
            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
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
            email = (email ?? "").Trim().ToLower();


            // 2. Verificar si el usuario ya existe (por Email o Cédula)
            var usuarioEmail = Crud<Usuario>.GetSingle($"{Crud<Usuario>.EndPoint}/ByEmail/{email}");
            var usuarioCedula = Crud<Usuario>.GetSingle($"{Crud<Usuario>.EndPoint}/ByCedula/{cedula}");
            
            if (usuarioEmail != null || usuarioCedula != null)
            {
                ViewBag.ErrorMessage = "El correo o número de identificación ya están registrados.";
                CargarCombosRegister();
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
                // Al ser SuperAdmin creando usuarios, redirigimos al dashboard o lista de usuarios
                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Ocurrió un error al procesar el registro.";
            CargarCombosRegister();
            return View();
        }

        private void CargarCombosRegister()
        {
            ViewBag.Roles = Crud<Rol>.GetAll()?.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.NombreRol }).ToList() ?? new List<SelectListItem>();
            ViewBag.Generos = Crud<Genero>.GetAll()?.Select(g => new SelectListItem { Value = g.IdGenero.ToString(), Text = g.DetalleGenero }).ToList() ?? new List<SelectListItem>();
            ViewBag.TiposIdentificacion = Crud<TipoIdentificacion>.GetAll()?.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.DetalleTipIdentifiacion }).ToList() ?? new List<SelectListItem>();
        }

        public async Task<IActionResult> Logout()
        {
            // Elimina la cookie de autenticación
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Account");
        }

        // GET: Account/UsuariosBloqueados
        public IActionResult UsuariosBloqueados()
        {
            // Obtenemos todos los usuarios desde la API usando el DTO correcto para Listas
            var todosLosUsuarios = Crud<UsuarioListDto>.GetAll();

            // Filtramos solo los que tienen la cuenta bloqueada
          
            var bloqueados = todosLosUsuarios.Where(u => u.CuentaBloqueada ?? false).ToList();

            return View(bloqueados);
        }

        // POST: Account/Desbloquear/5
        [HttpPost]
        public IActionResult Desbloquear(int id)
        {
            // 1. Buscamos al usuario por su ID
            var usuario = Crud<Usuario>.GetById(id);

            if (usuario != null)
            {
                // 2. Reseteamos los valores de seguridad
                usuario.CuentaBloqueada = false;
                usuario.IntentosFallidos = 0;

                // 3. Actualizamos el registro en la base de datos a través del CRUD
                Crud<Usuario>.Update(id, usuario);
            }

            return RedirectToAction("UsuariosBloqueados");
        }
    }
}