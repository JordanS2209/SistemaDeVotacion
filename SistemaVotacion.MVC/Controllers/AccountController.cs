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
            // 1. Buscamos al usuario en la base de datos a través de la API
            var usuarios = Crud<Usuario>.GetAll();
            var usuario = usuarios.FirstOrDefault(u => u.Email.ToLower() == email.Trim().ToLower());

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
            try
            {
                // 1. Limpieza y validaciones básicas
                email = email.Trim().ToLower();

                // 2. Verificar si el usuario ya existe (Cédula o Email)
                var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();
                var existe = usuarios.Any(u => u.Email.ToLower() == email || u.NumeroIdentificacion == cedula);

                if (existe)
                {
                    throw new Exception("El correo o número de identificación ya están registrados.");
                }

                // 3. Llamar al servicio (El servicio ahora lanzará excepciones en lugar de solo devolver false)
                await _authService.Register(
                    email, nombre, apellido, password, fechaNac,
                    cedula, codigoDactilar, idRol, idTipoIdent, idGenero
                );

                // Si llega aquí, el registro fue exitoso
                return RedirectToAction("Index", "Account");
            }
            catch (Exception ex)
            {
                // 4. CAPTURA DEL ERROR: Aquí es donde verás el mensaje real
                ViewBag.ErrorMessage = ex.Message;

                // 5. RECARGA CRÍTICA: Debemos repoblar los ViewBags para que los Dropdowns no crasheen al recargar la vista
                ViewBag.Roles = Crud<Rol>.GetAll() ?? new List<Rol>();
                ViewBag.Generos = Crud<Genero>.GetAll() ?? new List<Genero>();
                ViewBag.TiposIdentificacion = Crud<TipoIdentificacion>.GetAll() ?? new List<TipoIdentificacion>();

                return View();
            }
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
            // Obtenemos todos los usuarios desde la API
            var todosLosUsuarios = Crud<Usuario>.GetAll();

            // Filtramos solo los que tienen la cuenta bloqueada
            // Usamos '?? false' para manejar el tipo bool? correctamente
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