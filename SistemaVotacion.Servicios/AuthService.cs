using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;
using System.Security.Claims;

namespace SistemaVotacion.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Login(string email, string password)
        {
            // Nota: Es más eficiente buscar solo el usuario por email que traer todos
            var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == email);

            if (usuario != null)
            {
                // Verificamos si la cuenta está bloqueada antes de intentar el login
                if (usuario.CuentaBloqueada == true) return false;

                // BCrypt compara el texto plano con el Hash de la base de datos
                if (BCrypt.Net.BCrypt.Verify(password, usuario.ContrasenaHash))
                {
                    var datosUsuario = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombres),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? "Usuario") // Agregamos el rol a la identidad
                    };

                    var credencialDigital = new ClaimsIdentity(datosUsuario, "Cookies");
                    var usuarioAutenticado = new ClaimsPrincipal(credencialDigital);

                    await _httpContextAccessor.HttpContext.SignInAsync("Cookies", usuarioAutenticado);
                    return true;
                }
            }
            return false;
        }

        // Adaptado para recibir los campos obligatorios de tu modelo
        public async Task<bool> Register(
            string email,
            string nombre,
            string apellido,
            string password,
            DateTime fechaNacimiento,
            string numeroIdentificacion,
            string codigoDactilar,
            int idRol,
            int idTipoIdentificacion,
            int idGenero)
        {
            var usuarioExistente = Crud<Usuario>.GetAll()
                .FirstOrDefault(u => u.Email == email || u.NumeroIdentificacion == numeroIdentificacion);

            if (usuarioExistente != null) return false;

            try
            {
                // CREACIÓN DEL OBJETO USUARIO CON HASH SEGURIDAD
                var nuevoUsuario = new Usuario
                {
                    Id = 0,
                    Email = email,
                    Nombres = nombre,
                    Apellidos = apellido,
                    ContrasenaHash = password,
                    FechaNacimiento = fechaNacimiento,
                    NumeroIdentificacion = numeroIdentificacion,
                    CodigoDactilar = codigoDactilar,
                    IdRol = idRol,
                    IdTipoIdentificacion = idTipoIdentificacion,
                    IdGenero = idGenero,
                    IntentosFallidos = 0,
                    CuentaBloqueada = false
                };

                Crud<Usuario>.Create(nuevoUsuario);

                //SERVICIO CORREO
                var emailService = new EmailService();
                await emailService.EnviarCorreoRegistro(email, nombre);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return false;
            }
        }
    }
}